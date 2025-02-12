namespace Server.Mobiles
{
	[CorpseName( "a giant ice worm corpse" )]
	public class GiantIceWorm : BaseCreature
	{
		public override bool SubdueBeforeTame => true;

        [Constructable]
		public GiantIceWorm() : base( AIType.AI_SphereMelee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 89;
			Name = "giant ice worm";
			BaseSoundID = 0xDC;

			SetStr( 216, 245 );
			SetDex( 76, 100 );
			SetInt( 66, 85 );

			SetHits( 130, 147 );

			SetDamage( 7, 17 );

			SetDamageType( ResistanceType.Physical, 10 );
			SetDamageType( ResistanceType.Cold, 90 );

			SetResistance( ResistanceType.Physical, 30, 35 );
			SetResistance( ResistanceType.Fire, 0 );
			SetResistance( ResistanceType.Cold, 80, 90 );
			SetResistance( ResistanceType.Poison, 15, 25 );
			SetResistance( ResistanceType.Energy, 10, 20 );

			SetSkill( SkillName.Poisoning, 75.1, 95.0 );
			SetSkill( SkillName.MagicResist, 45.1, 60.0 );
			SetSkill( SkillName.Tactics, 75.1, 80.0 );
			SetSkill( SkillName.Wrestling, 60.1, 80.0 );

			Fame = 4500;
			Karma = -4500;

			VirtualArmor = 40;

			Tamable = false;
			ControlSlots = 1;
			MinTameSkill = 71.1;
		}

		public override Poison PoisonImmune => Poison.Greater;

        public override Poison HitPoison => Poison.Greater;

        public override FoodType FavoriteFood => FoodType.Meat;

        public GiantIceWorm( Serial serial ) : base ( serial )
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize( writer );

			writer.Write( 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}