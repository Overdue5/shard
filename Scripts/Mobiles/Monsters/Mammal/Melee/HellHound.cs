using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a hell hound corpse" )]
	public class HellHound : BaseCreature
	{
		[Constructable]
		public HellHound() : base( AIType.AI_SphereMelee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Hell Hound";
			Body = 98;
			BaseSoundID = 229;

			SetStr( 102, 150 );
			SetDex( 81, 105 );
			SetInt( 36, 60 );

			SetHits( 66, 125 );

			SetDamage( 11, 17 );

			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Fire, 80 );

			SetResistance( ResistanceType.Physical, 25, 35 );
			SetResistance( ResistanceType.Fire, 30, 40 );
			SetResistance( ResistanceType.Poison, 10, 20 );
			SetResistance( ResistanceType.Energy, 10, 20 );

			Fame = 3400;

			VirtualArmor = 30;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 85.5;

			PackItem( new SulfurousAsh( 5 ) );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
		}

		public override bool HasBreath => true; // fire breath enabled
		public override int Meat => 1;
        public override FoodType FavoriteFood => FoodType.Meat;
        public override PackInstinct PackInstinct => PackInstinct.Canine;

        public HellHound( Serial serial ) : base( serial )
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