namespace Server.Items
{
	[Flipable( 0xF61, 0xF60 )]
	public class Longsword : BaseSword
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ArmorIgnore; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ConcussionBlow; } }

		public override int AosStrengthReq => 35;
        public override int AosMinDamage => 15;
        public override int AosMaxDamage => 16;
        public override int AosSpeed => 30;

        public override int OldStrengthReq => 25;
        public override int OldMinDamage => 13;
        public override int OldMaxDamage => 30;
        public override int OldSpeed => 401;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 110;

        [Constructable]
		public Longsword() : base( 0xF61 )
		{
			Weight = 7.0;
			//Name = "longsword";
		}

		public Longsword( Serial serial ) : base( serial )
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
                MinDamage = 13;
                MaxDamage = 30;
            }
		}
	}
}