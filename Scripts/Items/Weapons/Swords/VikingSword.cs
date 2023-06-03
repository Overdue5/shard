namespace Server.Items
{
	[Flipable( 0x13B9, 0x13Ba )]
	public class VikingSword : BaseSword
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.CrushingBlow; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ParalyzingBlow; } }

		public override int AosStrengthReq => 40;
        public override int AosMinDamage => 15;
        public override int AosMaxDamage => 17;
        public override int AosSpeed => 28;

        public override int OldStrengthReq => 40;
        public override int OldMinDamage => 12;
        public override int OldMaxDamage => 35;
        public override int OldSpeed => 442;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 100;

        [Constructable]
		public VikingSword() : base( 0x13B9 )
		{
			Weight = 6.0;
			//Name = "viking sword";
		}

		public VikingSword( Serial serial ) : base( serial )
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
                MaxDamage = 35;
            }
		}
	}
}