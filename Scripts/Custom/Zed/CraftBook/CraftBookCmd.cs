using Server.Commands.GMUtils;
using Server.Commands;
using Server.Gumps;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Server.Logging;

namespace Server.Items
{
    public partial class CraftBook
    {
        public static void Initialize()
        {
            CommandSystem.Register("CraftBookRespawn", AccessLevel.Owner, CraftBookRespawn_onCommand);
            CommandSystem.Register("CraftBookGenerate", AccessLevel.Owner, CraftBookGenerate_onCommand);
            CommandSystem.Register("CraftBookInfo", AccessLevel.Owner, CraftBookInfo_onCommand);
            CommandSystem.Register("CraftBookChange", AccessLevel.Owner, CraftBookChange_onCommand);

            string filePath = Path.Combine(Core.BaseDirectory, "Data/craft_books_name.txt");

            if (File.Exists(filePath))
            {
                using (StreamReader ip = new StreamReader(filePath))
                {
                    string line;

                    while ((line = ip.ReadLine()) != null)
                    {
                        try
                        {
                            string[] split = line.Split('$');
                            m_BookNames[(SkillName)Enum.Parse(typeof(SkillName), split[0], true)] = split[1].Split(';');
                        }
                        catch
                        {
                            ConsoleLog.Write.Error($"Error parse row:{line}");
                            throw;
                        }
                    }
                }
            }
            else
            {
                ConsoleLog.Write.Error($"Error load file:{filePath}");
                throw new Exception("File not found");
            }

            foreach (var cf in m_AllCraftBooks)
            {
                if (!m_Books.Keys.Contains(cf.SkillToLearn))
                    m_Books[cf.SkillToLearn] = new HashSet<CraftBook>();
                m_Books[cf.SkillToLearn].Add(cf);
            }

            m_CraftBooksLocation = new List<string>
            {
                "Unveiling the arcane mysteries of $Skill$, a selection of tomes awaits your perusal, with one nestled among the hallowed halls of the city of $City$.",
                "Amidst the vast realm of knowledge, volumes dedicated to the art of $Skill$ beckon, revealing their secrets to those who dare delve within, with one such tome residing in the illustrious city of $City$.",
                "Embark on a quest for knowledge as you explore the realm's diverse tomes on the subject of $Skill$, with a particular volume awaiting your discovery in the city of $City$.",
                "Discover the secrets of $Skill$ within the hallowed pages of books, including one located in the city of $City$",
                "Unleash the power of $Skill$ through the sacred knowledge contained within books, with a tome of great wisdom awaiting you in the city of $City$.",
                "Books on $Skill$ abound,\r\n$City$'s treasure holds one close,\r\nKnowledge, waiting, grows."
            };
            m_CraftBooksLocationWithName = new List<string>
            {
                "I heard vendor $Name$ from $City$ recently received a book about all the secrets of $Skill$",
                "Hear ye, hear ye! Vendor $Name$ in $City$ is the proud owner of the exclusive book that unveils the mysteries of $Skill$.",
                "Calling all knowledge seekers! Head to $City$ and visit merchant $Name$ to uncover the ancient tome on $Skill$.",
                "Word has spread that merchant $Name$ in $City$ has come into possession of a tome containing the profound secrets of $Skill$.",
                "Rumor has it that a renowned merchant named $Name$ in $City$ has acquired a book that unveils the hidden knowledge of $Skill$.",
                "Whispers circulate about a valuable tome discovered by merchant $Name$ in $City$, containing the coveted secrets of $Skill$",
                "The news has spread far and wide that merchant $Name$ in $City$ has obtained a rare book divulging the ancient wisdom of $Skill$.",
                "There is talk of a remarkable book now in the possession of merchant $Name$ in $City$, which reveals the enigmatic secrets of $Skill$."
            };

            Timer.DelayCall(TimeSpan.FromDays(1), TimeSpan.FromDays(1), CraftBook.CraftBookRespawn);
        }

        [Usage("CraftBookChange")]
        [Description("Change Craftbook parameters")]
        private static void CraftBookChange_onCommand(CommandEventArgs e)
        {
            if (Books.Count == 0)
                e.Mobile.SendAsciiMessage("No any craft books found");
            try
            {
                Dictionary<string, string> values = new Dictionary<string, string>();
                for (int i = 0; i < e.Arguments.Length; i = i + 2)
                    values[e.GetString(i)] = e.GetString(i + 1);
                var props = Books[0].GetType().GetProperties();
                foreach (string value in values.Keys)
                {
                    PropertyInfo propToChange = null;
                    foreach (var propertyInfo in props)
                    {
                        if (propertyInfo.Name.ToLower() == value.ToLower())
                        {
                            propToChange = propertyInfo;
                            break;
                        }
                    }

                    if (propToChange == null)
                    {
                        e.Mobile.SendAsciiMessage($"Prop with name {value} not found in CraftBooks");
                        continue;
                    }
                    foreach (var craftBook in Books)
                    {
                        PropertyInfo prop = craftBook.GetType().GetProperty(propToChange.Name, BindingFlags.Public | BindingFlags.Instance);
                        if (null != prop && prop.CanWrite)
                        {
                            if (prop.PropertyType == typeof(int))
                            {
                                prop.SetValue(craftBook, Convert.ToInt32(values[value]), null);
                            }
                            else
                            {
                                prop.SetValue(craftBook, values[value], null);
                            }


                        }

                    }
                }
            }
            catch (Exception exception)
            {
                e.Mobile.SendAsciiMessage($"Ex {exception}");
            }

        }

        [Usage("CraftBookInfo")]
        [Description("Get craftbook info")]
        private static void CraftBookInfo_onCommand(CommandEventArgs e)
        {
            if (Books.Count > 0)
            {
                e.Mobile.SendGump(new PropertiesGump(e.Mobile, Books.First()));
                e.Mobile.SendAsciiMessage("Choose Books variable for get list of all books");
            }
            else
                e.Mobile.SendAsciiMessage("No any craft books found");
        }


        [Usage("CraftBookGenerate")]
        [Description("Respawn craftbook")]
        private static void CraftBookGenerate_onCommand(CommandEventArgs e)
        {
            int count = 1;
            if (e.Length >= 1)
                count = e.GetInt32(0);
            if (count > 0)
            {
                foreach (var skill in (SkillName[])Enum.GetValues(typeof(SkillName)))
                {
                    var total = m_AllCraftBooks.Count(m => m.Map == e.Mobile.Map && m.SkillToLearn == skill);
                    while (total < count)
                    {
                        var book = new CraftBook();
                        book.SkillToLearn = skill;
                        book.Map = e.Mobile.Map;
                        book.Visible = false;
                        Timer.DelayCall(TimeSpan.FromSeconds(Utility.Random(5)), () =>
                        {
                            book.ChangePlace();
                            if (book.Location == Point3D.Zero)
                            {
                                book.Delete();
                                e.Mobile.SendAsciiMessage($"Cant find location for put {skill} book");
                            }
                            else
                            {
                                book.Visible = true;
                            }
                        });
                        total++;
                    }
                }
            }
        }

        [Usage("CraftBookRespawn")]
        [Description("Change Craftbooks location on the world")]
        private static void CraftBookRespawn_onCommand(CommandEventArgs e)
        {
            CraftBookRespawn();
        }

        private static void CraftBookRespawn()
        {
            var count = 0;
            ConsoleLog.Write.Information("Books respawn started");
            foreach (var book in m_AllCraftBooks)
            {
                if (book.CanChangePlace())
                {
                    count++;
                    Timer.DelayCall(TimeSpan.FromSeconds(Utility.Random(m_AllCraftBooks.Count + 1)), () =>
                    {
                        book.ChangePlace();
                        if (--count == 0)
                        {
                            ConsoleLog.Write.Information("Books respawn completed");
                        }
                    });
                }
            }
        }

        private static Point3D GetNewLocation(Mobile vendor)
        {
            var items = vendor.GetObjectsInRange(m_MaxVendorRange);
            var places = new List<Point3D>();
            var allItems = new List<Point3D>();
            foreach (object item in items)
            {
                if (item is Item it && !it.Movable)
                {
                    if (m_TableTypes.Contains(it.ItemID) && m_AllCraftBooks.Count(m => m.Location.X == it.X && m.Location.Y == it.Y) == 0)
                        places.Add(new Point3D(it.X, it.Y, it.Z + 7));
                    allItems.Add(it.Location);
                }
            }
            items.Free();
            if (places.Count > 0)
                return Utility.RandomList(places);
            var locs = vendor.Map.Tiles.GetStaticTileAround(vendor.Location, m_MaxVendorRange, m_TableTypes);
            if (locs.Count == 0) return Point3D.Zero;
            Point3D val;
            for (int j = 0; j < 5; j++)
            {
                val = Utility.RandomList(locs);
                val.Z += 6;
                if (allItems.Count(m => m.X == val.X && m.Y == val.Y && Math.Abs(m.Z - val.Z) < 10) == 0 && m_AllCraftBooks.Count(m => m.Location.X == val.X && m.Location.Y == val.Y) == 0) return val;
            }

            return Point3D.Zero;
        }

        private void ChangePlace()
        {
            if (!CanChangePlace())
                return;
            bool moved = false;
            if (!BaseVendor.SkilledVendors.ContainsKey(SkillToLearn))
            {
                ConsoleLog.Write.Information($"Vendors with {SkillToLearn} skill not found in {Map.Name}");
                return;
            }

            var vendors = BaseVendor.SkilledVendors[SkillToLearn].Where(m => m.Map == Map).ToArray();
            if (!vendors.Any())
            {
                ConsoleLog.Write.Information($"Vendors with {SkillToLearn} skill not found in {Map.Name}");
                return;
            }

            for (int i = 0; i < vendors.Count(); i++)
            {
                var vendor = vendors.ElementAt(Utility.Random(vendors.Count()));
                if (vendor.Alive && !vendor.Warmode)
                {
                    var loc = GetNewLocation(vendor);
                    if (loc.X > 0)
                    {
                        Location = loc;
                        Map = vendor.Map;
                        moved = true;
                        SkillToLearn = SkillToLearn;
                        m_NearestVendorName = vendor.Name;
                        break;
                    }
                }
            }

            if (!moved)
            {
                ConsoleLog.Write.Information($"Can't move book with skill {SkillToLearn}");
            }
        }

        private bool CanChangePlace()
        {
            return Student == null && IsActive && Map != Map.Internal && !Deleted && Parent == null;
        }

    }
}
