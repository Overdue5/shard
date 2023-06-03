namespace Server.Items
{
	[Flipable( 0x26C0, 0x26CA )]
	public class Lance : BaseSword
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.Dismount; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ConcussionBlow; } }

		public override int AosStrengthReq => 95;
        public override int AosMinDamage => 17;
        public override int AosMaxDamage => 18;
        public override int AosSpeed => 24;

        public override int OldStrengthReq => 95;
        public override int OldMinDamage => 17;
        public override int OldMaxDamage => 18;
        public override int OldSpeed => 537;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 110;

        public override SkillName DefSkill => SkillName.Fencing;
        public override WeaponType DefType => WeaponType.Piercing;
        public override WeaponAnimation DefAnimation => WeaponAnimation.Pierce1H;

        [Constructable]
		public Lance() : base( 0x26C0 )
		{
			Weight = 12.0;
			//Name = "lance";
		}

		public Lance( Serial serial ) : base( serial )
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
		}
	}
}