namespace Server.Items
{
	[Flipable( 0x1405, 0x1404 )]
	public class WarFork : BaseSpear
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.BleedAttack; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.Disarm; } }

		public override int AosStrengthReq => 45;
        public override int AosMinDamage => 12;
        public override int AosMaxDamage => 13;
        public override int AosSpeed => 43;

        public override int OldStrengthReq => 35;
        public override int OldMinDamage => 16;
        public override int OldMaxDamage => 27;
        public override int OldSpeed => 390;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 110;

        //public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Pierce1H; } }

        	public override int GetSwingAnim(Mobile from)
	        {
        	    if (from.Mounted)
                	return 26;
	            else
        	        return Utility.RandomList(9, 10);
	        }

		[Constructable]
		public WarFork() : base( 0x1405 )
		{
			Weight = 9.0;
			//Name = "war fork";
		}

		public WarFork( Serial serial ) : base( serial )
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