using System;
using System.Linq;
using Server.Items;
using Server.Mobiles;
using Server.SkillHandlers;

namespace Server.Custom.Zed
{
    public partial class BtceKeyStone
    {
        private class BtceGuards : BaseCreature
        {
            private static readonly TimeSpan m_FindDelay = TimeSpan.FromSeconds(1);
            private DateTime m_NextFind;

            public override Poison PoisonImmune => Poison.Lethal;

            public override bool BardImmune => true;

            public BtceGuards(int level, bool register = true) : this((double)level, register)
            {
            }

            public BtceGuards(double level, bool register = true) : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.2)
            {
                Female = true;
                Body = 0x191;
                Name = "Blackthorn Elite";
                var eq_Name = "Blackthorn Elite Ammunition";
                m_NextFind = DateTime.UtcNow;
                AddItem(new PlateChest { Resource = CraftResource.BlackRock, LootType = LootType.Blessed, Name = eq_Name });
                AddItem(new PlateArms { Resource = CraftResource.BlackRock, LootType = LootType.Blessed, Name = eq_Name });
                AddItem(new PlateLegs { Resource = CraftResource.BlackRock, LootType = LootType.Blessed, Name = eq_Name });
                AddItem(new PlateGloves { Resource = CraftResource.BlackRock, LootType = LootType.Blessed, Name = eq_Name });
                AddItem(new PlateGorget { Resource = CraftResource.BlackRock, LootType = LootType.Blessed, Name = eq_Name });
                AddItem(new PlateHelm { Resource = CraftResource.BlackRock, LootType = LootType.Blessed, Name = eq_Name });
                AddItem(new MetalShield { Resource = CraftResource.Gold, LootType = LootType.Blessed, Name = eq_Name });
                AddItem(level >= 2
                    ? new VikingSword
                    {
                        Resource = CraftResource.Gold, MaxDamage = (int)(20 * level), MinDamage = (int)(15 * level),
                        LootType = LootType.Blessed, Poison = Poison.Lesser, PoisonCharges = Utility.Random(5)
                    }
                    : new VikingSword
                    {
                        Resource = CraftResource.Gold, MaxDamage = (int)(20 * level), MinDamage = (int)(15 * level),
                        LootType = LootType.Blessed
                    });
                SetDamage((int)(15 * level));
                HitsMaxSeed = (int)(50 * level);
                SetStr((int)(100 * level));
                SetInt((int)(100 * level));
                SetDex((int)(100 * level));

                Skills[SkillName.Anatomy].Base = (int)(100 * level);
                Skills[SkillName.Archery].Base = (int)(100 * level);
                Skills[SkillName.ArmsLore].Base = (int)(100 * level);
                Skills[SkillName.Tactics].Base = (int)(100 * level);
                Skills[SkillName.Swords].Base = (int)(100 * level);
                Skills[SkillName.Fencing].Base = (int)(100 * level);
                Skills[SkillName.Parry].Base = (int)(100 * level);
                Skills[SkillName.Wrestling].Base = (int)(100 * level);
                Skills[SkillName.Macing].Base = (int)(100 * level);
                Skills[SkillName.MagicResist].Base = (int)(100 * level);
                Skills[SkillName.DetectHidden].Base = (int)(100 * level);
                Skills[SkillName.Healing].Base = (int)(100 * level);
                Skills[SkillName.Hiding].Base = (int)(100 * level);
                Skills[SkillName.Stealth].Base = (int)(100 * level);

                SetDamageType(ResistanceType.Physical, 100);

                SetResistance(ResistanceType.Physical, (int)(20 * level));
                SetResistance(ResistanceType.Fire, Utility.LimitMinMax(20, (int)(30 * level), 90));
                SetResistance(ResistanceType.Cold, Utility.LimitMinMax(20, (int)(30 * level), 90));
                SetResistance(ResistanceType.Poison, Utility.LimitMinMax(20, (int)(30 * level), 90));
                SetResistance(ResistanceType.Energy, Utility.LimitMinMax(20, (int)(30 * level), 90));

                Fame = -5000;
                Karma = -5000;
                if (register) BtceTimer.Guards.Add(this);
                VirtualArmor = 50;
            }

            private void FindEnemies()
            {
                if (DateTime.UtcNow < m_NextFind) return;
                if (Combatant != null) return;
                var enemy = Region.GetMobiles()
                    .Where(x => !(x is BtceGuards) && x.GetDistanceToSqrt(this) <= 10 && Math.Abs(this.Z - x.Z) < 12 &&
                                x.AccessLevel < AccessLevel.Counselor).OrderBy(x => x.GetDistanceToSqrt(this)).FirstOrDefault();
                if (enemy != null)
                    if (enemy.Hidden)
                    {
                        DetectHidden.OnUse(this);
                    }
                    else
                    {
                        this.Combatant = enemy;
                    }

                m_NextFind = DateTime.UtcNow + m_FindDelay;
            }

            private void SayNearestGuards()
            {
                var guards = Region.GetMobiles().Where(x =>
                    (x is BtceGuards bg) && x.GetDistanceToSqrt(this) <= BtceSettings.GuardFindDistance && 
                    Math.Abs(this.Z - x.Z) < 10 && x.Combatant == null && bg.FightMode != FightMode.None && !bg.Deleted).ToList();
                foreach (var mobile in guards)
                {
                    var guard = (BtceGuards)mobile;
                    if (GetDistanceToSqrt(guard) <= BtceSettings.GuardSayHelpDistance)
                        guard.Combatant = this.Combatant;
                    else
                    {
                        Timer.DelayCall(TimeSpan.FromMilliseconds(Utility.Random(400, 200)), guard.FindEnemies);
                    }
                }
            }

            public BtceGuards(Serial serial) : base(serial)
            {
            }

            public override void OnActionCombat()
            {
                if (Utility.RandomDouble() > 0.9)
                    SayNearestGuards();
                base.OnActionCombat();
            }

            public override void OnActionWander()
            {
                base.OnActionWander();
            }

            public override void OnWarmodeChanged()
            {
                base.OnWarmodeChanged();
                SayNearestGuards();
            }

            public override void OnDelete()
            {
                if (BtceTimer.Guards.Contains(this))
                    BtceTimer.Guards.Remove(this);
                base.OnDelete();
            }

            public override bool OnBeforeDeath()
            {
                Delete();
                return base.OnBeforeDeath();
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                Delete();
            }
        }
    }
    }