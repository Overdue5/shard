using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a liche's corpse" )]
	public class LunarLich : BaseCreature
	{
		[Constructable]
		public LunarLich() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Lunar Lich";
			Body = 79;
			BaseSoundID = 412;
            Hue = 2944;

			SetStr( 640, 700 );
			SetDex( 180, 205 );
			SetInt( 700, 755 );

			SetHits( 550, 600 );

			SetDamage( 40, 60 );

			SetDamageType( ResistanceType.Physical, 0 );
			SetDamageType( ResistanceType.Cold, 60 );
			SetDamageType( ResistanceType.Energy, 40 );

			SetResistance( ResistanceType.Physical, 40, 50 );
			SetResistance( ResistanceType.Fire, 30, 40 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 50, 60 );
			SetResistance( ResistanceType.Energy, 40, 50 );

			SetSkill( SkillName.EvalInt, 100.0, 140.0 );
			SetSkill( SkillName.Magery, 120.0, 160.0 );
			SetSkill( SkillName.MagicResist, 150.0, 175.0 );
			SetSkill( SkillName.Tactics, 100.0, 120.0 );
			SetSkill( SkillName.Wrestling, 160.0, 180.0 );

			Fame = 8000;
			Karma = -8000;

			VirtualArmor = 50;
			PackItem( new GnarledStaff() );
            
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.MedScrolls, 2 );
            AddItem(new LunarBone(Utility.RandomMinMax(4, 7)));
            PackGold(500);
		}

		public override bool CanRummageCorpses => true;
        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override bool BardImmune => true;
        public override bool Uncalmable => true;
        public override bool Unprovokable => true;
        public override bool AlwaysMurderer => true;

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

		public LunarLich( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}