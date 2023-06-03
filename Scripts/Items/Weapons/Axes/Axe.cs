namespace Server.Items
{
	[Flipable( 0xF49, 0xF4a )]
	public class Axe : BaseAxe
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.CrushingBlow; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.Dismount; } }

		public override int AosStrengthReq => 35;
        public override int AosMinDamage => 14;
        public override int AosMaxDamage => 16;
        public override int AosSpeed => 37;

        public override int OldStrengthReq => 35;
        public override int OldMinDamage => 18;
        public override int OldMaxDamage => 33; //Loki edit: was 15 - 32
		public override int OldSpeed => 383;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 110;

        [Constructable]
		public Axe() : base( 0xF49 )
		{
			Weight = 4.0;
			//Name = "axe";
		}

		public Axe( Serial serial ) : base( serial )
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
                MinDamage = 18;
                MaxDamage = 33;
            }
		}
	}
}