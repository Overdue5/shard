using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a lunar corpse")]
    public class LunarSkeleton : BaseCreature
    {
        [Constructable]
        public LunarSkeleton()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Lunar Skeleton";
            Body = 50;
            Hue = 2944;
            SetStr(600);
            SetDex(120);
            SetInt(200);

            SetHits(450, 500);
            SetStam(100);
            SetMana(600);

            SetDamage(35);

            SetSkill(SkillName.Parry, 105.0);
            SetSkill(SkillName.Magery, 95.0);
            SetSkill(SkillName.MagicResist, 95.0);
            SetSkill(SkillName.Tactics, 140.0);
            SetSkill(SkillName.Wrestling, 120.0);

            Fame = 5500;
            Karma = -7500;

            VirtualArmor = Utility.RandomMinMax(3, 6);
        }

        public LunarSkeleton(Serial serial)
            : base(serial)
        {
        }

        public override bool CanRummageCorpses => true;
        public override bool BardImmune => true;
        public override bool Uncalmable => true;
        public override bool Unprovokable => true;
        public override bool AlwaysMurderer => true;

        public override Poison PoisonImmune => Poison.Lethal;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
            AddItem(new LunarBone(Utility.RandomMinMax(2, 3)));
            AddItem(new Bone(Utility.RandomMinMax(1, 6)));
            PackGold(500);
        }

        public override WeaponAbility GetWeaponAbility()
        {
            switch (Utility.Random(3))
            {
                default:
                case 0: return WeaponAbility.BleedAttack;
                case 1: return WeaponAbility.ArmorIgnore;
                case 2: return WeaponAbility.ParalyzingBlow;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

}