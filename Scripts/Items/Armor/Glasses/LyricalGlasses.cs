namespace Server.Items
{
	public class LyricalGlasses : ElvenGlasses
	{
		public override int LabelNumber => 1073382; //Lyrical Reading Glasses

		public override int BasePhysicalResistance => 10;
        public override int BaseFireResistance => 10;
        public override int BaseColdResistance => 10;
        public override int BasePoisonResistance => 10;
        public override int BaseEnergyResistance => 10;

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        //private AosWeaponAttributes m_AosWeaponAttributes;

		[Constructable]
		public LyricalGlasses()
		{
			WeaponAttributes.HitLowerDefend = 20;
			Attributes.NightSight = 1;
			Attributes.ReflectPhysical = 15;
		}
		public LyricalGlasses( Serial serial ) : base( serial )
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
