using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a plant corpse" )]
	public class Bogling : BaseCreature
	{
		[Constructable]
		public Bogling() : base( AIType.AI_SphereMelee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Bogling";
			Body = 779;
			BaseSoundID = 422;

			SetStr( 96, 120 );
			SetDex( 91, 115 );
			SetInt( 21, 45 );

			SetHits( 58, 72 );

			SetDamage( 5, 7 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 20, 25 );
			SetResistance( ResistanceType.Fire, 10, 20 );
			SetResistance( ResistanceType.Cold, 15, 25 );
			SetResistance( ResistanceType.Poison, 15, 25 );
			SetResistance( ResistanceType.Energy, 15, 25 );

			SetSkill( SkillName.MagicResist, 75.1, 100.0 );
			SetSkill( SkillName.Tactics, 55.1, 80.0 );
			SetSkill( SkillName.Wrestling, 55.1, 75.0 );

			Fame = 450;

			VirtualArmor = 28;

			PackItem( new Log( 4 ) );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager );
		}

		public override int Hides => 6;
        public override int Meat => 1;

        public Bogling( Serial serial ) : base( serial )
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