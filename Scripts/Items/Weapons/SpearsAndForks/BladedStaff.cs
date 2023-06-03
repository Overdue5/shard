namespace Server.Items
{
	[Flipable( 0x26BD, 0x26C7 )]
	public class BladedStaff : BaseSpear
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ArmorIgnore; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.Dismount; } }

		public override int AosStrengthReq => 40;
        public override int AosMinDamage => 14;
        public override int AosMaxDamage => 16;
        public override int AosSpeed => 37;

        public override int OldStrengthReq => 40;
        public override int OldMinDamage => 14;
        public override int OldMaxDamage => 16;
        public override int OldSpeed => 383;

        public override int InitMinHits => 21;
        public override int InitMaxHits => 110;

        public override SkillName DefSkill => SkillName.Swords;

        public override int GetSwingAnim(Mobile from)
	        {
        	    if (from.Mounted)
                	return 29;
	            else
        	        return Utility.RandomList(12, 13, 14);
	        }

		[Constructable]
		public BladedStaff() : base( 0x26BD )
		{
			Weight = 4.0;
			//Name = "bladed staff";
		}

		public BladedStaff( Serial serial ) : base( serial )
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