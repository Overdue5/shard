using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a decaying zombie's corpse" )]
	public class DecayingZombie : BaseCreature
	{
		[Constructable]
		public DecayingZombie() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Decaying Zombie";
			Body = 0x3;
			BaseSoundID = 471;
			Hue = 371;

			SetStr( 95, 115 );
			SetDex( 65, 80 );
			SetInt( 16, 30 );

			SetHits( 320, 340 );
			SetMana( 0 );

			SetDamage( 15, 20 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 42, 47 );
			SetResistance( ResistanceType.Fire, 30, 50 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Cold, 30, 45 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.MagicResist, 45.1, 55.0 );
			SetSkill( SkillName.Tactics, 72.3, 92.0 );
			SetSkill( SkillName.Wrestling, 76.3, 92.0 );

			Fame = 7000;
			Karma = -7000;

			VirtualArmor = 20;
		
			switch ( Utility.Random( 8 ) )
                                 {
                                   case 0: PackItem( new DecayingHead() ); break;
                                 }
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
			
		}

		
		public override bool AlwaysMurderer => true;

        public DecayingZombie( Serial serial ) : base( serial )
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