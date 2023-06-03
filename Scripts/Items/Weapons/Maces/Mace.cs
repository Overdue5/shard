namespace Server.Items
{
	[Flipable( 0xF5C, 0xF5D )]
	public class Mace : BaseBashing
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ConcussionBlow; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.Disarm; } }

		public override int AosStrengthReq => 45;
        public override int AosMinDamage => 12;
        public override int AosMaxDamage => 14;
        public override int AosSpeed => 40;

        public override int OldStrengthReq => 20;
        public override int OldMinDamage => 12;
        public override int OldMaxDamage => 33;
        public override int OldSpeed => 441;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 70;

        [Constructable]
		public Mace() : base( 0xF5C )
		{
			Weight = 14.0;
			//Name = "mace";
		}

		public Mace( Serial serial ) : base( serial )
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
                MaxDamage = 33;
            }
		}
	}
}