using System;
using System.Collections;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class TurboSlotStats : Item
    {
        public enum PaybackType { Loose, Normal, Tight, ExtremelyTight, CasinoCheats, Random }
        private readonly TimeSpan m_UpdateTimer = TimeSpan.FromHours(24); // Minimum time to regenerate the slot array
        private DateTime m_LastBuild = DateTime.UtcNow;
        private TimeSpan m_TimeOut;
        private bool m_refresh = false;
        ArrayList itemarray = null;
        readonly ArrayList turboslotsarray = new ArrayList();

        [CommandProperty(AccessLevel.GameMaster)]
        public bool RefreshSlotList
        {
            get => m_refresh;
            set {
                if (value)
                    BuildArrayList(null);
                m_refresh = false;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan NextRefresh => (m_UpdateTimer - (DateTime.UtcNow - m_LastBuild));

        [Constructable]
        public TurboSlotStats()
            : base(8977)
        {
            Movable = false;
            Hue = 56;
            Name = "TurboSlots Top Ten";
        }

        public override void OnDoubleClick(Mobile from)
        {
            if ((!from.InRange(GetWorldLocation(), 2) || !from.InLOS(this)) && from.AccessLevel == AccessLevel.Player)
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                return;
            }
            m_TimeOut = DateTime.UtcNow - m_LastBuild;
            if (m_UpdateTimer < m_TimeOut || itemarray == null)
                BuildArrayList(from);
            else if (turboslotsarray != null)
            {
                foreach (TurboSlot t in turboslotsarray)
                {
                    if ((t == null || t.Deleted) )
                    {
                        BuildArrayList(from);
                        break;
                    }
                }
            }
            if (turboslotsarray != null)
            {
                from.CloseGump(typeof(TurboSlotsStatGump));
                from.SendGump(new TurboSlotsStatGump(from, turboslotsarray));
            }
        }

        public void BuildArrayList(Mobile from)
        {
            try
            {
                itemarray = new ArrayList(World.Items.Values);
                m_LastBuild = DateTime.UtcNow;
                if (turboslotsarray == null || turboslotsarray.Count > 0)
                    turboslotsarray.Clear();
                foreach (Item i in itemarray)
                {
                    if (i is TurboSlot && !i.Deleted && i != null)
                        turboslotsarray.Add(i);
                }
            }
            catch { if (from != null) from.SendMessage("Unable to search World.Items"); }
        }

        public TurboSlotStats(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}