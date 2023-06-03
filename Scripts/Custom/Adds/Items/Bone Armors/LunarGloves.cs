namespace Server.Items
{
    [Flipable(0x1450, 0x1455)]
	public class LunarGloves : BaseArmor
	{
		public override int BasePhysicalResistance => 5;
        public override int BaseFireResistance => 4;
        public override int BaseColdResistance => 3;
        public override int BasePoisonResistance => 3;
        public override int BaseEnergyResistance => 3;

        public override int InitMinHits => 50;
        public override int InitMaxHits => 60;

        public override int AosStrReq => 20;
        public override int OldStrReq => 10;

        public override int ArmorBase => 45;

        public override ArmorMaterialType MaterialType => ArmorMaterialType.Leather;
        public override CraftResource DefaultResource => CraftResource.RegularLeather;

        [Constructable]
		public LunarGloves() : base( 0x1450 )
		{
			Weight = 1.0;
            Name = "Lunar Bone Gloves";
            Hue = 2944;
            BaseArmorRating = 53;
		}

		public LunarGloves( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}