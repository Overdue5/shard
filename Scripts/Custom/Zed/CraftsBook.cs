
using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Mobiles;
using Server.Commands;
using System.IO;
using System.Linq;
using Server.Commands.GMUtils;
using Server.Engines;
using Server.Network;

namespace Server.Items
{
    public class CraftBook : Item
    {
        private Mobile m_Student;
        private SkillName m_SkillToLearn;
        private Timer m_Timer;
        private int m_StudyTime;
        private int m_Ouch;
        private double m_MinSkill;
        private double m_MaxSkill;

        private static DateTime m_LastHelpInfo = DateTime.MinValue; 
        private static string m_cityName = "$City$";
        private static string m_SkillName = "$Skill$";
        private static int m_MaxVendorRange = 10;
        private static string m_UnknownBook = "Unknown book";
        private static Dictionary<SkillName, HashSet<CraftBook>> m_Books = new Dictionary<SkillName, HashSet<CraftBook>>();
        private static Dictionary<SkillName, string[]> m_BookNames = new Dictionary<SkillName, string[]>();
        private static List<CraftBook> m_AllCraftBooks = new List<CraftBook>();
        private static List<string> m_CraftBooksLocation;

        private static HashSet<int> m_TableTypes = new HashSet<int>()
        {
            0xB34, 0xB35, 0xB36, 0xB37, 0xB38, 0xB39, 0xB3A, 0xB3B, 0xB3E, 0xB3F, 0xB40,
            0xB6B, 0xB6C, 0xB6D, 0xB6E, 0xB6F,
            0xB70, 0xB71, 0xB72, 0xB73, 0xB74, 0xB75, 0xB76, 0xB77, 0xB78, 0xB79, 0xB7A, 0xB7B, 0xB7C, 0xB7D, 0xB7E, 0xB7F,
            0xB80, 0xB81, 0xB82, 0xB83, 0xB84, 0xB85, 0xB86, 0xB87, 0xB88, 0xB89, 0xB8A, 0xB8B, 0xB8C, 0xB8D, 0xB8E, 0xB8F,
            0xB90, 0x118B, 0x118C, 0x118D, 0x118E, 0x118F, 0x1190, 0x1191, 0x1192, 0x11D6, 0x11D7, 0x11D8, 0x11D9, 0x11DA, 0x11DB, 0x11DC,
            0x11DD, 0x11DE, 0x11DF, 0x11E0, 0x11E1,
            0x1201, 0x1202, 0x1203, 0x1204, 0x1205, 0x1206, 0x1667, 0x1668, 0x1669, 0x166A, 0x166B, 0x166C, 0x1DA5, 0x1DA6, 0x1DA7, 0x1DA8,
            0x1DA9, 0x1DAA, 0x1DAB, 0x1DAC,
            0x1DBB, 0x1DBC, 0x1DBD, 0x1DBE, 0x1DBF, 0x1DC0, 0x1DC1, 0x1DC2, 0x1DC3, 0x1DC4, 0x1DC5, 0x1DC6, 0x2819, 0x281A, 0x2DE1, 0x2DE2
        };

        public DateTime LastUsage;

        public static void Initialize()
        {
            CommandSystem.Register("CraftBookRespawn", AccessLevel.Owner, new CommandEventHandler(CraftBookRespawn_onCommand));
            CommandSystem.Register("CraftBookGenerate", AccessLevel.Owner, new CommandEventHandler(CraftBookGenerate_onCommand));
            CommandSystem.Register("CraftBookInfo", AccessLevel.Owner, new CommandEventHandler(CraftBookInfo_onCommand));

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
                            Utility.ConsoleWriteLine(Utility.ConsoleMsgType.Error, $"Error parse row:{line}");
                            throw;
                        }
                    }
                }
            }
            else
            {
                Utility.ConsoleWriteLine(Utility.ConsoleMsgType.Error, $"Error load file:{filePath}");
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
                "\r\nBooks on $Skill$ abound,\r\n$City$'s treasure holds one close,\r\nKnowledge, waiting, grows."
            };

            Timer.DelayCall(TimeSpan.FromDays(1), TimeSpan.FromDays(1), CraftBook.CraftBookRespawn);
        }

        [Usage("CraftBookInfo")]
        [Description("Respawn craftbook")]
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
            Utility.ConsoleWriteLine(Utility.ConsoleMsgType.Info, "Books respawn started");
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
                            Utility.ConsoleWriteLine(Utility.ConsoleMsgType.Info, "Books respawn completed");
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
                if (item is Item it  && !it.Movable)
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
                if (allItems.Count(m => m.X == val.X && m.Y == val.Y && Math.Abs(m.Z-val.Z)<10) == 0 && m_AllCraftBooks.Count(m => m.Location.X == val.X && m.Location.Y == val.Y) == 0) return val;
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
                Console.WriteLine($"Vendors with {SkillToLearn} skill not found in {Map.Name}");
                return;
            }

            var vendors = BaseVendor.SkilledVendors[SkillToLearn].Where(m => m.Map == Map);
            if (!vendors.Any())
            {
                Console.WriteLine($"Vendors with {SkillToLearn} skill not found in {Map.Name}");
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
                        break;
                    }
                }
            }

            if (!moved)
            {
                Console.WriteLine($"Can't move book with skill {SkillToLearn}");
            }
        }

        private bool CanChangePlace()
        {
            return Student == null && IsActive && Map != Map.Internal && !Deleted && Parent == null;
        }
        private static string GetNewName(SkillName skill)
        {
            if (m_BookNames.Keys.Contains(skill))
                return m_BookNames[skill][Utility.Random(m_BookNames[skill].Length)];
            return m_UnknownBook;
        }

        public static void FindDublicates(Mobile mob)
        {
            for (int x = 0; x < Books.Count - 1; x++)
            {
                for (int y = x+1; y < Books.Count; y++)
                {
                    if (Books[x].X == Books[y].X && Books[x].Y == Books[y].Y)
                    {
                        mob.SendAsciiMessage("Found");
                    }
                }

            }
        }

        public static void PrintBooksLocation(Mobile vendor, SkillName skill)
        {
            if (DateTime.UtcNow < m_LastHelpInfo + TimeSpan.FromMinutes(1)) return;
            m_LastHelpInfo = DateTime.UtcNow;
            var books = Books.Where(m => m.Map == vendor.Map && m.SkillToLearn == skill && m.IsActive && !String.IsNullOrEmpty(Region.Find(m.Location, m.Map).Name)).Select(m => Region.Find(m.Location, m.Map).Name).Distinct().ToArray();
            if (!books.Any()) return;
            var city = Utility.RandomList(books);
            var text =  Utility.RandomList(m_CraftBooksLocation).Replace(m_cityName, city.ToString()).Replace(m_SkillName, skill.ToString());
            Timer.DelayCall(TimeSpan.FromMilliseconds(500), () =>
            {
                vendor.PublicOverheadMessage(MessageType.Emote, 1510, false, text);
            });

        }

        [CommandProperty(AccessLevel.GameMaster)]
        public static List<CraftBook> Books
        {
            get { return m_AllCraftBooks.OrderBy(m => m.SkillToLearn).ToList(); }
            set { m_AllCraftBooks = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SkillName SkillToLearn
        {
            get { return m_SkillToLearn; }
            set
            {
                m_SkillToLearn = value;
                Name = GetNewName(value);
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int StudyTime
        {
            get { return m_StudyTime; }
            set
            {
                m_StudyTime = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Ouch
        {
            get { return m_Ouch; }
            set
            {
                m_Ouch = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double MinSkill
        {
            get { return m_MinSkill; }
            set { m_MinSkill = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double MaxSkill
        {
            get { return m_MaxSkill; }
            set { m_MaxSkill = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Student
        {
            get { return m_Student; }
            set { m_Student = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsActive => Name != m_UnknownBook;

        [Constructable]
        public CraftBook() : base(0x1E20)
        {
            Name = m_UnknownBook;
            Movable = false;
            LootType = LootType.Blessed;
            Weight = 5;
            StudyTime = 3;
            Ouch = 0;
            MinSkill = 30.0;
            MaxSkill = 80.0;
            m_AllCraftBooks.Add(this);
        }

        public CraftBook(Serial serial) : base(serial)
        {
        }

        public void UseCraftTrainer(Mobile from)
        {
            if (Student == null || this.LastUsage >= DateTime.UtcNow + TimeSpan.FromSeconds(this.StudyTime * 2))
            {
                if (m_Timer != null && m_Timer.Running)
                    m_Timer.Stop();
                if (!from.BeginAction(typeof(IAction)))
                {
                    from.SendAsciiMessage("You must wait to perform another action.");
                    return;
                }

                m_Timer = new StudyTimer(from, this);
                m_Timer.Start();
                this.m_Student = from;
            }
            else if (from == m_Student)
                from.SendMessage("You haven't read the previous page yet");
            else
                from.SendMessage($"{m_Student.Name} is reading this book");
        }

        public bool CheckUsage(Mobile m_From)
        {
            if (Name == m_UnknownBook)
            {
                if (m_From.AccessLevel >= AccessLevel.Administrator)
                {
                    m_From.SendGump(new PropertiesGump(m_From, this));
                }
                else
                {
                    m_From.SendMessage($"You don't understand what this book is about.");
                }

                return false;
            }

            if (!m_From.Alive || m_From.Warmode)
            {
                m_From.SendMessage($"You don't have time to read now");
                return false;
            }

            if (m_From.GetDistanceToSqrt(Location) > 2 && Math.Abs(m_From.Location.Z - Location.Z) >= 5)
            {
                m_From.SendMessage($"You do not see what is written in the book, come closer");
                return false;
            }

            if (m_From.Mounted)
            {
                if (m_From.Mount is Mobile mob)
                    m_From.SendMessage($"You can't read, because the {mob.Name} keeps turning the page with its nose.");
                else if (m_From.Mount is Item item)
                    m_From.SendMessage($"You can't read, because the {item.Name} distracts you from reading.");
                return false;
            }

            if (m_From.Skills[this.SkillToLearn].Base < this.MinSkill)
            {
                m_From.SendMessage("You can't understand anything that's written in the book");
                return false;
            }

            if (m_From.Skills[this.SkillToLearn].Base > this.MaxSkill)
            {
                m_From.SendMessage("You already know everything in this book.");
                return false;
            }

            if (m_From.Stam <= 2 * this.Ouch)
            {
                string val = m_From.Female ? "madam" : "sir";
                m_From.SendMessage($"Thou art too fatigued to focus on reading, good {val}");
                return false;
            }

            if (m_From.Skills[SkillToLearn].Lock == SkillLock.Down)
            {
                m_From.SendMessage($"Are you sure that's how it works?");
                return false;
            }

            return true;
        }

        public override void OnDelete()
        {
            base.OnDelete();
            m_AllCraftBooks.Remove(this);
        }

        public override void OnDoubleClick(Mobile m_From)
        {
            FindDublicates(m_From);
            m_From.RevealingAction();
            m_From.Direction = m_From.GetDirectionTo(this.Location);
            if (CheckUsage(m_From))
            {
                UseCraftTrainer(m_From);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version 
            writer.Write((int)m_SkillToLearn);
            writer.Write((int)m_StudyTime);
            writer.Write((int)m_Ouch);

            writer.Write(m_MinSkill);
            writer.Write(m_MaxSkill);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_SkillToLearn = (SkillName)Enum.ToObject(typeof(SkillName), reader.ReadInt());
            m_StudyTime = reader.ReadInt();
            m_Ouch = reader.ReadInt();

            m_MinSkill = reader.ReadDouble();
            m_MaxSkill = reader.ReadDouble();
            m_AllCraftBooks.Add(this);
        }

        public override string ToString()
        {
            return $"{this.SkillToLearn};X:{X};Y:{Y}:{this.Map}";
        }

    }

    public class StudyTimer : Timer
    {
        private int m_hits;
        private DateTime m_moveTime;
        private Direction m_direction;

        static string[] ok = new[]
        {
            "As you gain knowledge, your mind expands like a balloon.",
            "A flash of insight illuminates your mind like a bolt of lightning.",
            "Your understanding grows as if a fog has lifted from your mind.",
            "The mysteries of the world unfold before you like a blossoming flower.",
            "You feel a surge of enlightenment, as if a switch has been flipped in your brain."
        };

        static string[] nok = new[]
        {
            "You can't seem to figure this one out. Try again later.",
            "No matter how hard you try, this just isn't making sense.",
            "You're feeling a bit dull today and can't quite grasp the concept.",
            "This one's a toughie, maybe you need some rest before trying again.",
            "You scratch your head in confusion, this is beyond your current knowledge.",
        };

        static string[] action = new[]
        {
            "You see $Player$ engrossed in a book.",
            "You see $Player$ appears to be deep in thought while reading a book.",
            "You see $Player$ start reading a book.",
            "You see $Player$ has started reading a book and seems fully absorbed in it."
        };

        private Mobile m_From;
        private CraftBook m_Book;

        private void PrintAction()
        {
            m_From.SayAction(GMExtendMethods.EmotionalTextHue.NormalAction, Utility.RandomList(action).Replace("$Player$", m_From.Name));
        }

        public StudyTimer(Mobile from, CraftBook craftBook) : base(TimeSpan.FromSeconds(craftBook.StudyTime),
            TimeSpan.FromSeconds(craftBook.StudyTime), 0)
        {
            m_From = from;

            m_moveTime = m_From.LastMoveTime;
            m_direction = m_From.Direction;
            m_hits = m_From.Hits;
            m_Book = craftBook;
            m_Book.LastUsage = DateTime.UtcNow;
            if (from.Body.IsHuman && !from.Mounted)
                from.Animate(34, 15, 1, true, false, 1);
            PrintAction();
            from.PlaySound(0x249);
        }

        protected override void OnTick()
        {
            if ((m_direction == m_From.Direction && m_moveTime == m_From.LastMoveTime && m_hits <= m_From.Hits && !m_From.Hidden &&
                 !m_From.Meditating) && m_Book.CheckUsage(m_From))
            {
                m_Book.LastUsage = DateTime.UtcNow;
                if (UtilityWorldTime.IsDark(m_From) && Utility.RandomDouble() > 0.25)
                {
                    m_From.SendMessage($"It's too dark, you can hardly read the text");
                }
                else
                {
                    m_From.Stam -= Utility.LimitMinMax(m_Book.Ouch, (int)m_From.Skills[m_Book.SkillToLearn].Base / 20, 2 * m_Book.Ouch);
                    int m_Str = m_From.Str;
                    int m_Dex = m_From.Dex;
                    m_hits = m_From.Hits;
                    var val = m_From.Skills[m_Book.SkillToLearn].Base;
                    m_From.CheckSkill(m_Book.SkillToLearn, m_Book.MinSkill, m_Book.MaxSkill);
                    if (val < m_From.Skills[m_Book.SkillToLearn].Base)
                    {
                        m_From.SendMessage(Utility.RandomList(ok));
                    }
                    else
                    {
                        m_From.SendMessage(Utility.RandomList(nok));
                    }

                    if (m_From.Str != m_Str)
                    {
                        m_From.Str = m_Str;
                    }

                    if (m_From.Dex != m_Dex)
                    {
                        m_From.Dex = m_Dex;
                    }
                }

                if (m_From.Body.IsHuman && !m_From.Mounted)
                    m_From.Animate(34, 15, 1, true, false, 1);
                m_From.PlaySound(0x249);
            }
            else
            {
                m_From.SendMessage("You stop reading the book");
                m_Book.Student = null;
                m_From.EndAction(typeof(IAction));
                Stop();
            }
        }
    }
}