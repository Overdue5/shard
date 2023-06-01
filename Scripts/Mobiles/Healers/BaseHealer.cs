using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Items;
using Server.Regions;

namespace Server.Mobiles
{
	public abstract class BaseHealer : BaseVendor
	{
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

		public override bool IsActiveVendor{ get{ return true; } }
		public override bool IsInvulnerable{ get{ return false; } }

		public override void InitSBInfo()
        {
            if (this.Criminal || !(this.Region is GuardedRegion gr && !gr.Disabled) || this.AlwaysMurderer)
            {
                SBInfos.Add(new SBHealerRich());
            }
            else
            {
                SBInfos.Add(new SBHealer());
            }
        }

	    public BaseHealer() : base( null )
		{
            AI = AIType.AI_Vendor;
            ActiveSpeed = 0.2;
            PassiveSpeed = 0.8;
            RangePerception = DefaultRangePerception;
            FightMode = FightMode.Aggressor;

			SpeechHue = 0;

			SetStr( 304, 400 );
			SetDex( 102, 150 );
			SetInt( 204, 300 );

			SetDamage( 10, 23 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 40, 50 );
			SetResistance( ResistanceType.Fire, 40, 50 );
			SetResistance( ResistanceType.Cold, 40, 50 );
			SetResistance( ResistanceType.Poison, 40, 50 );
			SetResistance( ResistanceType.Energy, 40, 50 );

			SetSkill( SkillName.Anatomy, 75.0, 97.5 );
			SetSkill( SkillName.EvalInt, 82.0, 100.0 );
			SetSkill( SkillName.Healing, 75.0, 97.5 );
			SetSkill( SkillName.Magery, 82.0, 100.0 );
			SetSkill( SkillName.MagicResist, 82.0, 100.0 );
			SetSkill( SkillName.Tactics, 82.0, 100.0 );

			Fame = 1000;
			Karma = 10000;

			PackItem( new Bandage( Utility.RandomMinMax( 5, 10 ) ) );
			PackItem( new HealPotion() );
			PackItem( new CurePotion() );
		}

		public override VendorShoeType ShoeType{ get{ return VendorShoeType.Sandals; } }

		public virtual int GetRobeColor()
		{
			return Utility.RandomYellowHue();
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

			AddItem( new Robe( GetRobeColor() ) );
		}

		public virtual bool HealsYoungPlayers{ get{ return true; } }

		public virtual bool CheckResurrect( Mobile m )
		{
			return true;
		}

		private DateTime m_NextResurrect;
		private static readonly TimeSpan ResurrectDelay = TimeSpan.FromSeconds( 2.0 );

		public virtual void OfferResurrection( Mobile m )
		{
			Direction = GetDirectionTo( m );

			m.PlaySound( 0x214 );
			m.FixedEffect( 0x376A, 10, 16 );

			m.CloseGump( typeof( ResurrectGump ) );
			m.SendGump( new ResurrectGump( m, this, ResurrectMessage.Healer, false ) );
		}

		public virtual void OfferHeal( PlayerMobile m )
		{
			Direction = GetDirectionTo( m );

			if ( m.CheckYoungHealTime() )
			{
				Say( 501229 ); // You look like you need some healing my child.

				m.PlaySound( 0x1F2 );
				m.FixedEffect( 0x376A, 9, 32 );

				m.Hits = m.HitsMax;
			}
			else
			{
				Say( 501228 ); // I can do no more for you at this time.
			}
		}

        public override void OnDoubleClickDead(Mobile m)
        {
            if (!Resurect(m, m.Location, false))
			    base.OnDoubleClickDead(m);
        }

        public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			Resurect(m, oldLocation, true);
		}

        public bool Resurect(Mobile m, Point3D oldLocation, bool onMovement)
        {
            if (m.Frozen)
            {
				if (!onMovement)
					m.SendAsciiMessage("You are frozen and cannot be healed");
                return false;
            }

            if (DateTime.UtcNow < m_NextResurrect)
            {
                if (!onMovement)
                    this.Say("I need to take a break.");
                return false;
            }

			if (!m.Alive || ((HealsYoungPlayers && m.Hits < m.HitsMax && m is PlayerMobile pl && pl.Young)))
            {
                if (InRange(m, 4) && InLOS(m) && (!onMovement || !InRange(oldLocation, 4)))
                {
                    if (!m.Alive)
                    {

                        if (m.Map == null || !m.Map.CanFit(m.Location, 16, false, false))
                        {
                            m.SendLocalizedMessage(502391); // Thou can not be resurrected there!
                        }
                        else if (m.Region is HouseRegion)
                        {
                            m.SendAsciiMessage("You can't get resurrected in house regions.");
                        }
                        else if (CheckResurrect(m))
                        {
                            m_NextResurrect = DateTime.UtcNow + ResurrectDelay;
                            OfferResurrection(m);
                        }
                    }
                    else
                    {
                        m_NextResurrect = DateTime.UtcNow + ResurrectDelay;
                        OfferHeal((PlayerMobile)m);
                    }
                }
                else
                {
                    if (!onMovement)
                        m.SendLocalizedMessage(501853);// Target is too far away.
                    return false;
                }
            }

            return true;
        }

        public BaseHealer( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			if ( !IsInvulnerable )
			{
				AI = AIType.AI_Mage;
				ActiveSpeed = 0.2;
				PassiveSpeed = 0.8;
				RangePerception = DefaultRangePerception;
				FightMode = FightMode.Aggressor;
			}
		}
	}
}