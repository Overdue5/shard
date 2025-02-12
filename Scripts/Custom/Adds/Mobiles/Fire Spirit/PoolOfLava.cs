using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
	public class PoolOfLava : Item
	{
		private TimeSpan m_Duration;
		private int m_MinDamage;
		private int m_MaxDamage;

		private readonly DateTime m_Created;

		private bool m_Drying;

		private readonly Timer m_Timer;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Drying
		{
			get => m_Drying;
            set
			{
				m_Drying = value;

				if( m_Drying )
					ItemID = 0x122A; 
				else
					ItemID = 0x122B;
			}
		}


		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan Duration{ get => m_Duration;
            set => m_Duration = value;
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public int MinDamage
		{
			get => m_MinDamage;
            set
			{
				if ( value < 1 )
					value = 1;

				m_MinDamage = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int MaxDamage
		{
			get => m_MaxDamage;
            set
			{
				if ( value < 1 )
					value = 1;

				if ( value < MinDamage )
					value = MinDamage;

				m_MaxDamage = value;
			}
		}

		[Constructable]
		public PoolOfLava() : this( TimeSpan.FromSeconds( 10.0 ), 2, 5 )
		{
		}

		public override string DefaultName => "lava";

        [Constructable]
		public PoolOfLava( TimeSpan duration, int minDamage, int maxDamage )
			: base( 0x122A )
		{
			Hue = Utility.RandomList(2518, 2519, 2905, 2873);  
			Movable = false;
            
			m_MinDamage = minDamage;
			m_MaxDamage = maxDamage;
			m_Created = DateTime.UtcNow;
			m_Duration = duration;

			m_Timer = Timer.DelayCall( TimeSpan.Zero, TimeSpan.FromSeconds( 1 ), new TimerCallback( OnTick ) );
		}

		public override void OnAfterDelete()
		{
			if( m_Timer != null )
				m_Timer.Stop();
		}

		private void OnTick()
		{
			DateTime now = DateTime.UtcNow;
			TimeSpan age = now - m_Created;

			if( age > m_Duration )
				Delete();
			else
			{
				if( !Drying && age > (m_Duration - age) )
					Drying = true;

                List<Mobile> toDamage = new List<Mobile>();

				foreach( Mobile m in GetMobilesInRange( 0 ) )
				{
					BaseCreature bc = m as BaseCreature;

					if( m.Alive && !m.IsDeadBondedPet && (bc == null || bc.Controlled || bc.Summoned) )
					{
						toDamage.Add( m );
					}
				}

				for( int i = 0; i < toDamage.Count; i++ )
                    Damage(toDamage[i]);

			}
		}


		public override bool OnMoveOver( Mobile m )
		{
			Damage( m );
            m.PlaySound(841);
            m.SendAsciiMessage("The lava beneath you is blistering your skin!");
			return true;
		}

		public void Damage( Mobile m )
		{
			m.Damage( Utility.RandomMinMax( MinDamage, MaxDamage ) );
		}

		public PoolOfLava( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			//Don't serialize these
		}

		public override void Deserialize( GenericReader reader )
		{
		}
	}
}
