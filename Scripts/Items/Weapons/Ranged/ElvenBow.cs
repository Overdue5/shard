using System;

namespace Server.Items
{
	[Flipable( 0x13B2, 0x13B1 )]
	public class ElvenBow : BaseRanged
	{
		public override int EffectID => 0xF42;
        public override Type AmmoType => typeof( Arrow );
        public override BaseAmmo Ammo => new Arrow();

        public override WeaponAbility PrimaryAbility => WeaponAbility.ParalyzingBlow;
        public override WeaponAbility SecondaryAbility => WeaponAbility.MortalStrike;

        public override int AosStrengthReq => 35;
        public override int AosMinDamage => 20;
        public override int AosMaxDamage => 24;
        public override int AosSpeed => 21;

        public override int OldStrengthReq => 25;
        public override int OldMinDamage => 15;
        public override int OldMaxDamage => 21;
        public override int OldSpeed => 265;

        public override int DefMaxRange => 10;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 60;

        private SkillMod m_ARCHERYMod;
        private SkillMod m_TACTICSMod;

		//public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.ShootBow; } }

		[Constructable]
		public ElvenBow() : base( 0x13B2 )
		{
			Weight = 6.0;
			Name = "Elven bow";
            Hue = 0x237;
			Layer = Layer.TwoHanded;
		}

		public ElvenBow( Serial serial ) : base( serial )
		{
		}

        public override void OnDoubleClick(Mobile m)
        {
            base.OnDoubleClick(m);
        }

        public override bool OnEquip(Mobile from)
        {
            if (UseSkillMod)
            {
                if (from.FindItemOnLayer(Layer.TwoHanded) != this && m_ARCHERYMod == null && m_TACTICSMod == null)
                {
                    m_TACTICSMod = new DefaultSkillMod(SkillName.Tactics, true, 15);
                    from.AddSkillMod(m_TACTICSMod);
                    m_ARCHERYMod = new DefaultSkillMod(SkillName.Archery, true, 10);
                    from.AddSkillMod(m_ARCHERYMod);
                }
            }
            return base.OnEquip(from);
        }

        public override void OnRemoved(object parent)
        {
            Mobile pl = null;
            if (parent is Mobile)
                pl = (Mobile)parent;
            if (UseSkillMod && m_ARCHERYMod != null && m_TACTICSMod != null && pl != null)
            {
                if (pl.FindItemOnLayer(Layer.TwoHanded) != this)
                {
                    m_ARCHERYMod.Remove();
                    m_ARCHERYMod = null;
                    m_TACTICSMod.Remove();
                    m_TACTICSMod = null;
                }
            }
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
                MinDamage = 15;
                MaxDamage = 21;
            }
		}
	}
}
