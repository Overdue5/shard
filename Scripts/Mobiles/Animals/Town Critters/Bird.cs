namespace Server.Mobiles
{
	[CorpseName( "a bird corpse" )]
	public class Bird : BaseCreature
	{
		[Constructable]
		public Bird() : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
		    Name = NameList.RandomName("bird");
            Hue = Utility.RandomBirdHue();
			Body = 6;
			BaseSoundID = 0x1B;

			VirtualArmor = Utility.RandomMinMax( 0, 6 );

			SetStr( 10 );
			SetDex( 25, 35 );
			SetInt( 10 );

			SetDamage( 0 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetSkill( SkillName.Wrestling, 4.2, 6.4 );
			SetSkill( SkillName.Tactics, 4.0, 6.0 );
			SetSkill( SkillName.MagicResist, 4.0, 5.0 );

			Fame = 150;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = -6.9;
		}

		public override MeatType MeatType => MeatType.Bird;
        public override int Meat => 1;
        public override int Feathers => 2;
        public override FoodType FavoriteFood => FoodType.FruitsAndVegies | FoodType.GrainsAndHay;

        public Bird( Serial serial ) : base( serial )
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

			if ( Hue == 0 )
				Hue = Utility.RandomBirdHue();
		} 
	}

	[CorpseName( "a bird corpse" )]
	public class TropicalBird : BaseCreature
	{
		[Constructable]
		public TropicalBird() : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Hue = Utility.RandomBirdHue();
			Name = "Tropical Bird";

			Body = 6;
			BaseSoundID = 0xBF;

			VirtualArmor = Utility.RandomMinMax( 0, 6 );

			SetStr( 10 );
			SetDex( 25, 35 );
			SetInt( 10 );

			SetDamage( 0 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetSkill( SkillName.Wrestling, 4.2, 6.4 );
			SetSkill( SkillName.Tactics, 4.0, 6.0 );
			SetSkill( SkillName.MagicResist, 4.0, 5.0 );

			Fame = 150;
			Karma = 0;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = -6.9;
		}

		public override MeatType MeatType => MeatType.Bird;
        public override int Meat => 1;
        public override int Feathers => 25;
        public override FoodType FavoriteFood => FoodType.FruitsAndVegies | FoodType.GrainsAndHay;

        public TropicalBird( Serial serial ) : base( serial )
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