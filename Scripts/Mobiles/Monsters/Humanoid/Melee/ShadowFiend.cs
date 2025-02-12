using System;
using Server.Items;

namespace Server.Mobiles
{
	public class ShadowFiend : BaseCreature
	{
		public override bool DeleteCorpseOnDeath => true;

        private UnhideTimer m_Timer;

		[Constructable]
		public ShadowFiend() : base( AIType.AI_SphereMelee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Shadow Fiend";
			Body = 0xA8;

			// this to allow shadow fiend to loot from corpses
			Backpack backpack = new Backpack();
			backpack.Movable = false;
			AddItem( backpack );

			SetStr( 46, 55 );
			SetDex( 121, 130 );
			SetInt( 46, 55 );

			SetHits( 28, 33 );
			SetStam( 46, 55 );

			SetDamage( 10, 22 );

			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Cold, 80 );

			SetResistance( ResistanceType.Physical, 20, 25 );
			SetResistance( ResistanceType.Fire, 20, 25 );
			SetResistance( ResistanceType.Cold, 40, 45 );
			SetResistance( ResistanceType.Poison, 60, 70 );
			SetResistance( ResistanceType.Energy, 5, 10 );

			SetSkill( SkillName.MagicResist, 20.1, 30.0 );
			SetSkill( SkillName.Tactics, 20.1, 30.0 );
			SetSkill( SkillName.Wrestling, 20.1, 30.0 );

			Fame = 1000;

			m_Timer = new UnhideTimer( this );
			m_Timer.Start();
		}

        public override bool IsNight => true;
        
        public override int GetIdleSound()
		{
			return 0x37A;
		}

		public override int GetAngerSound()
		{
			return 0x379;
		}

		public override int GetDeathSound()
		{
			return 0x381;
		}

		public override int GetAttackSound()
		{
			return 0x37F;
		}

		public override int GetHurtSound()
		{
			return 0x380;
		}

		public override bool CanRummageCorpses => true;

        public override bool OnBeforeDeath()
		{
			if ( Backpack != null )
				Backpack.Destroy();

			Effects.SendLocationEffect( Location, Map, 0x376A, 10, 1 );
			return true;
		}

		public ShadowFiend( Serial serial ) : base( serial )
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

			m_Timer = new UnhideTimer( this );
			m_Timer.Start();
		}

		public override void OnAfterDelete()
		{
			if ( m_Timer != null )
				m_Timer.Stop();

			m_Timer = null;

			base.OnAfterDelete();
		}

		private class UnhideTimer : Timer
		{
			private readonly ShadowFiend m_Owner;

			public UnhideTimer( ShadowFiend owner ) : base( TimeSpan.FromSeconds( 30.0 ) )
			{
				m_Owner = owner;
				Priority = TimerPriority.OneSecond;
			}

			protected override void OnTick()
			{
				if ( m_Owner.Deleted )
				{
					Stop();
					return;
				}

				foreach ( Mobile m in m_Owner.GetMobilesInRange( 3 ) )
				{
					if ( m != m_Owner && m.Player && m.Hidden && m_Owner.CanBeHarmful( m ) && m.AccessLevel == AccessLevel.Player )
						m.Hidden = false;
				}
			}
		}
	}
}