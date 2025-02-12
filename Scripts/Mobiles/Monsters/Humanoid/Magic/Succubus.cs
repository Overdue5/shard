using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a succubus corpse" )]
	public class Succubus : BaseCreature
	{
		[Constructable]
		public Succubus () : base( AIType.AI_SphereMage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Succubus";
			Body = 149;
			BaseSoundID = 0x4B0;

			SetStr( 488, 620 );
			SetDex( 121, 170 );
			SetInt( 498, 657 );

			SetHits( 312, 353 );

			SetDamage( 18, 28 );

			SetDamageType( ResistanceType.Physical, 75 );
			SetDamageType( ResistanceType.Energy, 25 );

			SetResistance( ResistanceType.Physical, 80, 90 );
			SetResistance( ResistanceType.Fire, 70, 80 );
			SetResistance( ResistanceType.Cold, 40, 50 );
			SetResistance( ResistanceType.Poison, 50, 60 );
			SetResistance( ResistanceType.Energy, 50, 60 );

			SetSkill( SkillName.EvalInt, 90.1, 100.0 );
			SetSkill( SkillName.Magery, 99.1, 100.0 );
			SetSkill( SkillName.Meditation, 90.1, 100.0 );
			SetSkill( SkillName.MagicResist, 100.5, 150.0 );
			SetSkill( SkillName.Tactics, 80.1, 90.0 );
			SetSkill( SkillName.Wrestling, 80.1, 90.0 );

			Fame = 24000;
			Karma = -18000;

			VirtualArmor = 80;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich );
			AddLoot( LootPack.MedScrolls, 2 );
            PackGold(800, 1000);
            if (Utility.RandomDouble() <= 0.2)
                AddItem(new RandomAccWeap(Utility.RandomMinMax(3, 4)));
		}

		public override int Meat => 1;
        public override int TreasureMapLevel => 5;

        public void DrainLife()
		{
			ArrayList list = new ArrayList();

			foreach ( Mobile m in GetMobilesInRange( 2 ) )
			{
				if ( m == this || !CanBeHarmful( m ) )
					continue;

				if ( m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned || ((BaseCreature)m).Team != Team) )
					list.Add( m );
				else if ( m.Player )
					list.Add( m );
			}

			foreach ( Mobile m in list )
			{
				DoHarmful( m );

				m.FixedParticles( 0x374A, 10, 15, 5013, 0x496, 0, EffectLayer.Waist );
				m.PlaySound( 0x231 );

				m.SendMessage( "You feel the life drain out of you!" );

				int toDrain = Utility.RandomMinMax( 10, 40 );

				Hits += toDrain;
				m.Damage( toDrain, this );
			}
		}

		public override void OnGaveMeleeAttack( Mobile defender )
		{
			base.OnGaveMeleeAttack( defender );

			if ( 0.1 >= Utility.RandomDouble() )
				DrainLife();
		}

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );

			if ( 0.1 >= Utility.RandomDouble() )
				DrainLife();
		}

		public Succubus( Serial serial ) : base( serial )
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