using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an ophidian corpse" )]
	[TypeAlias( "Server.Mobiles.OphidianAvenger" )]
	public class OphidianKnight : BaseCreature
	{
		private static readonly string[] m_Names = new string[]
			{
				"Ophidian Knight-Errant",
				"Ophidian Avenger"
			};

		[Constructable]
		public OphidianKnight() : base( AIType.AI_SphereMelee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = m_Names[Utility.Random( m_Names.Length )];
			Body = 86;
			BaseSoundID = 634;

			SetStr( 417, 595 );
			SetDex( 166, 175 );
			SetInt( 46, 70 );

			SetHits( 266, 342 );
			SetMana( 0 );

			SetDamage( 16, 19 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 35, 40 );
			SetResistance( ResistanceType.Fire, 30, 40 );
			SetResistance( ResistanceType.Cold, 35, 45 );
			SetResistance( ResistanceType.Poison, 90, 100 );
			SetResistance( ResistanceType.Energy, 35, 45 );

			SetSkill( SkillName.Poisoning, 60.1, 80.0 );
			SetSkill( SkillName.MagicResist, 65.1, 80.0 );
			SetSkill( SkillName.Tactics, 90.1, 100.0 );
			SetSkill( SkillName.Wrestling, 90.1, 100.0 );

			Fame = 10000;
			Karma = -8000;

			VirtualArmor = 40;

			PackItem( new LesserPoisonPotion() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
            PackGold(100);
		}

		public override int Meat => 2;

        public override Poison PoisonImmune => Poison.Lethal;
        public override Poison HitPoison => Poison.Lethal;
        public override int TreasureMapLevel => 3;

        //public override OppositionGroup OppositionGroup
		//{
		//	get{ return OppositionGroup.TerathansAndOphidians; }
		//}

		public OphidianKnight( Serial serial ) : base( serial )
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