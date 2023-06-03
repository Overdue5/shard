namespace Server.Items
{
	[Flipable( 0x26BF, 0x26C9 )]
	public class DoubleBladedStaff : BaseSpear
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.DoubleStrike; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.InfectiousStrike; } }

		public override int AosStrengthReq => 50;
        public override int AosMinDamage => 12;
        public override int AosMaxDamage => 13;
        public override int AosSpeed => 49;

        public override int OldStrengthReq => 50;
        public override int OldMinDamage => 12;
        public override int OldMaxDamage => 13;
        public override int OldSpeed => 314;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 80;

        public override int GetSwingAnim(Mobile from)
	        {
        	    if (from.Mounted)
                	return 26;
	            else
        	        return Utility.RandomList(12, 13);
	        }

		[Constructable]
		public DoubleBladedStaff() : base( 0x26BF )
		{
			Weight = 2.0;
			//Name = "double bladed staff";
		}

		public DoubleBladedStaff( Serial serial ) : base( serial )
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