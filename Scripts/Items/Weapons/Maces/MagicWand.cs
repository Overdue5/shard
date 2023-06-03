namespace Server.Items
{
	public class MagicWand : BaseBashing
	{
        //public override WeaponAbility PrimaryAbility { get { return WeaponAbility.Dismount; } }
        //public override WeaponAbility SecondaryAbility { get { return WeaponAbility.Disarm; } }

		public override int AosStrengthReq => 5;
        public override int AosMinDamage => 9;
        public override int AosMaxDamage => 11;
        public override int AosSpeed => 40;

        public override int OldStrengthReq => 0;
        public override int OldMinDamage => 2;
        public override int OldMaxDamage => 6;
        public override int OldSpeed => 400;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 110;

        [Constructable]
		public MagicWand() : base( 0xDF2 )
		{
			Weight = 1.0;
			Name = "Magic wand";
		}

		public MagicWand( Serial serial ) : base( serial )
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
		}
	}
}