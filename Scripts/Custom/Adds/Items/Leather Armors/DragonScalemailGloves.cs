namespace Server.Items
{
	[Flipable]
	public class DragonScalemailGloves : BaseArmor
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

        public override int ArmorBase => 46;

        public override ArmorMaterialType MaterialType => ArmorMaterialType.Leather;
        public override CraftResource DefaultResource => CraftResource.RegularLeather;

        public override ArmorMeditationAllowance DefMedAllowance => ArmorMeditationAllowance.All;

        [Constructable]
		public DragonScalemailGloves() : base( 0x13C6 )
		{
			Weight = 1.0;
            Name = "Dragon Scalemail Gloves";
            Hue = 2534;
            BaseArmorRating = 48;
		}

		public DragonScalemailGloves( Serial serial ) : base( serial )
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

            if (BaseArmorRating == 46)
                BaseArmorRating = 48;
		}
	}
}