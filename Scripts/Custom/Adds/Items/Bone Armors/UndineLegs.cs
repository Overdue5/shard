namespace Server.Items
{
    [Flipable(0x1452, 0x1457)]
	public class UndineLegs : BaseArmor
	{
		public override int BasePhysicalResistance => 5;
        public override int BaseFireResistance => 4;
        public override int BaseColdResistance => 3;
        public override int BasePoisonResistance => 3;
        public override int BaseEnergyResistance => 3;

        public override int InitMinHits => 60;
        public override int InitMaxHits => 70;

        public override int AosStrReq => 20;
        public override int OldStrReq => 10;

        public override int ArmorBase => 45;

        public override ArmorMaterialType MaterialType => ArmorMaterialType.Bone;
        public override CraftResource DefaultResource => CraftResource.RegularLeather;

        [Constructable]
		public UndineLegs() : base( 0x1452 )
		{
			Weight = 4.0;
            Name = "Undine bone leggings";
            Hue = 2515;
            BaseArmorRating = 45;
		}

		public UndineLegs( Serial serial ) : base( serial )
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