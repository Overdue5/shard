namespace Server.Mobiles
{
	[CorpseName( "a dragon corpse" )]
	public class SerpentineDragon : BaseCreature
	{
		[Constructable]
		public SerpentineDragon() : base( AIType.AI_SphereMage, FightMode.Evil, 10, 1, 0.2, 0.4 )
		{
			Name = "Serpentine Dragon";
			Body = 103;
			BaseSoundID = 362;

			SetStr( 111, 140 );
			SetDex( 201, 220 );
			SetInt( 1001, 1040 );

			SetHits( 480 );

			SetDamage( 5, 12 );

			SetDamageType( ResistanceType.Physical, 75 );
			SetDamageType( ResistanceType.Poison, 25 );

			SetResistance( ResistanceType.Physical, 35, 40 );
			SetResistance( ResistanceType.Fire, 25, 35 );
			SetResistance( ResistanceType.Cold, 25, 35 );
			SetResistance( ResistanceType.Poison, 25, 35 );
			SetResistance( ResistanceType.Energy, 25, 35 );

			SetSkill( SkillName.EvalInt, 100.1, 110.0 );
			SetSkill( SkillName.Magery, 110.1, 120.0 );
			SetSkill( SkillName.Meditation, 100.0 );
			SetSkill( SkillName.MagicResist, 100.0 );
			SetSkill( SkillName.Tactics, 50.1, 60.0 );
			SetSkill( SkillName.Wrestling, 30.1, 100.0 );

			Fame = 15000;
			Karma = 15000;

			VirtualArmor = 36;

            if (Core.ML && Utility.RandomDouble() < .33)
                PackItem(Engines.Plants.Seed.RandomPeculiarSeed(2));
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich, 2);
			AddLoot( LootPack.Gems, 2 );
		}

		public override int GetIdleSound()
		{
			return 0x2C4;
		}

		public override int GetAttackSound()
		{
			return 0x2C0;
		}

		public override int GetDeathSound()
		{
			return 0x2C1;
		}

		public override int GetAngerSound()
		{
			return 0x2C4;
		}

		public override int GetHurtSound()
		{
			return 0x2C3;
		}

		public override bool ReacquireOnMovement => true;
        public override bool HasBreath => true; // fire breath enabled
		public override double BonusPetDamageScalar => (Core.SE) ? 3.0 : 1.0;

        public override bool AutoDispel => true;

        public override HideType HideType
        {
            get
            {
                double roll = Utility.RandomDouble();

                if (roll <= 0.05)
                    return HideType.Barbed;
                if (roll <= 0.2)
                    return HideType.Horned;
                if (roll <= 0.5)
                    return HideType.Spined;

                return HideType.Regular;
            }
        }
		public override int Hides => 20;
        public override int Meat => 19;
        public override int Scales => 6;
        public override ScaleType ScaleType => ( Utility.RandomBool() ? ScaleType.Black : ScaleType.White );
        public override int TreasureMapLevel => 4;

        public SerpentineDragon( Serial serial ) : base( serial )
		{
		}

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );

			if ( !Core.SE && 0.2 > Utility.RandomDouble() && attacker is BaseCreature )
			{
				BaseCreature c = (BaseCreature)attacker;

				if ( c.Controlled && c.ControlMaster != null )
				{
					c.ControlTarget = c.ControlMaster;
					c.ControlOrder = OrderType.Attack;
					c.Combatant = c.ControlMaster;
				}
			}
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