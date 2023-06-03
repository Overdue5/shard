namespace Server.Items
{
	[Flipable( 0x27AE, 0x27F9 )]
	public class Nunchaku : BaseBashing
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.Block; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.Feint; } }

		public override int AosStrengthReq => 15;
        public override int AosMinDamage => 11;
        public override int AosMaxDamage => 13;
        public override int AosSpeed => 47;

        public override int OldStrengthReq => 15;
        public override int OldMinDamage => 11;
        public override int OldMaxDamage => 13;
        public override int OldSpeed => 323;

        public override int DefHitSound => 0x535;
        public override int DefMissSound => 0x239;

        public override int InitMinHits => 40;
        public override int InitMaxHits => 55;

        [Constructable]
		public Nunchaku() : base( 0x27AE )
		{
			Weight = 5.0;
		}

		public Nunchaku( Serial serial ) : base( serial )
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