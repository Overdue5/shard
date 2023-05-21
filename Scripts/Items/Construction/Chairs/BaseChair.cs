using System;
using System.Collections.Generic;

namespace Server.Items.Construction.Chairs
{
    public class BaseChair:Item
    {
        private Timer m_Timer;
        public HashSet<Mobile> SitMobile;
        public BaseChair(int id) : base(id)
        {
            
        } 
        public BaseChair(Serial serial) : base(serial)
        {
            
        }

        public override bool OnMoveOff(Mobile m)
        {
            if (!m.Alive || m.Mounted || !m.Body.IsHuman || !m.Player)
                return true;
            if (SitMobile != null && SitMobile.Contains(m))
                SitMobile.Remove(m);
            if ((SitMobile == null || SitMobile.Count==0) && m_Timer!=null && m_Timer.Running)
                m_Timer.Stop();
            return base.OnMoveOff(m);
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (!m.Alive || m.Mounted || !m.Body.IsHuman || !m.Player)
                return true;
            if (SitMobile == null)
                SitMobile = new HashSet<Mobile>();
            SitMobile.Add(m);
            if (m_Timer == null || (m_Timer != null && !m_Timer.Running))
            {
                m_Timer = new SittingTimer(this);
                m_Timer.Start();
            }
            
            return base.OnMoveOver(m);
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            base.OnMovement(m, oldLocation);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }

        public class SittingTimer : Timer
        {
            private static TimeSpan m_waitTimer = TimeSpan.FromSeconds(5);
            private static TimeSpan m_usageTimer = TimeSpan.FromSeconds(1);
            private struct StatPlus
            {
                public int Hits;
                public int Stam;
                public int Mana;

                public StatPlus(int hits, int stam, int mana)
                {
                    Hits = hits;
                    Stam = stam;
                    Mana = mana;
                }

            }

            private static HashSet<Mobile> m_Delete = new HashSet<Mobile>();
            private BaseChair m_Chair;
            private static Dictionary<int, StatPlus> Stat = new Dictionary<int, StatPlus>()
            {
                {0, new StatPlus(0,1,0)},
                { 0x7DA , new StatPlus(0,2,0)},
                { 1755 , new StatPlus(0,3,0)},
                { 1756 , new StatPlus(0,4,0)},
                { 1757 , new StatPlus(1,2,0)},
                { 1758 , new StatPlus(2,2,0)},
                { 1761 , new StatPlus(1,2,1)},
                { 1551 , new StatPlus(1,3,2)},
                { 1172 , new StatPlus(2,2,1)},
                { 1944 , new StatPlus(2,2,2)},
                { 1952 , new StatPlus(4,4,4)},
            };

            public SittingTimer(BaseChair chair) : base(m_usageTimer, m_usageTimer, 0)
            {
                m_Chair = chair;
            }

            protected override void OnTick()
            {
                foreach (Mobile mob in m_Chair.SitMobile)
                {
                    if (mob.Location == m_Chair.Location)
                        UpdateStats(m_Chair, mob, this);
                    else
                    {
                        m_Delete.Add(mob);
                    }
                }
                
                foreach (var mob in m_Delete)
                {
                    m_Chair.SitMobile.Remove(mob);
                }
                m_Delete.Clear();
            }

            private static void UpdateStats(BaseChair chair, Mobile mob, Timer timer)
            {
                var stat = SittingTimer.Stat[0];
                if (SittingTimer.Stat.ContainsKey(chair.Hue))
                    stat = SittingTimer.Stat[chair.Hue];
                bool usage = false;
                
                if (stat.Hits > 0 && mob.Hits < mob.HitsMax)
                {
                    mob.Hits += stat.Hits;
                    usage = true;
                }

                if (stat.Stam > 0 && mob.Stam < mob.StamMax)
                {
                    mob.Stam += stat.Stam;
                    usage = true;
                }

                if (stat.Mana > 0 && mob.Mana < mob.ManaMax)
                {
                    mob.Mana += stat.Mana;
                    usage = true;
                }

                if (usage)
                {
                    timer.Interval = m_usageTimer;
                    mob.FixedParticles(0x376A, 10, 15, 5007, EffectLayer.Waist);
                    //Effects.PlaySound(chair.Location, chair.Map, 0x3BD);
                }
                else
                {
                    timer.Interval = m_waitTimer;
                }
            }

        }
    }
}
