namespace Server.Items
{
	public class PlateHaidate : BaseArmor
	{
		public override int BasePhysicalResistance => 5;
        public override int BaseFireResistance => 3;
        public override int BaseColdResistance => 2;
        public override int BasePoisonResistance => 3;
        public override int BaseEnergyResistance => 2;

        public override int InitMinHits => 55;
        public override int InitMaxHits => 65;

        public override int AosStrReq => 80;
        public override int OldStrReq => 80;

        public override int ArmorBase => 3;

        public override ArmorMaterialType MaterialType => ArmorMaterialType.Plate;

        [Constructable]
		public PlateHaidate() : base( 0x278D )
		{
			Weight = 7.0;
		}

		public PlateHaidate( Serial serial ) : base( serial )
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
		}
	}
}