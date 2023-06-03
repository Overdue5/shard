namespace Server.Items
{
	[Flipable( 0x1403, 0x1402 )]
	public class ShortSpear : BaseSpear
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ShadowStrike; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.MortalStrike; } }

		public override int AosStrengthReq => 40;
        public override int AosMinDamage => 10;
        public override int AosMaxDamage => 13;
        public override int AosSpeed => 55;

        public override int OldStrengthReq => 15;
        public override int OldMinDamage => 15;
        public override int OldMaxDamage => 29; //Loki edit: was 10-28
		public override int OldSpeed => 306;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 70;

        //public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Pierce1H; } }

        	public override int GetSwingAnim(Mobile from)
	        {
        	    if (from.Mounted)
                	return 26;
	            else
        	        return Utility.RandomList(11, 12, 13);
	        }

		[Constructable]
		public ShortSpear() : base( 0x1403 )
		{
			Weight = 4.0;
			//Name = "short spear";
		}

		public ShortSpear( Serial serial ) : base( serial )
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
                MinDamage = 15;
                MaxDamage = 29;
            }
		}
	}
}