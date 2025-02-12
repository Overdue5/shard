namespace Server.Items
{
	[Flipable( 0x1410, 0x1417 )]
	public class PlateArms : BaseArmor
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

        public override ArmorMaterialType MaterialType => ArmorMaterialType.Plate;

        [Constructable]
		public PlateArms() : base( 0x1410 )
		{
			Weight = 5.0;
		    BaseArmorRating = 28;
		}

		public PlateArms( Serial serial ) : base( serial )
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