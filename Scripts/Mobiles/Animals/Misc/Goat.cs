namespace Server.Mobiles
{
	[CorpseName( "a goat corpse" )]
	public class Goat : BaseCreature
	{
		[Constructable]
		public Goat() : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			//Name = "Goat";
            switch (Utility.Random(5))
            {
                case 0: Name = "Pygmy goat"; break;
                case 1: Name = "Nubian goat"; break;
                case 2: Name = "Alpine goat"; break;
                case 3: Name = "Saanen goat"; break;
                case 4: Name = "Boer goat"; break;
            }
			Body = 0xD1;
			BaseSoundID = 0x99;

			SetStr( 19 );
			SetDex( 15 );
			SetInt( 5 );

			SetHits( 12 );
			SetMana( 0 );

			SetDamage( 3, 4 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 5, 15 );

			SetSkill( SkillName.MagicResist, 5.0 );
			SetSkill( SkillName.Tactics, 5.0 );
			SetSkill( SkillName.Wrestling, 5.0 );

			Fame = 150;

			VirtualArmor = 10;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 11.1;
		}

		public override int Meat => 2;
        public override int Hides => 2;
        public override FoodType FavoriteFood => FoodType.GrainsAndHay | FoodType.FruitsAndVegies;

        public Goat(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}