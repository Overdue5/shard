namespace Server.Items
{
	[Flipable( 0x143E, 0x143F )]
	public class Halberd : BasePoleArm
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.WhirlwindAttack; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ConcussionBlow; } }

		public override int AosStrengthReq => 95;
        public override int AosMinDamage => 18;
        public override int AosMaxDamage => 19;
        public override int AosSpeed => 25;

        public override int OldStrengthReq => 45;

        //public override int OldMinDamage{ get{ return 20; } }
		//public override int OldMaxDamage{ get{ return 57; } }
        public override int OldMinDamage => 25;
        public override int OldMaxDamage => 52;
        public override int OldSpeed => 616;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 80;

        [Constructable]
		public Halberd() : base( 0x143E )
		{
			Weight = 16.0;
			//Name = "halberd";
		}

		public Halberd( Serial serial ) : base( serial )
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
                MinDamage = 25;
                MaxDamage = 52;
            }
		}
	}
}