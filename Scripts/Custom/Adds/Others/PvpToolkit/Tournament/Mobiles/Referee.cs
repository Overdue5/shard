using System;

namespace Server.Mobiles
{
    [CorpseName("an referee corpse")]
    public class Referee : BaseCreature
    {
        [Constructable]
        public Referee() : base(AIType.AI_EvAndBsAI, FightMode.Closest, 10, 1, 0.1, 0.1)
        {
            Name = "Referee";

            Body = 987;
            Hue = 1940;

            SetStr(200);
            SetDex(200);
            SetInt(60);

            SetHits(65000);
            SetStam(200);
            SetMana(0);

            SetDamage(35, 60);

            SetDamageType(ResistanceType.Physical, 0);
            SetDamageType(ResistanceType.Energy, 100);

            SetResistance(ResistanceType.Physical, 100, 100);
            SetResistance(ResistanceType.Fire, 100, 100);
            SetResistance(ResistanceType.Cold, 100, 100);
            SetResistance(ResistanceType.Poison, 100, 100);
            SetResistance(ResistanceType.Energy, 100, 100);

            SetSkill(SkillName.MagicResist, 200.9);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.Wrestling, 200.0);

            Fame = 222;

            VirtualArmor = 200;

            BaseSoundID = 0;
        }

        public Referee(Serial serial) : base(serial)
        {
        }

        public override bool ParalyzeImmune => true;

        public override bool IgnoreYoungProtection => true;

        public override bool DeleteCorpseOnDeath => true;

        public override bool AlwaysMurderer => true;

        public override double DispelDifficulty => 200.0;

        public override double DispelFocus => 200.0;

        public override bool BleedImmune => true;

        public override Poison PoisonImmune => Poison.Lethal;

        public override double GetFightModeRanking(Mobile m, FightMode acqType, bool bPlayerOnly)
        {
            return (m.Int + m.Skills[SkillName.Magery].Value) / Math.Max(GetDistanceToSqrt(m), 1.0);
        }

        public override bool IsEnemy(Mobile m)
        {
            return true; // Attack everything
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}