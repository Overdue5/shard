namespace Server.Items
{
	public class WoodenKiteShield : BaseShield
	{
		public override int BasePhysicalResistance => 0;
        public override int BaseFireResistance => 0;
        public override int BaseColdResistance => 0;
        public override int BasePoisonResistance => 0;
        public override int BaseEnergyResistance => 1;

        public override int InitMinHits => 50;
        public override int InitMaxHits => 65;

        public override int AosStrReq => 20;

        //public override int ArmorBase{ get{ return 9; } }

		[Constructable]
		public WoodenKiteShield() : base( 0x1B79 )
		{
			Weight = 5.0;
            BaseArmorRating = 10;
		}

		public WoodenKiteShield( Serial serial ) : base(serial)
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            if (BaseArmorRating == 12)
                BaseArmorRating = 10;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 );//version
		}
	}
}
