namespace Server.Items
{
	[Flipable( 0x13F8, 0x13F9 )]
	public class GnarledStaff : BaseStaff
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ConcussionBlow; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ParalyzingBlow; } }

		public override int AosStrengthReq => 20;
        public override int AosMinDamage => 15;
        public override int AosMaxDamage => 17;
        public override int AosSpeed => 33;

        public override int OldStrengthReq => 20;
        public override int OldMinDamage => 14;
        public override int OldMaxDamage => 33;
        public override int OldSpeed => 392;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 50;

        [Constructable]
		public GnarledStaff() : base( 0x13F8 )
		{
			Weight = 3.0;
			//Name = "gnarled staff";
		}

		public GnarledStaff( Serial serial ) : base( serial )
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