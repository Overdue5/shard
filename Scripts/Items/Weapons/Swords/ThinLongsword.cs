namespace Server.Items
{
	[Flipable( 0x13B8, 0x13B7 )]
	public class ThinLongsword : BaseSword
	{
		public override int AosStrengthReq => 35;
        public override int AosMinDamage => 15;
        public override int AosMaxDamage => 16;
        public override int AosSpeed => 30;

        public override int OldStrengthReq => 25;
        public override int OldMinDamage => 8;
        public override int OldMaxDamage => 34;
        public override int OldSpeed => 396;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 110;

        [Constructable]
		public ThinLongsword() : base( 0x13B8 )
		{
			Weight = 1.0;
			//Name = "thin longsword";
		}

		public ThinLongsword( Serial serial ) : base( serial )
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