namespace Server.Items
{
	[Flipable( 0x13B6, 0x13B5 )]
	public class Scimitar : BaseSword
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.DoubleStrike; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ParalyzingBlow; } }

		public override int AosStrengthReq => 25;
        public override int AosMinDamage => 13;
        public override int AosMaxDamage => 15;
        public override int AosSpeed => 37;

        public override int OldStrengthReq => 10;
        public override int OldMinDamage => 12;
        public override int OldMaxDamage => 25;
        public override int OldSpeed => 342;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 90;

        [Constructable]
		public Scimitar() : base( 0x13B6 )
		{
			Weight = 5.0;
			//Name = "scimitar";
		}

		public Scimitar( Serial serial ) : base( serial )
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
                MinDamage = 12;
                MaxDamage = 25;
            }
		}
	}
}