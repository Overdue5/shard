namespace Server.Items
{
	[Flipable( 0xf4b, 0xf4c )]
	public class DoubleAxe : BaseAxe
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.DoubleStrike; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.WhirlwindAttack; } }

		public override int AosStrengthReq => 45;
        public override int AosMinDamage => 15;
        public override int AosMaxDamage => 17;
        public override int AosSpeed => 33;

        public override int OldStrengthReq => 45;
        public override int OldMinDamage => 19;
        public override int OldMaxDamage => 36; //Loki edit: Was 15 - 34
		public override int OldSpeed => 401;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 110;

        public override int GetSwingAnim(Mobile from)
	        {
        	    if (from.Mounted)
                	return 29;
	            else
        	        return Utility.RandomList(13, 14);
	        }

		[Constructable]
		public DoubleAxe() : base( 0xF4B )
		{
			Weight = 8.0;
			//Name = "double axe";
		}

		public DoubleAxe( Serial serial ) : base( serial )
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
                MaxDamage = 36;
            }
		}
	}
}