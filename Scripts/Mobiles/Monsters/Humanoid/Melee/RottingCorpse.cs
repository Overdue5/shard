namespace Server.Mobiles
{
	[CorpseName( "a rotting corpse" )]
	public class RottingCorpse : BaseCreature
	{
		[Constructable]
		public RottingCorpse() : base( AIType.AI_SphereMelee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Rotting Corpse";
			Body = 155;
			BaseSoundID = 471;

			SetStr( 301, 350 );
			SetDex( 75 );
			SetInt( 151, 200 );

			SetHits( 1200 );
			SetStam( 150 );
			SetMana( 0 );

			SetDamage( 8, 10 );

			SetDamageType( ResistanceType.Physical, 0 );
			SetDamageType( ResistanceType.Cold, 50 );
			SetDamageType( ResistanceType.Poison, 50 );

			SetResistance( ResistanceType.Physical, 35, 45 );
			SetResistance( ResistanceType.Fire, 20, 30 );
			SetResistance( ResistanceType.Cold, 50, 70 );
			SetResistance( ResistanceType.Poison, 40, 50 );
			SetResistance( ResistanceType.Energy, 20, 30 );

			SetSkill( SkillName.Poisoning, 120.0 );
			SetSkill( SkillName.MagicResist, 250.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.Wrestling, 90.1, 100.0 );

			Fame = 6000;

			VirtualArmor = 40;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
		}

		public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override Poison HitPoison => Poison.Lethal;
        public override int TreasureMapLevel => 5;

        public RottingCorpse( Serial serial ) : base( serial )
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