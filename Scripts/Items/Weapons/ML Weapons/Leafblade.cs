namespace Server.Items
{
	[FlipableAttribute( 0x2D22, 0x2D2E )]
	public class Leafblade : BaseKnife
	{
		public override WeaponAbility PrimaryAbility => WeaponAbility.Feint;
        public override WeaponAbility SecondaryAbility => WeaponAbility.ArmorIgnore;

        public override int AosStrengthReq => 20;
        public override int AosMinDamage => 13;
        public override int AosMaxDamage => 15;
        public override int AosSpeed => 42;
        public override float MlSpeed => 2.75f;

        public override int OldStrengthReq => 20;
        public override int OldMinDamage => 13;
        public override int OldMaxDamage => 15;
        public override int OldSpeed => 42;

        public override int DefMissSound => 0x239;

        public override int InitMinHits => 30; // TODO
		public override int InitMaxHits => 60; // TODO

		[Constructable]
		public Leafblade() : base( 0x2D22 )
		{
			Weight = 8.0;
		}

		public Leafblade( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}