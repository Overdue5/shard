using Server.Items;
namespace Server.Mobiles
{
	[CorpseName( "a blood elemental corpse" )]
	public class BloodElemental : BaseCreature
	{
		[Constructable]
		public BloodElemental () : base( AIType.AI_SphereMage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Blood Elemental";
			Body = 159;
			BaseSoundID = 278;

			SetStr( 526, 615 );
			SetDex( 66, 85 );
			SetInt( 226, 350 );

			SetHits( 316, 369 );

			SetDamage( 17, 27 );

			SetDamageType( ResistanceType.Physical, 0 );
			SetDamageType( ResistanceType.Poison, 50 );
			SetDamageType( ResistanceType.Energy, 50 );

			SetResistance( ResistanceType.Physical, 55, 65 );
			SetResistance( ResistanceType.Fire, 20, 30 );
			SetResistance( ResistanceType.Cold, 40, 50 );
			SetResistance( ResistanceType.Poison, 50, 60 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.EvalInt, 85.1, 100.0 );
			SetSkill( SkillName.Magery, 85.1, 100.0 );
			SetSkill( SkillName.Meditation, 10.4, 50.0 );
			SetSkill( SkillName.MagicResist, 80.1, 95.0 );
			SetSkill( SkillName.Tactics, 80.1, 100.0 );
			SetSkill( SkillName.Wrestling, 80.1, 100.0 );

			Fame = 12500;
			Karma = -12500;

			VirtualArmor = 60;

            if (Core.ML && Utility.RandomDouble() < 0.23)
                PackItem(Engines.Plants.Seed.RandomPeculiarSeed(1));
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
		    AddLoot(LootPack.Average);
            AddLoot(LootPack.Gems, 4);
            PackGold(200);
            if (Utility.RandomDouble() <= 0.25)
                AddItem(new RandomAccWeap(3));
		}

		public override int TreasureMapLevel => 5;

        public BloodElemental( Serial serial ) : base( serial )
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