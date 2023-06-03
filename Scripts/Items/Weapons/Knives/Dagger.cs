namespace Server.Items
{
	[Flipable( 0xF52, 0xF51 )]
	public class Dagger : BaseKnife
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.InfectiousStrike; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ShadowStrike; } }

		public override int AosStrengthReq => 10;
        public override int AosMinDamage => 10;
        public override int AosMaxDamage => 11;
        public override int AosSpeed => 56;

        public override int OldStrengthReq => 1;
        public override int OldMinDamage => 1;
        public override int OldMaxDamage => 4;
        public override int OldSpeed => 200;

        public override int InitMinHits => 51;
        public override int InitMaxHits => 60;

        public override SkillName DefSkill => SkillName.Fencing;

        public override WeaponType DefType => WeaponType.Piercing;
        //public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Pierce1H; } }

		[Constructable]
		public Dagger() : base( 0xF52 )
		{
            
		}

		public Dagger( Serial serial ) : base( serial )
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