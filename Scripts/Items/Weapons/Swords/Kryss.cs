namespace Server.Items
{
	[Flipable( 0x1401, 0x1400 )]
	public class Kryss : BaseSword
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ArmorIgnore; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.InfectiousStrike; } }

		public override int AosStrengthReq => 10;
        public override int AosMinDamage => 10;
        public override int AosMaxDamage => 12;
        public override int AosSpeed => 53;

        public override int OldStrengthReq => 10;
        public override int OldMinDamage => 11;
        public override int OldMaxDamage => 18;
        public override int OldSpeed => 248;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 90;

        public override SkillName DefSkill => SkillName.Fencing;
        public override WeaponType DefType => WeaponType.Piercing;
        public override WeaponAnimation DefAnimation => WeaponAnimation.Pierce1H;

        [Constructable]
		public Kryss() : base( 0x1401 )
		{
			Weight = 2.0;
			//Name = "kryss";
		}

		public Kryss( Serial serial ) : base( serial )
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
                MinDamage = 11;
                MaxDamage = 18;
            }
		}
	}
}