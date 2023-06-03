namespace Server.Items
{
	[Flipable( 0xF62, 0xF63 )]
	public class TribalSpear : BaseSpear
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ArmorIgnore; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ParalyzingBlow; } }

		public override int AosStrengthReq => 50;
        public override int AosMinDamage => 13;
        public override int AosMaxDamage => 15;
        public override int AosSpeed => 42;

        public override int OldStrengthReq => 30;
        public override int OldMinDamage => 13;
        public override int OldMaxDamage => 30;
        public override int OldSpeed => 362;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 80;

        public override int VirtualDamageBonus => 10;

        [Constructable]
		public TribalSpear() : base( 0xF62 )
		{
			Weight = 7.0;
			Hue = 837;
			//Name = "tribal spear";
		}

		public TribalSpear( Serial serial ) : base( serial )
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