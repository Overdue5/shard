namespace Server.Items
{
	[Flipable( 0x1c08, 0x1c09 )]
	public class ElvenLeatherSkirt : BaseArmor
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

        public override int ArmorBase => 39;

        public override ArmorMaterialType MaterialType => ArmorMaterialType.Leather;
        public override CraftResource DefaultResource => CraftResource.RegularLeather;

        public override ArmorMeditationAllowance DefMedAllowance => ArmorMeditationAllowance.All;

        [Constructable]
		public ElvenLeatherSkirt() : base( 0x1C08 )
		{
			Weight = 1.0;
            Name = "Magical Elven Leather Skirt";
            Hue = 2549;
            BaseArmorRating = 39;
		}

		public ElvenLeatherSkirt( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 );

			if ( Weight == 3.0 )
				Weight = 1.0;
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}