namespace Server.Items
{
	[Flipable( 0x1c02, 0x1c03 )]
	public class FemaleStuddedChest : BaseArmor
	{
		public override int BasePhysicalResistance => 2;
        public override int BaseFireResistance => 4;
        public override int BaseColdResistance => 3;
        public override int BasePoisonResistance => 3;
        public override int BaseEnergyResistance => 4;

        public override int InitMinHits => 35;
        public override int InitMaxHits => 45;

        public override int AosStrReq => 35;
        public override int OldStrReq => 35;

        public override ArmorMaterialType MaterialType => ArmorMaterialType.Studded;
        public override CraftResource DefaultResource => CraftResource.RegularLeather;

        public override ArmorMeditationAllowance DefMedAllowance => ArmorMeditationAllowance.Half;

        [Constructable]
		public FemaleStuddedChest() : base( 0x1C02 )
		{
			Weight = 6.0;
		    BaseArmorRating = 18;
		}

		public FemaleStuddedChest( Serial serial ) : base( serial )
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

			if ( Weight == 1.0 )
				Weight = 6.0;

            if (BaseArmorRating == 20)
                BaseArmorRating = 18;
		}
	}
}