using Server.Engines.Harvest;

namespace Server.Items
{
	[Flipable( 0x26BA, 0x26C4 )]
	public class Scythe : BasePoleArm
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.BleedAttack; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ParalyzingBlow; } }

		public override int AosStrengthReq => 45;
        public override int AosMinDamage => 15;
        public override int AosMaxDamage => 18;
        public override int AosSpeed => 32;

        public override int OldStrengthReq => 45;
        public override int OldMinDamage => 25;
        public override int OldMaxDamage => 39;
        public override int OldSpeed => 475;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 100;

        public override HarvestSystem HarvestSystem => null;

        [Constructable]
		public Scythe() : base( 0x26BA )
		{
			Weight = 5.0;
			//Name = "scythe";
		}

		public Scythe( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            if (version == 0)
            {
                MinDamage = 25;
                MaxDamage = 39;
            }
		}
	}
}