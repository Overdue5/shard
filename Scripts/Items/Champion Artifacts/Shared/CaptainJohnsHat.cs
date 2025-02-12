namespace Server.Items
{
	public class CaptainJohnsHat : TricorneHat
	{
		public override int LabelNumber => 1094911; // Captain John's Hat [Replica]

		public override int BasePhysicalResistance => 2;
        public override int BaseFireResistance => 6;
        public override int BaseColdResistance => 9;
        public override int BasePoisonResistance => 7;
        public override int BaseEnergyResistance => 23;

        public override int InitMinHits => 150;
        public override int InitMaxHits => 150;

        public override bool CanFortify => false;

        [Constructable]
		public CaptainJohnsHat()
		{
			Hue = 0x455;

			Attributes.BonusDex = 8;
			Attributes.NightSight = 1;
			Attributes.AttackChance = 15;

			SkillBonuses.Skill_1_Name = SkillName.Swords;
			SkillBonuses.Skill_1_Value = 20;
		}

		public CaptainJohnsHat( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
