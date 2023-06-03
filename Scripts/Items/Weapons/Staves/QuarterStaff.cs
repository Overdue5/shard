namespace Server.Items
{
	[Flipable( 0xE89, 0xE8a )]
	public class QuarterStaff : BaseStaff
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.DoubleStrike; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ConcussionBlow; } }

		public override int AosStrengthReq => 30;
        public override int AosMinDamage => 11;
        public override int AosMaxDamage => 14;
        public override int AosSpeed => 48;

        public override int OldStrengthReq => 30;
        public override int OldMinDamage => 9;
        public override int OldMaxDamage => 35;
        public override int OldSpeed => 355;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 60;

        [Constructable]
		public QuarterStaff() : base( 0xE89 )
		{
			Weight = 4.0;
			//Name = "quarter staff";
		}

		public QuarterStaff( Serial serial ) : base( serial )
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