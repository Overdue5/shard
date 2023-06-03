namespace Server.Items
{
	[Flipable( 0x13FF, 0x13FE )]
	public class Katana : BaseSword
	{
        //public override WeaponAbility PrimaryAbility { get { return WeaponAbility.ArmorIgnore; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ArmorIgnore; } }

		public override int AosStrengthReq => 25;
        public override int AosMinDamage => 11;
        public override int AosMaxDamage => 13;
        public override int AosSpeed => 46;

        public override int OldStrengthReq => 10;
        public override int OldMinDamage => 13;
        public override int OldMaxDamage => 24;
        public override int OldSpeed => 331;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 90;

        [Constructable]
		public Katana() : base( 0x13FF )
		{
			Weight = 6.0;
			//Name = "katana";
		}

		public Katana( Serial serial ) : base( serial )
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