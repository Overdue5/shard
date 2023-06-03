namespace Server.Items
{
	[Flipable( 0xf45, 0xf46 )]
	public class ExecutionersAxe : BaseAxe
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.BleedAttack; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.MortalStrike; } }

		public override int AosStrengthReq => 40;
        public override int AosMinDamage => 15;
        public override int AosMaxDamage => 17;
        public override int AosSpeed => 33;

        public override int OldStrengthReq => 35;
        public override int OldMinDamage => 24;
        public override int OldMaxDamage => 33; //Loki edit: was 20 - 30
		public override int OldSpeed => 408;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 70;

        [Constructable]
		public ExecutionersAxe() : base( 0xF45 )
		{
			Weight = 8.0;
			HitSound = 566;
			MissSound = 568;
			//Name = "executioner's axe";
		}

		public ExecutionersAxe( Serial serial ) : base( serial )
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
                MinDamage = 24;
                MaxDamage = 33;
            }
		}
	}
}