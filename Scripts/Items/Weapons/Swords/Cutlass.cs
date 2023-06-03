namespace Server.Items
{
	[Flipable( 0x1441, 0x1440 )]
	public class Cutlass : BaseSword
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.BleedAttack; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ShadowStrike; } }

		public override int AosStrengthReq => 25;
        public override int AosMinDamage => 11;
        public override int AosMaxDamage => 13;
        public override int AosSpeed => 44;

        public override int OldStrengthReq => 10;
        public override int OldMinDamage => 12;
        public override int OldMaxDamage => 24;
        public override int OldSpeed => 332;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 70;

        [Constructable]
		public Cutlass() : base( 0x1441 )
		{
			Weight = 8.0;
			//Name = "cutlass";
		}

		public Cutlass( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            if (version == 0)
            {
                MinDamage = 12;
                MaxDamage = 24;
            }
		}
	}
}