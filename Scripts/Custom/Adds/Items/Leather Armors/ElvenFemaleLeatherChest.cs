namespace Server.Items
{
	[Flipable( 0x1c06, 0x1c07 )]
	public class ElvenFemaleLeatherChest : BaseArmor
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

        public override int ArmorBase => 39;

        public override ArmorMaterialType MaterialType => ArmorMaterialType.Leather;
        public override CraftResource DefaultResource => CraftResource.RegularLeather;

        public override ArmorMeditationAllowance DefMedAllowance => ArmorMeditationAllowance.All;

        [Constructable]
		public ElvenFemaleLeatherChest() : base( 0x1C06 )
		{
			Weight = 4.0;
            Name = "Magical Elven Female Armor";
            Hue = 2549;
            BaseArmorRating = 39;
		}

		public ElvenFemaleLeatherChest( Serial serial ) : base( serial )
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
		}
	}
}