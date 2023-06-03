namespace Server.Items
{
	[Flipable( 0x13cd, 0x13c5 )]
	public class DragonScalemailArms : BaseArmor
	{
		public override int BasePhysicalResistance => 5;
        public override int BaseFireResistance => 4;
        public override int BaseColdResistance => 3;
        public override int BasePoisonResistance => 3;
        public override int BaseEnergyResistance => 3;

        public override int InitMinHits => 60;
        public override int InitMaxHits => 65;

        public override int AosStrReq => 20;
        public override int OldStrReq => 15;

        public override int ArmorBase => 46;

        public override ArmorMaterialType MaterialType => ArmorMaterialType.Leather;
        public override CraftResource DefaultResource => CraftResource.RegularLeather;

        public override ArmorMeditationAllowance DefMedAllowance => ArmorMeditationAllowance.All;

        [Constructable]
		public DragonScalemailArms() : base( 0x13CD )
		{
			Weight = 2.0;
            Name = "Dragon Scalemail Sleeves";
            Hue = 2534;
            BaseArmorRating = 48;
		}

		public DragonScalemailArms( Serial serial ) : base( serial )
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

			if (BaseArmorRating == 46)
                BaseArmorRating = 48;
		}
	}
}