namespace Server.Items
{
	[Flipable( 0x13FB, 0x13FA )]
	public class LargeBattleAxe : BaseAxe
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.WhirlwindAttack; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.BleedAttack; } }

		public override int AosStrengthReq => 80;
        public override int AosMinDamage => 16;
        public override int AosMaxDamage => 17;
        public override int AosSpeed => 29;

        public override int OldStrengthReq => 40;
        public override int OldMinDamage => 22;
        public override int OldMaxDamage => 40; //Loki edit: Was 17 - 38
		public override int OldSpeed => 455;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 70;

        [Constructable]
		public LargeBattleAxe() : base( 0x13FB )
		{
			Weight = 6.0;
			//Name = "large battle axe";
		}

		public LargeBattleAxe( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 2 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            if (version < 2)
            {
                MinDamage = 22;
                MaxDamage = 40;
            }
		}
	}
}