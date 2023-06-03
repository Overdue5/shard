namespace Server.Items
{
	public class TreasureTrinketGlasses : ElvenGlasses
	{
		public override int LabelNumber => 1073373; //Treasures and Trinkets Reading Glasses

		public override int BasePhysicalResistance => 10;
        public override int BaseFireResistance => 10;
        public override int BaseColdResistance => 10;
        public override int BasePoisonResistance => 10;
        public override int BaseEnergyResistance => 10;

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
		public TreasureTrinketGlasses()
		{
			Attributes.BonusInt = 10;
			Attributes.BonusHits = 5;
			Attributes.SpellDamage = 10;
		}
		public TreasureTrinketGlasses( Serial serial ) : base( serial )
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
