namespace Server.Items
{
	[Flipable( 0x1443, 0x1442 )]
	public class TwoHandedAxe : BaseAxe
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.DoubleStrike; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ShadowStrike; } }

		public override int AosStrengthReq => 40;
        public override int AosMinDamage => 16;
        public override int AosMaxDamage => 17;
        public override int AosSpeed => 31;

        public override int OldStrengthReq => 35;
        public override int OldMinDamage => 19;
        public override int OldMaxDamage => 41; //Loki edit: was 15 - 38
		public override int OldSpeed => 430;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 90;

        [Constructable]
		public TwoHandedAxe() : base( 0x1443 )
		{
			Weight = 8.0;
			//Name = "two handed axe";
		}

		public TwoHandedAxe( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 2 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            if (version < 2)
            {
                MinDamage = 19;
                MaxDamage = 41;
            }
		}
	}
}