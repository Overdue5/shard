namespace Server.Items
{
	public class TradeGlasses : ElvenGlasses
	{
		public override int LabelNumber => 1073362; //Reading Glasses of the Trades

		public override int BasePhysicalResistance => 10;
        public override int BaseFireResistance => 10;
        public override int BaseColdResistance => 10;
        public override int BasePoisonResistance => 10;
        public override int BaseEnergyResistance => 10;

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
		public TradeGlasses()
		{
			Attributes.BonusStr = 10;
			Attributes.BonusInt = 10;
		}
		public TradeGlasses( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
