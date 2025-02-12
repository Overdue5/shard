namespace Server.Items
{
	public class WoodenShield : BaseShield
	{
		public override int BasePhysicalResistance => 0;
        public override int BaseFireResistance => 0;
        public override int BaseColdResistance => 0;
        public override int BasePoisonResistance => 0;
        public override int BaseEnergyResistance => 1;

        public override int InitMinHits => 20;
        public override int InitMaxHits => 25;

        public override int AosStrReq => 20;

        //public override int ArmorBase{ get{ return 4; } }

		[Constructable]
		public WoodenShield() : base( 0x1B7A )
		{
			Weight = 5.0;
            BaseArmorRating = 5;
		}

		public WoodenShield( Serial serial ) : base(serial)
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            if (BaseArmorRating == 7)
                BaseArmorRating = 5;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 );//version
		}
	}
}
