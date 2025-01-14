namespace Server.Items
{
	[Flipable( 0x13ee, 0x13ef )]
	public class RingmailArms : BaseArmor
	{
		public override int BasePhysicalResistance => 3;
        public override int BaseFireResistance => 3;
        public override int BaseColdResistance => 1;
        public override int BasePoisonResistance => 5;
        public override int BaseEnergyResistance => 3;

        public override int InitMinHits => 40;
        public override int InitMaxHits => 50;

        public override int AosStrReq => 40;
        public override int OldStrReq => 20;

        //public override int OldDexBonus{ get{ return -1; } }

		public override ArmorMaterialType MaterialType => ArmorMaterialType.Ringmail;

        [Constructable]
		public RingmailArms() : base( 0x13EE )
		{
			Weight = 15.0;
		    BaseArmorRating = 22;
		}

		public RingmailArms( Serial serial ) : base( serial )
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

            if (BaseArmorRating == 26)
                BaseArmorRating = 22;
		}
	}
}