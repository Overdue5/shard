using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an ancient liche's corpse" )]
	public class AncientLich : BaseCreature
	{
		[Constructable]
		public AncientLich() : base( AIType.AI_SphereMage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "Ancient Lich" );
			Body = 78;
			BaseSoundID = 412;

			SetStr( 216, 305 );
			SetDex( 96, 115 );
			SetInt( 966, 1045 );

			SetHits( 560, 595 );

			SetDamage( 15, 27 );

			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Cold, 40 );
			SetDamageType( ResistanceType.Energy, 40 );

			SetResistance( ResistanceType.Physical, 55, 65 );
			SetResistance( ResistanceType.Fire, 25, 30 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 50, 60 );
			SetResistance( ResistanceType.Energy, 25, 30 );

			SetSkill( SkillName.EvalInt, 120.1, 130.0 );
			SetSkill( SkillName.Magery, 120.1, 130.0 );
			SetSkill( SkillName.Meditation, 100.1, 101.0 );
			SetSkill( SkillName.Poisoning, 100.1, 101.0 );
			SetSkill( SkillName.MagicResist, 175.2, 200.0 );
			SetSkill( SkillName.Tactics, 90.1, 100.0 );
			SetSkill( SkillName.Wrestling, 75.1, 100.0 );

			Fame = 23000;
			Karma = -16000;

			VirtualArmor = 60;
			PackNecroReg( 30, 275 );

            Spellbook book = new Spellbook();
            book.Content = ulong.MaxValue;
            book.LootType = LootType.Regular;
            AddItem(book);

		}

		public override OppositionGroup OppositionGroup => OppositionGroup.FeyAndUndead;

        public override int GetIdleSound()
		{
			return 0x19D;
		}

		public override int GetAngerSound()
		{
			return 0x175;
		}

		public override int GetDeathSound()
		{
			return 0x108;
		}

		public override int GetAttackSound()
		{
			return 0xE2;
		}

		public override int GetHurtSound()
		{
			return 0x28B;
		}

		public override void GenerateLoot()
		{
            PackGold(2000);
			AddLoot( LootPack.UltraRich );
			AddLoot( LootPack.MedScrolls, 2 );
            AddLoot(LootPack.HighScrolls, 2);
            if (Utility.RandomDouble() <= 0.2)
                AddLoot(LootPack.RandomWand, 1);
            if (Utility.RandomDouble() <= 0.1)
                AddItem(new RandomAccWeap(Utility.RandomMinMax(3, 4)));
		}

		public override bool Unprovokable => true;
        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override int TreasureMapLevel => 5;

        public AncientLich( Serial serial ) : base( serial )
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