namespace Server.Items
{
	[Flipable( 0xE87, 0xE88 )]
	public class Pitchfork : BaseSpear
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.BleedAttack; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.Dismount; } }

		public override int AosStrengthReq => 55;
        public override int AosMinDamage => 13;
        public override int AosMaxDamage => 14;
        public override int AosSpeed => 43;

        public override int OldStrengthReq => 15;
        public override int OldMinDamage => 4;
        public override int OldMaxDamage => 16;
        public override int OldSpeed => 333;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 60;

        public override int GetSwingAnim(Mobile from)
	        {
        	    if (from.Mounted)
                	return 29;
	            else
        	        return Utility.RandomList(12, 13, 14);
	        }

		[Constructable]
		public Pitchfork() : base( 0xE87 )
		{
			Weight = 11.0;
			//Name = "pitchfork";
		}

		public Pitchfork( Serial serial ) : base( serial )
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

			if ( Weight == 10.0 )
				Weight = 11.0;
		}
	}
}