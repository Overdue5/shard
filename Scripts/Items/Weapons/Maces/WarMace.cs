namespace Server.Items
{
	[Flipable( 0x1407, 0x1406 )]
	public class WarMace : BaseBashing
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.CrushingBlow; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.BleedAttack; } }

		public override int AosStrengthReq => 80;
        public override int AosMinDamage => 16;
        public override int AosMaxDamage => 17;
        public override int AosSpeed => 26;

        public override int OldStrengthReq => 30;
        public override int OldMinDamage => 14;
        public override int OldMaxDamage => 30;
        public override int OldSpeed => 420;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 110;

        [Constructable]
		public WarMace() : base( 0x1407 )
		{
			Weight = 17.0;
			//Name = "war mace";
		}

		public WarMace( Serial serial ) : base( serial )
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
                MinDamage = 14;
                MaxDamage = 30;
            }
		}
	}
}