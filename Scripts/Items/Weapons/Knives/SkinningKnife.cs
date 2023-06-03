namespace Server.Items
{
	[Flipable( 0xEC4, 0xEC5 )]
	public class SkinningKnife : BaseKnife
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ShadowStrike; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.Disarm; } }

		public override int AosStrengthReq => 5;
        public override int AosMinDamage => 9;
        public override int AosMaxDamage => 11;
        public override int AosSpeed => 49;

        public override int OldStrengthReq => 5;
        public override int OldMinDamage => 1;
        public override int OldMaxDamage => 10;
        public override int OldSpeed => 362;

        public override int InitMinHits => 51;
        public override int InitMaxHits => 60;

        [Constructable]
		public SkinningKnife() : base( 0xEC4 )
		{
			Weight = 1.0;
			//Name = "skinning knife";
		}

		public SkinningKnife( Serial serial ) : base( serial )
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