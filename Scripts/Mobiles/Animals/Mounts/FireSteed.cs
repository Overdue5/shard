using System;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a fire steed corpse" )]
	public class FireSteed : BaseMount
	{
		[Constructable]
		public FireSteed() : this( "Fire Steed" )
		{
		}

		[Constructable]
		public FireSteed( string name ) : base( name, 0xBE, 0x3E9E, AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			BaseSoundID = 0xA8;

			SetStr( 376, 400 );
			SetDex( 91, 120 );
			SetInt( 291, 300 );

			SetHits( 226, 240 );

			SetDamage( 11, 30 );

			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Fire, 80 );

			SetResistance( ResistanceType.Physical, 30, 40 );
			SetResistance( ResistanceType.Fire, 70, 80 );
			SetResistance( ResistanceType.Cold, 20, 30 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.MagicResist, 100.0, 120.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.Wrestling, 100.0 );

			Fame = 20000;

			Tamable = true;
			ControlSlots = 2;
			MinTameSkill = 106.0;
		}

		public override void GenerateLoot()
		{
			PackItem( new SulfurousAsh( Utility.RandomMinMax( 151, 300 ) ) );
			PackItem( new Ruby( Utility.RandomMinMax( 16, 30 ) ) );
		}

		public override bool HasBreath => true; // fire breath enabled
		public override FoodType FavoriteFood => FoodType.Meat;
        public override PackInstinct PackInstinct => PackInstinct.Daemon | PackInstinct.Equine;

        public FireSteed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( BaseSoundID <= 0 )
				BaseSoundID = 0xA8;

            if (version < 1)
            {
                for (int i = 0; i < Skills.Length; ++i)
                {
                    Skills[i].Cap = Math.Max(100.0, Skills[i].Cap * 0.9);

                    if (Skills[i].Base > Skills[i].Cap)
                    {
                        Skills[i].Base = Skills[i].Cap;
                    }
                }
            }
		}
	}
}