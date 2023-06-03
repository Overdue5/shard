namespace Server.Items
{
    [Flipable]
	public class RadioactiveGloves : BaseArmor
	{
        public override int InitMinHits => 50;
        public override int InitMaxHits => 60;

        public override int AosStrReq => 20;
        public override int OldStrReq => 10;

        public override int ArmorBase => 40;

        public override ArmorMaterialType MaterialType => ArmorMaterialType.Leather;
        public override CraftResource DefaultResource => CraftResource.RegularLeather;

        [Constructable]
		public RadioactiveGloves() : base (0x13C6)
		{
            Weight = 1.0;
            Hue = 1957;
            Name = "Radioactive Gloves";
            BaseArmorRating = 40;
            IsRenamed = true;
		}

		public RadioactiveGloves( Serial serial ) : base( serial )
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