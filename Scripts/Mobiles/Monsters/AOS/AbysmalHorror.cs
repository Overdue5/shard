using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an abyssmal horror corpse" )]
	public class AbysmalHorror : BaseCreature
	{
		public override WeaponAbility GetWeaponAbility()
		{
			return Utility.RandomBool() ? WeaponAbility.MortalStrike : WeaponAbility.WhirlwindAttack;
		}

		public override bool IgnoreYoungProtection => Core.ML;

        [Constructable]
		public AbysmalHorror() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Abyssmal Horror";
			Body = 312;
			BaseSoundID = 0x451;

			SetStr( 401, 420 );
			SetDex( 81, 90 );
			SetInt( 401, 420 );

			SetHits( 6000 );

			SetDamage( 13, 17 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Poison, 50 );

			SetResistance( ResistanceType.Physical, 30, 35 );
			SetResistance( ResistanceType.Fire, 100 );
			SetResistance( ResistanceType.Cold, 50, 55 );
			SetResistance( ResistanceType.Poison, 60, 65 );
			SetResistance( ResistanceType.Energy, 77, 80 );

			SetSkill( SkillName.EvalInt, 200.0 );
			SetSkill( SkillName.Magery, 112.6, 117.5 );
			SetSkill( SkillName.Meditation, 200.0 );
			SetSkill( SkillName.MagicResist, 117.6, 120.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.Wrestling, 84.1, 88.0 );

			Fame = 15000;
			Karma = -16000;

			VirtualArmor = 54;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.SuperBoss );
		}

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );

			//if ( !Summoned && !NoKillAwards && DemonKnight.CheckArtifactChance( this ) )
			//	DemonKnight.DistributeArtifact( this );
		}

		public override bool BardImmune => !Core.SE;
        public override bool Unprovokable => Core.SE;
        public override bool AreaPeaceImmune => Core.SE;
        public override Poison PoisonImmune => Poison.Lethal;
        public override int TreasureMapLevel => 1;

        public AbysmalHorror( Serial serial ) : base( serial )
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

			if ( BaseSoundID == 357 )
				BaseSoundID = 0x451;
		}
	}
}