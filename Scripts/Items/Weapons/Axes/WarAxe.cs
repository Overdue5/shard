using Server.Engines.Harvest;

namespace Server.Items
{
	[Flipable( 0x13B0, 0x13AF )]
	public class WarAxe: BaseAxe
	{
		////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ArmorIgnore; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.BleedAttack; } }

		public override int AosStrengthReq => 35;
        public override int AosMinDamage => 14;
        public override int AosMaxDamage => 15;
        public override int AosSpeed => 33;

        public override int OldStrengthReq => 35;
        public override int OldMinDamage => 13;
        public override int OldMaxDamage => 26;
        public override int OldSpeed => 350;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 80;

        public override SkillName DefSkill => SkillName.Macing;

        public override WeaponType DefType => WeaponType.Bashing;
        //public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Bash1H; } }

      		public override int DefHitSound => 0x232;
            public override int DefMissSound => Utility.RandomList(0x238, 0x239, 0x23A);

            public override int GetSwingAnim(Mobile from)
	        {
        	    if (from.Mounted)
                	return 26;
	            else
        	        return Utility.RandomList(12, 13, 14);
	        }

		public override HarvestSystem HarvestSystem => null;

        [Constructable]
		public WarAxe() : base( 0x13B0 )
		{
			Weight = 8.0;
			//Name = "war axe";
			//Layer = Layer.TwoHanded;
		}

		public WarAxe( Serial serial ) : base( serial )
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
                MaxDamage = 26;
            }
		}
	}
}