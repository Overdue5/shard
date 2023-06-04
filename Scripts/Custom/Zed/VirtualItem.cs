using System;

namespace Server.Custom.Zed
{
    public class VirtualItem : Item
    {
        private Timer m_Timer;
        private DateTime m_End;

        public VirtualItem(int itemId) : base(itemId)
        {
            Visible = true;
            Movable = false;
            m_End = DateTime.MinValue;
        }


        public VirtualItem(Point3D loc, Map map, TimeSpan timeSpan) : base(0x80)
        {
            Visible = true;
            Movable = false;

            MoveToWorld(loc, map);

            m_Timer = new InternalTimer(this, timeSpan);
            m_Timer.Start();
            m_End = DateTime.UtcNow + timeSpan;
        }

        public VirtualItem(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version

            writer.WriteDeltaTime(m_End);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            m_End = reader.ReadDeltaTime();
            if (m_End<DateTime.UtcNow)
                Delete();
            else
            {
                m_Timer = new InternalTimer(this, m_End - DateTime.UtcNow);
                m_Timer.Start();
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();
            m_Timer?.Stop();
        }

        private class InternalTimer : Timer
        {
            private readonly VirtualItem m_Item;
            public InternalTimer(VirtualItem item, TimeSpan duration) : base(duration)
            {
                Priority = TimerPriority.OneSecond;
                m_Item = item;
            }
            protected override void OnTick()
            {
                m_Item.Delete();
            }
        }
    }
}
