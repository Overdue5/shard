namespace Server.Items
{
	public class MetalKiteShield : BaseShield, IDyable
	{
		public override int BasePhysicalResistance => 0;
        public override int BaseFireResistance => 0;
        public override int BaseColdResistance => 0;
        public override int BasePoisonResistance => 0;
        public override int BaseEnergyResistance => 1;

        public override int InitMinHits => 45;
        public override int InitMaxHits => 60;

        public override int AosStrReq => 45;

        //public override int ArmorBase{ get{ return 10; } }

		[Constructable]
		public MetalKiteShield() : base( 0x1B74 )
		{
			Weight = 7.0;
            BaseArmorRating = 11;
		}

		public MetalKiteShield( Serial serial ) : base(serial)
		{
		}

		public bool Dye( Mobile from, DyeTub sender )
		{
			if ( Deleted )
				return false;

			Hue = sender.DyedHue;

			return true;
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            if (BaseArmorRating == 13)
                BaseArmorRating = 11;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 );//version
		}
	}
}
