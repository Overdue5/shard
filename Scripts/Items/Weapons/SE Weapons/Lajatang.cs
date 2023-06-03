namespace Server.Items
{
	[Flipable( 0x27A7, 0x27F2 )]
	public class Lajatang : BaseKnife
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.DefenseMastery; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.FrenziedWhirlwind; } }

		public override int AosStrengthReq => 65;
        public override int AosMinDamage => 16;
        public override int AosMaxDamage => 18;
        public override int AosSpeed => 32;

        public override int OldStrengthReq => 65;
        public override int OldMinDamage => 16;
        public override int OldMaxDamage => 18;
        public override int OldSpeed => 290;

        public override int DefHitSound => 0x232;
        public override int DefMissSound => 0x238;

        public override int InitMinHits => 90;
        public override int InitMaxHits => 95;

        public override SkillName DefSkill => SkillName.Fencing;
        public override WeaponType DefType => WeaponType.Piercing;
        public override WeaponAnimation DefAnimation => WeaponAnimation.Pierce1H;

        [Constructable]
		public Lajatang() : base( 0x27A7 )
		{
			Weight = 12.0;
			Layer = Layer.TwoHanded;
		}

		public Lajatang( Serial serial ) : base( serial )
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