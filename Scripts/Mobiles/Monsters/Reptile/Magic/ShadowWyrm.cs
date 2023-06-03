using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a shadow wyrm corpse" )]
	public class ShadowWyrm : BaseCreature
	{
		[Constructable]
		public ShadowWyrm() : base( AIType.AI_SphereMage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Shadow Wyrm";
			Body = 106;
			BaseSoundID = 362;

			SetStr( 898, 1030 );
			SetDex( 68, 200 );
			SetInt( 488, 620 );

			SetHits( 558, 599 );

			SetDamage( 29, 35 );

			SetDamageType( ResistanceType.Physical, 75 );
			SetDamageType( ResistanceType.Cold, 25 );

			SetResistance( ResistanceType.Physical, 65, 75 );
			SetResistance( ResistanceType.Fire, 50, 60 );
			SetResistance( ResistanceType.Cold, 45, 55 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 50, 60 );

			SetSkill( SkillName.EvalInt, 80.1, 100.0 );
			SetSkill( SkillName.Magery, 80.1, 100.0 );
			SetSkill( SkillName.Meditation, 52.5, 75.0 );
			SetSkill( SkillName.MagicResist, 100.3, 130.0 );
			SetSkill( SkillName.Tactics, 97.6, 100.0 );
			SetSkill( SkillName.Wrestling, 97.6, 100.0 );

			Fame = 22500;
			Karma = -22500;

			VirtualArmor = 70;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich );
		    AddLoot(LootPack.Rich);
			AddLoot( LootPack.Gems, 5 );
            PackGold(1300);
		}

		public override int GetIdleSound()
		{
			return 0x2D5;
		}

		public override int GetHurtSound()
		{
			return 0x2D1;
		}

		public override bool ReacquireOnMovement => true;
        public override bool HasBreath => true; // fire breath enabled
		public override bool AutoDispel => true;
        public override Poison PoisonImmune => Poison.Deadly;
        public override Poison HitPoison => Poison.Deadly;
        public override int TreasureMapLevel => 5;

        public override int Meat => 19;
        public override int Hides => 20;
        public override int Scales => 10;
        public override ScaleType ScaleType => ScaleType.Black;

        public override HideType HideType
        {
            get
            {
                double roll = Utility.RandomDouble();

                if (roll <= 0.1)
                    return HideType.Barbed;
                if (roll <= 0.3)
                    return HideType.Horned;
                if (roll <= 0.6)
                    return HideType.Spined;

                return HideType.Regular;
            }
        }

        public override void OnCarve(Mobile from, Corpse corpse, Item item)
        {
            corpse.DropItem(new WyrmsHeart(2));
            base.OnCarve(from, corpse, item);

        }

		public ShadowWyrm( Serial serial ) : base( serial )
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