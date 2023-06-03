namespace Server.Items
{
	[Flipable( 0x1439, 0x1438 )]
	public class WarHammer : BaseBashing
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.WhirlwindAttack; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.CrushingBlow; } }

		public override int AosStrengthReq => 95;
        public override int AosMinDamage => 17;
        public override int AosMaxDamage => 18;
        public override int AosSpeed => 28;

        public override int OldStrengthReq => 40;
        public override int OldMinDamage => 29;
        public override int OldMaxDamage => 47;
        public override int OldSpeed => 618;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 110;

        //public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Bash2H; } }

        	public override int GetSwingAnim(Mobile from)
	        {
        	    if (from.Mounted)
                	return 29;
	            else
        	        return Utility.RandomList(12, 13, 14);
	        }

		[Constructable]
		public WarHammer() : base( 0x1439 )
		{
			Weight = 10.0;
			Layer = Layer.TwoHanded;
			//Name = "war hammer";
		}

		public WarHammer( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            if (version == 0)
            {
                MinDamage = 29;
                MaxDamage = 47;
            }
		}
	}
}