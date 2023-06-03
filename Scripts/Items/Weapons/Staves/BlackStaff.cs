namespace Server.Items
{
	[Flipable( 0xDF1, 0xDF0 )]
	public class BlackStaff : BaseStaff
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.WhirlwindAttack; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ParalyzingBlow; } }

		public override int AosStrengthReq => 35;
        public override int AosMinDamage => 13;
        public override int AosMaxDamage => 16;
        public override int AosSpeed => 39;

        public override int OldStrengthReq => 35;
        public override int OldMinDamage => 15;
        public override int OldMaxDamage => 33;
        public override int OldSpeed => 391;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 70;

        [Constructable]
		public BlackStaff() : base( 0xDF0 )
		{
			Weight = 6.0;
			//Name = "black staff";
		}

		public BlackStaff( Serial serial ) : base( serial )
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