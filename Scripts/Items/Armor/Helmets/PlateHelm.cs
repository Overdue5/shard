namespace Server.Items
{
	public class PlateHelm : BaseArmor
	{
		public override int BasePhysicalResistance => 5;
        public override int BaseFireResistance => 3;
        public override int BaseColdResistance => 2;
        public override int BasePoisonResistance => 3;
        public override int BaseEnergyResistance => 2;

        public override int InitMinHits => 50;
        public override int InitMaxHits => 65;

        public override int AosStrReq => 80;
        public override int OldStrReq => 40;

        public override int OldDexBonus => 0;

        public override int ArmorBase => 40;

        public override ArmorMaterialType MaterialType => ArmorMaterialType.Plate;

        [Constructable]
		public PlateHelm() : base( 0x1412 )
		{
            Name = "Platemail helm";
			Weight = 5.0;
		    BaseArmorRating = 28;
		}

		public PlateHelm( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            if (BaseArmorRating == 32)
                BaseArmorRating = 28;
		}
	}
}