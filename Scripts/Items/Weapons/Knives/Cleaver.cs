namespace Server.Items
{
	[Flipable( 0xEC3, 0xEC2 )]
	public class Cleaver : BaseKnife
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.BleedAttack; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.InfectiousStrike; } }

		public override int AosStrengthReq => 10;
        public override int AosMinDamage => 11;
        public override int AosMaxDamage => 13;
        public override int AosSpeed => 46;

        public override int OldStrengthReq => 10;
        public override int OldMinDamage => 2;
        public override int OldMaxDamage => 5;
        public override int OldSpeed => 200;

        public override int InitMinHits => 51;
        public override int InitMaxHits => 70;

        [Constructable]
		public Cleaver() : base( 0xEC3 )
		{
			Weight = 2.0;
			//Name = "cleaver";
		}

		public Cleaver( Serial serial ) : base( serial )
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

			if ( Weight == 1.0 )
				Weight = 2.0;
		}
	}
}