namespace Server.Items
{
	[Flipable( 0x26BE, 0x26C8 )]
	public class Pike : BaseSpear
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ParalyzingBlow; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.InfectiousStrike; } }

		public override int AosStrengthReq => 50;
        public override int AosMinDamage => 14;
        public override int AosMaxDamage => 16;
        public override int AosSpeed => 37;

        public override int OldStrengthReq => 50;
        public override int OldMinDamage => 14;
        public override int OldMaxDamage => 16;
        public override int OldSpeed => 383;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 110;

        public override int GetSwingAnim(Mobile from)
	        {
        	    if (from.Mounted)
                	return 26;
	            else
        	        return Utility.RandomList(9, 10);
	        }

		[Constructable]
		public Pike() : base( 0x26BE )
		{
			Weight = 8.0;
			//Name = "pike";
		}

		public Pike( Serial serial ) : base( serial )
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