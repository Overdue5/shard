using Server.Engines.Harvest;

namespace Server.Items
{
    [Flipable(0xE86, 0xE85)]
    public class Pickaxe : BaseAxe
    {
        public override HarvestSystem HarvestSystem => Mining.System;
        public override bool ShowContextMenu => true;

        ////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.DoubleStrike; } }
        //public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.Disarm; } }

        public override int AosStrengthReq => 50;
        public override int AosMinDamage => 13;
        public override int AosMaxDamage => 15;
        public override int AosSpeed => 35;

        public override int OldStrengthReq => 25;
        public override int OldMinDamage => 1;
        public override int OldMaxDamage => 15;
        public override int OldSpeed => 400;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 60;

        public override int DefHitSound => 0x232;
        public override int DefMissSound => Utility.RandomList(0x238, 0x239, 0x23A);

        public override int GetSwingAnim(Mobile from)
        {
            if (from.Mounted)
                return 26;
            else
                return Utility.RandomList(12, 13, 14);
        }

        [Constructable]
        public Pickaxe()
            : base(0xE86)
        {
            Weight = 11.0;
            UsesRemaining = 50;
            ShowUsesRemaining = true;
            //Name = "pickaxe";
        }

        public Pickaxe(Serial serial)
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
            ShowUsesRemaining = true;
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (HarvestSystem == null)
                return;

            double check = Utility.RandomDouble();

            if (IsChildOf(from.Backpack) || Parent == from)
            {
                HarvestSystem.BeginHarvesting(from, this);
            }
            else
            {
                from.SendAsciiMessage("That must be in your pack for you to use it.");
            }
        }
    }
}