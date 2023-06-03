namespace Server.Items
{
	[Flipable( 0xF47, 0xF48 )]
	public class BattleAxe : BaseAxe
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.BleedAttack; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ConcussionBlow; } }

		public override int AosStrengthReq => 35;
        public override int AosMinDamage => 15;
        public override int AosMaxDamage => 17;
        public override int AosSpeed => 31;

        public override int OldStrengthReq => 40;

        //public override int OldMinDamage { get { return 18; } }
        //public override int OldMaxDamage { get { return 36; } }
		public override int OldMinDamage => 25;
        public override int OldMaxDamage => 36; //Loki edit: Was 21 - 33
		public override int OldSpeed => 429;

        public override int InitMinHits => 60;
        public override int InitMaxHits => 140;

        [Constructable]
		public BattleAxe() : base( 0xF47 )
		{
			Weight = 4.0;
			//Name = "battle axe";
			Layer = Layer.TwoHanded;
		}

		public BattleAxe( Serial serial ) : base( serial )
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
                MinDamage = 25;
                MaxDamage = 36;
            }
		}
	}
}