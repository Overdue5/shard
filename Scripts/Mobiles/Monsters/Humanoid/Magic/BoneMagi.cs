using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a skeletal corpse" )]
	public class BoneMagi : BaseCreature
	{
		[Constructable]
		public BoneMagi() : base( AIType.AI_SphereMage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Bone Mage";
			Body = 148;
			BaseSoundID = 451;

			SetStr( 76, 100 );
			SetDex( 56, 75 );
			SetInt( 186, 200 );

			SetHits( 46, 60 );

			SetDamage( 3, 7 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 35, 40 );
			SetResistance( ResistanceType.Fire, 20, 30 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.EvalInt, 60.1, 65.0 );
			SetSkill( SkillName.Magery, 60.1, 65.0 );
			SetSkill( SkillName.MagicResist, 55.1, 70.0 );
			SetSkill( SkillName.Tactics, 45.1, 60.0 );
			SetSkill( SkillName.Wrestling, 45.1, 55.0 );

			Fame = 3000;

			VirtualArmor = 38;

			PackReg( 3 );
			PackNecroReg( 3, 10 );
			PackItem( new Bone() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.LowScrolls );
			AddLoot( LootPack.Potions );
		}
		
		public override bool BleedImmune => true;

        public override OppositionGroup OppositionGroup => OppositionGroup.FeyAndUndead;

        public override Poison PoisonImmune => Poison.Regular;

        public BoneMagi( Serial serial ) : base( serial )
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