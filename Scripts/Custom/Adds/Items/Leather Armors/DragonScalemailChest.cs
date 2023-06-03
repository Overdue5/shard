namespace Server.Items
{
	[Flipable( 0x13cc, 0x13d3 )]
	public class DragonScalemailChest : BaseArmor
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
		public DragonScalemailChest() : base( 0x13CC )
		{
			Weight = 6.0;
            Name = "Dragon Scalemail Tunic";
            Hue = 2534;
            BaseArmorRating = 48;
		}

		public DragonScalemailChest( Serial serial ) : base( serial )
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