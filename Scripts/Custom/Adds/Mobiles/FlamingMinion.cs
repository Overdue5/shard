//revised

using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a smoldering pile of ash" )]
	public class FlamingMinion : BaseCreature
	{
		private Mobile m_Owner;
		private DateTime m_ExpireTime;

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Owner
		{
			get => m_Owner;
            set => m_Owner = value;
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime ExpireTime
		{
			get => m_ExpireTime;
            set => m_ExpireTime = value;
        }

		[Constructable]
		public FlamingMinion() : this( null )
		{
		}
        public override bool AlwaysMurderer => true;
        public override bool HasBreath => false;
        public override bool Uncalmable => Core.SE;
        public override bool BardImmune => !Core.SE;
        public override bool Unprovokable => Core.SE;

        public override void DisplayPaperdollTo(Mobile to)
		{
		}
        public override void OnBeforeSpawn(Point3D location, Map m)
        {
            Effects.SendLocationEffect(location, m, 0x3709, 30);
            base.OnBeforeSpawn(location, m);
        }
		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			for ( int i = 0; i < list.Count; ++i )
			{
				if ( list[i] is ContextMenus.PaperdollEntry )
					list.RemoveAt( i-- );
			}
		}

		public override void OnThink()
		{
			bool expired;

			expired = ( DateTime.UtcNow >= m_ExpireTime );

			if ( !expired && m_Owner != null )
				expired = m_Owner.Deleted || Map != m_Owner.Map || !InRange( m_Owner, 16 );

			if ( expired )
			{
				PlaySound( GetIdleSound() );
				Delete();
			}
			else
			{
				base.OnThink();
			}
		}

        public FlamingMinion(Mobile owner): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            m_Owner = owner;
            m_ExpireTime = DateTime.UtcNow + TimeSpan.FromMinutes(4.0);

            Name = "Flaming Minion";
            Hue = 2519;

            switch (Utility.Random(10))
            {
                case 0: // efreet
                    Body = 131;
                    BaseSoundID = 768;
                    break;
                case 1: // meer eternal
                    Body = 772;
                    BaseSoundID = 0x28B;
                    break;
                case 2: // terathan warrior
                    Body = 70;
                    BaseSoundID = 589;
                    break;
                default:
                case 3: // blood elemental
                    Body = 159;
                    BaseSoundID = 278;
                    break;
            }

                    SetStr(150, 350);
                    SetDex(280, 325);
                    SetInt(260, 300);

                    SetHits(400, 580);

                    SetDamage(12, 15);

                    SetDamageType(ResistanceType.Physical, 40);

                    SetResistance(ResistanceType.Physical, 65, 75);
                    SetResistance(ResistanceType.Fire, 30, 40);
                    SetResistance(ResistanceType.Cold, 25, 35);
                    SetResistance(ResistanceType.Poison, 65, 75);
                    SetResistance(ResistanceType.Energy, 55, 65);

                    SetSkill(SkillName.MagicResist, 25.0);
                    SetSkill(SkillName.Tactics, 25.0);
                    SetSkill(SkillName.Wrestling, 50.0);

                    Fame = 1000;
                    Karma = -1000;

                    Tamable = false;

                    VirtualArmor = 40;
            
        }
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Gems);


        }
        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
            c.Stackable = true;
            c.Amount = 51;
            c.Stackable = false;
            c.Hue = 1900;
            c.Name = "a smoldering pile of ash";
        }
        public FlamingMinion(Serial serial) : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}