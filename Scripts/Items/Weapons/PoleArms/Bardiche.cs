namespace Server.Items
{
	[Flipable( 0xF4D, 0xF4E )]
	public class Bardiche : BasePoleArm
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ParalyzingBlow; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.Dismount; } }

		public override int AosStrengthReq => 45;
        public override int AosMinDamage => 17;
        public override int AosMaxDamage => 18;
        public override int AosSpeed => 28;

        public override int OldStrengthReq => 40;

        //public override int OldMinDamage{ get{ return 19; } }
		//public override int OldMaxDamage{ get{ return 48; } }
        public override int OldMinDamage => 25;
        public override int OldMaxDamage => 42;
        public override int OldSpeed => 516;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 100;

        [Constructable]
		public Bardiche() : base( 0xF4D )
		{
			Weight = 15.0; //Loki edit: Was 7.0
			//Name = "bardiche";
		}

		public Bardiche( Serial serial ) : base( serial )
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

            if (version == 0)
            {
                MinDamage = 25;
                MaxDamage = 42;
            }
            if (version < 2)
                Weight = 15.0;
		}
	}
}