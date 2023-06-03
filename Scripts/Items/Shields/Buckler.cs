namespace Server.Items
{
	public class Buckler : BaseShield
	{
		public override int BasePhysicalResistance => 0;
        public override int BaseFireResistance => 0;
        public override int BaseColdResistance => 0;
        public override int BasePoisonResistance => 1;
        public override int BaseEnergyResistance => 0;

        public override int InitMinHits => 40;
        public override int InitMaxHits => 50;

        public override int AosStrReq => 20;

        //public override int ArmorBase{ get{ return 7; } }

		[Constructable]
		public Buckler() : base( 0x1B73 )
		{
            Name = "Buckler Shield";
			Weight = 5.0;
            BaseArmorRating = 5;
		}

		public Buckler( Serial serial ) : base(serial)
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
