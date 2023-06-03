namespace Server.Items
{
	[Flipable( 0x1c06, 0x1c07 )]
	public class DragonScalemailFemaleChest : BaseArmor
	{
		public override int BasePhysicalResistance => 5;
        public override int BaseFireResistance => 4;
        public override int BaseColdResistance => 3;
        public override int BasePoisonResistance => 3;
        public override int BaseEnergyResistance => 3;

        public override int InitMinHits => 60;
        public override int InitMaxHits => 70;

        public override int AosStrReq => 25;
        public override int OldStrReq => 15;

        public override int ArmorBase => 46;

        public override ArmorMaterialType MaterialType => ArmorMaterialType.Leather;
        public override CraftResource DefaultResource => CraftResource.RegularLeather;

        public override ArmorMeditationAllowance DefMedAllowance => ArmorMeditationAllowance.All;

        [Constructable]
		public DragonScalemailFemaleChest() : base( 0x1C06 )
		{
			Weight = 4.0;
            Name = "Female Dragon Scalemail";
            Hue = 2534;
            BaseArmorRating = 48;
		}

        public DragonScalemailFemaleChest(Serial serial)  : base(serial)
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 1 );
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