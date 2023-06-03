namespace Server.Items
{
	[Flipable( 0x26BC, 0x26C6 )]
	public class Scepter : BaseBashing
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.CrushingBlow; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.MortalStrike; } }

		public override int AosStrengthReq => 40;
        public override int AosMinDamage => 14;
        public override int AosMaxDamage => 17;
        public override int AosSpeed => 30;

        public override int OldStrengthReq => 40;
        public override int OldMinDamage => 14;
        public override int OldMaxDamage => 17;
        public override int OldSpeed => 450;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 110;

        [Constructable]
		public Scepter() : base( 0x26BC )
		{
			Weight = 8.0;
			//Name = "scepter";
		}

		public Scepter( Serial serial ) : base( serial )
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