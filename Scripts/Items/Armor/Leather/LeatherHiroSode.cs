namespace Server.Items
{
	public class LeatherHiroSode : BaseArmor
	{
		public override int BasePhysicalResistance => 2;
        public override int BaseFireResistance => 4;
        public override int BaseColdResistance => 3;
        public override int BasePoisonResistance => 3;
        public override int BaseEnergyResistance => 3;

        public override int InitMinHits => 40;
        public override int InitMaxHits => 50;

        public override int AosStrReq => 25;
        public override int OldStrReq => 25;

        public override int ArmorBase => 3;

        public override ArmorMaterialType MaterialType => ArmorMaterialType.Leather;
        public override CraftResource DefaultResource => CraftResource.RegularLeather;

        public override ArmorMeditationAllowance DefMedAllowance => ArmorMeditationAllowance.All;

        [Constructable]
		public LeatherHiroSode() : base( 0x277E )
		{
			Weight = 1.0;
		}

		public LeatherHiroSode( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 );
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}