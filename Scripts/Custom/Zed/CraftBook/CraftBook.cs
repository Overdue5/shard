
using System;
using System.Collections.Generic;
using Server.Gumps;
using System.Linq;
using Server.Network;

namespace Server.Items
{
    public partial class CraftBook : Item
    {
        private Mobile m_Student;
        private SkillName m_SkillToLearn;
        private Timer m_Timer;
        private int m_StudyTime;
        private int m_StamCost;
        private double m_MinSkill;
        private double m_MaxSkill;

        private static string m_cityName = "$City$";
        private static string m_SkillName = "$Skill$";
        private static string m_VendorName = "$Name$";
        private string m_NearestVendorName;
        private static int m_MaxVendorRange = 10;
        private static string m_UnknownBook = "Unknown book";
        private static Dictionary<SkillName, HashSet<CraftBook>> m_Books = new Dictionary<SkillName, HashSet<CraftBook>>();
        private static Dictionary<SkillName, string[]> m_BookNames = new Dictionary<SkillName, string[]>();
        private static List<CraftBook> m_AllCraftBooks = new List<CraftBook>();
        private static List<string> m_CraftBooksLocationWithName;
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
            var books = Books.Where(m => m.Map == vendor.Map && m.SkillToLearn == skill && m.IsActive && !String.IsNullOrEmpty(Region.Find(m.Location, m.Map).Name)).ToArray();
            if (!books.Any()) return;
            var  rnd_book = Utility.RandomList(books);
            var loc = "";
            if (String.IsNullOrEmpty( rnd_book.m_NearestVendorName))
            {
                loc = Utility.RandomList(m_CraftBooksLocation).Replace(m_cityName, Region.Find(rnd_book.Location, rnd_book.Map).Name).Replace(m_SkillName, skill.ToString());
            }
            else
            {
                loc = Utility.RandomList(m_CraftBooksLocationWithName).Replace(m_cityName, Region.Find( rnd_book.Location,  rnd_book.Map).Name).Replace(m_SkillName, skill.ToString()).Replace(m_VendorName,  rnd_book.m_NearestVendorName);
            }
                
            Timer.DelayCall(TimeSpan.FromMilliseconds(500), () =>
            {
                vendor.PublicOverheadMessage(MessageType.Emote, 1510, false, loc);
            });

        }

        [CommandProperty(AccessLevel.GameMaster)]
        public static List<CraftBook> Books
        {
            get { return m_AllCraftBooks.OrderBy(m => m.SkillToLearn).ToList(); }
            set => m_AllCraftBooks = value;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SkillName SkillToLearn
        {
            get => m_SkillToLearn;
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
            get => m_StudyTime;
            set
            {
                m_StudyTime = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int StamCost
        {
            get => m_StamCost;
            set
            {
                m_StamCost = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double MinSkill
        {
            get => m_MinSkill;
            set => m_MinSkill = value;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double MaxSkill
        {
            get => m_MaxSkill;
            set => m_MaxSkill = value;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Student
        {
            get => m_Student;
            set => m_Student = value;
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
            StamCost = 0;
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

        public bool CheckUsage(Mobile from)
        {
            if (Name == m_UnknownBook)
            {
                if (from.AccessLevel >= AccessLevel.Administrator)
                {
                    from.SendGump(new PropertiesGump(from, this));
                }
                else
                {
                    from.SendMessage($"You don't understand what this book is about.");
                }

                return false;
            }

            if (!from.Alive || from.Warmode)
            {
                from.SendMessage($"You don't have time to read now");
                return false;
            }

            if (from.GetDistanceToSqrt(Location) > 3 && Math.Abs(from.Location.Z - Location.Z) >= 5)
            {
                from.SendMessage($"You do not see what is written in the book, come closer");
                return false;
            }

            if (from.Mounted)
            {
                if (from.Mount is Mobile mob)
                    from.SendMessage($"You can't read, because the {mob.Name} keeps turning the page with its nose.");
                else if (from.Mount is Item item)
                    from.SendMessage($"You can't read, because the {item.Name} distracts you from reading.");
                return false;
            }

            if (from.Skills[this.SkillToLearn].Base < this.MinSkill)
            {
                from.SendMessage("You can't understand anything that's written in the book");
                return false;
            }

            if (from.Skills[this.SkillToLearn].Base > this.MaxSkill)
            {
                from.SendMessage("You already know everything in this book.");
                return false;
            }

            if (from.Stam <= 2 * this.StamCost)
            {
                string val = from.Female ? "madam" : "sir";
                from.SendMessage($"Thou art too fatigued to focus on reading, good {val}");
                return false;
            }

            if (from.Skills[SkillToLearn].Lock == SkillLock.Down)
            {
                from.SendMessage($"Are you sure that's how it works?");
                return false;
            }

            return true;
        }

        public override void OnDelete()
        {
            base.OnDelete();
            m_AllCraftBooks.Remove(this);
        }

        public override void OnDoubleClick(Mobile from)
        {
            FindDublicates(from);
            from.RevealingAction();
            from.Direction = from.GetDirectionTo(this.Location);
            if (CheckUsage(from))
            {
                UseCraftTrainer(from);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // version 
            writer.Write(m_NearestVendorName);
            writer.Write((int)m_SkillToLearn);
            writer.Write(m_StudyTime);
            writer.Write(m_StamCost);

            writer.Write(m_MinSkill);
            writer.Write(m_MaxSkill);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            var version = reader.ReadInt();
            switch (version)
            {
                case 1:
                {
                    m_NearestVendorName = reader.ReadString();
                    goto case 0;
                }
                case 0:
                {
                    m_SkillToLearn = (SkillName)Enum.ToObject(typeof(SkillName), reader.ReadInt());
                    m_StudyTime = reader.ReadInt();
                    m_StamCost = reader.ReadInt();

                    m_MinSkill = reader.ReadDouble();
                    m_MaxSkill = reader.ReadDouble();
                    m_AllCraftBooks.Add(this);
                    break;
                }
            }
        }

        public override string ToString()
        {
            return $"{this.SkillToLearn};X:{X};Y:{Y}:{this.Map}";
        }

    }
}