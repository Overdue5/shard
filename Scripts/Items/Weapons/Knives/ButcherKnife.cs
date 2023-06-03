namespace Server.Items
{
	[Flipable( 0x13F6, 0x13F7 )]
	public class ButcherKnife : BaseKnife
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.InfectiousStrike; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.Disarm; } }

		public override int AosStrengthReq => 5;
        public override int AosMinDamage => 9;
        public override int AosMaxDamage => 11;
        public override int AosSpeed => 49;

        public override int OldStrengthReq => 5;
        public override int OldMinDamage => 2;
        public override int OldMaxDamage => 5;
        public override int OldSpeed => 200;

        public override int InitMinHits => 51;
        public override int InitMaxHits => 60;

        [Constructable]
		public ButcherKnife() : base( 0x13F6 )
		{
			Weight = 1.0;
			//Name = "butcher knife";
		}

		public ButcherKnife( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}