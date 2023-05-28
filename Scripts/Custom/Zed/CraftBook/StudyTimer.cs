using System;
using Server.Commands.GMUtils;
using Server.Engines;

namespace Server.Items
{
    public class StudyTimer : Timer
    {
        private int m_Hits;
        private DateTime m_MoveTime;
        private Direction m_Direction;

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

            m_MoveTime = m_From.LastMoveTime;
            m_Direction = m_From.Direction;
            m_Hits = m_From.Hits;
            m_Book = craftBook;
            m_Book.LastUsage = DateTime.UtcNow;
            if (from.Body.IsHuman && !from.Mounted)
                from.Animate(34, 15, 1, true, false, 1);
            PrintAction();
            from.PlaySound(0x249);
        }

        protected override void OnTick()
        {
            if (m_Book == null || m_Book.Deleted)
            {
                Stop();
            }

            if ((m_Direction == m_From.Direction && m_MoveTime == m_From.LastMoveTime && m_Hits <= m_From.Hits && !m_From.Hidden &&
                 !m_From.Meditating) && m_Book.CheckUsage(m_From))
            {
                m_Book.LastUsage = DateTime.UtcNow;
                if (UtilityWorldTime.IsDark(m_From) && Utility.RandomDouble() > 0.25)
                {
                    m_From.SendMessage($"It's too dark, you can hardly read the text");
                }
                else
                {
                    m_From.Stam -= Utility.LimitMinMax(m_Book.StamCost, (int)m_From.Skills[m_Book.SkillToLearn].Base / 20, 2 * m_Book.StamCost);
                    int str = m_From.Str;
                    int dex = m_From.Dex;
                    m_Hits = m_From.Hits;
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

                    if (m_From.Str != str)
                    {
                        m_From.Str = str;
                    }

                    if (m_From.Dex != dex)
                    {
                        m_From.Dex = dex;
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
