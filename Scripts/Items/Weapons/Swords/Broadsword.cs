namespace Server.Items
{
	[Flipable( 0xF5E, 0xF5F )]
	public class Broadsword : BaseSword
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.CrushingBlow; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ArmorIgnore; } }

		public override int AosStrengthReq => 30;
        public override int AosMinDamage => 14;
        public override int AosMaxDamage => 15;
        public override int AosSpeed => 33;

        public override int OldStrengthReq => 25;
        public override int OldMinDamage => 10;
        public override int OldMaxDamage => 25;
        public override int OldSpeed => 334;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 100;

        [Constructable]
		public Broadsword() : base( 0xF5E )
		{
			Weight = 6.0;
			//Name = "broadsword";
		}

		public Broadsword( Serial serial ) : base( serial )
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
                MinDamage = 10;
                MaxDamage = 25;
            }
		}
	}
}