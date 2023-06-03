namespace Server.Items
{
    [Flipable(0x144f, 0x1454)]
	public class LunarChest : BaseArmor
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

        public override int ArmorBase => 45;

        public override ArmorMaterialType MaterialType => ArmorMaterialType.Leather;
        public override CraftResource DefaultResource => CraftResource.RegularLeather;

        [Constructable]
		public LunarChest() : base( 0x144F )
		{
			Weight = 6.0;
            Name = "Lunar Bone Armor";
            Hue = 2944;
            BaseArmorRating = 53;
		}

		public LunarChest( Serial serial ) : base( serial )
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
				Weight = 6.0;
		}
	}
}