using System;
using System.Collections;
using Server.Engines.CannedEvil;
using Server.Engines.Plants;
using Server.Items;

namespace Server.Mobiles
{
	public class Serado : BaseChampion
	{
		public override ChampionSkullType SkullType => ChampionSkullType.Power;

        public override Type[] UniqueList { get { return new Type[] { typeof(Pacify) }; } }
        public override Type[] SharedList
        {
            get
            {
                return new Type[] { 	typeof( BraveKnightOfTheBritannia ),
										typeof( DetectiveBoots ),
										typeof( EmbroideredOakLeafCloak ),
										typeof( LieutenantOfTheBritannianRoyalGuard ) };
            }
        }
        public override Type[] DecorativeList { get { return new Type[] { typeof(Futon), typeof(SwampTile) }; } }

        public override MonsterStatuetteType[] StatueTypes { get { return new MonsterStatuetteType[] { }; } }


		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.DoubleStrike;
		}

		[Constructable]
		public Serado() : base( AIType.AI_SphereMelee )
		{
			Name = "Serado";
			Title = "the awakened";

			Body = 249;
			Hue = 0x96C;

			SetStr( 1000 );
			SetDex( 150 );
			SetInt( 300 );

			SetHits( 9000 );
			SetMana( 300 );

			SetDamage( 29, 35 );

			SetDamageType( ResistanceType.Physical, 70 );
			SetDamageType( ResistanceType.Poison, 20 );
			SetDamageType( ResistanceType.Energy, 10 );

			SetResistance( ResistanceType.Physical, 30 );
			SetResistance( ResistanceType.Fire, 60 );
			SetResistance( ResistanceType.Cold, 60 );
			SetResistance( ResistanceType.Poison, 90 );
			SetResistance( ResistanceType.Energy, 50 );

			SetSkill( SkillName.MagicResist, 120.0 );
			SetSkill( SkillName.Tactics, 120.0 );
			SetSkill( SkillName.Wrestling, 70.0 );
			SetSkill( SkillName.Poisoning, 150.0 );

			Fame = 22500;
			Karma = -22500;

			PackItem( Seed.RandomBonsaiSeed() );

            Container backpack = Backpack;
            if (backpack == null)
            {
                backpack = new Backpack();

                backpack.Movable = false;

                AddItem(backpack);
            }
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 4 );
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.Gems, 6 );
		}

		public override int TreasureMapLevel => 5;

        public override Poison HitPoison => Poison.Lethal;
        public override Poison PoisonImmune => Poison.Lethal;
        public override double HitPoisonChance => 0.8;

        public override int Feathers => 30;

        public override bool ShowFameTitle => false;
        public override bool ClickTitle => false;

        // TODO: Hit Lightning Area

		public override void OnDamagedBySpell( Mobile attacker )
		{
			base.OnDamagedBySpell( attacker );

			ScaleResistances();
			DoCounter( attacker );
		}

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );

			ScaleResistances();
			DoCounter( attacker );
		}

		private void ScaleResistances()
		{
			double hitsLost = (HitsMax - Hits) / (double)HitsMax;

			SetResistance( ResistanceType.Physical,	30 + (int)(hitsLost * ( 95 - 30 )) );
			SetResistance( ResistanceType.Fire,		60 + (int)(hitsLost * ( 95 - 60 )) );
			SetResistance( ResistanceType.Cold,		60 + (int)(hitsLost * ( 95 - 60 )) );
			SetResistance( ResistanceType.Poison,	90 + (int)(hitsLost * ( 95 - 90 )) );
			SetResistance( ResistanceType.Energy,	50 + (int)(hitsLost * ( 95 - 50 )) );
		}

		private void DoCounter( Mobile attacker )
		{
			if ( Map == null || ( attacker is BaseCreature && ((BaseCreature)attacker).BardProvoked ) )
				return;

			if ( 0.2 > Utility.RandomDouble() )
			{
				/* Counterattack with Hit Poison Area
				 * 20-25 damage, unresistable
				 * Lethal poison, 100% of the time
				 * Particle effect: Type: "2" From: "0x4061A107" To: "0x0" ItemId: "0x36BD" ItemIdName: "explosion" FromLocation: "(296 615, 17)" ToLocation: "(296 615, 17)" Speed: "1" Duration: "10" FixedDirection: "True" Explode: "False" Hue: "0xA6" RenderMode: "0x0" Effect: "0x1F78" ExplodeEffect: "0x1" ExplodeSound: "0x0" Serial: "0x4061A107" Layer: "255" Unknown: "0x0"
				 * Doesn't work on provoked monsters
				 */

				Mobile target = null;

				if ( attacker is BaseCreature )
				{
					Mobile m = ((BaseCreature)attacker).GetMaster();

					if ( m != null )
						target = m;
				}

				if ( target == null || !target.InRange( this, 25 ) )
					target = attacker;

				Animate( 10, 4, 1, true, false, 0 );

				ArrayList targets = new ArrayList();

				foreach ( Mobile m in target.GetMobilesInRange( 8 ) )
				{
					if ( m == this || !CanBeHarmful( m ) )
						continue;

					if ( m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned || ((BaseCreature)m).Team != Team) )
						targets.Add( m );
					else if ( m.Player )
						targets.Add( m );
				}

				for ( int i = 0; i < targets.Count; ++i )
				{
					Mobile m = (Mobile)targets[i];

					DoHarmful( m );

					AOS.Damage( m, this, Utility.RandomMinMax( 20, 25 ), true, 0, 0, 0, 100, 0 );

					m.FixedParticles( 0x36BD, 1, 10, 0x1F78, 0xA6, 0, (EffectLayer)255 );
					m.ApplyPoison( this, Poison.Lethal );
				}
			}
		}

		public Serado( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
