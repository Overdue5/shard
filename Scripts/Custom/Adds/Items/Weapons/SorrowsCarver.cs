﻿using Server.Mobiles;
using Server.Network;
using Server.Regions;

namespace Server.Items
{
    public class SorrowsCarver : Wakizashi
    {
        [Constructable]
        public SorrowsCarver()
        {
            Hue = 2529;
            Name = "Sorrow's Carver";
            Weight = 8;
            Speed = 475;
            MinDamage = 14;
            MaxDamage = 40;
            DamageLevel = WeaponDamageLevel.Vanq;
            DurabilityLevel = WeaponDurabilityLevel.Substantial;
            IsRenamed = true;
        }

        public SorrowsCarver(Serial serial)
            : base(serial)
        {
        }

        public override int OldStrengthReq => 90;

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, Sphere.ComputeCustomWeaponName(this));
        }

        public override bool OnEquip(Mobile from)
        {
            if (from.Karma > -5000)
            {
                from.PrivateOverheadMessage(MessageType.Regular, 0x3b2, true, "Your karma is too high to equip this", from.NetState);
                return false;
            }

            if (from.Skills[SkillName.Swords].Base < 100.0)
            {
                from.PrivateOverheadMessage(MessageType.Regular, 0x3b2, true, "You don't know enough about swordfighting to equip this", from.NetState);
                return false;
            }

            return base.OnEquip(from);
        }

        public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
        {
            CustomRegion cR = defender.Region as CustomRegion;
            double bonus = damageBonus;

            if (cR == null || cR.Controller.AllowSpecialAttacks)
            {
                if (defender is PlayerMobile && defender.Karma > 5000)
                {
                    if (Utility.RandomDouble() < 0.14) //14% chance of bonus damage
                    {
                        Effects.SendLocationParticles(EffectItem.Create(defender.Location, defender.Map, EffectItem.DefaultDuration), 0x37C4, 10, 10, 2023);
                        attacker.SendAsciiMessage("The Sorrow's Carver strikes at their very soul!");
                        bonus = 1.15;
                    }
                }
            }

            base.OnHit(attacker, defender, bonus);
        }
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}