using System;
using System.Collections.Generic;

namespace Server.Factions
{
	public class FactionState
	{
		private readonly Faction m_Faction;
		private Mobile m_Commander;
		private int m_Tithe;
		private int m_Silver;
		private List<PlayerState> m_Members;
		private Election m_Election;
		private List<FactionItem> m_FactionItems;
		private List<BaseFactionTrap> m_FactionTraps;
        private DateTime m_LastAtrophy;

		private const int BroadcastsPerPeriod = 2;
		private static readonly TimeSpan BroadcastPeriod = TimeSpan.FromHours( 1.0 );

		private readonly DateTime[] m_LastBroadcasts = new DateTime[BroadcastsPerPeriod];

        public DateTime LastAtrophy { get => m_LastAtrophy;
            set => m_LastAtrophy = value;
        }

		public bool FactionMessageReady
		{
			get
			{
				for ( int i = 0; i < m_LastBroadcasts.Length; ++i )
				{
					if ( DateTime.UtcNow >= (m_LastBroadcasts[i] + BroadcastPeriod) )
						return true;
				}

				return false;
			}
		}

        public bool IsAtrophyReady => DateTime.UtcNow >= (m_LastAtrophy + TimeSpan.FromHours(47.0));

        public int CheckAtrophy()
        {
            if (DateTime.UtcNow < (m_LastAtrophy + TimeSpan.FromHours(47.0)))
                return 0;

            int distrib = 0;
            m_LastAtrophy = DateTime.UtcNow;

            List<PlayerState> members = new List<PlayerState>(m_Members);

            for (int i = 0; i < members.Count; ++i)
            {
                PlayerState ps = members[i];

                if (ps.IsActive)
                {
                    ps.IsActive = false;
                    continue;
                }
                else if (ps.KillPoints > 0)
                {
                    int atrophy = (ps.KillPoints + 9) / 10;
                    ps.KillPoints -= atrophy;
                    distrib += atrophy;
                }
            }

            return distrib;
        }

		public void RegisterBroadcast()
		{
			for ( int i = 0; i < m_LastBroadcasts.Length; ++i )
			{
				if ( DateTime.UtcNow >= (m_LastBroadcasts[i] + BroadcastPeriod) )
				{
					m_LastBroadcasts[i] = DateTime.UtcNow;
					break;
				}
			}
		}

		public List<FactionItem> FactionItems
		{
			get => m_FactionItems;
            set => m_FactionItems = value;
        }

		public List<BaseFactionTrap> Traps
		{
			get => m_FactionTraps;
            set => m_FactionTraps = value;
        }

		public Election Election
		{
			get => m_Election;
            set => m_Election = value;
        }

		public Mobile Commander
		{
			get => m_Commander;
            set
			{
				if ( m_Commander != null )
					m_Commander.InvalidateProperties();

				m_Commander = value;

				if ( m_Commander != null )
				{
					m_Commander.SendLocalizedMessage( 1042227 ); // You have been elected Commander of your faction

					m_Commander.InvalidateProperties();

					PlayerState pl = PlayerState.Find( m_Commander );

					if ( pl != null && pl.Finance != null )
						pl.Finance.Finance = null;

					if ( pl != null && pl.Sheriff != null )
						pl.Sheriff.Sheriff = null;
				}
			}
		}

		public int Tithe
		{
			get => m_Tithe;
            set => m_Tithe = value;
        }

		public int Silver
		{
			get => m_Silver;
            set => m_Silver = value;
        }

		public List<PlayerState> Members
		{
			get => m_Members;
            set => m_Members = value;
        }

		public FactionState( Faction faction )
		{
			m_Faction = faction;
			m_Tithe = 50;
			m_Members = new List<PlayerState>();
			m_Election = new Election( faction );
			m_FactionItems = new List<FactionItem>();
			m_FactionTraps = new List<BaseFactionTrap>();
		}

		public FactionState( GenericReader reader )
		{
			int version = reader.ReadEncodedInt();

			switch ( version )
			{
                case 5:
                    {
                        m_LastAtrophy = reader.ReadDateTime();
                        goto case 4;
                    }
				case 4:
				{
					int count = reader.ReadEncodedInt();

					for ( int i = 0; i < count; ++i )
					{
						DateTime time = reader.ReadDateTime();

						if ( i < m_LastBroadcasts.Length )
							m_LastBroadcasts[i] = time;
					}

					goto case 3;
				}
				case 3:
				case 2:
				case 1:
				{
					m_Election = new Election( reader );

					goto case 0;
				}
				case 0:
				{
					m_Faction = Faction.ReadReference( reader );

					m_Commander = reader.ReadMobile();

                    if (version < 5)
                        m_LastAtrophy = DateTime.UtcNow;

					if ( version < 4 )
					{
						DateTime time = reader.ReadDateTime();

						if ( m_LastBroadcasts.Length > 0 )
							m_LastBroadcasts[0] = time;
					}

					m_Tithe = reader.ReadEncodedInt();
					m_Silver = reader.ReadEncodedInt();

					int memberCount = reader.ReadEncodedInt();

					m_Members = new List<PlayerState>();

					for ( int i = 0; i < memberCount; ++i )
					{
						PlayerState pl = new PlayerState( reader, m_Faction, m_Members );

						if ( pl.Mobile != null )
							m_Members.Add( pl );
					}

					m_Faction.State = this;

                    m_Faction.ZeroRankOffset = m_Members.Count;
                    m_Members.Sort();

                    for (int i = m_Members.Count - 1; i >= 0; i--)
                    {
                        PlayerState player = m_Members[i];

                        if (player.KillPoints <= 0)
                            m_Faction.ZeroRankOffset = i;
                        else
                            player.RankIndex = i;
                    }


					m_FactionItems = new List<FactionItem>();

					if ( version >= 2 )
					{
						int factionItemCount = reader.ReadEncodedInt();

						for ( int i = 0; i < factionItemCount; ++i )
						{
							FactionItem factionItem = new FactionItem( reader, m_Faction );

                            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(factionItem.CheckAttach)); // sandbox attachment
						}
					}

					m_FactionTraps = new List<BaseFactionTrap>();

					if ( version >= 3 )
					{
						int factionTrapCount = reader.ReadEncodedInt();

						for ( int i = 0; i < factionTrapCount; ++i )
						{
							BaseFactionTrap trap = reader.ReadItem() as BaseFactionTrap;

							if ( trap != null && !trap.CheckDecay() )
								m_FactionTraps.Add( trap );
						}
					}

					break;
				}
			}

			if ( version < 1 )
				m_Election = new Election( m_Faction );
		}

		public void Serialize( GenericWriter writer )
		{
            writer.WriteEncodedInt((int)5); // version

            writer.Write(m_LastAtrophy);

			writer.WriteEncodedInt( 4 ); // version

			writer.WriteEncodedInt( m_LastBroadcasts.Length );

			for ( int i = 0; i < m_LastBroadcasts.Length; ++i )
				writer.Write( m_LastBroadcasts[i] );

			m_Election.Serialize( writer );

			Faction.WriteReference( writer, m_Faction );

			writer.Write( m_Commander );

			writer.WriteEncodedInt( m_Tithe );
			writer.WriteEncodedInt( m_Silver );

			writer.WriteEncodedInt( m_Members.Count );

			for ( int i = 0; i < m_Members.Count; ++i )
			{
				PlayerState pl = m_Members[i];

				pl.Serialize( writer );
			}

			writer.WriteEncodedInt( m_FactionItems.Count );

			for ( int i = 0; i < m_FactionItems.Count; ++i )
				m_FactionItems[i].Serialize( writer );

			writer.WriteEncodedInt( m_FactionTraps.Count );

			for ( int i = 0; i < m_FactionTraps.Count; ++i )
				writer.Write( m_FactionTraps[i] );
		}
	}
}