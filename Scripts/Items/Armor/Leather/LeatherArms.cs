namespace Server.Items
{
	[Flipable( 0x13cd, 0x13c5 )]
	public class LeatherArms : BaseArmor
	{
		public override int BasePhysicalResistance => 2;
        public override int BaseFireResistance => 4;
        public override int BaseColdResistance => 3;
        public override int BasePoisonResistance => 3;
        public override int BaseEnergyResistance => 3;

        public override int InitMinHits => 30;
        public override int InitMaxHits => 40;

        public override int AosStrReq => 20;
        public override int OldStrReq => 15;

        public override ArmorMaterialType MaterialType => ArmorMaterialType.Leather;
        public override CraftResource DefaultResource => CraftResource.RegularLeather;

        public override ArmorMeditationAllowance DefMedAllowance => ArmorMeditationAllowance.All;

        [Constructable]
		public LeatherArms() : base( 0x13CD )
		{
			Weight = 2.0;
		    BaseArmorRating = 18;
		}

		public LeatherArms( Serial serial ) : base( serial )
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

			if ( Weight == 1.0 )
				Weight = 2.0;

            if (BaseArmorRating == 28)
                BaseArmorRating = 18;
		}
	}
}