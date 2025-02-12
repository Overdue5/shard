using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a dragon corpse" )]
	public class Dragon : BaseCreature
	{
		[Constructable]
        public Dragon()
            : base(AIType.AI_SphereMelee, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			Name = "Dragon";
			Body = Utility.RandomList( 12, 59 );
			BaseSoundID = 362;

			SetStr( 650, 700 );
			SetDex( 86, 105 );
			SetInt( 436, 475 );

			SetHits( 548, 575 );

			SetDamage( 16, 22 );

			SetDamageType( ResistanceType.Physical, 90 );

			SetResistance( ResistanceType.Physical, 55, 65 );
			SetResistance( ResistanceType.Fire, 60, 70 );
			SetResistance( ResistanceType.Cold, 30, 40 );
			SetResistance( ResistanceType.Poison, 25, 35 );
			SetResistance( ResistanceType.Energy, 35, 45 );

			SetSkill( SkillName.EvalInt, 30.1, 40.0 );
			SetSkill( SkillName.Magery, 30.1, 40.0 );
			SetSkill( SkillName.MagicResist, 99.1, 100.0 );
			SetSkill( SkillName.Tactics, 97.6, 100.0 );
			SetSkill( SkillName.Wrestling, 90.1, 92.5 );

            AddItem(new DragonsBlood(Utility.RandomMinMax(4, 9)));

			Fame = 15000;
			Karma = -11000;

			VirtualArmor = 55;

			Tamable = true;
			ControlSlots = 3;
			MinTameSkill = 93.9;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.Gems, 8 );
            AddLoot(LootPack.MedScrolls, 1);
            AddLoot(LootPack.HighScrolls, 1);
            PackGold(1000);
            if (Utility.RandomDouble() <= 0.1)
                AddItem(new RandomAccWeap(Utility.RandomMinMax(2, 3)));
		}

	    public override bool ReacquireOnMovement => !Controlled;

        public override bool HasBreath => true; // fire breath enabled
//		public override bool AutoDispel{ get{ return !Controlled; } }
		public override int TreasureMapLevel => 4;
        public override int Meat => 19;
        public override int Hides => 20;

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
		public override int Scales => 7;
        public override ScaleType ScaleType => ( Body == 12 ? ScaleType.Yellow : ScaleType.Red );
        public override FoodType FavoriteFood => FoodType.Meat;
        public override bool CanAngerOnTame => true;

        public Dragon( Serial serial ) : base( serial )
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