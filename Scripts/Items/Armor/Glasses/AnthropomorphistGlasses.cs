namespace Server.Items
{
	public class AnthropomorphistGlasses : ElvenGlasses
	{
		public override int LabelNumber => 1073379; //Anthropomorphist Reading Glasses

		public override int BasePhysicalResistance => 5;
        public override int BaseFireResistance => 5;
        public override int BaseColdResistance => 10;
        public override int BasePoisonResistance => 20;
        public override int BaseEnergyResistance => 20;

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;


        [Constructable]
		public AnthropomorphistGlasses()
		{
			Attributes.BonusHits = 5;
			Attributes.RegenMana = 3;
			Attributes.ReflectPhysical = 20;
		}
		public AnthropomorphistGlasses( Serial serial ) : base( serial )
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
