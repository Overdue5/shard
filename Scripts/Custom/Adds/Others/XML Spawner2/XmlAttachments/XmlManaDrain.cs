using System;
using Server.Items;

namespace Server.Engines.XmlSpawner2
{
	public class XmlManaDrain : XmlAttachment
	{
		private int m_Drain = 0;
		private TimeSpan m_Refractory = TimeSpan.FromSeconds(5);    // 5 seconds default time between activations
		private DateTime m_EndTime;
		private int proximityrange = 5;     // default movement activation from 5 tiles away

		[CommandProperty( AccessLevel.GameMaster )]
		public int Drain { get => m_Drain;
            set => m_Drain = value;
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Range { get => proximityrange;
            set => proximityrange  = value;
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan Refractory { get => m_Refractory;
            set => m_Refractory  = value;
        }

		// These are the various ways in which the message attachment can be constructed.
		// These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
		// Other overloads could be defined to handle other types of arguments

		// a serial constructor is REQUIRED
		public XmlManaDrain(ASerial serial) :  base(serial)
		{
		}
        
		[Attachable]
		public XmlManaDrain(int drain)
		{
			m_Drain = drain;
		}

		[Attachable]
		public XmlManaDrain(int drain, double refractory)
		{
			m_Drain = drain;
			Refractory = TimeSpan.FromSeconds(refractory);

		}
        
		[Attachable]
		public XmlManaDrain(int drain, double refractory, double expiresin)
		{
			m_Drain = drain;
			Expiration = TimeSpan.FromMinutes(expiresin);
			Refractory = TimeSpan.FromSeconds(refractory);
		}


		// note that this method will be called when attached to either a mobile or a weapon
		// when attached to a weapon, only that weapon will do additional damage
		// when attached to a mobile, any weapon the mobile wields will do additional damage
		public override void OnWeaponHit(Mobile attacker, Mobile defender, BaseWeapon weapon, int damageGiven)
		{
			// if it is still refractory then return
			if(DateTime.UtcNow < m_EndTime) return;

			int drain = 0;

			if(m_Drain > 0)
				drain = Utility.Random(m_Drain);

			if(defender != null && attacker != null && drain > 0)
			{
				defender.Mana -= drain;
				if(defender.Mana < 0) defender.Mana = 0;
				attacker.Mana += drain;
				if(attacker.Mana < 0) attacker.Mana = 0;

				m_EndTime = DateTime.UtcNow + Refractory;
			}
		}
        
		public override bool HandlesOnMovement => true;

        public override void OnMovement(MovementEventArgs e )
		{
			base.OnMovement(e);
		    
			if(e.Mobile == null || e.Mobile.AccessLevel > AccessLevel.Player) return;

			if(AttachedTo is Item && (((Item)AttachedTo).Parent == null) && Utility.InRange( e.Mobile.Location, ((Item)AttachedTo).Location, proximityrange ))
			{
				OnTrigger(null, e.Mobile);
			} 
			else
				return;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);

			writer.Write( 1 );
			// version 1
			writer.Write(proximityrange);
			// version 0
			writer.Write(m_Drain);
			writer.Write(m_Refractory);
			writer.Write(m_EndTime - DateTime.UtcNow);

		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			switch(version)
			{
				case 1:
					// version 1
					Range = reader.ReadInt();
					goto case 0;
				case 0:
					// version 0
					m_Drain = reader.ReadInt();
					Refractory = reader.ReadTimeSpan();
					TimeSpan remaining = reader.ReadTimeSpan();
					m_EndTime = DateTime.UtcNow + remaining;
					break;
			}
		}

		public override string OnIdentify(Mobile from)
		{
			string msg = null;

			if(Expiration > TimeSpan.Zero)
			{
				msg = String.Format("Mana drain {0} expires in {1} mins", m_Drain, Expiration.TotalMinutes);
			} 
			else
			{
				msg = String.Format("Mana drain {0}",m_Drain);
			}
            
			if(Refractory > TimeSpan.Zero)
			{
				return String.Format("{0} : {1} secs between uses",msg, Refractory.TotalSeconds);
			} 
			else
				return msg;
		}
		
		public override void OnAttach()
		{
			base.OnAttach();

			// announce it to the mob
			if(AttachedTo is Mobile)
			{
				if(m_Drain > 0)
					((Mobile)AttachedTo).SendMessage("You have been granted the power of Mana Drain!");
				else
					((Mobile)AttachedTo).SendMessage("You have been cursed with Mana Drain!");
			}
		}
		
		public override void OnTrigger(object activator, Mobile m)
		{
			if(m == null ) return;

			// if it is still refractory then return
			if(DateTime.UtcNow < m_EndTime) return;

			int drain = 0;

			if(m_Drain > 0)
				drain = Utility.Random(m_Drain);

			if(drain > 0)
			{
				m.Mana -= drain;
				if(m.Mana < 0) m.Mana = 0;

			}

			m_EndTime = DateTime.UtcNow + Refractory;

		}    
	}
}
