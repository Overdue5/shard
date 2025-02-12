namespace Server.Items
{
	public class OrcChieftainHelm : OrcHelm
	{
		public override int LabelNumber => 1094924; // Orc Chieftain Helm [Replica]

		public override int BasePhysicalResistance => 23;
        public override int BaseFireResistance => 1;
        public override int BaseColdResistance => 23;
        public override int BasePoisonResistance => 3;
        public override int BaseEnergyResistance => 5;

        public override int InitMinHits => 150;
        public override int InitMaxHits => 150;

        public override bool CanFortify => false;

        [Constructable]
		public OrcChieftainHelm()
		{
			Hue = 0x2a3;

			Attributes.Luck = 100;
			Attributes.RegenHits = 3;

			if( Utility.RandomBool() )
				Attributes.BonusHits = 30;
			else
				Attributes.AttackChance = 30;
		}

		public OrcChieftainHelm( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
            if (version < 1 && Hue == 0x3f) /* Pigmented? */
            {
                Hue = 0x2a3;
            }
		}
	}
}
