namespace Server.Items
{
	[Flipable( 0x143D, 0x143C )]
	public class HammerPick : BaseBashing
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ArmorIgnore; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.MortalStrike; } }

		public override int AosStrengthReq => 45;
        public override int AosMinDamage => 15;
        public override int AosMaxDamage => 17;
        public override int AosSpeed => 28;

        public override int OldStrengthReq => 35;

        //public override int OldMinDamage{ get{ return 20; } }
		//public override int OldMaxDamage{ get{ return 45; } }
        public override int OldMinDamage => 25;
        public override int OldMaxDamage => 40;
        public override int OldSpeed => 604;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 70;

        [Constructable]
		public HammerPick() : base( 0x143D )
		{
			Weight = 9.0;
			Layer = Layer.OneHanded;
			//Name = "hammer pick";
		}

		public HammerPick( Serial serial ) : base( serial )
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
                MaxDamage = 40;
            }
		}
	}
}