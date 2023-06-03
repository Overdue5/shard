using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a balron corpse" )]
	public class Balron : BaseCreature
	{
		[Constructable]
		public Balron () : base( AIType.AI_SphereMage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "Balron" );
			Body = 40;
			BaseSoundID = 357;

			SetStr( 986, 1185 );
			SetDex( 177, 255 );
			SetInt( 151, 250 );

			SetHits( 592, 711 );

			SetDamage( 22, 29 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Fire, 25 );
			SetDamageType( ResistanceType.Energy, 25 );

			SetResistance( ResistanceType.Physical, 65, 80 );
			SetResistance( ResistanceType.Fire, 60, 80 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 100 );
			SetResistance( ResistanceType.Energy, 40, 50 );

			SetSkill( SkillName.Anatomy, 25.1, 50.0 );
			SetSkill( SkillName.EvalInt, 90.1, 100.0 );
			SetSkill( SkillName.Magery, 95.5, 100.0 );
			SetSkill( SkillName.Meditation, 25.1, 50.0 );
			SetSkill( SkillName.MagicResist, 100.5, 150.0 );
			SetSkill( SkillName.Tactics, 90.1, 100.0 );
			SetSkill( SkillName.Wrestling, 90.1, 100.0 );

			Fame = 24000;
			Karma = -18000;

			VirtualArmor = 90;

			PackItem( new Longsword() );

            Spellbook book = new Spellbook();
            book.Content = ulong.MaxValue;
            book.LootType = LootType.Regular;
            AddItem(book);
		}

		public override void GenerateLoot()
		{
            AddLoot(LootPack.FilthyRich);
            PackGold(400, 600);
			AddLoot( LootPack.MedScrolls, 2 );
            AddLoot(LootPack.HighScrolls, 1);
            if (Utility.RandomDouble() <= 0.2)
                AddLoot(LootPack.RandomWand, 1 );

            if (Utility.RandomDouble() <= 0.05)
                AddItem(new RandomAccWeap((Utility.RandomMinMax(2, 4))));

            if (Utility.RandomDouble() <= 0.3)
            {
                BaseArmor armor = Loot.RandomArmorOrShield();
                armor.ProtectionLevel = (ArmorProtectionLevel)(Utility.RandomMinMax(2, 4));
                AddItem(armor);
            }
		}

		public override bool CanRummageCorpses => false;
        public override Poison PoisonImmune => Poison.Deadly;
        public override int TreasureMapLevel => 5;
        public override int Meat => 1;

        public Balron( Serial serial ) : base( serial )
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