namespace Server.Mobiles
{
	public class Healer : BaseHealer
	{
		public override bool CanTeach => true;

        public override bool CheckTeach( SkillName skill, Mobile from )
		{
			if ( !base.CheckTeach( skill, from ) )
				return false;

			return ( skill == SkillName.Forensics )
				|| ( skill == SkillName.Healing )
				|| ( skill == SkillName.SpiritSpeak )
				|| ( skill == SkillName.Swords );
		}

		[Constructable]
		public Healer()
		{
			Title = "the healer";

			if ( !Core.AOS )
				NameHue = -1;

			SetSkill( SkillName.Forensics, 80.0, 100.0 );
			SetSkill( SkillName.SpiritSpeak, 80.0, 100.0 );
			SetSkill( SkillName.Swords, 80.0, 100.0 );
		}

		public override bool IsActiveVendor => true;
        public override bool IsInvulnerable => false;

        public override bool CheckResurrect( Mobile m )
		{
			if ( m.Criminal )
			{
				Say( 501222 ); // Thou art a criminal.  I shall not resurrect thee.
				return false;
			}
			else if ( Misc.NotorietyHandlers.IsGuardCandidate(m) )
			{
				Say( 501223 ); // Thou'rt not a decent and good person. I shall not resurrect thee.
				return false;
			}
            else if (m.Karma < 0)
            {
                Say(501224); // Thou hast strayed from the path of virtue, but thou still deservest a second chance.
            }

			return true;
		}

		public Healer( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( Core.AOS && NameHue == 0x35 )
				NameHue = -1;
		}
	}
}