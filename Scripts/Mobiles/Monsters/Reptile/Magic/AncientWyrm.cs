using Server.Items;
namespace Server.Mobiles
{
	[CorpseName( "a dragon corpse" )]
	public class AncientWyrm : BaseCreature
	{
		[Constructable]
		public AncientWyrm () : base( AIType.AI_SphereMage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Ancient Wyrm";
			Body = 46;
			BaseSoundID = 362;

			SetStr( 1096, 1185 );
			SetDex( 86, 175 );
			SetInt( 686, 775 );

			SetHits( 658, 711 );

			SetDamage( 29, 35 );

			SetDamageType( ResistanceType.Physical, 75 );
			SetDamageType( ResistanceType.Fire, 25 );

			SetResistance( ResistanceType.Physical, 65, 75 );
			SetResistance( ResistanceType.Fire, 80, 90 );
			SetResistance( ResistanceType.Cold, 70, 80 );
			SetResistance( ResistanceType.Poison, 60, 70 );
			SetResistance( ResistanceType.Energy, 60, 70 );

			SetSkill( SkillName.EvalInt, 80.1, 100.0 );
			SetSkill( SkillName.Magery, 80.1, 100.0 );
			SetSkill( SkillName.Meditation, 52.5, 75.0 );
			SetSkill( SkillName.MagicResist, 100.5, 150.0 );
			SetSkill( SkillName.Tactics, 97.6, 100.0 );
			SetSkill( SkillName.Wrestling, 97.6, 100.0 );

			Fame = 22500;
			Karma = -22500;

			VirtualArmor = 70;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich);
			AddLoot( LootPack.Gems, 5 );
            AddLoot(LootPack.MedScrolls, 2);
            AddLoot(LootPack.HighScrolls, 2);
            PackGold(1800,2500);
            if (Utility.RandomDouble() <= 0.3)
                AddItem(new RandomAccWeap(Utility.RandomMinMax(3, 5)));

		}

		public override int GetIdleSound()
		{
			return 0x2D3;
		}

		public override int GetHurtSound()
		{
			return 0x2D1;
		}

		public override bool ReacquireOnMovement => true;
        public override bool HasBreath => true; // fire breath enabled
		public override bool AutoDispel => false;

        public override HideType HideType
        {
            get
            {
                double roll = Utility.RandomDouble();

                if (roll <= 0.3)
                    return HideType.Barbed;
                if (roll <= 0.8)
                    return HideType.Horned;

                return HideType.Spined;
            }
        }
		public override int Hides => 40;
        public override int Meat => 19;
        public override int Scales => 12;
        public override ScaleType ScaleType => (ScaleType)Utility.Random( 4 );
        public override Poison PoisonImmune => Poison.Regular;
        public override Poison HitPoison => Utility.RandomBool() ? Poison.Lesser : Poison.Regular;
        public override int TreasureMapLevel => 5;

        public override void OnCarve(Mobile from, Corpse corpse, Item item)
        {
            corpse.DropItem(new WyrmsHeart(2));
            base.OnCarve(from, corpse, item);

        }

		public AncientWyrm( Serial serial ) : base( serial )
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