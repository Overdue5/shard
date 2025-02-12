using System;
using System.Collections.Generic;
using Server.Commands.GMUtils;
using Server.Engines.Craft;
using Server.Ethics;
using Server.Factions;
using Server.Mobiles;
using Server.Network;
using Server.Regions;
using Server.SkillHandlers;
using Server.Spells;
using Server.Spells.Bushido;
using Server.Spells.Chivalry;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;
using Server.Spells.Spellweaving;
using Solaris.CliLocHandler;

namespace Server.Items
{
	public interface ISlayer
	{
		SlayerName Slayer { get; set; }
		SlayerName Slayer2 { get; set; }
	}

	public abstract class BaseWeapon : Item, IWeapon, IFactionItem, ICraftable, ISlayer, IDurability
	{
        private string m_EngravedText;

        [CommandProperty(AccessLevel.GameMaster)]
        public string EngravedText
        {
            get => m_EngravedText;
            set { m_EngravedText = value; InvalidateProperties(); }
        }

        //Swing chance modifier. Modifies the percentage to hit or miss depending on how many consecutive hits/misses you have
        private static readonly Dictionary<int, double> m_HitChanceModifier = new Dictionary<int, double> { { 0, 1 }, { 1, 1 }, { 2, 0.9 }, { 3, .8 }, { 4, .2 }, { 5, .1 }, { 6, -1 }, { 7, -3 }, { 8, -5 }, { 9, int.MinValue } };

		#region Factions
		private FactionItem m_FactionState;

		public FactionItem FactionItemState
		{
			get => m_FactionState;
            set
			{
				m_FactionState = value;

				if ( m_FactionState == null )
					Hue = CraftResources.GetHue( Resource );

				LootType = ( m_FactionState == null ? LootType.Regular : LootType.Blessed );
			}
		}
		#endregion

		/* Weapon internals work differently now (Mar 13 2003)
		 * 
		 * The attributes defined below default to -1.
		 * If the value is -1, the corresponding virtual 'Aos/Old' property is used.
		 * If not, the attribute value itself is used. Here's the list:
		 *  - MinDamage
		 *  - MaxDamage
		 *  - Speed
		 *  - HitSound
		 *  - MissSound
		 *  - StrRequirement, DexRequirement, IntRequirement
		 *  - WeaponType
		 *  - WeaponAnimation
		 *  - MaxRange
		 */

		#region Var declarations

		// Instance values. These values are unique to each weapon.
		private WeaponDamageLevel m_DamageLevel;
		private WeaponAccuracyLevel m_AccuracyLevel;
		private WeaponDurabilityLevel m_DurabilityLevel;
		private WeaponQuality m_Quality;
		private Mobile m_Crafter;
		private Poison m_Poison;
		private int m_PoisonCharges;
		private bool m_Identified;
		private int m_Hits;
		private int m_MaxHits;
		private SlayerName m_Slayer;
		private SlayerName m_Slayer2;
		private SkillMod m_SkillMod, m_MageMod;
		private CraftResource m_Resource;
		private bool m_PlayerConstructed;

		private bool m_Cursed; // Is this weapon cursed via Curse Weapon necromancer spell? Temporary; not serialized.
		private bool m_Consecrated; // Is this weapon blessed via Consecrate Weapon paladin ability? Temporary; not serialized.

		private AosAttributes m_AosAttributes;
		private AosWeaponAttributes m_AosWeaponAttributes;
		private AosSkillBonuses m_AosSkillBonuses;
		private AosElementAttributes m_AosElementDamages;

		// Overridable values. These values are provided to override the defaults which get defined in the individual weapon scripts.
		private int m_StrReq, m_DexReq, m_IntReq;
		private int m_MinDamage, m_MaxDamage;
		private int m_HitSound, m_MissSound;
		private int m_Speed;
		private int m_MaxRange;
		private SkillName m_Skill;
		private WeaponType m_Type;
		private WeaponAnimation m_Animation;

        //Maka
        //private const double m_SphereHitPercetage = 0.65;
        private const double m_SphereHitPercetage = 0.69;
        private bool m_IsRenamed ;

		//Onhit workaround
		private static readonly PlateChest m_GenericOnHitPlate = new PlateChest();

		#endregion

		#region Virtual Properties
		public virtual WeaponAbility PrimaryAbility => null;
        public virtual WeaponAbility SecondaryAbility => null;

        public virtual int DefMaxRange => 1;
        public virtual int DefHitSound => 0;
        public virtual int DefMissSound => 0;
        public virtual SkillName DefSkill => SkillName.Swords;
        public virtual WeaponType DefType => WeaponType.Slashing;
        public virtual WeaponAnimation DefAnimation => WeaponAnimation.Slash1H;

        public virtual int AosStrengthReq => 0;
        public virtual int AosDexterityReq => 0;
        public virtual int AosIntelligenceReq => 0;
        public virtual int AosMinDamage => 0;
        public virtual int AosMaxDamage => 0;
        public virtual int AosSpeed => 0;
        public virtual float MlSpeed => 0.0f;
        public virtual int AosMaxRange => DefMaxRange;
        public virtual int AosHitSound => DefHitSound;
        public virtual int AosMissSound => DefMissSound;
        public virtual SkillName AosSkill => DefSkill;
        public virtual WeaponType AosType => DefType;
        public virtual WeaponAnimation AosAnimation => DefAnimation;

        public virtual int OldStrengthReq => 0;
        public virtual int OldDexterityReq => 0;
        public virtual int OldIntelligenceReq => 0;
        public virtual int OldMinDamage => 0;
        public virtual int OldMaxDamage => 0;
        public virtual int OldSpeed => 0;
        public virtual int OldMaxRange => DefMaxRange;
        public virtual int OldHitSound => DefHitSound;
        public virtual int OldMissSound => DefMissSound;
        public virtual SkillName OldSkill => DefSkill;
        public virtual WeaponType OldType => DefType;
        public virtual WeaponAnimation OldAnimation => DefAnimation;

        public virtual int InitMinHits => 0;
        public virtual int InitMaxHits => 0;

        public virtual bool CanFortify => false;

        public virtual int GetSwingAnim(Mobile from) { return -1; }

		public override int PhysicalResistance => m_AosWeaponAttributes.ResistPhysicalBonus;
        public override int FireResistance => m_AosWeaponAttributes.ResistFireBonus;
        public override int ColdResistance => m_AosWeaponAttributes.ResistColdBonus;
        public override int PoisonResistance => m_AosWeaponAttributes.ResistPoisonBonus;
        public override int EnergyResistance => m_AosWeaponAttributes.ResistEnergyBonus;

        public virtual SkillName AccuracySkill => SkillName.Tactics;

        #endregion

		#region Getters & Setters

        [CommandProperty(AccessLevel.GameMaster)]
	    public bool IsRenamed
        {
            get => m_IsRenamed;
            set => m_IsRenamed = value;
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public AosAttributes Attributes => m_AosAttributes;

        [CommandProperty( AccessLevel.GameMaster )]
		public AosWeaponAttributes WeaponAttributes => m_AosWeaponAttributes;

        [CommandProperty( AccessLevel.GameMaster )]
		public AosSkillBonuses SkillBonuses => m_AosSkillBonuses;

        [CommandProperty( AccessLevel.GameMaster )]
		public AosElementAttributes AosElementDamages => m_AosElementDamages;

        [CommandProperty( AccessLevel.GameMaster )]
		public bool Cursed
		{
			get => m_Cursed;
            set => m_Cursed = value;
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Consecrated
		{
			get => m_Consecrated;
            set => m_Consecrated = value;
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Identified
		{
			get => m_Identified;
            set{ m_Identified = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int HitPoints
		{
			get => m_Hits;
            set
			{
				if ( m_Hits == value )
					return;

				if ( value > m_MaxHits )
					value = m_MaxHits;

				m_Hits = value;

				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int MaxHitPoints
		{
			get => m_MaxHits;
            set{ m_MaxHits = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int PoisonCharges
		{
			get => m_PoisonCharges;
            set{ m_PoisonCharges = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Poison Poison
		{
			get => m_Poison;
            set{ m_Poison = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public WeaponQuality Quality
		{
			get => m_Quality;
            set{ UnscaleDurability(); m_Quality = value; ScaleDurability(); InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Crafter
		{
			get => m_Crafter;
            set{ m_Crafter = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public SlayerName Slayer
		{
			get => m_Slayer;
            set
			{
                if (value == SlayerName.Silver)
                    Hue = 1953;
                else
                    Hue = Hue;
               
                m_Slayer = value; 
                InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public SlayerName Slayer2
		{
			get => m_Slayer2;
            set { m_Slayer2 = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public CraftResource Resource
		{
			get => m_Resource;
            set{ UnscaleDurability(); m_Resource = value; Hue = CraftResources.GetHue( m_Resource ); InvalidateProperties(); ScaleDurability(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public WeaponDamageLevel DamageLevel
		{
			get => m_DamageLevel;
            set{ m_DamageLevel = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public WeaponDurabilityLevel DurabilityLevel
		{
			get => m_DurabilityLevel;
            set{ UnscaleDurability(); m_DurabilityLevel = value; InvalidateProperties(); ScaleDurability(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool PlayerConstructed
		{
			get => m_PlayerConstructed;
            set => m_PlayerConstructed = value;
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public int MaxRange
		{
			get => ( m_MaxRange == -1 ? Core.AOS ? AosMaxRange : OldMaxRange : m_MaxRange );
            set{ m_MaxRange = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public WeaponAnimation Animation
		{
			get => ( m_Animation == (WeaponAnimation)(-1) ? Core.AOS ? AosAnimation : OldAnimation : m_Animation );
            set => m_Animation = value;
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public WeaponType Type
		{
			get => ( m_Type == (WeaponType)(-1) ? Core.AOS ? AosType : OldType : m_Type );
            set => m_Type = value;
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public SkillName Skill
		{
			get => ( m_Skill == (SkillName)(-1) ? Core.AOS ? AosSkill : OldSkill : m_Skill );
            set{ m_Skill = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int HitSound
		{
			get => ( m_HitSound == -1 ? Core.AOS ? AosHitSound : OldHitSound : m_HitSound );
            set => m_HitSound = value;
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public int MissSound
		{
			get => ( m_MissSound == -1 ? Core.AOS ? AosMissSound : OldMissSound : m_MissSound );
            set => m_MissSound = value;
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public int MinDamage
		{
			get => ( m_MinDamage == -1 ? Core.AOS ? AosMinDamage : OldMinDamage : m_MinDamage );
            set{ m_MinDamage = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int MaxDamage
		{
			get => ( m_MaxDamage == -1 ? Core.AOS ? AosMaxDamage : OldMaxDamage : m_MaxDamage );
            set{ m_MaxDamage = value; InvalidateProperties(); }
		}

        [CommandProperty(AccessLevel.GameMaster)]
        public int Speed
        {
            get => (m_Speed == -1 ? Core.AOS ? AosSpeed : OldSpeed : m_Speed);
            set { m_Speed = value; InvalidateProperties(); }
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public int StrRequirement
		{
			get => ( m_StrReq == -1 ? Core.AOS ? AosStrengthReq : OldStrengthReq : m_StrReq );
            set{ m_StrReq = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int DexRequirement
		{
			get => ( m_DexReq == -1 ? Core.AOS ? AosDexterityReq : OldDexterityReq : m_DexReq );
            set => m_DexReq = value;
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public int IntRequirement
		{
			get => ( m_IntReq == -1 ? Core.AOS ? AosIntelligenceReq : OldIntelligenceReq : m_IntReq );
            set => m_IntReq = value;
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public WeaponAccuracyLevel AccuracyLevel
		{
			get => m_AccuracyLevel;
            set
			{
				if ( m_AccuracyLevel != value )
				{
					m_AccuracyLevel = value;

					if ( UseSkillMod )
					{
						if ( m_AccuracyLevel == WeaponAccuracyLevel.Regular )
						{
							if ( m_SkillMod != null )
								m_SkillMod.Remove();

							m_SkillMod = null;
						}
						else if ( m_SkillMod == null && Parent is Mobile )
						{
							m_SkillMod = new DefaultSkillMod( AccuracySkill, true, (int)m_AccuracyLevel * 5 );
							((Mobile)Parent).AddSkillMod( m_SkillMod );
						}
						else if ( m_SkillMod != null )
						{
							m_SkillMod.Value = (int)m_AccuracyLevel * 5;
						}
					}

					InvalidateProperties();
				}
			}
		}

		#endregion

		public static BaseWeapon CreateRandomWeapon()
        {
            switch (Utility.Random(29))
            {
                default:return new Axe();
                case 1: return new BattleAxe();
                case 2: return new DoubleAxe();
                case 3: return new ExecutionersAxe();
                case 4: return new LargeBattleAxe();
                case 5: return new TwoHandedAxe();
                case 6: return new WarAxe();
                case 7: return new Club();
                case 8: return new HammerPick();
                case 9: return new Mace();
                case 10: return new Maul();
                case 11: return new WarHammer();
                case 12: return new WarMace();
                case 13: return new Bardiche();
                case 14: return new Halberd();
                case 15: return new Bow();
                case 16: return new Crossbow();
                case 17: return new HeavyCrossbow();
                case 18: return new ShortSpear();
                case 19: return new Spear();
                case 20: return new WarFork();
                case 21: return new Broadsword();
                case 22: return new Cutlass();
                case 23: return new Katana();
                case 24: return new Kryss();
                case 25: return new Scimitar();
                case 26: return new Longsword();
                case 27: return new VikingSword();
                case 28: return new ThinLongsword();

                //case 7: return new Dagger();
                //case 22: return new BlackStaff();
                //case 23: return new GnarledStaff();
                //case 24: return new QuarterStaff();
            }
        }

        /*
        public override void OnAfterDuped(Item newItem)
        {
            BaseWeapon weap = newItem as BaseWeapon;

            if (weap == null)
                return;

            weap.m_AosAttributes = new AosAttributes(newItem, m_AosAttributes);
            weap.m_AosElementDamages = new AosElementAttributes(newItem, m_AosElementDamages);
            weap.m_AosSkillBonuses = new AosSkillBonuses(newItem, m_AosSkillBonuses);
            weap.m_AosWeaponAttributes = new AosWeaponAttributes(newItem, m_AosWeaponAttributes);
        }
        */

		public virtual void UnscaleDurability()
		{
			int scale = 150 + GetDurabilityBonus();

			m_Hits = ((m_Hits * 100) + (scale - 1)) / scale;
			m_MaxHits = ((m_MaxHits * 100) + (scale - 1)) / scale;
			InvalidateProperties();
		}

		public virtual void ScaleDurability()
		{
			int scale = 150 + GetDurabilityBonus();

			m_Hits = ((m_Hits * scale) + 99) / 100;
			m_MaxHits = ((m_MaxHits * scale) + 99) / 100;
			InvalidateProperties();
		}

		public int GetDurabilityBonus()
		{
			int bonus = 0;

			if ( m_Quality == WeaponQuality.Exceptional )
				bonus += 20;

			switch ( m_DurabilityLevel )
			{
				case WeaponDurabilityLevel.Durable: bonus += 20; break;
				case WeaponDurabilityLevel.Substantial: bonus += 50; break;
				case WeaponDurabilityLevel.Massive: bonus += 70; break;
				case WeaponDurabilityLevel.Fortified: bonus += 100; break;
				case WeaponDurabilityLevel.Indestructible: bonus += 120; break;
			}

			if ( Core.AOS )
			{
				bonus += m_AosWeaponAttributes.DurabilityBonus;

				CraftResourceInfo resInfo = CraftResources.GetInfo( m_Resource );
				CraftAttributeInfo attrInfo = null;

				if ( resInfo != null )
					attrInfo = resInfo.AttributeInfo;

				if ( attrInfo != null )
					bonus += attrInfo.WeaponDurability;
			}

			return bonus;
		}

        public static void ValidateMobile(Mobile m)
        {
            if (m.AccessLevel >= AccessLevel.GameMaster)
                return;

            bool hasgotmessage = false;
            for (int i = m.Items.Count - 1; i >= 0; --i)
            {
                if (i >= m.Items.Count)
                    continue;

                Item item = m.Items[i];

                if (item is BaseWeapon)
                {
                    BaseWeapon weapon = (BaseWeapon)item;

                    if (!m.Body.IsHuman && m.BodyValue != 9)
                    {
                        if (!hasgotmessage)
                        {
                            m.SendAsciiMessage("You may not use weapons while polymorphed to something other than a daemon");
                            hasgotmessage = true;
                        }

                        m.AddToBackpack(weapon);
                    }
                }
            }
        }

	    public int GetLowerStatReq()
		{
			if ( !Core.AOS )
				return 0;

			int v = m_AosWeaponAttributes.LowerStatReq;

			CraftResourceInfo info = CraftResources.GetInfo( m_Resource );

			if ( info != null )
			{
				CraftAttributeInfo attrInfo = info.AttributeInfo;

				if ( attrInfo != null )
					v += attrInfo.WeaponLowerRequirements;
			}

			if ( v > 100 )
				v = 100;

			return v;
		}

		public static void BlockEquip( Mobile m, TimeSpan duration )
		{
			if ( m.BeginAction( typeof( BaseWeapon ) ) )
				new ResetEquipTimer( m, duration ).Start();
		}

		private class ResetEquipTimer : Timer
		{
			private readonly Mobile m_Mobile;

			public ResetEquipTimer( Mobile m, TimeSpan duration ) : base( duration )
			{
				m_Mobile = m;
			}

			protected override void OnTick()
			{
				m_Mobile.EndAction( typeof( BaseWeapon ) );
			}
		}

		public override bool CheckConflictingLayer( Mobile m, Item item, Layer layer )
		{
			if ( base.CheckConflictingLayer( m, item, layer ) )
				return true;

			if ( Layer == Layer.TwoHanded && layer == Layer.OneHanded )
			{
				m.SendLocalizedMessage( 500214 ); // You already have something in both hands.
				return true;
			}
		    if ( Layer == Layer.OneHanded && layer == Layer.TwoHanded && !(item is BaseShield) && !(item is BaseEquipableLight) )
		    {
		        m.SendLocalizedMessage( 500215 ); // You can only wield one weapon at a time.
		        return true;
		    }

		    return false;
		}

		public override bool AllowSecureTrade( Mobile from, Mobile to, Mobile newOwner, bool accepted )
		{
			if ( !Ethic.CheckTrade( from, to, newOwner, this ) )
				return false;

			return base.AllowSecureTrade( from, to, newOwner, accepted );
		}

		public virtual Race RequiredRace => null; //On OSI, there are no weapons with race requirements, this is for custom stuff

		public override bool CanEquip( Mobile from )
		{
			if ( !Ethic.CheckEquip( from, this ) )
				return false;

            if (from.AccessLevel < AccessLevel.GameMaster)
            {
                if (!from.Body.IsHuman && from.BodyValue != 9)
                {
                    from.SendAsciiMessage("You may not use weapons while polymorphed to something other than a daemon");
                    return false;
                }/* Taran: No race restrictions
                if (RequiredRace != null && from.Race != RequiredRace)
                {
                    if (RequiredRace == Race.Elf)
                        from.SendLocalizedMessage(1072203); // Only Elves may use this.
                    else
                        from.SendMessage("Only {0} may use this.", RequiredRace.PluralName);

                    return false;
                }*/
                if (from.Dex < DexRequirement)
                {
                    from.SendMessage("You are not nimble enough to equip that.");
                    return false;
                }
                if (from.Str < AOS.Scale(StrRequirement, 100 - GetLowerStatReq()))
                {
                    from.SendLocalizedMessage(500213); // You are not strong enough to equip that.
                    return false;
                }
                if (from.Int < IntRequirement)
                {
                    from.SendMessage("You are not smart enough to equip that.");
                    return false;
                }
                if (!from.CanBeginAction(typeof (BaseWeapon)))
                {
                    return false;
                }
            }
		    return true;// base.CanEquip(from);
		}

		public virtual bool UseSkillMod => !Core.AOS;

        public override bool OnEquip( Mobile from )
		{
            if (Amount > 1)
            {
                from.SendAsciiMessage("You can only equip one weapon at a time");
                return false;
            }

			int strBonus = m_AosAttributes.BonusStr;
			int dexBonus = m_AosAttributes.BonusDex;
			int intBonus = m_AosAttributes.BonusInt;

			if ( (strBonus != 0 || dexBonus != 0 || intBonus != 0) )
			{
				Mobile m = from;

				string modName = Serial.ToString();

				if ( strBonus != 0 )
					m.AddStatMod( new StatMod( StatType.Str, modName + "Str", strBonus, TimeSpan.Zero ) );

				if ( dexBonus != 0 )
					m.AddStatMod( new StatMod( StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero ) );

				if ( intBonus != 0 )
					m.AddStatMod( new StatMod( StatType.Int, modName + "Int", intBonus, TimeSpan.Zero ) );
			}

            //Maka
            if (from is PlayerMobile)
                (from as PlayerMobile).WeaponTimerCheck();

            if ( UseSkillMod && m_AccuracyLevel != WeaponAccuracyLevel.Regular )
			{
				if ( m_SkillMod != null )
					m_SkillMod.Remove();

				m_SkillMod = new DefaultSkillMod( AccuracySkill, true, (int)m_AccuracyLevel * 3 );
				from.AddSkillMod( m_SkillMod );
			}

			if ( Core.AOS && m_AosWeaponAttributes.MageWeapon != 0 && m_AosWeaponAttributes.MageWeapon != 30 )
			{
				if ( m_MageMod != null )
					m_MageMod.Remove();

				m_MageMod = new DefaultSkillMod( SkillName.Magery, true, -30 + m_AosWeaponAttributes.MageWeapon );
				from.AddSkillMod( m_MageMod );
			}

			return true;
		}

        public override void OnAdded( object parent )
		{
			base.OnAdded( parent );

			if ( parent is Mobile )
			{
				Mobile from = (Mobile)parent;

				if ( Core.AOS )
					m_AosSkillBonuses.AddTo( from );

				from.CheckStatTimers();
				from.Delta( MobileDelta.WeaponDamage );
			}
		}

		public override void OnRemoved( object parent )
		{
			if ( parent is Mobile )
			{
				Mobile m = (Mobile)parent;
				BaseWeapon weapon = m.Weapon as BaseWeapon;

				string modName = Serial.ToString();

				m.RemoveStatMod( modName + "Str" );
				m.RemoveStatMod( modName + "Dex" );
				m.RemoveStatMod( modName + "Int" );

                if (m is PlayerMobile)
                    (m as PlayerMobile).WeaponTimerCheck();

				if ( weapon != null && m.Warmode)
					m.NextCombatTime = DateTime.UtcNow + weapon.GetDelay( m );

				if ( UseSkillMod && m_SkillMod != null )
				{
					m_SkillMod.Remove();
					m_SkillMod = null;
				}

				if ( m_MageMod != null )
				{
					m_MageMod.Remove();
					m_MageMod = null;
				}

				if ( Core.AOS )
					m_AosSkillBonuses.Remove();

                ImmolatingWeaponSpell.StopImmolating(this);

				m.CheckStatTimers();

				m.Delta( MobileDelta.WeaponDamage );
			}
		}

		public virtual SkillName GetUsedSkill( Mobile m, bool checkSkillAttrs )
		{
			SkillName sk;

			if ( checkSkillAttrs && m_AosWeaponAttributes.UseBestSkill != 0 )
			{
				double swrd = m.Skills[SkillName.Swords].Value;
				double fenc = m.Skills[SkillName.Fencing].Value;
				double mcng = m.Skills[SkillName.Macing].Value;

			    sk = SkillName.Swords;
				double val = swrd;

				if ( fenc > val ){ sk = SkillName.Fencing; val = fenc; }
				if ( mcng > val ){ sk = SkillName.Macing; val = mcng; }
			}
			else if ( m_AosWeaponAttributes.MageWeapon != 0 )
			{
				if ( m.Skills[SkillName.Magery].Value > m.Skills[Skill].Value )
					sk = SkillName.Magery;
				else
					sk = Skill;
			}
			else
			{
				sk = Skill;

				if ( sk != SkillName.Wrestling && !m.Player && !m.Body.IsHuman && m.Skills[SkillName.Wrestling].Value > m.Skills[sk].Value )
					sk = SkillName.Wrestling;
			}

			return sk;
		}

		public virtual double GetAttackSkillValue( Mobile attacker, Mobile defender )
		{
			return attacker.Skills[GetUsedSkill( attacker, true )].Value;
		}

		public virtual double GetDefendSkillValue( Mobile attacker, Mobile defender )
		{
			return defender.Skills[GetUsedSkill( defender, true )].Value;
		}

		private static bool CheckAnimal( Mobile m, Type type )
		{
			return AnimalForm.UnderTransformation( m, type );
		}


	    public virtual bool CheckHit( Mobile attacker, Mobile defender )
		{
            //MAKA - Currently monsters and players use the same algos
			if ( !Core.AOS )
                return CheckHit(attacker, defender, null);

            BaseWeapon atkWeapon = (BaseWeapon)attacker.Weapon;
            BaseWeapon defWeapon = (BaseWeapon)defender.Weapon;

            Skill atkSkill = attacker.Skills[atkWeapon.Skill];
            //Skill defSkill = defender.Skills[defWeapon.Skill];

            double atkValue = atkWeapon.GetAttackSkillValue(attacker, defender);
            double defValue = defWeapon.GetDefendSkillValue(attacker, defender);

            //attacker.CheckSkill( atkSkill.SkillName, defValue - 20.0, 120.0 );
            //defender.CheckSkill( defSkill.SkillName, atkValue - 20.0, 120.0 );

	        int bonus = GetHitChanceBonus();

	        if (atkValue <= -20.0)
	            atkValue = -19.9;

	        if (defValue <= -20.0)
	            defValue = -19.9;

	        // Hit Chance Increase = 45%
	        int atkChance = AosAttributes.GetValue(attacker, AosAttribute.AttackChance);
	        if (atkChance > 45)
	            atkChance = 45;

	        bonus += atkChance;

	        if (DivineFurySpell.UnderEffect(attacker))
	            bonus += 10; // attacker gets 10% bonus when they're under divine fury

	        if (CheckAnimal(attacker, typeof(GreyWolf)) || CheckAnimal(attacker, typeof(BakeKitsune)))
	            bonus += 20; // attacker gets 20% bonus when under Wolf or Bake Kitsune form

	        if (HitLower.IsUnderAttackEffect(attacker))
	            bonus -= 25; // Under Hit Lower Attack effect -> 25% malus

	        double ourValue = (atkValue + 20.0) * (100 + bonus);

	        // Defense Chance Increase = 45%
	        bonus = AosAttributes.GetValue(defender, AosAttribute.DefendChance);
	        if (bonus > 45)
	            bonus = 45;

	        if (DivineFurySpell.UnderEffect(defender))
	            bonus -= 20; // defender loses 20% bonus when they're under divine fury

	        if (HitLower.IsUnderDefenseEffect(defender))
	            bonus -= 25; // Under Hit Lower Defense effect -> 25% malus

	        int blockBonus = 0;

	        if (Block.GetBonus(defender, ref blockBonus))
	            bonus += blockBonus;

	        int surpriseMalus = 0;

	        if (SurpriseAttack.GetMalus(defender, ref surpriseMalus))
	            bonus -= surpriseMalus;

	        int discordanceEffect = 0;

	        // Defender loses -0/-28% if under the effect of Discordance.
	        if (Discordance.GetEffect(attacker, ref discordanceEffect))
	            bonus -= discordanceEffect;

	        double theirValue = (defValue + 20.0) * (100 + bonus);

	        bonus = 0;

	        double chance = ourValue / (theirValue * 2.0);

			chance *= 1.0 + ((double)bonus / 100);

			if ( Core.AOS && chance < 0.02 )
				chance = 0.02;

			WeaponAbility ability = WeaponAbility.GetCurrentAbility( attacker );

			if ( ability != null )
				chance *= ability.AccuracyScalar;

			SpecialMove move = SpecialMove.GetCurrentMove( attacker );

			if ( move != null )
				chance *= move.GetAccuracyScalar( attacker );

			return attacker.CheckSkill( atkSkill.SkillName, chance );
		}

        public virtual bool CheckHit(Mobile attacker, Mobile defender, PlayerMobile playerMobile)
        {
            BaseWeapon atkWeapon = (BaseWeapon) attacker.Weapon;

            Skill atkSkill = attacker.Skills[atkWeapon.Skill];

            //Maka: Remade hitting formula
            double hitChance = (m_SphereHitPercetage*((atkSkill.Value + (attacker.Skills.Tactics.Value*2))/(300)));

            //Skill gain, chance doesnt matter
            attacker.CheckSkill(attacker.FindItemOnLayer(Layer.OneHanded) is SmithHammer ? SkillName.Macing : atkSkill.SkillName, 1.0);

            #region HitChanceModifier
            /*
            double hitChanceModifier;

            if (playerMobile == null)
                hitChanceModifier = 1;
            else
            {
                if (playerMobile.LastSwingActionResult == PlayerMobile.SwingAction.Hit)
                    hitChanceModifier = m_HitChanceModifier[playerMobile.SwingCount];
                else
                    hitChanceModifier = 1 + (1 - m_HitChanceModifier[playerMobile.SwingCount]);
            }
            */
            #endregion


            return Utility.RandomDouble() <= (hitChance);// * hitChanceModifier);
        }

	    public virtual TimeSpan GetDelay(Mobile m)
        {
            BaseWeapon w = (BaseWeapon)m.Weapon;

            //Speed is in 1/100 of a second
            double speedInSeconds = ((double)w.Speed / 100);

            //Loki edit: High dex swing bonus and some limits for new PvP
                double bonusDex = m.Dex - 100;
                if (bonusDex > 25)
                    bonusDex = 25;
                else if (bonusDex < -50)
                    bonusDex = -50;

                if (!(m is PlayerMobile))
                    bonusDex = 0;

                if (bonusDex > 0)
                    speedInSeconds += speedInSeconds * bonusDex * 0.008;
                else if (bonusDex < 0)
                    speedInSeconds += speedInSeconds * bonusDex * 0.004;

            //Only players have "timed" swing anims, thats why we need to remove the anim
            //delay from the next hit time. Other mobiles have not and to therefore not need that
            if (m is PlayerMobile)
            {
                PlayerMobile pm = ((PlayerMobile) m);

                if (pm.SwingSpeed > 0)
                {
                    speedInSeconds = ((double)pm.SwingSpeed/100);
                    return (speedInSeconds - 1 < 0) ? TimeSpan.Zero : TimeSpan.FromSeconds(speedInSeconds - 1);
                }

                TimeSpan animDelay = GetAnimDelay(w);
                return (speedInSeconds - animDelay.TotalSeconds) < 0 ? TimeSpan.Zero : TimeSpan.FromSeconds(speedInSeconds - animDelay.TotalSeconds);
            }
	        return TimeSpan.FromSeconds(speedInSeconds);
        }

        public TimeSpan GetAnimDelay(BaseWeapon w)
        {
            if (w.Speed >= 430)
                return new TimeSpan(0, 0, 0, 1, 400);
            return new TimeSpan(0, 0, 1);
        }

	    public virtual void OnBeforeSwing( Mobile attacker, Mobile defender )
		{
            //WeaponAbility a = WeaponAbility.GetCurrentAbility( attacker );

            //if( a != null && !a.OnBeforeSwing( attacker, defender ) )
            //    WeaponAbility.ClearCurrentAbility( attacker );

            //SpecialMove move = SpecialMove.GetCurrentMove( attacker );

            //if( move != null && !move.OnBeforeSwing( attacker, defender ) )
            //    SpecialMove.ClearCurrentMove( attacker );
		}

		public virtual TimeSpan OnSwing( Mobile attacker, Mobile defender )
		{
            return OnSwing(attacker, defender, 1.0);
		}

		public virtual TimeSpan OnSwing( Mobile attacker, Mobile defender, double damageBonus )
		{
			if ( attacker.HarmfulCheck( defender ) )
			{
				attacker.DisruptiveAction();

				if ( attacker.NetState != null )
					attacker.Send( new Swing( 0, attacker, defender ) );

                if (attacker is BaseCreature)
                {   
                    if (!(attacker is BaseGuard))                                               //Taran: We prefer delayed anims before delayed hits (before you could run away 10 tiles before getting hit)
                        new HitDelayTimer((BaseCreature)attacker, defender, this, damageBonus, TimeSpan.FromMilliseconds(0)).Start(); //GetAnimDelay((BaseWeapon)attacker.Weapon)).Start();
                    else //Taran: Guards have a slight hitdelay so you can run for a bit if you are lucky
                        new HitDelayTimer((BaseCreature)attacker, defender, this, damageBonus, TimeSpan.FromMilliseconds(300)).Start();
                }
                else if (attacker is PlayerMobile)
                {
                    PlayerMobile pm = (PlayerMobile)attacker;

                    if (pm.CurrentSwingTimer == null)
                        new SwingDelayTimer(pm, defender, this, GetAnimDelay((BaseWeapon)attacker.Weapon)).Start();
                }

				SpellHelper.Turn(attacker, defender);
				PlaySwingAnimation(attacker);
			}

            return GetDelay( attacker );
		}

		// Yes yes, this is redundant and hackish. I will fix it later... - Jonny
        private class HitDelayTimer : Timer
        {
            private readonly BaseCreature m_Attacker;
            private readonly Mobile m_Attackee;
            private readonly BaseWeapon m_Weapon;
            private readonly double m_DamageBonus;

            public HitDelayTimer(BaseCreature attacker, Mobile attackee, BaseWeapon weapon, double damageBonus, TimeSpan delay)
                : base(delay)
            {
                m_Attacker = attacker;
                m_Attackee = attackee;
                m_Weapon = weapon;
                m_DamageBonus = damageBonus;
            }

            protected override void OnTick()
            {
                WeaponAbility ab = m_Attacker.GetWeaponAbility();

                if (ab != null)
                {
                    if (m_Attacker.WeaponAbilityChance > Utility.RandomDouble())
                        WeaponAbility.SetCurrentAbility(m_Attacker, ab);
                    else
                        WeaponAbility.ClearCurrentAbility(m_Attacker);
                }

                if (m_Weapon.CheckHit(m_Attacker, m_Attackee))
                    m_Weapon.OnHit(m_Attacker, m_Attackee, m_DamageBonus);
                else
                    m_Weapon.OnMiss(m_Attacker, m_Attackee);
            }
        }

        //TODO: Convert SwingDelayTimer to IAction implementation
        private class SwingDelayTimer : Timer
        {
            private readonly static TimeSpan NextHitChance = TimeSpan.FromSeconds(0.25);
            private const TimerPriority DelayTimerPriority = TimerPriority.TenMS;

            private readonly PlayerMobile m_Attacker;
            private readonly Mobile m_Defender;
            private readonly BaseWeapon m_Weapon;

            //TODO: Convert BandageCheck and SpellCheck to IAction implementation. 
            public SwingDelayTimer(PlayerMobile attacker, Mobile defender, BaseWeapon weapon, TimeSpan duration) : base(duration)
            {
                Priority = DelayTimerPriority;
                m_Attacker = attacker;
                m_Defender = defender;
                m_Weapon = weapon;

                m_Attacker.AbortCurrentPlayerAction();
                m_Attacker.CurrentSwingTimer = this;

                m_Attacker.BandageCheck();
                m_Attacker.SpellCheck();
            }

            protected override void OnTick()
            {
                /*
                Not really sure why these checks were here, but they seemed to mess up auto-hitting back
                There is another check for auto-attack back further down anyway (around line 1850) //Taran
                 
                //We don't attack if we are not in warmode or if our combatant has changed since the timer started
                if (!m_Attacker.Warmode || m_Attacker.Combatant != m_Defender)
                    return;

                We don't attack if our combatant has changed since the timer started
                if (m_Attacker.Combatant != m_Defender)
                    return;
               */

                if (m_Attacker.InRange(m_Defender, 1))
                {
                    m_Attacker.CurrentSwingTimer = null;
                    SpellHelper.Turn(m_Attacker, m_Defender);

                    if (m_Weapon.CheckHit(m_Attacker, m_Defender, m_Attacker))
                    {
                        if (m_Attacker.LastSwingActionResult == PlayerMobile.SwingAction.Hit)
                            m_Attacker.SwingCount++;
                        else
                        {
                            m_Attacker.LastSwingActionResult = PlayerMobile.SwingAction.Hit;
                            m_Attacker.SwingCount = 2;
                        }

                        m_Weapon.OnHit(m_Attacker, m_Defender);
                    }
                    else
                    {
                        if (m_Attacker.LastSwingActionResult == PlayerMobile.SwingAction.Miss)
                            m_Attacker.SwingCount++;
                        else
                        {
                            m_Attacker.LastSwingActionResult = PlayerMobile.SwingAction.Miss;
                            m_Attacker.SwingCount = 2;
                        }

                        m_Weapon.OnMiss(m_Attacker, m_Defender);
                    }
                }
                else
                    new SwingDelayTimer(m_Attacker, m_Defender, m_Weapon, NextHitChance).Start();
            }
        }


		#region Sounds
		public virtual int GetHitAttackSound( Mobile attacker, Mobile defender )
		{
            if (this is Fists)
            {
                int sound = attacker.GetAttackSound();

                if (sound == -1)
                    sound = HitSound;

                return sound;
            }

		    return HitSound;
		}

		public virtual int GetHitDefendSound( Mobile attacker, Mobile defender )
		{
			return defender.GetHurtSound();
		}

		public virtual int GetMissAttackSound( Mobile attacker, Mobile defender )
		{
            if (this is Fists)
            {
                int sound = attacker.GetAttackSound();

                if (sound == -1)
                    sound = MissSound;

                return sound;
            }

		    return MissSound;
		}

	    public virtual int GetMissDefendSound( Mobile attacker, Mobile defender )
		{
			return -1;
		}
		#endregion

		public static bool CheckParry( Mobile defender )
		{
			if ( defender == null )
				return false;

			BaseShield shield = defender.FindItemOnLayer( Layer.TwoHanded ) as BaseShield;

			double parry = defender.Skills[SkillName.Parry].Value;
			double bushidoNonRacial = defender.Skills[SkillName.Bushido].NonRacialValue;
			double bushido = defender.Skills[SkillName.Bushido].Value;

			if ( shield != null )
			{
                double chance = (parry - bushidoNonRacial) / 400.0;	// As per OSI, no negitive effect from the Racial stuffs, ie, 120 parry and '0' bushido with humans

                if (chance < 0) // chance shouldn't go below 0
                    chance = 0;

                // Parry/Bushido over 100 grants a 5% bonus.
                if (parry >= 100.0 || bushido >= 100.0)
					chance += 0.05;

				// Evasion grants a variable bonus post ML. 50% prior.
				if ( Evasion.IsEvading( defender ) )
                    chance *= Evasion.GetParryScalar( defender );

				// Low dexterity lowers the chance.
				if ( defender.Dex < 80 )
					chance = chance * (20 + defender.Dex) / 100;

				return defender.CheckSkill( SkillName.Parry, chance );
			}
		    if ( !(defender.Weapon is Fists) && !(defender.Weapon is BaseRanged) )
		    {
		        BaseWeapon weapon = (BaseWeapon)defender.Weapon;

		        double divisor = (weapon.Layer == Layer.OneHanded) ? 48000.0 : 41140.0;

		        double chance = (parry * bushido) / divisor;

		        double aosChance = parry / 800.0;

		        // Parry or Bushido over 100 grant a 5% bonus.
		        if( parry >= 100.0 )
		        {
		            chance += 0.05;
		            aosChance += 0.05;
		        }
		        else if( bushido >= 100.0 )
		        {
		            chance += 0.05;
		        }

		        // Evasion grants a variable bonus post ML. 50% prior.
		        if( Evasion.IsEvading( defender ) )
		            chance *= Evasion.GetParryScalar( defender );

		        // Low dexterity lowers the chance.
		        if( defender.Dex < 80 )
		            chance = chance * (20 + defender.Dex) / 100;

		        if ( chance > aosChance )
		            return defender.CheckSkill( SkillName.Parry, chance );
		        return (aosChance > Utility.RandomDouble()); // Only skillcheck if wielding a shield & there's no effect from Bushido
		    }

		    return false;
		}

		public virtual int AbsorbDamageAOS( Mobile attacker, Mobile defender, int damage )
		{
			bool blocked = false;

			if ( defender.Player || defender.Body.IsHuman )
			{
				blocked = CheckParry( defender );

				if ( blocked )
				{
					defender.FixedEffect( 0x37B9, 10, 16 );
					damage = 0;

					// Successful block removes the Honorable Execution penalty.
					HonorableExecution.RemovePenalty( defender );

					if ( CounterAttack.IsCountering( defender ) )
					{
						BaseWeapon weapon = defender.Weapon as BaseWeapon;

                        if (weapon != null)
                        {
                            defender.FixedParticles(0x3779, 1, 15, 0x158B, 0x0, 0x3, EffectLayer.Waist);
                            weapon.OnSwing(defender, attacker);
                        }

					    CounterAttack.StopCountering( defender );
					}

					if ( Confidence.IsConfident( defender ) )
					{
						defender.SendLocalizedMessage( 1063117 ); // Your confidence reassures you as you successfully block your opponent's blow.

						double bushido = defender.Skills.Bushido.Value;

						defender.Hits += Utility.RandomMinMax( 1, (int)(bushido / 12) );
						defender.Stam += Utility.RandomMinMax( 1, (int)(bushido / 5) );
					}

					BaseShield shield = defender.FindItemOnLayer( Layer.TwoHanded ) as BaseShield;

					if ( shield != null )
					{
						shield.OnHit( this, damage );
					}
				}
			}

			if ( !blocked )
			{
				double positionChance = Utility.RandomDouble();

				Item armorItem;

				if( positionChance < 0.07 )
					armorItem = defender.NeckArmor;
				else if( positionChance < 0.14 )
					armorItem = defender.HandArmor;
				else if( positionChance < 0.28 )
					armorItem = defender.ArmsArmor;
				else if( positionChance < 0.43 )
					armorItem = defender.HeadArmor;
				else if( positionChance < 0.65 )
					armorItem = defender.LegsArmor;
				else
					armorItem = defender.ChestArmor;

				IWearableDurability armor = armorItem as IWearableDurability;

				if ( armor != null )
					armor.OnHit( this, damage ); // call OnHit to lose durability
			}

			return damage;
		}

		public virtual int AbsorbDamage( Mobile attacker, Mobile defender, int damage )
		{
			if ( Core.AOS )
				return AbsorbDamageAOS( attacker, defender, damage );

			double chance = Utility.RandomDouble();

			Item armorItem;

			if( chance < 0.07 )
				armorItem = defender.NeckArmor;
			else if( chance < 0.14 )
				armorItem = defender.HandArmor;
			else if( chance < 0.28 )
				armorItem = defender.ArmsArmor;
			else if( chance < 0.43 )
				armorItem = defender.HeadArmor;
			else if( chance < 0.65 )
				armorItem = defender.LegsArmor;
			else
				armorItem = defender.ChestArmor;

			IWearableDurability armor = armorItem as IWearableDurability;

            //Maka: Reduce damage even if no armor were hit
            if (armor == null)
            {
				if( chance > 0.05 )
					damage = m_GenericOnHitPlate.OnHit( this, damage, defender as PlayerMobile );
				else //Critical hit 5%
					damage = (int)( OldMaxDamage + ( (double)GetDamageBonus() ) / 2 );
            }
            else
            {
                if (chance > 0.05)
                {
                    if (armorItem is BaseArmor)
                        damage = ((BaseArmor) armorItem).OnHit(this, damage, defender as PlayerMobile);
                    else //For cloths ?
                        damage = armor.OnHit(this, damage);
                }
                else //Critical hit 5%
                    damage = (int)(OldMaxDamage + ((double)GetDamageBonus()) / 2);
            }

		    BaseShield shield = defender.FindItemOnLayer( Layer.TwoHanded ) as BaseShield;

			if ( shield != null )
				damage = shield.OnHit( this, damage );

            //int virtualArmor = defender.VirtualArmor + defender.VirtualArmorMod;

            //if ( virtualArmor > 0 )
            //{
            //    double scalar;

            //    if ( chance < 0.14 )
            //        scalar = 0.07;
            //    else if ( chance < 0.28 )
            //        scalar = 0.14;
            //    else if ( chance < 0.43 )
            //        scalar = 0.15;
            //    else if ( chance < 0.65 )
            //        scalar = 0.22;
            //    else
            //        scalar = 0.35;

            //    int from = (int)(virtualArmor * scalar) / 2;
            //    int to = (int)(virtualArmor * scalar);

            //    damage -= Utility.Random( from, (to - from) + 1 );
            //}

			return damage;
		}

		public virtual int GetPackInstinctBonus( Mobile attacker, Mobile defender )
		{
			if ( attacker.Player || defender.Player )
				return 0;

			BaseCreature bc = attacker as BaseCreature;

			if ( bc == null || bc.PackInstinct == PackInstinct.None || (!bc.Controlled && !bc.Summoned) )
				return 0;

			Mobile master = bc.ControlMaster;

			if ( master == null )
				master = bc.SummonMaster;

			if ( master == null )
				return 0;

			int inPack = 1;

			foreach ( Mobile m in defender.GetMobilesInRange( 1 ) )
			{
				if ( m != attacker && m is BaseCreature )
				{
					BaseCreature tc = (BaseCreature)m;

					if ( (tc.PackInstinct & bc.PackInstinct) == 0 || (!tc.Controlled && !tc.Summoned) )
						continue;

					Mobile theirMaster = tc.ControlMaster;

					if ( theirMaster == null )
						theirMaster = tc.SummonMaster;

					if ( master == theirMaster && tc.Combatant == defender )
						++inPack;
				}
			}

			if ( inPack >= 5 )
				return 100;
		    if ( inPack >= 4 )
		        return 75;
		    if ( inPack >= 3 )
		        return 50;
		    if ( inPack >= 2 )
		        return 25;

		    return 0;
		}

		private static bool m_InDoubleStrike;

		public static bool InDoubleStrike
		{
			get => m_InDoubleStrike;
            set => m_InDoubleStrike = value;
        }

		public virtual void OnHit( Mobile attacker, Mobile defender )
		{
			OnHit( attacker, defender, 1.0 );
		}

        public virtual void OnHit(Mobile attacker, Mobile defender, double damageBonus)
        {
            //Allows special attack if the attacker is a base creature, or if the defender is not a player mobile. Not allowed in PvP.
            bool allowSpecialAbilities = (attacker is BaseCreature) || !(defender is PlayerMobile);

            if (MirrorImage.HasClone(defender) && (defender.Skills.Ninjitsu.Value/150.0) > Utility.RandomDouble())
            {
                Clone bc;

                foreach (Mobile m in defender.GetMobilesInRange(4))
                {
                    bc = m as Clone;

                    if (bc != null && bc.Summoned && bc.SummonMaster == defender)
                    {
                        attacker.SendLocalizedMessage(1063141); // Your attack has been diverted to a nearby mirror image of your target!
                        defender.SendLocalizedMessage(1063140); // You manage to divert the attack onto one of your nearby mirror images.

                        /*
                         * TODO: What happens if the Clone parries a blow?
                         * And what about if the attacker is using Honorable Execution
                         * and kills it?
                         */

                        defender = m;
                        break;
                    }
                }
            }

            PlayHurtAnimation(defender);
            HitMessages(attacker, defender);

            attacker.PlaySound(GetHitAttackSound(attacker, defender));
            defender.PlaySound(GetHitDefendSound(attacker, defender));

            int damage;

            if (attacker is PlayerMobile)
            {
                PlayerMobile pm = ((PlayerMobile) attacker);
                if (pm.MaxDamage > 0 && pm.MinDamage <= pm.MaxDamage)
                    damage = Utility.RandomMinMax(pm.MinDamage, pm.MaxDamage); //Polymorphed damage
                else
                    damage = ComputeDamage(attacker, defender);
            }
            else
                damage = ComputeDamage(attacker, defender);

            #region Damage Multipliers

            /*
			 * The following damage bonuses multiply damage by a factor.
			 * Capped at x3 (300%).
			 */

            int percentageBonus = 0;

            if (damageBonus > 1)
                percentageBonus = (int)((damageBonus - 1) * 100) + 1;

            WeaponAbility a = null;
            SpecialMove move = null;

            //Only allow creatures special moves, players cannot have them yet
            if (allowSpecialAbilities)
            {
                a = WeaponAbility.GetCurrentAbility(attacker);
                move = SpecialMove.GetCurrentMove(attacker);

                if (a != null)
                    percentageBonus += (int) (a.DamageScalar*100) - 100;

                if (move != null)
                    percentageBonus += (int) (move.GetDamageScalar(attacker, defender)*100) - 100;

                percentageBonus += (int) (damageBonus*100) - 100;
            }

            CheckSlayerResult cs = CheckSlayers(attacker, defender);

            if (cs != CheckSlayerResult.None)
            {
                if (cs == CheckSlayerResult.Slayer)
                {
                    defender.FixedEffect(0x37B9, 10, 5);
                    percentageBonus += 25;
                }
            }

            //Silver weapons deal 25% less damage to players
            if (defender is PlayerMobile && (m_Slayer == SlayerName.Silver || m_Slayer2 == SlayerName.Silver))
                percentageBonus -= 25;

            /*
            if (allowSpecialAbilities)
            {
                if (!attacker.Player)
                {
                    if (defender is PlayerMobile)
                    {
                        PlayerMobile pm = (PlayerMobile) defender;

                        if (pm.EnemyOfOneType != null && pm.EnemyOfOneType != attacker.GetType())
                        {
                            percentageBonus += 100;
                        }
                    }
                }
                else if (!defender.Player)
                {
                    if (attacker is PlayerMobile)
                    {
                        PlayerMobile pm = (PlayerMobile) attacker;

                        if (pm.WaitingForEnemy)
                        {
                            pm.EnemyOfOneType = defender.GetType();
                            pm.WaitingForEnemy = false;
                        }

                        if (pm.EnemyOfOneType == defender.GetType())
                        {
                            defender.FixedEffect(0x37B9, 10, 5, 1160, 0);
                            percentageBonus += 50;
                        }
                    }
                }
             

                int packInstinctBonus = GetPackInstinctBonus(attacker, defender);

                if (packInstinctBonus != 0)
                {
                    percentageBonus += packInstinctBonus;
                }

                if (m_InDoubleStrike)
                {
                    percentageBonus -= 10;
                }
            }
            */
            
            TransformContext context = TransformationSpellHelper.GetContext(defender);
            /*
            if (context != null && context.Spell is NecromancerSpell && context.Type != typeof (HorrificBeastSpell))
            {
                factor *= 1.25; // Every necromancer transformation other than horrific beast takes an additional 25% damage
            }
            */
            //No PvP bonuses
            //if (attacker is PlayerMobile && !(Core.ML && defender is PlayerMobile))
            //{
            //    PlayerMobile pmAttacker = (PlayerMobile)attacker;

            //    if (pmAttacker.HonorActive && pmAttacker.InRange(defender, 1))
            //    {
            //        //factor *= 1.25;
            //        percentageBonus += 25;
            //    }

            //    if (pmAttacker.SentHonorContext != null && pmAttacker.SentHonorContext.Target == defender)
            //    {
            //        //pmAttacker.SentHonorContext.ApplyPerfectionDamageBonus( ref factor );
            //        percentageBonus += pmAttacker.SentHonorContext.PerfectionDamageBonus;
            //    }
            //}

            //if ( factor > 3.0 )
            //	factor = 3.0;

            BaseTalisman talisman = attacker.Talisman as BaseTalisman;

            if (talisman != null && talisman.Killer != null)
                percentageBonus += talisman.Killer.DamageBonus(defender);

            percentageBonus = Math.Min(percentageBonus, 300);

            damage = AOS.Scale(damage, 100 + percentageBonus);

            damage = attacker.ChangeDamage(defender, damage);
            
            #endregion

            if (attacker is BaseCreature)
                ((BaseCreature) attacker).AlterMeleeDamageTo(defender, ref damage);

            if (defender is BaseCreature)
                ((BaseCreature) defender).AlterMeleeDamageFrom(attacker, ref damage);

            damage = AbsorbDamage(attacker, defender, damage);

            if (!Core.AOS && damage < 1)
                damage = 1;
            //else if (Core.AOS && damage == 0) // parried
            //{
            //    if (a != null && a.Validate(attacker) /*&& a.CheckMana( attacker, true )*/ ) // Parried special moves have no mana cost 
            //    {
            //        a = null;
            //        WeaponAbility.ClearCurrentAbility(attacker);

            //        attacker.SendLocalizedMessage(1061140); // Your attack was parried!
            //    }
            //}

            AddBlood(attacker, defender, damage);

            int phys, fire, cold, pois, nrgy, chaos, direct;

            GetDamageTypes(attacker, out phys, out fire, out cold, out pois, out nrgy, out chaos, out direct);

            if (Core.ML && this is BaseRanged)
            {
                BaseQuiver quiver = attacker.FindItemOnLayer(Layer.Cloak) as BaseQuiver;

                if (quiver != null)
                    quiver.AlterBowDamage(ref phys, ref fire, ref cold, ref pois, ref nrgy, ref chaos, ref direct);
            }

            if (m_Consecrated)
            {
                phys = defender.PhysicalResistance;
                fire = defender.FireResistance;
                cold = defender.ColdResistance;
                pois = defender.PoisonResistance;
                nrgy = defender.EnergyResistance;

                int low = phys, type = 0;

                if (fire < low)
                {
                    low = fire;
                    type = 1;
                }
                if (cold < low)
                {
                    low = cold;
                    type = 2;
                }
                if (pois < low)
                {
                    low = pois;
                    type = 3;
                }
                if (nrgy < low)
                {
                    //low = nrgy;
                    type = 4;
                }

                phys = fire = cold = pois = nrgy = chaos = direct = 0;

                if (type == 0) phys = 100;
                else if (type == 1) fire = 100;
                else if (type == 2) cold = 100;
                else if (type == 3) pois = 100;
                else if (type == 4) nrgy = 100;
            }

            // TODO: Scale damage, alongside the leech effects below, to weapon speed.
            if (ImmolatingWeaponSpell.IsImmolating(this) && damage > 0)
                ImmolatingWeaponSpell.DoEffect(this, defender);

            //Creatures can use special abilities on everyone, players can use them on creatures

            //Playes can attack creatures and creatures can attack players, PvP disabled.

            #region SpecialMoves and WeaponAbilities

            if (a != null && !a.OnBeforeDamage(attacker, defender))
            {
                WeaponAbility.ClearCurrentAbility(attacker);
                a = null;
            }

            if (move != null && !move.OnBeforeDamage(attacker, defender))
            {
                SpecialMove.ClearCurrentMove(attacker);
                move = null;
            }

            bool ignoreArmor = (a is ArmorIgnore || (move != null && move.IgnoreArmor(attacker)));

            #endregion

            int damageGiven = AOS.Damage(defender, attacker, damage, ignoreArmor, phys, fire, cold, pois, nrgy, chaos, direct, false, this is BaseRanged);
            
            //Auto attack on hit
            if (defender is PlayerMobile)
            {
                if (defender.Region is CustomRegion && ((CustomRegion)defender.Region).Controller.FizzlePvP)
                {
                    ((PlayerMobile)defender).BandageCheck();
                }

                //Only attack back if you're not casting a spell (to avoid easy fizzling)
                if (defender.Spell == null || !defender.Spell.IsCasting)
                {
                    PlayerMobile pm = (PlayerMobile) defender;

                    if (pm.Combatant == null && pm.Combatant != attacker)
                        pm.Combatant = attacker;

                    //Autoattack someone if we have if it was a long time since we attacked them
                    //or if our new combat time is far in the future. Auto attacking reacts slower then manual attacking.
                    if (pm.NextCombatTime > DateTime.UtcNow + TimeSpan.FromSeconds(GetDelay(pm).TotalSeconds*3) ||
                        (DateTime.UtcNow - TimeSpan.FromSeconds(GetDelay(pm).TotalSeconds*3)) > pm.NextCombatTime)
                        pm.NextCombatTime = DateTime.UtcNow + TimeSpan.FromSeconds(GetDelay(pm).TotalSeconds*3);
                }
            }
            
            double propertyBonus = (move == null) ? 1.0 : move.GetPropertyBonus(attacker);

            if (allowSpecialAbilities) // if (Core.AOS)
            {
                int lifeLeech = 0;
                int stamLeech = 0;
                int manaLeech = 0;
                int wraithLeech;

                if ((int) (m_AosWeaponAttributes.HitLeechHits*propertyBonus) > Utility.Random(100))
                    lifeLeech += 30; // HitLeechHits% chance to leech 30% of damage as hit points

                if ((int) (m_AosWeaponAttributes.HitLeechStam*propertyBonus) > Utility.Random(100))
                    stamLeech += 100; // HitLeechStam% chance to leech 100% of damage as stamina

                if ((int) (m_AosWeaponAttributes.HitLeechMana*propertyBonus) > Utility.Random(100))
                    manaLeech += 40; // HitLeechMana% chance to leech 40% of damage as mana

                if (m_Cursed)
                    lifeLeech += 50; // Additional 50% life leech for cursed weapons (necro spell)

                context = TransformationSpellHelper.GetContext(attacker);

                if (context != null && context.Type == typeof (VampiricEmbraceSpell))
                    lifeLeech += 20; // Vampiric embrace gives an additional 20% life leech

                if (context != null && context.Type == typeof (WraithFormSpell))
                {
                    wraithLeech = (5 + (int) ((15*attacker.Skills.SpiritSpeak.Value)/100)); // Wraith form gives an additional 5-20% mana leech

                    // Mana leeched by the Wraith Form spell is actually stolen, not just leeched.
                    defender.Mana -= AOS.Scale(damageGiven, wraithLeech);

                    manaLeech += wraithLeech;
                }

                if (lifeLeech != 0)
                    attacker.Hits += AOS.Scale(damageGiven, lifeLeech);

                if (stamLeech != 0)
                    attacker.Stam += AOS.Scale(damageGiven, stamLeech);

                if (manaLeech != 0)
                    attacker.Mana += AOS.Scale(damageGiven, manaLeech);

                if (lifeLeech != 0 || stamLeech != 0 || manaLeech != 0)
                    attacker.PlaySound(0x44D);
            }

            if (m_MaxHits > 0 && ((MaxRange <= 1 && (defender is Slime || defender is ToxicElemental)) || Utility.Random(25) == 0)) // Stratics says 50% chance, seems more like 4%..
            {
                if (DurabilityLevel != WeaponDurabilityLevel.Indestructible)
                {
                    if (MaxRange <= 1 && (defender is Slime || defender is ToxicElemental))
                        attacker.LocalOverheadMessage(MessageType.Regular, 0x3B2, 500263);
                            // *Acid blood scars your weapon!*

                    if (m_AosWeaponAttributes.SelfRepair > Utility.Random(10))
                    {
                        HitPoints += 2;
                    }
                    else
                    {
                        if (m_Hits > 0)
                        {
                            --HitPoints;
                        }
                        else if (m_MaxHits > 1)
                        {
                            --MaxHitPoints;

                            if (Parent is Mobile)
                                ((Mobile) Parent).LocalOverheadMessage(MessageType.Regular, 0x3B2, 1061121);
                                    // Your equipment is severely damaged.
                        }
                        else
                        {
                            if (Parent is Mobile)
                                ((Mobile)Parent).PublicOverheadMessage(MessageType.Emote, 0x22, true, string.Format("*{0}'s {1} is destroyed", ((Mobile)Parent).Name, string.IsNullOrEmpty(Name) ? CliLoc.LocToString(LabelNumber) : Name));

                            Delete();
                        }
                    }
                }
            }

            if (attacker is VampireBatFamiliar)
            {
                BaseCreature bc = (BaseCreature) attacker;
                Mobile caster = bc.ControlMaster;

                if (caster == null)
                    caster = bc.SummonMaster;

                if (caster != null && caster.Map == bc.Map && caster.InRange(bc, 2))
                    caster.Hits += damage;
                else
                    bc.Hits += damage;
            }

            if (allowSpecialAbilities) //( if (Core.AOS)
            {
                int physChance = (int) (m_AosWeaponAttributes.HitPhysicalArea*propertyBonus);
                int fireChance = (int) (m_AosWeaponAttributes.HitFireArea*propertyBonus);
                int coldChance = (int) (m_AosWeaponAttributes.HitColdArea*propertyBonus);
                int poisChance = (int) (m_AosWeaponAttributes.HitPoisonArea*propertyBonus);
                int nrgyChance = (int) (m_AosWeaponAttributes.HitEnergyArea*propertyBonus);

                if (physChance != 0 && physChance > Utility.Random(100))
                    DoAreaAttack(attacker, defender, 0x10E, 50, 100, 0, 0, 0, 0);

                if (fireChance != 0 && fireChance > Utility.Random(100))
                    DoAreaAttack(attacker, defender, 0x11D, 1160, 0, 100, 0, 0, 0);

                if (coldChance != 0 && coldChance > Utility.Random(100))
                    DoAreaAttack(attacker, defender, 0x0FC, 2100, 0, 0, 100, 0, 0);

                if (poisChance != 0 && poisChance > Utility.Random(100))
                    DoAreaAttack(attacker, defender, 0x205, 1166, 0, 0, 0, 100, 0);

                if (nrgyChance != 0 && nrgyChance > Utility.Random(100))
                    DoAreaAttack(attacker, defender, 0x1F1, 120, 0, 0, 0, 0, 100);

                int maChance = (int) (m_AosWeaponAttributes.HitMagicArrow*propertyBonus);
                int harmChance = (int) (m_AosWeaponAttributes.HitHarm*propertyBonus);
                int fireballChance = (int) (m_AosWeaponAttributes.HitFireball*propertyBonus);
                int lightningChance = (int) (m_AosWeaponAttributes.HitLightning*propertyBonus);
                int dispelChance = (int) (m_AosWeaponAttributes.HitDispel*propertyBonus);

                if (maChance != 0 && maChance > Utility.Random(100))
                    DoMagicArrow(attacker, defender);

                if (harmChance != 0 && harmChance > Utility.Random(100))
                    DoHarm(attacker, defender);

                if (fireballChance != 0 && fireballChance > Utility.Random(100))
                    DoFireball(attacker, defender);

                if (lightningChance != 0 && lightningChance > Utility.Random(100))
                    DoLightning(attacker, defender);

                if (dispelChance != 0 && dispelChance > Utility.Random(100))
                    DoDispel(attacker, defender);

                int laChance = (int) (m_AosWeaponAttributes.HitLowerAttack*propertyBonus);
                int ldChance = (int) (m_AosWeaponAttributes.HitLowerDefend*propertyBonus);

                if (laChance != 0 && laChance > Utility.Random(100))
                    DoLowerAttack(attacker, defender);

                if (ldChance != 0 && ldChance > Utility.Random(100))
                    DoLowerDefense(attacker, defender);
            }

            if (attacker is BaseCreature)
                ((BaseCreature) attacker).OnGaveMeleeAttack(defender);

            if (defender is BaseCreature)
                ((BaseCreature) defender).OnGotMeleeAttack(attacker);

            //Moves and hits are null if it is a PvP situation
            if (a != null)
                a.OnHit(attacker, defender, damage);

            if (move != null)
                move.OnHit(attacker, defender, damage);

            if (defender is IHonorTarget && ((IHonorTarget) defender).ReceivedHonorContext != null)
                ((IHonorTarget) defender).ReceivedHonorContext.OnTargetHit(attacker);

            if (!(this is BaseRanged))
            {
                if (AnimalForm.UnderTransformation(attacker, typeof (GiantSerpent)))
                    defender.ApplyPoison(attacker, Poison.Lesser);

                if (AnimalForm.UnderTransformation(defender, typeof (BullFrog)))
                    attacker.ApplyPoison(defender, Poison.Regular);
            }

            // hook for attachment OnWeaponHit method
            Engines.XmlSpawner2.XmlAttach.OnWeaponHit(this, attacker, defender, damageGiven);
            ApplyPoison(attacker, defender);
        }

        public virtual void ApplyPoison(Mobile attacker, Mobile defender)
        {
	        if (!Core.AOS && Poison != null && PoisonCharges > 0)
	        {
		        --PoisonCharges;
		        Poison.AddDelay = BaseWeapon.CalculatePoisonDelay(Poison);
		        if (Utility.RandomDouble() >= 0.5) // 50% chance to poison
			        defender.ApplyPoison(attacker, Poison);
		        Poison.AddDelay = new TimeSpan(0);
	        }
		}

        public static TimeSpan CalculatePoisonDelay(Poison poison)
        {
            return new TimeSpan(0, 0, Utility.LimitMinMax(1, (5 - poison.Level) * 10, 50));
        }

        #region OldOnHitINX
        //        public virtual void OnHit( Mobile attacker, Mobile defender, double damageBonus )
        //        {
        //            if ( MirrorImage.HasClone( defender ) && (defender.Skills.Ninjitsu.Value / 150.0) > Utility.RandomDouble() )
        //            {
        //                Clone bc;

        //                foreach ( Mobile m in defender.GetMobilesInRange( 4 ) )
        //                {
        //                    bc = m as Clone;

        //                    if ( bc != null && bc.Summoned && bc.SummonMaster == defender )
        //                    {
        //                        attacker.SendLocalizedMessage( 1063141 ); // Your attack has been diverted to a nearby mirror image of your target!
        //                        defender.SendLocalizedMessage( 1063140 ); // You manage to divert the attack onto one of your nearby mirror images.

        //                        
        //                         * TODO: What happens if the Clone parries a blow?
        //                         * And what about if the attacker is using Honorable Execution
        //                         * and kills it?
        //                         

        //                        defender = m;
        //                        break;
        //                    }
        //                }
        //            }

        //            PlayHurtAnimation( defender );
        //            HitMessages(attacker, defender);

        //            attacker.PlaySound( GetHitAttackSound( attacker, defender ) );
        //            defender.PlaySound( GetHitDefendSound( attacker, defender ) );

        //            int damage = ComputeDamage( attacker, defender );

        //            #region Damage Multipliers
        //            //double factor = 1.0;
        //            int percentageBonus = 0;

        //            //WeaponAbility a = WeaponAbility.GetCurrentAbility(attacker);
        //            //SpecialMove move = SpecialMove.GetCurrentMove(attacker);

        //            //if (a != null)
        //            //{
        //            //    //factor *= a.DamageScalar;
        //            //    percentageBonus += (int)(a.DamageScalar * 100) - 100;
        //            //}

        //            //if (move != null)
        //            //{
        //            //    //factor *= move.GetDamageScalar( attacker, defender );
        //            //    percentageBonus += (int)(move.GetDamageScalar(attacker, defender) * 100) - 100;
        //            //}

        //            ////factor *= damageBonus;
        //            //percentageBonus += (int)(damageBonus * 100) - 100;

        //            CheckSlayerResult cs = CheckSlayers(attacker, defender);

        //            if (cs != CheckSlayerResult.None)
        //            {
        //                if (cs == CheckSlayerResult.Slayer)
        //                    defender.FixedEffect(0x37B9, 10, 5);

        //                //factor *= 2.0;
        //                percentageBonus += 100;
        //            }

        //            if (!attacker.Player)
        //            {
        //                if (defender is PlayerMobile)
        //                {
        //                    PlayerMobile pm = (PlayerMobile)defender;

        //                    if (pm.EnemyOfOneType != null && pm.EnemyOfOneType != attacker.GetType())
        //                    {
        //                        //factor *= 2.0;
        //                        percentageBonus += 100;
        //                    }
        //                }
        //            }
        //            //else if (!defender.Player)
        //            //{
        //            //    if (attacker is PlayerMobile)
        //            //    {
        //            //        PlayerMobile pm = (PlayerMobile)attacker;

        //            //        if (pm.WaitingForEnemy)
        //            //        {
        //            //            pm.EnemyOfOneType = defender.GetType();
        //            //            pm.WaitingForEnemy = false;
        //            //        }

        //            //        if (pm.EnemyOfOneType == defender.GetType())
        //            //        {
        //            //            defender.FixedEffect(0x37B9, 10, 5, 1160, 0);
        //            //            //factor *= 1.5;
        //            //            percentageBonus += 50;
        //            //        }
        //            //    }
        //            //}

        //            //int packInstinctBonus = GetPackInstinctBonus(attacker, defender);

        //            //if (packInstinctBonus != 0)
        //            //{
        //            //    //factor *= 1.0 + (double)packInstinctBonus / 100.0;
        //            //    percentageBonus += packInstinctBonus;
        //            //}

        //            //if (m_InDoubleStrike)
        //            //{
        //            //    //factor *= 0.9; // 10% loss when attacking with double-strike
        //            //    percentageBonus -= 10;
        //            //}

        //            TransformContext context = TransformationSpellHelper.GetContext(defender);

        //            //if ((m_Slayer == SlayerName.Silver || m_Slayer2 == SlayerName.Silver) && context != null && context.Spell is NecromancerSpell && context.Type != typeof(HorrificBeastSpell))
        //            //{
        //            //    //factor *= 1.25; // Every necromancer transformation other than horrific beast takes an additional 25% damage
        //            //    percentageBonus += 25;
        //            //}

        //            //if (attacker is PlayerMobile && !(Core.ML && defender is PlayerMobile))
        //            //{
        //            //    PlayerMobile pmAttacker = (PlayerMobile)attacker;

        //            //    if (pmAttacker.HonorActive && pmAttacker.InRange(defender, 1))
        //            //    {
        //            //        //factor *= 1.25;
        //            //        percentageBonus += 25;
        //            //    }

        //            //    if (pmAttacker.SentHonorContext != null && pmAttacker.SentHonorContext.Target == defender)
        //            //    {
        //            //        //pmAttacker.SentHonorContext.ApplyPerfectionDamageBonus( ref factor );
        //            //        percentageBonus += pmAttacker.SentHonorContext.PerfectionDamageBonus;
        //            //    }
        //            //}

        //            ////if ( factor > 3.0 )
        //            ////	factor = 3.0;

        //            //percentageBonus = Math.Min(percentageBonus, 300);

        //            ////damage = (int)(damage * factor);
        //            //damage = AOS.Scale(damage, 100 + percentageBonus);
        //            #endregion

        //            if ( attacker is BaseCreature )
        //                ((BaseCreature)attacker).AlterMeleeDamageTo( defender, ref damage );

        //            if ( defender is BaseCreature )
        //                ((BaseCreature)defender).AlterMeleeDamageFrom( attacker, ref damage );

        //            damage = AbsorbDamage( attacker, defender, damage );

        //            if ( !Core.AOS && damage < 1 )
        //                damage = 1;

        //            AddBlood( attacker, defender, damage );

        //            int phys, fire, cold, pois, nrgy;

        //            GetDamageTypes( attacker, out phys, out fire, out cold, out pois, out nrgy );

        //            if ( m_Consecrated )
        //            {
        //                phys = defender.PhysicalResistance;
        //                fire = defender.FireResistance;
        //                cold = defender.ColdResistance;
        //                pois = defender.PoisonResistance;
        //                nrgy = defender.EnergyResistance;

        //                int low = phys, type = 0;

        //                if ( fire < low ){ low = fire; type = 1; }
        //                if ( cold < low ){ low = cold; type = 2; }
        //                if ( pois < low ){ low = pois; type = 3; }
        //                if ( nrgy < low ){ low = nrgy; type = 4; }

        //                phys = fire = cold = pois = nrgy = 0;

        //                if ( type == 0 ) phys = 100;
        //                else if ( type == 1 ) fire = 100;
        //                else if ( type == 2 ) cold = 100;
        //                else if ( type == 3 ) pois = 100;
        //                else if ( type == 4 ) nrgy = 100;
        //            }

        //            //if ( a != null && !a.OnBeforeDamage( attacker, defender ) )
        //            //{
        //            //    WeaponAbility.ClearCurrentAbility( attacker );
        //            //    a = null;
        //            //}

        //            //if ( move != null && !move.OnBeforeDamage( attacker, defender ) )
        //            //{
        //            //    SpecialMove.ClearCurrentMove( attacker );
        //            //    move = null;
        //            //}

        //            bool ignoreArmor = false; //(a is ArmorIgnore || (move != null && move.IgnoreArmor(attacker)));

        //            damageGiven = AOS.Damage( defender, attacker, damage, ignoreArmor, phys, fire, cold, pois, nrgy, chaos, direct, false, this is BaseRanged, false );

        //            //Auto attack on hit
        //            if (defender is PlayerMobile)
        //            {
        //                PlayerMobile pm = (PlayerMobile)defender;

        //                if (pm.Combatant == null && pm.Combatant != attacker)
        //                    pm.Combatant = attacker;

        //                //Autoattack someone if we have if it was a long time since we attacked them
        //                //or if our new combat time is far in the future. Auto attacking reacts slower then manual attacking.
        //                if (pm.NextCombatTime > DateTime.UtcNow + TimeSpan.FromSeconds(GetDelay(pm).TotalSeconds * 3) || (DateTime.UtcNow - TimeSpan.FromSeconds(GetDelay(pm).TotalSeconds * 3)) > pm.NextCombatTime)
        //                    pm.NextCombatTime = DateTime.UtcNow + TimeSpan.FromSeconds(GetDelay(pm).TotalSeconds * 3);
        //            }

        //            double propertyBonus = 1.0;// (move == null) ? 1.0 : move.GetPropertyBonus(attacker);

        //            if ( Core.AOS )
        //            {
        //                int lifeLeech = 0;
        //                int stamLeech = 0;
        //                int manaLeech = 0;
        //                int wraithLeech = 0;

        //                if ( (int)(m_AosWeaponAttributes.HitLeechHits * propertyBonus) > Utility.Random( 100 ) )
        //                    lifeLeech += 30; // HitLeechHits% chance to leech 30% of damage as hit points

        //                if ( (int)(m_AosWeaponAttributes.HitLeechStam * propertyBonus) > Utility.Random( 100 ) )
        //                    stamLeech += 100; // HitLeechStam% chance to leech 100% of damage as stamina

        //                if ( (int)(m_AosWeaponAttributes.HitLeechMana * propertyBonus) > Utility.Random( 100 ) )
        //                    manaLeech += 40; // HitLeechMana% chance to leech 40% of damage as mana

        //                if ( m_Cursed )
        //                    lifeLeech += 50; // Additional 50% life leech for cursed weapons (necro spell)

        //                context = TransformationSpellHelper.GetContext(attacker);

        //                if ( context != null && context.Type == typeof( VampiricEmbraceSpell ) )
        //                    lifeLeech += 20; // Vampiric embrace gives an additional 20% life leech

        //                if ( context != null && context.Type == typeof( WraithFormSpell ) )
        //                {
        //                    wraithLeech = (5 + (int)((15 * attacker.Skills.SpiritSpeak.Value) / 100)); // Wraith form gives an additional 5-20% mana leech

        //                    // Mana leeched by the Wraith Form spell is actually stolen, not just leeched.
        //                    defender.Mana -= AOS.Scale( damageGiven, wraithLeech );

        //                    manaLeech += wraithLeech;
        //                }

        //                if ( lifeLeech != 0 )
        //                    attacker.Hits += AOS.Scale( damageGiven, lifeLeech );

        //                if ( stamLeech != 0 )
        //                    attacker.Stam += AOS.Scale( damageGiven, stamLeech );

        //                if ( manaLeech != 0 )
        //                    attacker.Mana += AOS.Scale( damageGiven, manaLeech );

        //                if ( lifeLeech != 0 || stamLeech != 0 || manaLeech != 0 )
        //                    attacker.PlaySound( 0x44D );
        //            }

        //            if ( m_MaxHits > 0 && ((MaxRange <= 1 && (defender is Slime || defender is ToxicElemental)) || Utility.Random( 25 ) == 0) ) // Stratics says 50% chance, seems more like 4%..
        //            {
        //                if ( MaxRange <= 1 && (defender is Slime || defender is ToxicElemental) )
        //                    attacker.LocalOverheadMessage( MessageType.Regular, 0x3B2, 500263 ); // *Acid blood scars your weapon!*

        //                if ( Core.AOS && m_AosWeaponAttributes.SelfRepair > Utility.Random( 10 ) )
        //                {
        //                    HitPoints += 2;
        //                }
        //                else
        //                {
        //                    if ( m_Hits > 0 )
        //                    {
        //                        --HitPoints;
        //                    }
        //                    else if ( m_MaxHits > 1 )
        //                    {
        //                        --MaxHitPoints;

        //                        if ( Parent is Mobile )
        //                            ((Mobile)Parent).LocalOverheadMessage( MessageType.Regular, 0x3B2, 1061121 ); // Your equipment is severely damaged.
        //                    }
        //                    else
        //                    {
        //                        Delete();
        //                    }
        //                }
        //            }

        //            if ( attacker is VampireBatFamiliar )
        //            {
        //                BaseCreature bc = (BaseCreature)attacker;
        //                Mobile caster = bc.ControlMaster;

        //                if ( caster == null )
        //                    caster = bc.SummonMaster;

        //                if ( caster != null && caster.Map == bc.Map && caster.InRange( bc, 2 ) )
        //                    caster.Hits += damage;
        //                else
        //                    bc.Hits += damage;
        //            }

        //            if ( Core.AOS )
        //            {
        //                int physChance = (int)(m_AosWeaponAttributes.HitPhysicalArea * propertyBonus);
        //                int fireChance = (int)(m_AosWeaponAttributes.HitFireArea * propertyBonus);
        //                int coldChance = (int)(m_AosWeaponAttributes.HitColdArea * propertyBonus);
        //                int poisChance = (int)(m_AosWeaponAttributes.HitPoisonArea * propertyBonus);
        //                int nrgyChance = (int)(m_AosWeaponAttributes.HitEnergyArea * propertyBonus);

        //                if ( physChance != 0 && physChance > Utility.Random( 100 ) )
        //                    DoAreaAttack( attacker, defender, 0x10E,   50, 100, 0, 0, 0, 0 );

        //                if ( fireChance != 0 && fireChance > Utility.Random( 100 ) )
        //                    DoAreaAttack( attacker, defender, 0x11D, 1160, 0, 100, 0, 0, 0 );

        //                if ( coldChance != 0 && coldChance > Utility.Random( 100 ) )
        //                    DoAreaAttack( attacker, defender, 0x0FC, 2100, 0, 0, 100, 0, 0 );

        //                if ( poisChance != 0 && poisChance > Utility.Random( 100 ) )
        //                    DoAreaAttack( attacker, defender, 0x205, 1166, 0, 0, 0, 100, 0 );

        //                if ( nrgyChance != 0 && nrgyChance > Utility.Random( 100 ) )
        //                    DoAreaAttack( attacker, defender, 0x1F1,  120, 0, 0, 0, 0, 100 );

        //                int maChance = (int)(m_AosWeaponAttributes.HitMagicArrow * propertyBonus);
        //                int harmChance = (int)(m_AosWeaponAttributes.HitHarm * propertyBonus);
        //                int fireballChance = (int)(m_AosWeaponAttributes.HitFireball * propertyBonus);
        //                int lightningChance = (int)(m_AosWeaponAttributes.HitLightning * propertyBonus);
        //                int dispelChance = (int)(m_AosWeaponAttributes.HitDispel * propertyBonus);

        //                if ( maChance != 0 && maChance > Utility.Random( 100 ) )
        //                    DoMagicArrow( attacker, defender );

        //                if ( harmChance != 0 && harmChance > Utility.Random( 100 ) )
        //                    DoHarm( attacker, defender );

        //                if ( fireballChance != 0 && fireballChance > Utility.Random( 100 ) )
        //                    DoFireball( attacker, defender );

        //                if ( lightningChance != 0 && lightningChance > Utility.Random( 100 ) )
        //                    DoLightning( attacker, defender );

        //                if ( dispelChance != 0 && dispelChance > Utility.Random( 100 ) )
        //                    DoDispel( attacker, defender );

        //                int laChance = (int)(m_AosWeaponAttributes.HitLowerAttack * propertyBonus);
        //                int ldChance = (int)(m_AosWeaponAttributes.HitLowerDefend * propertyBonus);

        //                if ( laChance != 0 && laChance > Utility.Random( 100 ) )
        //                    DoLowerAttack( attacker, defender );

        //                if ( ldChance != 0 && ldChance > Utility.Random( 100 ) )
        //                    DoLowerDefense( attacker, defender );
        //            }

        //            if ( attacker is BaseCreature )
        //                ((BaseCreature)attacker).OnGaveMeleeAttack( defender );

        //            if ( defender is BaseCreature )
        //                ((BaseCreature)defender).OnGotMeleeAttack( attacker );
        ///*
        //            if (a != null && !(defender is PlayerMobile))
        //                a.OnHit( attacker, defender, damage );

        //            if ( move != null && !(defender is PlayerMobile))
        //                move.OnHit( attacker, defender, damage );
        //*/
        //            if ( defender is IHonorTarget && ((IHonorTarget)defender).ReceivedHonorContext != null )
        //                ((IHonorTarget)defender).ReceivedHonorContext.OnTargetHit( attacker );

        //            if ( !(this is BaseRanged) )
        //            {
        //                if ( AnimalForm.UnderTransformation( attacker, typeof( GiantSerpent ) ) )
        //                    defender.ApplyPoison( attacker, Poison.Lesser );

        //                if (AnimalForm.UnderTransformation(defender, typeof(BullFrog)))
        //                    attacker.ApplyPoison(defender, Poison.Regular);
        //            }
        //            // hook for attachment OnWeaponHit method
        //            Server.Engines.XmlSpawner2.XmlAttach.OnWeaponHit(this, attacker, defender, damageGiven);
        //        }	

        #endregion

        public void HitMessages( Mobile attacker, Mobile defender )
		{
			if( attacker == null || defender == null )
				return;

            string[] hitMessages = Sphere.GetHitMessages();

            attacker.SendAsciiMessage(hitMessages[0], defender.Name);
            defender.SendAsciiMessage(hitMessages[1], attacker.Name);
		}

		public void MissMessages( Mobile attacker, Mobile defender )
		{
            string[] missMessages = Sphere.GetMissMessages();
			attacker.SendAsciiMessage( missMessages[0], defender.Name );
			defender.SendAsciiMessage( missMessages[1], attacker.Name );
		}

		public virtual double GetAosDamage( Mobile attacker, int bonus, int dice, int sides )
		{
			int damage = Utility.Dice( dice, sides, bonus ) * 100;
			int damageBonus = 0;

			// Inscription bonus
			int inscribeSkill = attacker.Skills[SkillName.Inscribe].Fixed;

			damageBonus += inscribeSkill / 200;

			if ( inscribeSkill >= 1000 )
				damageBonus += 5;

			if ( attacker.Player )
			{
				// Int bonus
				damageBonus += (attacker.Int / 10);

				// SDI bonus
				damageBonus += AosAttributes.GetValue( attacker, AosAttribute.SpellDamage );

                TransformContext context = TransformationSpellHelper.GetContext(attacker);

                if (context != null && context.Spell is ReaperFormSpell)
                    damageBonus += ((ReaperFormSpell)context.Spell).SpellDamageBonus;
            }

			damage = AOS.Scale( damage, 100 + damageBonus );

			return damage / 100;
		}

		#region Do<AoSEffect>
		public virtual void DoMagicArrow( Mobile attacker, Mobile defender )
		{
			if ( !attacker.CanBeHarmful( defender, false ) )
				return;

			attacker.DoHarmful( defender );

			double damage = GetAosDamage( attacker, 10, 1, 4 );

			attacker.MovingParticles( defender, 0x36E4, 5, 0, false, true, 3006, 4006, 0 );
			attacker.PlaySound( 0x1E5 );

			SpellHelper.Damage( TimeSpan.FromSeconds( 1.0 ), defender, attacker, damage, 0, 100, 0, 0, 0 );
		}

		public virtual void DoHarm( Mobile attacker, Mobile defender )
		{
			if ( !attacker.CanBeHarmful( defender, false ) )
				return;

			attacker.DoHarmful( defender );

			double damage = GetAosDamage( attacker, 17, 1, 5 );

			if ( !defender.InRange( attacker, 2 ) )
				damage *= 0.25; // 1/4 damage at > 2 tile range
			else if ( !defender.InRange( attacker, 1 ) )
				damage *= 0.50; // 1/2 damage at 2 tile range

			defender.FixedParticles( 0x374A, 10, 30, 5013, 1153, 2, EffectLayer.Waist );
			defender.PlaySound( 0x0FC );

			SpellHelper.Damage( TimeSpan.Zero, defender, attacker, damage, 0, 0, 100, 0, 0 );
		}

		public virtual void DoFireball( Mobile attacker, Mobile defender )
		{
			if ( !attacker.CanBeHarmful( defender, false ) )
				return;

			attacker.DoHarmful( defender );

			double damage = GetAosDamage( attacker, 19, 1, 5 );

			attacker.MovingParticles( defender, 0x36D4, 7, 0, false, true, 9502, 4019, 0x160 );
			attacker.PlaySound( 0x15E );

			SpellHelper.Damage( TimeSpan.FromSeconds( 1.0 ), defender, attacker, damage, 0, 100, 0, 0, 0 );
		}

		public virtual void DoLightning( Mobile attacker, Mobile defender )
		{
			if ( !attacker.CanBeHarmful( defender, false ) )
				return;

			attacker.DoHarmful( defender );

			double damage = GetAosDamage( attacker, 23, 1, 4 );

			defender.BoltEffect( 0 );

			SpellHelper.Damage( TimeSpan.Zero, defender, attacker, damage, 0, 0, 0, 0, 100 );
		}

		public virtual void DoDispel( Mobile attacker, Mobile defender )
		{
			bool dispellable = false;

			if ( defender is BaseCreature )
				dispellable = ((BaseCreature)defender).Summoned && !((BaseCreature)defender).IsAnimatedDead;

			if ( !dispellable )
				return;

			if ( !attacker.CanBeHarmful( defender, false ) )
				return;

			attacker.DoHarmful( defender );

            MagerySpell sp = new Spells.Sixth.DispelSpell(attacker, null);

			if ( sp.CheckResisted( defender ) )
			{
				defender.FixedEffect( 0x3779, 10, 20 );
			}
			else
			{
				Effects.SendLocationParticles( EffectItem.Create( defender.Location, defender.Map, EffectItem.DefaultDuration ), 0x3728, 8, 20, 5042 );
				Effects.PlaySound( defender, defender.Map, 0x201 );

				defender.Delete();
			}
		}

		public virtual void DoLowerAttack( Mobile from, Mobile defender )
		{
			if ( HitLower.ApplyAttack( defender ) )
			{
				defender.PlaySound( 0x28E );
				Effects.SendTargetEffect( defender, 0x37BE, 1, 4, 0xA, 3 );
			}
		}

		public virtual void DoLowerDefense( Mobile from, Mobile defender )
		{
			if ( HitLower.ApplyDefense( defender ) )
			{
				defender.PlaySound( 0x28E );
				Effects.SendTargetEffect( defender, 0x37BE, 1, 4, 0x23, 3 );
			}
		}

		public virtual void DoAreaAttack( Mobile from, Mobile defender, int sound, int hue, int phys, int fire, int cold, int pois, int nrgy )
		{
			Map map = from.Map;

			if ( map == null )
				return;

			List<Mobile> list = new List<Mobile>();

			foreach ( Mobile m in from.GetMobilesInRange( 10 ) )
			{
				if ( from != m && defender != m && SpellHelper.ValidIndirectTarget( from, m ) && from.CanBeHarmful( m, false ) && ( !Core.ML || from.InLOS( m ) ) )
					list.Add( m );
			}

			if ( list.Count == 0 )
				return;

			Effects.PlaySound( from.Location, map, sound );

			// TODO: What is the damage calculation?

			for ( int i = 0; i < list.Count; ++i )
			{
				Mobile m = list[i];

				double scalar = (11 - from.GetDistanceToSqrt( m )) / 10;

				if ( scalar > 1.0 )
					scalar = 1.0;
				else if ( scalar < 0.0 )
					continue;

				from.DoHarmful( m, true );
				m.FixedEffect( 0x3779, 1, 15, hue, 0 );
				AOS.Damage( m, from, (int)(GetBaseDamage( from ) * scalar), phys, fire, cold, pois, nrgy );
			}
		}
		#endregion

		public virtual CheckSlayerResult CheckSlayers( Mobile attacker, Mobile defender )
		{
			BaseWeapon atkWeapon = (BaseWeapon)attacker.Weapon;
			SlayerEntry atkSlayer = SlayerGroup.GetEntryByName( atkWeapon.Slayer );
			SlayerEntry atkSlayer2 = SlayerGroup.GetEntryByName( atkWeapon.Slayer2 );

			if ( atkSlayer != null && atkSlayer.Slays( defender )  || atkSlayer2 != null && atkSlayer2.Slays( defender ) )
				return CheckSlayerResult.Slayer;

            BaseTalisman talisman = attacker.Talisman as BaseTalisman;

            if (talisman != null && TalismanSlayer.Slays(talisman.Slayer, defender))
                return CheckSlayerResult.Slayer;

			if ( !Core.SE )
			{
				ISlayer defISlayer = Spellbook.FindEquippedSpellbook( defender );

				if( defISlayer == null )
					defISlayer = defender.Weapon as ISlayer;

				if( defISlayer != null )
				{
					SlayerEntry defSlayer = SlayerGroup.GetEntryByName( defISlayer.Slayer );
					SlayerEntry defSlayer2 = SlayerGroup.GetEntryByName( defISlayer.Slayer2 );

					if( defSlayer != null && defSlayer.Group.OppositionSuperSlays( attacker ) || defSlayer2 != null && defSlayer2.Group.OppositionSuperSlays( attacker ) )
						return CheckSlayerResult.Opposition;
				}
			}

			return CheckSlayerResult.None;
		}

		public virtual void AddBlood( Mobile attacker, Mobile defender, int damage )
		{
			if ( damage > 0 )
			{
				new Blood().MoveToWorld( defender.Location, defender.Map );

				int extraBlood = (Core.SE ? Utility.RandomMinMax( 3, 4 ) : Utility.RandomMinMax( 0, 1 ) );

				for( int i = 0; i < extraBlood; i++ )
				{
					new Blood().MoveToWorld( new Point3D(
						defender.X + Utility.RandomMinMax( -1, 1 ),
						defender.Y + Utility.RandomMinMax( -1, 1 ),
						defender.Z ), defender.Map );
				}
			}
		}

        public virtual void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
			if( wielder is BaseCreature )
			{
				BaseCreature bc = (BaseCreature)wielder;

				phys = bc.PhysicalDamage;
				fire = bc.FireDamage;
				cold = bc.ColdDamage;
				pois = bc.PoisonDamage;
				nrgy = bc.EnergyDamage;
                chaos = bc.ChaosDamage;
                direct = bc.DirectDamage;
			}
			else
			{
				fire = m_AosElementDamages.Fire;
				cold = m_AosElementDamages.Cold;
				pois = m_AosElementDamages.Poison;
				nrgy = m_AosElementDamages.Energy;
                chaos = m_AosElementDamages.Chaos;
                direct = m_AosElementDamages.Direct;

                phys = 100 - fire - cold - pois - nrgy - chaos - direct;

				CraftResourceInfo resInfo = CraftResources.GetInfo( m_Resource );

				if( resInfo != null )
				{
					CraftAttributeInfo attrInfo = resInfo.AttributeInfo;

					if( attrInfo != null )
					{
						int left = phys;

						left = ApplyCraftAttributeElementDamage( attrInfo.WeaponColdDamage,		ref cold, left );
						left = ApplyCraftAttributeElementDamage( attrInfo.WeaponEnergyDamage,	ref nrgy, left );
						left = ApplyCraftAttributeElementDamage( attrInfo.WeaponFireDamage,		ref fire, left );
						left = ApplyCraftAttributeElementDamage( attrInfo.WeaponPoisonDamage,	ref pois, left );
                        left = ApplyCraftAttributeElementDamage(attrInfo.WeaponChaosDamage, ref chaos, left);
                        left = ApplyCraftAttributeElementDamage(attrInfo.WeaponDirectDamage, ref direct, left);

						phys = left;
					}
				}
			}
		}

		private static int ApplyCraftAttributeElementDamage( int attrDamage, ref int element, int totalRemaining )
		{
			if( totalRemaining <= 0 )
				return 0;

			if ( attrDamage <= 0 )
				return totalRemaining;

			int appliedDamage = attrDamage;

			if ( (appliedDamage + element) > 100 )
				appliedDamage = 100 - element;

			if( appliedDamage > totalRemaining )
				appliedDamage = totalRemaining;

			element += appliedDamage;

			return totalRemaining - appliedDamage;
		}

		public virtual void OnMiss( Mobile attacker, Mobile defender )
		{
			//PlaySwingAnimation( attacker );
			attacker.PlaySound( GetMissAttackSound( attacker, defender ) );
			defender.PlaySound( GetMissDefendSound( attacker, defender ) );

            WeaponAbility ability = WeaponAbility.GetCurrentAbility(attacker);

            if (ability != null && !(defender is PlayerMobile))
                ability.OnMiss(attacker, defender);

            SpecialMove move = SpecialMove.GetCurrentMove(attacker);

            if (move != null && !(defender is PlayerMobile))
                move.OnMiss(attacker, defender );

			if ( defender is IHonorTarget && ((IHonorTarget)defender).ReceivedHonorContext != null )
				((IHonorTarget)defender).ReceivedHonorContext.OnTargetMissed( attacker );

            MissMessages(attacker, defender);
		}

		public virtual void GetBaseDamageRange( Mobile attacker, out int min, out int max )
		{
			if ( attacker is BaseCreature )
			{
				BaseCreature c = (BaseCreature)attacker;

				if ( c.DamageMin >= 0 )
				{
					min = c.DamageMin;
					max = c.DamageMax;
					return;
				}

				if ( this is Fists && !attacker.Body.IsHuman )
				{
					min = attacker.Str / 28;
					max = attacker.Str / 28;
					return;
				}
			}

			min = MinDamage;
			max = MaxDamage;
		}

		public virtual double GetBaseDamage( Mobile attacker )
		{
			int min, max;

			GetBaseDamageRange( attacker, out min, out max );

			return Utility.RandomMinMax( min, max );
		}

		public virtual double GetBonus( double value, double scalar, double threshold, double offset )
		{
			double bonus = value * scalar;

			if ( value >= threshold )
				bonus += offset;

			return bonus / 100;
		}

		public virtual int GetHitChanceBonus()
		{
			if ( !Core.AOS )
				return 0;

			int bonus = 0;

			switch ( m_AccuracyLevel )
			{
				case WeaponAccuracyLevel.Accurate:		bonus += 02; break;
				case WeaponAccuracyLevel.Surpassingly:	bonus += 04; break;
				case WeaponAccuracyLevel.Eminently:		bonus += 06; break;
				case WeaponAccuracyLevel.Exceedingly:	bonus += 08; break;
				case WeaponAccuracyLevel.Supremely:		bonus += 10; break;
			}

			return bonus;
		}

		public virtual int GetDamageBonus()
		{
			int bonus = VirtualDamageBonus;

            //switch ( m_Quality )
            //{
            //    case WeaponQuality.Low:			bonus -= 20; break;
            //    case WeaponQuality.Exceptional:	bonus += 20; break;
            //}


            //switch ( m_DamageLevel )
            //{
            //    case WeaponDamageLevel.Ruin:	bonus += 15; break;
            //    case WeaponDamageLevel.Might:	bonus += 20; break;
            //    case WeaponDamageLevel.Force:	bonus += 25; break;
            //    case WeaponDamageLevel.Power:	bonus += 30; break;
            //    case WeaponDamageLevel.Vanq:	bonus += 35; break;
            //}

            //return bonus;

            //Maka: Sphere style damage bonuses.
            if (m_DamageLevel != WeaponDamageLevel.Regular)
            {
                bonus += (int)((2.2 * (int)m_DamageLevel) - 1);
            }

            return bonus;
		}

		public virtual void GetStatusDamage( Mobile from, out int min, out int max )
		{
			int baseMin, baseMax;

			GetBaseDamageRange( from, out baseMin, out baseMax );

			if ( Core.AOS )
			{
				min = Math.Max( (int)ScaleDamageAOS( from, baseMin, false ), 1 );
				max = Math.Max( (int)ScaleDamageAOS( from, baseMax, false ), 1 );
			}
			else
			{
				min = Math.Max( (int)ScaleDamageOld( from, baseMin, false ), 1 );
				max = Math.Max( (int)ScaleDamageOld( from, baseMax, false ), 1 );
			}
		}

		public virtual double ScaleDamageAOS( Mobile attacker, double damage, bool checkSkills )
		{
			if ( checkSkills )
			{
                attacker.CheckSkill(SkillName.Tactics, 0.0, attacker.Skills[SkillName.Tactics].Cap); // Passively check tactics for gain
                attacker.CheckSkill(SkillName.Anatomy, 0.0, attacker.Skills[SkillName.Anatomy].Cap); // Passively check Anatomy for gain

				if ( Type == WeaponType.Axe )
					attacker.CheckSkill( SkillName.Lumberjacking, 0.0, 100.0 ); // Passively check Lumberjacking for gain
			}

			#region Physical bonuses
			/*
			 * These are the bonuses given by the physical characteristics of the mobile.
			 * No caps apply.
			 */
			double strengthBonus = GetBonus( attacker.Str,										0.300, 100.0,  5.00 );
			double  anatomyBonus = GetBonus( attacker.Skills[SkillName.Anatomy].Value,			0.500, 100.0,  5.00 );
			double  tacticsBonus = GetBonus( attacker.Skills[SkillName.Tactics].Value,			0.625, 100.0,  6.25 );
			double   lumberBonus = GetBonus( attacker.Skills[SkillName.Lumberjacking].Value,	0.200, 100.0, 10.00 );

			if ( Type != WeaponType.Axe )
				lumberBonus = 0.0;
			#endregion

			#region Modifiers
			/*
			 * The following are damage modifiers whose effect shows on the status bar.
			 * Capped at 100% total.
			 */
			int damageBonus = AosAttributes.GetValue( attacker, AosAttribute.WeaponDamage );

			// Horrific Beast transformation gives a +25% bonus to damage.
            if (TransformationSpellHelper.UnderTransformation(attacker, typeof(HorrificBeastSpell)))
                damageBonus += 25;

			// Divine Fury gives a +10% bonus to damage.
			if ( DivineFurySpell.UnderEffect( attacker ) )
				damageBonus += 10;

			int defenseMasteryMalus = 0;

			// Defense Mastery gives a -50%/-80% malus to damage.
			if ( DefenseMastery.GetMalus( attacker, ref defenseMasteryMalus ) )
				damageBonus -= defenseMasteryMalus;

			int discordanceEffect = 0;

			// Discordance gives a -2%/-48% malus to damage.
			if ( Discordance.GetEffect( attacker, ref discordanceEffect ) )
				damageBonus -= discordanceEffect * 2;

			if ( damageBonus > 100 )
				damageBonus = 100;
			#endregion

			double totalBonus = strengthBonus + anatomyBonus + tacticsBonus + lumberBonus + ((GetDamageBonus() + damageBonus) / 100.0);

			return damage + (int)(damage * totalBonus);
		}

		public virtual int VirtualDamageBonus => 0;

        public virtual int ComputeDamageAOS( Mobile attacker, Mobile defender )
		{
			return (int)ScaleDamageAOS( attacker, GetBaseDamage( attacker ), true );
		}

		public virtual double ScaleDamageOld( Mobile attacker, double damage, bool checkSkills )
		{
            //Maka: Sphere style skillgains/damages
            if (checkSkills)
            {
                attacker.CheckSkill(SkillName.Tactics, 0.0, 120.0); // Passively check tactics for gain
                attacker.CheckSkill(SkillName.Anatomy, 0.0, 120.0); // Passively check Anatomy for gain

                if (attacker is BaseCreature)
                    attacker.CheckSkill(SkillName.Magery, 0.0, 120.0); // Passively check Magery for gain
            }

            /*Taran: For testing purposes
            //Taran: Damage increases over 110 str and decreases under 110
		    damage += ((100 - attacker.Str)*(damage/200));
            */

            if (m_DamageLevel != WeaponDamageLevel.Regular)
                damage += (2.2 * (int)m_DamageLevel) - 1;

            return (int)(damage);
		}

		public virtual int ScaleDamageByDurability( int damage )
		{
			int scale = 100;

			if ( m_MaxHits > 0 && m_Hits < m_MaxHits )
				scale = 50 + ((50 * m_Hits) / m_MaxHits);

			return AOS.Scale( damage, scale );
		}

		public virtual int ComputeDamage( Mobile attacker, Mobile defender )
		{
			if ( Core.AOS )
				return ComputeDamageAOS( attacker, defender );

			return (int)ScaleDamageOld( attacker, GetBaseDamage( attacker ), true );
		}

		public virtual void PlayHurtAnimation( Mobile from )
		{
			int action;
			int frames;

			switch ( from.Body.Type )
			{
				case BodyType.Sea:
				case BodyType.Animal:
				{
					action = 7;
					frames = 5;
					break;
				}
				case BodyType.Monster:
				{
					action = 10;
					frames = 4;
					break;
				}
				case BodyType.Human:
				{
					action = 20;
					frames = 5;
					break;
				}
				default: return;
			}

			if ( from.Mounted )
				return;

            from.Animate(action, frames, 1, true, false, 0);
		}

		public virtual void PlaySwingAnimation( Mobile from )
		{
			int action;

			switch ( from.Body.Type )
			{
				case BodyType.Sea:
				case BodyType.Animal:
				{
					action = Utility.Random( 5, 2 );
					break;
				}
				case BodyType.Monster:
				{
					switch ( Animation )
					{
						default:
						/*case WeaponAnimation.Wrestle:
						case WeaponAnimation.Bash1H:
						case WeaponAnimation.Pierce1H:
						case WeaponAnimation.Slash1H:
						case WeaponAnimation.Bash2H:
						case WeaponAnimation.Pierce2H:
						case WeaponAnimation.Slash2H:*/ action = Utility.Random( 4, 3 ); break;
						case WeaponAnimation.ShootBow:  return; // 7
						case WeaponAnimation.ShootXBow: return; // 8
					}

					break;
				}
				case BodyType.Human:
				{
                    action = GetSwingAnim(from);

                    if (action == -1)
                    {
                        if (!from.Mounted)
                        {
                            switch (Animation)
                            {
                                case WeaponAnimation.Wrestle: action = 31; break;
                                case WeaponAnimation.Bash1H:
                                case WeaponAnimation.Pierce1H:
                                case WeaponAnimation.Slash1H: action = Utility.RandomList(9, 10, 11); break;
                                case WeaponAnimation.Pierce2H: action = Utility.RandomList(13, 14); break;
                                case WeaponAnimation.Bash2H:
                                case WeaponAnimation.Slash2H: action = Utility.RandomList(12, 13, 14); break;
                                case WeaponAnimation.ShootBow: action = 18; break;
                                case WeaponAnimation.ShootXBow: action = 19; break;
                                default: action = (int)Animation; break;
                            }
                        }
                        else
                        {
                            switch (Animation)
                            {
                                default:
                                /*case WeaponAnimation.Wrestle:
                                case WeaponAnimation.Bash1H:
                                case WeaponAnimation.Pierce1H:
                                case WeaponAnimation.Slash1H:*/ action = 26; break;
                                case WeaponAnimation.Bash2H:
                                case WeaponAnimation.Pierce2H:
                                case WeaponAnimation.Slash2H: action = 29; break;
                                case WeaponAnimation.ShootBow: action = 27; break;
                                case WeaponAnimation.ShootXBow: action = 28; break;
                            }
                        }
                    }
					break;
				}
				default: return;
			}

            int delay = 0;
            int frameCount = 7;

            if (GetAnimDelay((BaseWeapon)from.Weapon) > TimeSpan.FromSeconds(1.2))
                delay = 1;

            from.Animate(action, frameCount, 1, true, false, delay);
		}

		#region Serialization/Deserialization
		private static void SetSaveFlag( ref SaveFlag flags, SaveFlag toSet, bool setIf )
		{
			if ( setIf )
				flags |= toSet;
		}

		private static bool GetSaveFlag( SaveFlag flags, SaveFlag toGet )
		{
			return ( (flags & toGet) != 0 );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 9 ); // version

		    writer.Write(m_IsRenamed);

			SaveFlag flags = SaveFlag.None;

			SetSaveFlag( ref flags, SaveFlag.DamageLevel,		m_DamageLevel != WeaponDamageLevel.Regular );
			SetSaveFlag( ref flags, SaveFlag.AccuracyLevel,		m_AccuracyLevel != WeaponAccuracyLevel.Regular );
			SetSaveFlag( ref flags, SaveFlag.DurabilityLevel,	m_DurabilityLevel != WeaponDurabilityLevel.Regular );
			SetSaveFlag( ref flags, SaveFlag.Quality,			m_Quality != WeaponQuality.Regular );
			SetSaveFlag( ref flags, SaveFlag.Hits,				m_Hits != 0 );
			SetSaveFlag( ref flags, SaveFlag.MaxHits,			m_MaxHits != 0 );
			SetSaveFlag( ref flags, SaveFlag.Slayer,			m_Slayer != SlayerName.None );
			SetSaveFlag( ref flags, SaveFlag.Poison,			m_Poison != null );
			SetSaveFlag( ref flags, SaveFlag.PoisonCharges,		m_PoisonCharges != 0 );
			SetSaveFlag( ref flags, SaveFlag.Crafter,			m_Crafter != null );
			SetSaveFlag( ref flags, SaveFlag.Identified,		m_Identified );
			SetSaveFlag( ref flags, SaveFlag.StrReq,			m_StrReq != -1 );
			SetSaveFlag( ref flags, SaveFlag.DexReq,			m_DexReq != -1 );
			SetSaveFlag( ref flags, SaveFlag.IntReq,			m_IntReq != -1 );
			SetSaveFlag( ref flags, SaveFlag.MinDamage,			m_MinDamage != -1 );
			SetSaveFlag( ref flags, SaveFlag.MaxDamage,			m_MaxDamage != -1 );
			SetSaveFlag( ref flags, SaveFlag.HitSound,			m_HitSound != -1 );
			SetSaveFlag( ref flags, SaveFlag.MissSound,			m_MissSound != -1 );
			SetSaveFlag( ref flags, SaveFlag.Speed,				m_Speed != -1 );
			SetSaveFlag( ref flags, SaveFlag.MaxRange,			m_MaxRange != -1 );
			SetSaveFlag( ref flags, SaveFlag.Skill,				m_Skill != (SkillName)(-1) );
			SetSaveFlag( ref flags, SaveFlag.Type,				m_Type != (WeaponType)(-1) );
			SetSaveFlag( ref flags, SaveFlag.Animation,			m_Animation != (WeaponAnimation)(-1) );
			SetSaveFlag( ref flags, SaveFlag.Resource,			m_Resource != CraftResource.Iron );
			SetSaveFlag( ref flags, SaveFlag.xAttributes,		!m_AosAttributes.IsEmpty );
			SetSaveFlag( ref flags, SaveFlag.xWeaponAttributes,	!m_AosWeaponAttributes.IsEmpty );
			SetSaveFlag( ref flags, SaveFlag.PlayerConstructed,	m_PlayerConstructed );
			SetSaveFlag( ref flags, SaveFlag.SkillBonuses,		!m_AosSkillBonuses.IsEmpty );
			SetSaveFlag( ref flags, SaveFlag.Slayer2,			m_Slayer2 != SlayerName.None );
			SetSaveFlag( ref flags, SaveFlag.ElementalDamages,	!m_AosElementDamages.IsEmpty );
            SetSaveFlag(ref flags, SaveFlag.EngravedText, !String.IsNullOrEmpty(m_EngravedText));

			writer.Write( (int) flags );

			if ( GetSaveFlag( flags, SaveFlag.DamageLevel ) )
				writer.Write( (int) m_DamageLevel );

			if ( GetSaveFlag( flags, SaveFlag.AccuracyLevel ) )
				writer.Write( (int) m_AccuracyLevel );

			if ( GetSaveFlag( flags, SaveFlag.DurabilityLevel ) )
				writer.Write( (int) m_DurabilityLevel );

			if ( GetSaveFlag( flags, SaveFlag.Quality ) )
				writer.Write( (int) m_Quality );

			if ( GetSaveFlag( flags, SaveFlag.Hits ) )
				writer.Write( m_Hits );

			if ( GetSaveFlag( flags, SaveFlag.MaxHits ) )
				writer.Write( m_MaxHits );

			if ( GetSaveFlag( flags, SaveFlag.Slayer ) )
				writer.Write( (int) m_Slayer );

			if ( GetSaveFlag( flags, SaveFlag.Poison ) )
				Poison.Serialize( m_Poison, writer );

			if ( GetSaveFlag( flags, SaveFlag.PoisonCharges ) )
				writer.Write( m_PoisonCharges );

			if ( GetSaveFlag( flags, SaveFlag.Crafter ) )
				writer.Write( m_Crafter );

			if ( GetSaveFlag( flags, SaveFlag.StrReq ) )
				writer.Write( m_StrReq );

			if ( GetSaveFlag( flags, SaveFlag.DexReq ) )
				writer.Write( m_DexReq );

			if ( GetSaveFlag( flags, SaveFlag.IntReq ) )
				writer.Write( m_IntReq );

			if ( GetSaveFlag( flags, SaveFlag.MinDamage ) )
				writer.Write( m_MinDamage );

			if ( GetSaveFlag( flags, SaveFlag.MaxDamage ) )
				writer.Write( m_MaxDamage );

			if ( GetSaveFlag( flags, SaveFlag.HitSound ) )
				writer.Write( m_HitSound );

			if ( GetSaveFlag( flags, SaveFlag.MissSound ) )
				writer.Write( m_MissSound );

			if ( GetSaveFlag( flags, SaveFlag.Speed ) )
                writer.Write((int)m_Speed);

			if ( GetSaveFlag( flags, SaveFlag.MaxRange ) )
				writer.Write( m_MaxRange );

			if ( GetSaveFlag( flags, SaveFlag.Skill ) )
				writer.Write( (int) m_Skill );

			if ( GetSaveFlag( flags, SaveFlag.Type ) )
				writer.Write( (int) m_Type );

			if ( GetSaveFlag( flags, SaveFlag.Animation ) )
				writer.Write( (int) m_Animation );

			if ( GetSaveFlag( flags, SaveFlag.Resource ) )
				writer.Write( (int) m_Resource );

			if ( GetSaveFlag( flags, SaveFlag.xAttributes ) )
				m_AosAttributes.Serialize( writer );

			if ( GetSaveFlag( flags, SaveFlag.xWeaponAttributes ) )
				m_AosWeaponAttributes.Serialize( writer );

			if ( GetSaveFlag( flags, SaveFlag.SkillBonuses ) )
				m_AosSkillBonuses.Serialize( writer );

			if ( GetSaveFlag( flags, SaveFlag.Slayer2 ) )
				writer.Write( (int)m_Slayer2 );

			if( GetSaveFlag( flags, SaveFlag.ElementalDamages ) )
				m_AosElementDamages.Serialize( writer );

            if (GetSaveFlag(flags, SaveFlag.EngravedText))
                writer.Write((string)m_EngravedText);
		}

		[Flags]
		private enum SaveFlag
		{
			None					= 0x00000000,
			DamageLevel				= 0x00000001,
			AccuracyLevel			= 0x00000002,
			DurabilityLevel			= 0x00000004,
			Quality					= 0x00000008,
			Hits					= 0x00000010,
			MaxHits					= 0x00000020,
			Slayer					= 0x00000040,
			Poison					= 0x00000080,
			PoisonCharges			= 0x00000100,
			Crafter					= 0x00000200,
			Identified				= 0x00000400,
			StrReq					= 0x00000800,
			DexReq					= 0x00001000,
			IntReq					= 0x00002000,
			MinDamage				= 0x00004000,
			MaxDamage				= 0x00008000,
			HitSound				= 0x00010000,
			MissSound				= 0x00020000,
			Speed					= 0x00040000,
			MaxRange				= 0x00080000,
			Skill					= 0x00100000,
			Type					= 0x00200000,
			Animation				= 0x00400000,
			Resource				= 0x00800000,
			xAttributes				= 0x01000000,
			xWeaponAttributes		= 0x02000000,
			PlayerConstructed		= 0x04000000,
			SkillBonuses			= 0x08000000,
			Slayer2					= 0x10000000,
			ElementalDamages		= 0x20000000,
            EngravedText            = 0x40000000
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
                case 9:
                    m_IsRenamed = reader.ReadBool();
                    goto case 8;
				case 8:
				case 7:
				case 6:
				case 5:
				{
					SaveFlag flags = (SaveFlag)reader.ReadInt();

					if ( GetSaveFlag( flags, SaveFlag.DamageLevel ) )
					{
						m_DamageLevel = (WeaponDamageLevel)reader.ReadInt();

						if ( m_DamageLevel > WeaponDamageLevel.Vanq )
							m_DamageLevel = WeaponDamageLevel.Ruin;
					}

					if ( GetSaveFlag( flags, SaveFlag.AccuracyLevel ) )
					{
						m_AccuracyLevel = (WeaponAccuracyLevel)reader.ReadInt();

						if ( m_AccuracyLevel > WeaponAccuracyLevel.Supremely )
							m_AccuracyLevel = WeaponAccuracyLevel.Accurate;
					}

					if ( GetSaveFlag( flags, SaveFlag.DurabilityLevel ) )
					{
						m_DurabilityLevel = (WeaponDurabilityLevel)reader.ReadInt();

						if ( m_DurabilityLevel > WeaponDurabilityLevel.Indestructible )
							m_DurabilityLevel = WeaponDurabilityLevel.Durable;
					}

					if ( GetSaveFlag( flags, SaveFlag.Quality ) )
						m_Quality = (WeaponQuality)reader.ReadInt();
					else
						m_Quality = WeaponQuality.Regular;

					if ( GetSaveFlag( flags, SaveFlag.Hits ) )
						m_Hits = reader.ReadInt();

					if ( GetSaveFlag( flags, SaveFlag.MaxHits ) )
						m_MaxHits = reader.ReadInt();

					if ( GetSaveFlag( flags, SaveFlag.Slayer ) )
						m_Slayer = (SlayerName)reader.ReadInt();

					if ( GetSaveFlag( flags, SaveFlag.Poison ) )
						m_Poison = Poison.Deserialize( reader );

					if ( GetSaveFlag( flags, SaveFlag.PoisonCharges ) )
						m_PoisonCharges = reader.ReadInt();

					if ( GetSaveFlag( flags, SaveFlag.Crafter ) )
						m_Crafter = reader.ReadMobile();

					if ( GetSaveFlag( flags, SaveFlag.Identified ) )
						m_Identified = ( version >= 6 || reader.ReadBool() );

					if ( GetSaveFlag( flags, SaveFlag.StrReq ) )
						m_StrReq = reader.ReadInt();
					else
						m_StrReq = -1;

					if ( GetSaveFlag( flags, SaveFlag.DexReq ) )
						m_DexReq = reader.ReadInt();
					else
						m_DexReq = -1;

					if ( GetSaveFlag( flags, SaveFlag.IntReq ) )
						m_IntReq = reader.ReadInt();
					else
						m_IntReq = -1;

					if ( GetSaveFlag( flags, SaveFlag.MinDamage ) )
						m_MinDamage = reader.ReadInt();
					else
						m_MinDamage = -1;

					if ( GetSaveFlag( flags, SaveFlag.MaxDamage ) )
						m_MaxDamage = reader.ReadInt();
					else
						m_MaxDamage = -1;

					if ( GetSaveFlag( flags, SaveFlag.HitSound ) )
						m_HitSound = reader.ReadInt();
					else
						m_HitSound = -1;

					if ( GetSaveFlag( flags, SaveFlag.MissSound ) )
						m_MissSound = reader.ReadInt();
					else
						m_MissSound = -1;

					if ( GetSaveFlag( flags, SaveFlag.Speed ) )
                            m_Speed = reader.ReadInt();
					else
						m_Speed = -1;

					if ( GetSaveFlag( flags, SaveFlag.MaxRange ) )
						m_MaxRange = reader.ReadInt();
					else
						m_MaxRange = -1;

					if ( GetSaveFlag( flags, SaveFlag.Skill ) )
						m_Skill = (SkillName)reader.ReadInt();
					else
						m_Skill = (SkillName)(-1);

					if ( GetSaveFlag( flags, SaveFlag.Type ) )
						m_Type = (WeaponType)reader.ReadInt();
					else
						m_Type = (WeaponType)(-1);

					if ( GetSaveFlag( flags, SaveFlag.Animation ) )
						m_Animation = (WeaponAnimation)reader.ReadInt();
					else
						m_Animation = (WeaponAnimation)(-1);

					if ( GetSaveFlag( flags, SaveFlag.Resource ) )
						m_Resource = (CraftResource)reader.ReadInt();
					else
						m_Resource = CraftResource.Iron;

					if ( GetSaveFlag( flags, SaveFlag.xAttributes ) )
						m_AosAttributes = new AosAttributes( this, reader );
					else
						m_AosAttributes = new AosAttributes( this );

					if ( GetSaveFlag( flags, SaveFlag.xWeaponAttributes ) )
						m_AosWeaponAttributes = new AosWeaponAttributes( this, reader );
					else
						m_AosWeaponAttributes = new AosWeaponAttributes( this );

					if ( UseSkillMod && m_AccuracyLevel != WeaponAccuracyLevel.Regular && Parent is Mobile )
					{
						m_SkillMod = new DefaultSkillMod( AccuracySkill, true, (int)m_AccuracyLevel * 5 );
						((Mobile)Parent).AddSkillMod( m_SkillMod );
					}

					if ( version < 7 && m_AosWeaponAttributes.MageWeapon != 0 )
						m_AosWeaponAttributes.MageWeapon = 30 - m_AosWeaponAttributes.MageWeapon;

					if ( Core.AOS && m_AosWeaponAttributes.MageWeapon != 0 && m_AosWeaponAttributes.MageWeapon != 30 && Parent is Mobile )
					{
						m_MageMod = new DefaultSkillMod( SkillName.Magery, true, -30 + m_AosWeaponAttributes.MageWeapon );
						((Mobile)Parent).AddSkillMod( m_MageMod );
					}

					if ( GetSaveFlag( flags, SaveFlag.PlayerConstructed ) )
						m_PlayerConstructed = true;

					if( GetSaveFlag( flags, SaveFlag.SkillBonuses ) )
						m_AosSkillBonuses = new AosSkillBonuses( this, reader );
					else
						m_AosSkillBonuses = new AosSkillBonuses( this );

					if( GetSaveFlag( flags, SaveFlag.Slayer2 ) )
						m_Slayer2 = (SlayerName)reader.ReadInt();

					if( GetSaveFlag( flags, SaveFlag.ElementalDamages ) )
						m_AosElementDamages = new AosElementAttributes( this, reader );
					else
						m_AosElementDamages = new AosElementAttributes( this );

                    if (GetSaveFlag(flags, SaveFlag.EngravedText))
                        m_EngravedText = reader.ReadString();

					break;
				}
				case 4:
				{
					m_Slayer = (SlayerName)reader.ReadInt();

					goto case 3;
				}
				case 3:
				{
					m_StrReq = reader.ReadInt();
					m_DexReq = reader.ReadInt();
					m_IntReq = reader.ReadInt();

					goto case 2;
				}
				case 2:
				{
					m_Identified = reader.ReadBool();

					goto case 1;
				}
				case 1:
				{
					m_MaxRange = reader.ReadInt();

					goto case 0;
				}
				case 0:
				{
					if ( version == 0 )
						m_MaxRange = 1; // default

					if ( version < 5 )
					{
						m_Resource = CraftResource.Iron;
						m_AosAttributes = new AosAttributes( this );
						m_AosWeaponAttributes = new AosWeaponAttributes( this );
						m_AosElementDamages = new AosElementAttributes( this );
						m_AosSkillBonuses = new AosSkillBonuses( this );
					}

					m_MinDamage = reader.ReadInt();
					m_MaxDamage = reader.ReadInt();

					m_Speed = reader.ReadInt();

					m_HitSound = reader.ReadInt();
					m_MissSound = reader.ReadInt();

					m_Skill = (SkillName)reader.ReadInt();
					m_Type = (WeaponType)reader.ReadInt();
					m_Animation = (WeaponAnimation)reader.ReadInt();
					m_DamageLevel = (WeaponDamageLevel)reader.ReadInt();
					m_AccuracyLevel = (WeaponAccuracyLevel)reader.ReadInt();
					m_DurabilityLevel = (WeaponDurabilityLevel)reader.ReadInt();
					m_Quality = (WeaponQuality)reader.ReadInt();

					m_Crafter = reader.ReadMobile();

					m_Poison = Poison.Deserialize( reader );
					m_PoisonCharges = reader.ReadInt();

					if ( m_StrReq == OldStrengthReq )
						m_StrReq = -1;

					if ( m_DexReq == OldDexterityReq )
						m_DexReq = -1;

					if ( m_IntReq == OldIntelligenceReq )
						m_IntReq = -1;

					if ( m_MinDamage == OldMinDamage )
						m_MinDamage = -1;

					if ( m_MaxDamage == OldMaxDamage )
						m_MaxDamage = -1;

					if ( m_HitSound == OldHitSound )
						m_HitSound = -1;

					if ( m_MissSound == OldMissSound )
						m_MissSound = -1;

					if ( m_Speed == OldSpeed )
						m_Speed = -1;

					if ( m_MaxRange == OldMaxRange )
						m_MaxRange = -1;

					if ( m_Skill == OldSkill )
						m_Skill = (SkillName)(-1);

					if ( m_Type == OldType )
						m_Type = (WeaponType)(-1);

					if ( m_Animation == OldAnimation )
						m_Animation = (WeaponAnimation)(-1);

					if ( UseSkillMod && m_AccuracyLevel != WeaponAccuracyLevel.Regular && Parent is Mobile )
					{
						m_SkillMod = new DefaultSkillMod( AccuracySkill, true, (int)m_AccuracyLevel * 5);
						((Mobile)Parent).AddSkillMod( m_SkillMod );
					}

					break;
				}
			}

			if ( Core.AOS && Parent is Mobile )
				m_AosSkillBonuses.AddTo( (Mobile)Parent );

			int strBonus = m_AosAttributes.BonusStr;
			int dexBonus = m_AosAttributes.BonusDex;
			int intBonus = m_AosAttributes.BonusInt;

			if ( Parent is Mobile && (strBonus != 0 || dexBonus != 0 || intBonus != 0) )
			{
				Mobile m = (Mobile)Parent;

				string modName = Serial.ToString();

				if ( strBonus != 0 )
					m.AddStatMod( new StatMod( StatType.Str, modName + "Str", strBonus, TimeSpan.Zero ) );

				if ( dexBonus != 0 )
					m.AddStatMod( new StatMod( StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero ) );

				if ( intBonus != 0 )
					m.AddStatMod( new StatMod( StatType.Int, modName + "Int", intBonus, TimeSpan.Zero ) );
			}

			if ( Parent is Mobile )
				((Mobile)Parent).CheckStatTimers();

			if ( m_Hits <= 0 && m_MaxHits <= 0 )
			{
				m_Hits = m_MaxHits = Utility.RandomMinMax( InitMinHits, InitMaxHits );
			}

			if ( version < 6 )
				m_PlayerConstructed = true; // we don't know, so, assume it's crafted
		}
		#endregion

		public BaseWeapon( int itemID ) : base( itemID )
		{
			Layer = (Layer)ItemData.Quality;

			m_Quality = WeaponQuality.Regular;
			m_StrReq = -1;
			m_DexReq = -1;
			m_IntReq = -1;
			m_MinDamage = -1;
			m_MaxDamage = -1;
			m_HitSound = -1;
			m_MissSound = -1;
			m_Speed = -1;
			m_MaxRange = -1;
			m_Skill = (SkillName)(-1);
			m_Type = (WeaponType)(-1);
			m_Animation = (WeaponAnimation)(-1);

			m_Hits = m_MaxHits = Utility.RandomMinMax( InitMinHits, InitMaxHits );

			m_Resource = CraftResource.Iron;

			m_AosAttributes = new AosAttributes( this );
			m_AosWeaponAttributes = new AosWeaponAttributes( this );
			m_AosSkillBonuses = new AosSkillBonuses( this );
			m_AosElementDamages = new AosElementAttributes( this );
		}

		public BaseWeapon( Serial serial ) : base( serial )
		{
		}

		private string GetNameString()
		{
			string name = Name;

			if ( name == null )
				name = String.Format( "#{0}", LabelNumber );

			return name;
		}

		[Hue, CommandProperty( AccessLevel.GameMaster )]
		public override int Hue
		{
			get => base.Hue;
            set{ base.Hue = value; InvalidateProperties(); }
		}

		public int GetElementalDamageHue()
		{
            int phys, fire, cold, pois, nrgy, chaos, direct;
            GetDamageTypes(null, out phys, out fire, out cold, out pois, out nrgy, out chaos, out direct);
            //Order is Cold, Energy, Fire, Poison, Physical left

			int currentMax = 50;
			int hue = 0;

			if( pois >= currentMax )
			{
				hue = 1267 + (pois - 50) / 10;
				currentMax = pois;
			}

			if( fire >= currentMax )
			{
				hue = 1255 + (fire - 50) / 10;
				currentMax = fire;
			}

			if( nrgy >= currentMax )
			{
				hue = 1273 + (nrgy - 50) / 10;
				currentMax = nrgy;
			}

			if( cold >= currentMax )
			{
				hue = 1261 + (cold - 50) / 10;
				//currentMax = cold;
			}

			return hue;
		}

		public override void AddNameProperty( ObjectPropertyList list )
		{
			int oreType;

			switch ( m_Resource )
			{
				case CraftResource.DullCopper:		oreType = 1053108; break; // dull copper
				case CraftResource.ShadowIron:		oreType = 1053107; break; // shadow iron
				case CraftResource.Copper:			oreType = 1053106; break; // copper
				case CraftResource.Bronze:			oreType = 1053105; break; // bronze
				case CraftResource.Gold:			oreType = 1053104; break; // golden
				case CraftResource.Agapite:			oreType = 1053103; break; // agapite
				case CraftResource.Verite:			oreType = 1053102; break; // verite
				case CraftResource.Valorite:		oreType = 1053101; break; // valorite
				case CraftResource.SpinedLeather:	oreType = 1061118; break; // spined
				case CraftResource.HornedLeather:	oreType = 1061117; break; // horned
				case CraftResource.BarbedLeather:	oreType = 1061116; break; // barbed
				case CraftResource.RedScales:		oreType = 1060814; break; // red
				case CraftResource.YellowScales:	oreType = 1060818; break; // yellow
				case CraftResource.BlackScales:		oreType = 1060820; break; // black
				case CraftResource.GreenScales:		oreType = 1060819; break; // green
				case CraftResource.WhiteScales:		oreType = 1060821; break; // white
				case CraftResource.BlueScales:		oreType = 1060815; break; // blue
				default: oreType = 0; break;
			}

			if ( oreType != 0 )
				list.Add( 1053099, "#{0}\t{1}", oreType, GetNameString() ); // ~1_oretype~ ~2_armortype~
			else if ( Name == null )
				list.Add( LabelNumber );
			else
				list.Add( Name );

            /*
            * Want to move this to the engraving tool, let the non-harmful 
            * formatting show, and remove CLILOCs embedded: more like OSI
            * did with the books that had markup, etc.
            * 
            * This will have a negative effect on a few event things imgame 
            * as is.
            * 
            * If we cant find a more OSI-ish way to clean it up, we can 
            * easily put this back, and use it in the deserialize
            * method and engraving tool, to make it perm cleaned up.
            */

            if (!String.IsNullOrEmpty(m_EngravedText))
                list.Add(1062613, m_EngravedText);

            /* list.Add( 1062613, Utility.FixHtml( m_EngravedText ) ); */
        }

		public override bool AllowEquipedCast( Mobile from )
		{
			if ( base.AllowEquipedCast( from ) )
				return true;

			return ( m_AosAttributes.SpellChanneling != 0 );
		}

		public virtual int ArtifactRarity => 0;

        public virtual int GetLuckBonus()
		{
			CraftResourceInfo resInfo = CraftResources.GetInfo( m_Resource );

			if ( resInfo == null )
				return 0;

			CraftAttributeInfo attrInfo = resInfo.AttributeInfo;

			if ( attrInfo == null )
				return 0;

			return attrInfo.WeaponLuck;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( m_Crafter != null )
				list.Add( 1050043, m_Crafter.Name ); // crafted by ~1_NAME~

			#region Factions
			if ( m_FactionState != null )
				list.Add( 1041350 ); // faction item
			#endregion

			if ( m_AosSkillBonuses != null )
				m_AosSkillBonuses.GetProperties( list );

			if ( m_Quality == WeaponQuality.Exceptional )
				list.Add( 1060636 ); // exceptional

			if( RequiredRace == Race.Elf )
				list.Add( 1075086 ); // Elves Only

			if ( ArtifactRarity > 0 )
				list.Add( 1061078, ArtifactRarity.ToString() ); // artifact rarity ~1_val~

			if ( this is IUsesRemaining && ((IUsesRemaining)this).ShowUsesRemaining )
				list.Add( 1060584, ((IUsesRemaining)this).UsesRemaining.ToString() ); // uses remaining: ~1_val~

			if ( m_Poison != null && m_PoisonCharges > 0 )
				list.Add( 1062412 + m_Poison.Level, m_PoisonCharges.ToString() );

			if( m_Slayer != SlayerName.None )
			{
				SlayerEntry entry = SlayerGroup.GetEntryByName( m_Slayer );
				if( entry != null )
					list.Add( entry.Title );
			}

			if( m_Slayer2 != SlayerName.None )
			{
				SlayerEntry entry = SlayerGroup.GetEntryByName( m_Slayer2 );
				if( entry != null )
					list.Add( entry.Title );
			}

			base.AddResistanceProperties( list );

			int prop;

            if (Core.ML && this is BaseRanged && ((BaseRanged)this).Balanced)
                list.Add(1072792); // Balanced

			//if ( (prop = m_AosWeaponAttributes.UseBestSkill) != 0 )
			//	list.Add( 1060400 ); // use best weapon skill

			if ( (prop = (GetDamageBonus() + m_AosAttributes.WeaponDamage)) != 0 )
				list.Add( 1060401, prop.ToString() ); // damage increase ~1_val~%

			if ( (prop = m_AosAttributes.DefendChance) != 0 )
				list.Add( 1060408, prop.ToString() ); // defense chance increase ~1_val~%

			if ( (prop = m_AosAttributes.EnhancePotions) != 0 )
				list.Add( 1060411, prop.ToString() ); // enhance potions ~1_val~%

			if ( (prop = m_AosAttributes.CastRecovery) != 0 )
				list.Add( 1060412, prop.ToString() ); // faster cast recovery ~1_val~

			if ( (prop = m_AosAttributes.CastSpeed) != 0 )
				list.Add( 1060413, prop.ToString() ); // faster casting ~1_val~

			if ( (prop = (GetHitChanceBonus() + m_AosAttributes.AttackChance)) != 0 )
				list.Add( 1060415, prop.ToString() ); // hit chance increase ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitColdArea) != 0 )
				list.Add( 1060416, prop.ToString() ); // hit cold area ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitDispel) != 0 )
				list.Add( 1060417, prop.ToString() ); // hit dispel ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitEnergyArea) != 0 )
				list.Add( 1060418, prop.ToString() ); // hit energy area ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitFireArea) != 0 )
				list.Add( 1060419, prop.ToString() ); // hit fire area ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitFireball) != 0 )
				list.Add( 1060420, prop.ToString() ); // hit fireball ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitHarm) != 0 )
				list.Add( 1060421, prop.ToString() ); // hit harm ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitLeechHits) != 0 )
				list.Add( 1060422, prop.ToString() ); // hit life leech ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitLightning) != 0 )
				list.Add( 1060423, prop.ToString() ); // hit lightning ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitLowerAttack) != 0 )
				list.Add( 1060424, prop.ToString() ); // hit lower attack ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitLowerDefend) != 0 )
				list.Add( 1060425, prop.ToString() ); // hit lower defense ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitMagicArrow) != 0 )
				list.Add( 1060426, prop.ToString() ); // hit magic arrow ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitLeechMana) != 0 )
				list.Add( 1060427, prop.ToString() ); // hit mana leech ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitPhysicalArea) != 0 )
				list.Add( 1060428, prop.ToString() ); // hit physical area ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitPoisonArea) != 0 )
				list.Add( 1060429, prop.ToString() ); // hit poison area ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitLeechStam) != 0 )
				list.Add( 1060430, prop.ToString() ); // hit stamina leech ~1_val~%

            if (ImmolatingWeaponSpell.IsImmolating(this))
                list.Add(1111917); // Immolated

            if (Core.ML && this is BaseRanged && (prop = ((BaseRanged)this).Velocity) != 0)
                list.Add(1072793, prop.ToString()); // Velocity ~1_val~%

			if ( (prop = m_AosAttributes.BonusDex) != 0 )
				list.Add( 1060409, prop.ToString() ); // dexterity bonus ~1_val~

			if ( (prop = m_AosAttributes.BonusHits) != 0 )
				list.Add( 1060431, prop.ToString() ); // hit point increase ~1_val~

			if ( (prop = m_AosAttributes.BonusInt) != 0 )
				list.Add( 1060432, prop.ToString() ); // intelligence bonus ~1_val~

			if ( (prop = m_AosAttributes.LowerManaCost) != 0 )
				list.Add( 1060433, prop.ToString() ); // lower mana cost ~1_val~%

			if ( (prop = m_AosAttributes.LowerRegCost) != 0 )
				list.Add( 1060434, prop.ToString() ); // lower reagent cost ~1_val~%

			if ( (prop = GetLowerStatReq()) != 0 )
				list.Add( 1060435, prop.ToString() ); // lower requirements ~1_val~%

			if ( (prop = (GetLuckBonus() + m_AosAttributes.Luck)) != 0 )
				list.Add( 1060436, prop.ToString() ); // luck ~1_val~

			if ( (prop = m_AosWeaponAttributes.MageWeapon) != 0 )
				list.Add( 1060438, (30 - prop).ToString() ); // mage weapon -~1_val~ skill

			if ( (prop = m_AosAttributes.BonusMana) != 0 )
				list.Add( 1060439, prop.ToString() ); // mana increase ~1_val~

			if ( (prop = m_AosAttributes.RegenMana) != 0 )
				list.Add( 1060440, prop.ToString() ); // mana regeneration ~1_val~

			//if ( (prop = m_AosAttributes.NightSight) != 0 )
			//	list.Add( 1060441 ); // night sight

			if ( (prop = m_AosAttributes.ReflectPhysical) != 0 )
				list.Add( 1060442, prop.ToString() ); // reflect physical damage ~1_val~%

			if ( (prop = m_AosAttributes.RegenStam) != 0 )
				list.Add( 1060443, prop.ToString() ); // stamina regeneration ~1_val~

			if ( (prop = m_AosAttributes.RegenHits) != 0 )
				list.Add( 1060444, prop.ToString() ); // hit point regeneration ~1_val~

			if ( (prop = m_AosWeaponAttributes.SelfRepair) != 0 )
				list.Add( 1060450, prop.ToString() ); // self repair ~1_val~

			//if ( (prop = m_AosAttributes.SpellChanneling) != 0 )
			//	list.Add( 1060482 ); // spell channeling

			if ( (prop = m_AosAttributes.SpellDamage) != 0 )
				list.Add( 1060483, prop.ToString() ); // spell damage increase ~1_val~%

			if ( (prop = m_AosAttributes.BonusStam) != 0 )
				list.Add( 1060484, prop.ToString() ); // stamina increase ~1_val~

			if ( (prop = m_AosAttributes.BonusStr) != 0 )
				list.Add( 1060485, prop.ToString() ); // strength bonus ~1_val~

			if ( (prop = m_AosAttributes.WeaponSpeed) != 0 )
				list.Add( 1060486, prop.ToString() ); // swing speed increase ~1_val~%

            int phys, fire, cold, pois, nrgy, chaos, direct;

            GetDamageTypes(null, out phys, out fire, out cold, out pois, out nrgy, out chaos, out direct);

			if ( phys != 0 )
				list.Add( 1060403, phys.ToString() ); // physical damage ~1_val~%

			if ( fire != 0 )
				list.Add( 1060405, fire.ToString() ); // fire damage ~1_val~%

			if ( cold != 0 )
				list.Add( 1060404, cold.ToString() ); // cold damage ~1_val~%

			if ( pois != 0 )
				list.Add( 1060406, pois.ToString() ); // poison damage ~1_val~%

			if ( nrgy != 0 )
				list.Add( 1060407, nrgy.ToString() ); // energy damage ~1_val~%

            if (Core.ML && chaos != 0)
                list.Add(1072846, chaos.ToString()); // chaos damage ~1_val~%

            if (Core.ML && direct != 0)
                list.Add(1079978, direct.ToString()); // Direct Damage: ~1_PERCENT~%

			list.Add( 1061168, "{0}\t{1}", MinDamage.ToString(), MaxDamage.ToString() ); // weapon damage ~1_val~ - ~2_val~
            
            if (Core.ML)
                list.Add(1061167, String.Format("{0}s", Speed)); // weapon speed ~1_val~
            else
                list.Add(1061167, Speed.ToString());

			if ( MaxRange > 1 )
				list.Add( 1061169, MaxRange.ToString() ); // range ~1_val~

			int strReq = AOS.Scale( StrRequirement, 100 - GetLowerStatReq() );

			if ( strReq > 0 )
				list.Add( 1061170, strReq.ToString() ); // strength requirement ~1_val~

			if ( Layer == Layer.TwoHanded )
				list.Add( 1061171 ); // two-handed weapon
			else
				list.Add( 1061824 ); // one-handed weapon

			if ( Core.SE || m_AosWeaponAttributes.UseBestSkill == 0 )
			{
				switch ( Skill )
				{
					case SkillName.Swords:  list.Add( 1061172 ); break; // skill required: swordsmanship
					case SkillName.Macing:  list.Add( 1061173 ); break; // skill required: mace fighting
					case SkillName.Fencing: list.Add( 1061174 ); break; // skill required: fencing
					case SkillName.Archery: list.Add( 1061175 ); break; // skill required: archery
				}
			}

			if ( m_Hits >= 0 && m_MaxHits > 0 )
				list.Add( 1060639, "{0}\t{1}", m_Hits, m_MaxHits ); // durability ~1_val~ / ~2_val~
		}

        public override void OnDoubleClick(Mobile from)
        {
            if (!Sphere.EquipOnDouble(from, this))
                return;

            base.OnDoubleClick(from);
        }

        //Maka
        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, Sphere.ComputeName(this));

            //List<EquipInfoAttribute> attrs = new List<EquipInfoAttribute>();

            //if (DisplayLootType)
            //{
            //    if (LootType == LootType.Blessed)
            //        attrs.Add(new EquipInfoAttribute(1038021)); // blessed
            //    else if (LootType == LootType.Cursed)
            //        attrs.Add(new EquipInfoAttribute(1049643)); // cursed
            //}

            //#region Factions
            //if (m_FactionState != null)
            //    attrs.Add(new EquipInfoAttribute(1041350)); // faction item
            //#endregion

            //if (m_Quality == WeaponQuality.Exceptional)
            //    attrs.Add(new EquipInfoAttribute(1018305 - (int)m_Quality));

            //if (m_Identified || from.AccessLevel >= AccessLevel.GameMaster)
            //{
            //    if (m_Slayer != SlayerName.None)
            //    {
            //        SlayerEntry entry = SlayerGroup.GetEntryByName(m_Slayer);
            //        if (entry != null)
            //            attrs.Add(new EquipInfoAttribute(entry.Title));
            //    }

            //    if (m_Slayer2 != SlayerName.None)
            //    {
            //        SlayerEntry entry = SlayerGroup.GetEntryByName(m_Slayer2);
            //        if (entry != null)
            //            attrs.Add(new EquipInfoAttribute(entry.Title));
            //    }

            //    if (m_DurabilityLevel != WeaponDurabilityLevel.Regular)
            //        attrs.Add(new EquipInfoAttribute(1038000 + (int)m_DurabilityLevel));

            //    if (m_DamageLevel != WeaponDamageLevel.Regular)
            //        attrs.Add(new EquipInfoAttribute(1038015 + (int)m_DamageLevel));

            //    if (m_AccuracyLevel != WeaponAccuracyLevel.Regular)
            //        attrs.Add(new EquipInfoAttribute(1038010 + (int)m_AccuracyLevel));
            //}
            //else if (m_Slayer != SlayerName.None || m_Slayer2 != SlayerName.None || m_DurabilityLevel != WeaponDurabilityLevel.Regular || m_DamageLevel != WeaponDamageLevel.Regular || m_AccuracyLevel != WeaponAccuracyLevel.Regular)
            //    attrs.Add(new EquipInfoAttribute(1038000)); // Unidentified

            //if (m_Poison != null && m_PoisonCharges > 0)
            //    attrs.Add(new EquipInfoAttribute(1017383, m_PoisonCharges));

            //int number;

            //if (Name == null)
            //{
            //    number = LabelNumber;
            //}
            //else
            //{
            //    this.LabelTo(from, Name);
            //    number = 1041000;
            //}

            //if (attrs.Count == 0 && Crafter == null && Name != null)
            //    return;

            //EquipmentInfo eqInfo = new EquipmentInfo(number, m_Crafter, false, attrs.ToArray());

            //from.Send(new DisplayEquipmentInfo(this, eqInfo));
        }

		private static BaseWeapon m_Fists; // This value holds the default--fist--weapon

		public static BaseWeapon Fists
		{
			get => m_Fists;
            set => m_Fists = value;
        }

		#region ICraftable Members

		public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
		{
			Quality = (WeaponQuality)quality;

            if (makersMark)
            {
                /*
                if (Quality == WeaponQuality.Exceptional)
                {
                    if (DamageLevel < WeaponDamageLevel.Ruin)
                    {
                        DamageLevel = WeaponDamageLevel.Ruin;
                    }
                    else
                    {
                        Increase the damage values manually, check how ruin actually does it
                        MaxDamage += 3;
                        MinDamage += 3;
                    }

                    DurabilityLevel = GetDurabilityBonus(from, craftSystem, tool, craftItem);
                }
                */

                Crafter = from;
            }

			PlayerConstructed = true;

			Type resourceType = typeRes;

			if ( resourceType == null )
				resourceType = craftItem.Resources.GetAt( 0 ).ItemType;

			if ( Core.AOS )
			{
				Resource = CraftResources.GetFromType( resourceType );

				CraftContext context = craftSystem.GetContext( from );

				if ( context != null && context.DoNotColor )
					Hue = 0;

				if ( tool is BaseRunicTool )
					((BaseRunicTool)tool).ApplyAttributesTo( this );

                if (Quality == WeaponQuality.Exceptional)
                {
                    if (Attributes.WeaponDamage > 35)
                        Attributes.WeaponDamage -= 20;
                    else
                        Attributes.WeaponDamage = 15;

                    if (Core.ML)
                    {
                        Attributes.WeaponDamage += (int)(from.Skills.ArmsLore.Value / 20);

                        if (Attributes.WeaponDamage > 50)
                            Attributes.WeaponDamage = 50;

                        from.CheckSkill(SkillName.ArmsLore, 0, 100);
                    }
                }
			}
			else if ( tool is BaseRunicTool ) //This can be made into something really really cool
			{

                /**
				CraftResource thisResource = CraftResources.GetFromType( resourceType );

				if ( thisResource == ((BaseRunicTool)tool).Resource )
				{
					Resource = thisResource;

					CraftContext context = craftSystem.GetContext( from );

					if ( context != null && context.DoNotColor )
						Hue = 0;

					switch ( thisResource )
					{
						case CraftResource.DullCopper:
						{
							Identified = true;
							DurabilityLevel = WeaponDurabilityLevel.Durable;
							AccuracyLevel = WeaponAccuracyLevel.Accurate;
							break;
						}
						case CraftResource.ShadowIron:
						{
							Identified = true;
							DurabilityLevel = WeaponDurabilityLevel.Durable;
							DamageLevel = WeaponDamageLevel.Ruin;
							break;
						}
						case CraftResource.Copper:
						{
							Identified = true;
							DurabilityLevel = WeaponDurabilityLevel.Fortified;
							DamageLevel = WeaponDamageLevel.Ruin;
							AccuracyLevel = WeaponAccuracyLevel.Surpassingly;
							break;
						}
						case CraftResource.Bronze:
						{
							Identified = true;
							DurabilityLevel = WeaponDurabilityLevel.Fortified;
							DamageLevel = WeaponDamageLevel.Might;
							AccuracyLevel = WeaponAccuracyLevel.Surpassingly;
							break;
						}
						case CraftResource.Gold:
						{
							Identified = true;
							DurabilityLevel = WeaponDurabilityLevel.Indestructible;
							DamageLevel = WeaponDamageLevel.Force;
							AccuracyLevel = WeaponAccuracyLevel.Eminently;
							break;
						}
						case CraftResource.Agapite:
						{
							Identified = true;
							DurabilityLevel = WeaponDurabilityLevel.Indestructible;
							DamageLevel = WeaponDamageLevel.Power;
							AccuracyLevel = WeaponAccuracyLevel.Eminently;
							break;
						}
						case CraftResource.Verite:
						{
							Identified = true;
							DurabilityLevel = WeaponDurabilityLevel.Indestructible;
							DamageLevel = WeaponDamageLevel.Power;
							AccuracyLevel = WeaponAccuracyLevel.Exceedingly;
							break;
						}
						case CraftResource.Valorite:
						{
							Identified = true;
							DurabilityLevel = WeaponDurabilityLevel.Indestructible;
							DamageLevel = WeaponDamageLevel.Vanq;
							AccuracyLevel = WeaponAccuracyLevel.Supremely;
							break;
						}
					}
				}
               **/
			}

			return quality;
		}


        /// <summary>
        /// The paremeters are there so that we can extend the functionality. The method should take skill and difficulty in mind
        /// </summary>
        /// <param name="from"></param>
        /// <param name="craftSystem"></param>
        /// <param name="typeRes"></param>
        /// <param name="tool"></param>
        /// <param name="craftItem"></param>
        /// <returns></returns>
        /*
        private WeaponDurabilityLevel GetDurabilityBonus(Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem)
        {
            double rand = new Random().NextDouble();
            double offset = 1.0 / (5 + 4 + 3 + 2 + 1);

            double percentage = 0;
            for (int i = 1; i <= 5; i++)
            {
                percentage += (i * offset);

                if (percentage < rand)
                    return (WeaponDurabilityLevel)(6 - i);
            }

            return WeaponDurabilityLevel.Regular;
        }
        */

		#endregion
	}

	public enum CheckSlayerResult
	{
		None,
		Slayer,
		Opposition
	}
}
