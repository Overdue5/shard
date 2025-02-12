namespace Server.Items
{
	public class NecromanticGlasses : ElvenGlasses
	{
		public override int LabelNumber => 1073377; //Necromantic Reading Glasses

		public override int BasePhysicalResistance => 0;
        public override int BaseFireResistance => 0;
        public override int BaseColdResistance => 0;
        public override int BasePoisonResistance => 0;
        public override int BaseEnergyResistance => 0;

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
		public NecromanticGlasses()
		{
			Attributes.LowerManaCost = 15;
			Attributes.LowerRegCost = 30;
		}
		public NecromanticGlasses( Serial serial ) : base( serial )
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
