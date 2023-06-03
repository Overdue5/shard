namespace Server.Items
{
	[Flipable( 0x27A2, 0x27ED )]
	public class NoDachi : BaseSword
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.CrushingBlow; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.RidingSwipe; } }

		public override int AosStrengthReq => 40;
        public override int AosMinDamage => 16;
        public override int AosMaxDamage => 18;
        public override int AosSpeed => 35;

        public override int OldStrengthReq => 40;
        public override int OldMinDamage => 16;
        public override int OldMaxDamage => 18;
        public override int OldSpeed => 400;

        public override int DefHitSound => 0x23B;
        public override int DefMissSound => 0x23A;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 90;

        [Constructable]
		public NoDachi() : base( 0x27A2 )
		{
			Weight = 10.0;
			Layer = Layer.TwoHanded;
		}

		public NoDachi( Serial serial ) : base( serial )
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