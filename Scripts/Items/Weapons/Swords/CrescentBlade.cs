namespace Server.Items
{
	[Flipable( 0x26C1, 0x26CB )]
	public class CrescentBlade : BaseSword
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.DoubleStrike; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.MortalStrike; } }

		public override int AosStrengthReq => 55;
        public override int AosMinDamage => 11;
        public override int AosMaxDamage => 14;
        public override int AosSpeed => 47;

        public override int OldStrengthReq => 55;
        public override int OldMinDamage => 11;
        public override int OldMaxDamage => 14;
        public override int OldSpeed => 323;

        public override int InitMinHits => 51;
        public override int InitMaxHits => 80;

        [Constructable]
		public CrescentBlade() : base( 0x26C1 )
		{
			Weight = 1.0;
			//Name = "crescented blade";
		}

		public CrescentBlade( Serial serial ) : base( serial )
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