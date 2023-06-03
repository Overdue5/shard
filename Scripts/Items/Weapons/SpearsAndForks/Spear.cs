namespace Server.Items
{
	[Flipable( 0xF62, 0xF63 )]
	public class Spear : BaseSpear
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ArmorIgnore; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ParalyzingBlow; } }

		public override int AosStrengthReq => 50;
        public override int AosMinDamage => 13;
        public override int AosMaxDamage => 15;
        public override int AosSpeed => 42;

        public override int OldStrengthReq => 30;
        public override int OldMinDamage => 15;
        public override int OldMaxDamage => 34; //Loki edit: was 12-31
		public override int OldSpeed => 351;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 80;

        [Constructable]
		public Spear() : base( 0xF62 )
		{
			Weight = 7.0;
			//Name = "spear";
		}

		public Spear( Serial serial ) : base( serial )
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
                MinDamage = 15;
                MaxDamage = 34;
            }
		}
	}
}