
using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Commands;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Server.Engines;
using static Server.Games.PaintBall.PBGameItem;

namespace Server.Items
{
    public class CraftBook : Item
    {
        private StudyTimer m_StudyTimer;
        private Mobile m_Student;
        private SkillName m_SkillToLearn;
        private Timer m_Timer;

        private int m_StudyTime;
        private int m_Ouch;
        private double m_MinSkill;
        private double m_MaxSkill;

        private static string m_UnknownBook = "Unknown book";

        private static Dictionary<SkillName, string[]> m_BookNames = new Dictionary<SkillName, string[]>();
        public DateTime LastUsage;

        public static void Initialize()
        {
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
                            string[] split = line.Split(':');
                            m_BookNames[(SkillName)Enum.Parse(typeof(SkillName), split[0], true)] = split[1].Split(',');
                        }
                        catch(Exception e)
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

        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SkillName SkillToLearn { get { return m_SkillToLearn; } set { m_SkillToLearn = value;
            Name = GetName(value); InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int StudyTime { get { return m_StudyTime; } set { m_StudyTime = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Ouch { get { return m_Ouch; } set { m_Ouch = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public double MinSkill { get { return m_MinSkill; } set { m_MinSkill = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public double MaxSkill { get { return m_MaxSkill; } set { m_MaxSkill = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Student { get { return m_Student; } set { m_Student = value; } }


        [Constructable]
        public CraftBook() : base(0x1E20)
        {
            Name = m_UnknownBook;
            Movable = true;
            LootType = LootType.Blessed;
            Weight = 5;
            StudyTime = 3;
            Ouch = 0;
            MinSkill = 30.0;
            MaxSkill = 80.0;
        }

        public CraftBook(Serial serial) : base(serial)
        {
        }

        public static string GetName(SkillName skill)
        {
            if (m_BookNames.Keys.Contains(skill))
                return m_BookNames[skill][Utility.Random(m_BookNames[skill].Length)];
            return m_UnknownBook;
        }

        public void UseCraftTrainer(Mobile from)
        {
            if (Student == null || this.LastUsage >= DateTime.UtcNow + TimeSpan.FromSeconds(this.StudyTime*2))
            {
                if (m_Timer!= null && m_Timer.Running)
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
                m_From.SendMessage($"You don't understand what this book is about.");
                if (m_From.AccessLevel >= AccessLevel.Administrator)
                {
                    m_From.SendGump(new PropertiesGump(m_From, this));
                }
                return false;
            }

            if (!m_From.Alive || m_From.Warmode)
            {
                m_From.SendMessage($"You don't have time to read now");
                return false;
            }

            if (m_From.GetDistanceToSqrt(Location)>=2 && Math.Abs(m_From.Location.Z-Location.Z)>=5)
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

            if (!UtilityWorldTime.IsLight(m_From))
            {
                m_From.SendMessage($"It's too dark, you can't make out the words");
                return false;
            }

            return true;
        }

        public override void OnDoubleClick(Mobile m_From)
        {
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
        }
    }

    public class StudyTimer : Timer
    {
        private int m_start_x, m_start_y, m_hits;
        private Direction m_direction;
        static string [] ok = new[]
        {
            "As you gain knowledge, your mind expands like a balloon.",
            "A flash of insight illuminates your mind like a bolt of lightning.",
            "Your understanding grows as if a fog has lifted from your mind.",
            "The mysteries of the world unfold before you like a blossoming flower.",
            "You feel a surge of enlightenment, as if a switch has been flipped in your brain."
        };
        static string [] nok = new[]
        {
            "You can't seem to figure this one out. Try again later.",
            "No matter how hard you try, this just isn't making sense.",
            "You're feeling a bit dull today and can't quite grasp the concept.",
            "This one's a toughie, maybe you need some rest before trying again.",
            "You scratch your head in confusion, this is beyond your current knowledge.",
        };
        private Mobile m_From;
        private CraftBook m_Book;

        public StudyTimer(Mobile from, CraftBook craftBook) : base(TimeSpan.FromSeconds(craftBook.StudyTime), TimeSpan.FromSeconds(craftBook.StudyTime), 0)
        {
            m_From = from;
            m_start_x = m_From.Location.X;
            m_start_y = m_From.Location.Y;
            m_direction = m_From.Direction;
            m_hits = m_From.Hits;
            m_Book = craftBook;
            m_Book.LastUsage = DateTime.UtcNow;
            if (from.Body.IsHuman && !from.Mounted)
                from.Animate(34, 15, 1, true, true, 1);
            from.PlaySound(0x249);
        }

        protected override void OnTick()
        {
            if ((m_direction == m_From.Direction && m_start_x == m_From.Location.X && m_start_y == m_From.Location.Y && m_hits <= m_From.Hits && !m_From.Hidden && !m_From.Meditating) && m_Book.CheckUsage(m_From))
            {
                m_Book.LastUsage = DateTime.UtcNow;
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
                if (m_From.Body.IsHuman && !m_From.Mounted)
                    m_From.Animate(34, 15, 1, true, true, 1);
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