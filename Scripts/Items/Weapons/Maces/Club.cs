namespace Server.Items
{
	[Flipable( 0x13b4, 0x13b3 )]
	public class Club : BaseBashing
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ShadowStrike; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.Dismount; } }

		public override int AosStrengthReq => 40;
        public override int AosMinDamage => 11;
        public override int AosMaxDamage => 13;
        public override int AosSpeed => 44;

        public override int OldStrengthReq => 10;
        public override int OldMinDamage => 12;
        public override int OldMaxDamage => 19;
        public override int OldSpeed => 302;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 40;

        [Constructable]
		public Club() : base( 0x13B4 )
		{
			Weight = 3.0;
			//Name = "club";
		}

		public Club( Serial serial ) : base( serial )
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

            if (version == 0)
            {
                MinDamage = 12;
                MaxDamage = 19;
            }
		}
	}
}