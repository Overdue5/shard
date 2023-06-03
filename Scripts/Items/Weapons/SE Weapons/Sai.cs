namespace Server.Items
{
	[Flipable( 0x27AF, 0x27FA )]
	public class Sai : BaseKnife
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.Block; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ArmorPierce; } }

		public override int AosStrengthReq => 15;
        public override int AosMinDamage => 9;
        public override int AosMaxDamage => 11;
        public override int AosSpeed => 55;

        public override int OldStrengthReq => 15;
        public override int OldMinDamage => 9;
        public override int OldMaxDamage => 11;
        public override int OldSpeed => 290;

        public override int DefHitSound => 0x23C;
        public override int DefMissSound => 0x232;

        public override int InitMinHits => 55;
        public override int InitMaxHits => 60;

        public override SkillName DefSkill => SkillName.Fencing;
        public override WeaponType DefType => WeaponType.Piercing;
        public override WeaponAnimation DefAnimation => WeaponAnimation.Pierce1H;

        [Constructable]
		public Sai() : base( 0x27AF )
		{
			Weight = 7.0;
			Layer = Layer.TwoHanded;
		}

		public Sai( Serial serial ) : base( serial )
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