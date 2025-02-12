using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a mummy corpse" )]
	public class Mummy : BaseCreature
	{
		[Constructable]
		public Mummy() : base( AIType.AI_SphereMelee, FightMode.Closest, 10, 1, 0.4, 0.8 )
		{
			Name = "Mummy";
			Body = 154;
			BaseSoundID = 471;

			SetStr( 346, 370 );
			SetDex( 71, 90 );
			SetInt( 26, 40 );

			SetHits( 208, 222 );

			SetDamage( 13, 23 );

			SetDamageType( ResistanceType.Physical, 40 );
			SetDamageType( ResistanceType.Cold, 60 );

			SetResistance( ResistanceType.Physical, 45, 55 );
			SetResistance( ResistanceType.Fire, 10, 20 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 20, 30 );

			SetSkill( SkillName.MagicResist, 15.1, 40.0 );
			SetSkill( SkillName.Tactics, 35.1, 50.0 );
			SetSkill( SkillName.Wrestling, 35.1, 50.0 );

			Fame = 4000;

			VirtualArmor = 50;

            if (Core.ML && Utility.RandomDouble() < .33)
                PackItem(Engines.Plants.Seed.RandomPeculiarSeed(2));

			PackItem( new Garlic( 5 ) );
			PackItem( new Bandage( 10 ) );
		}

        public override bool IsNight => true;

        public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.Gems );
			AddLoot( LootPack.Potions );
		}

		public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lesser;

        public Mummy( Serial serial ) : base( serial )
		{
		}

		public override OppositionGroup OppositionGroup => OppositionGroup.FeyAndUndead;

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