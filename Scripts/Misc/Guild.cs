using System;
using System.Collections;
using System.Collections.Generic;
using Server.Commands;
using Server.Commands.Generic;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Guilds
{
	#region Ranks
	[Flags]
	public enum RankFlags
	{
		None				= 0x00000000,
		CanInvitePlayer		= 0x00000001,
		AccessGuildItems	= 0x00000002,
		RemoveLowestRank	= 0x00000004,
		RemovePlayers		= 0x00000008,
		CanPromoteDemote	= 0x00000010,
		ControlWarStatus	= 0x00000020,
		AllianceControl		= 0x00000040,
		CanSetGuildTitle	= 0x00000080,
		CanVote				= 0x00000100,

		All	= Member | CanInvitePlayer | RemovePlayers | CanPromoteDemote | ControlWarStatus | AllianceControl | CanSetGuildTitle,
		Member = RemoveLowestRank | AccessGuildItems | CanVote
	}

	public class RankDefinition
	{
		public static RankDefinition[] Ranks = new RankDefinition[]
			{
				new RankDefinition( 1062963, 0, RankFlags.None ),	//Ronin
				new RankDefinition( 1062962, 1, RankFlags.Member ),	//Member
				new RankDefinition( 1062961, 2, RankFlags.Member | RankFlags.RemovePlayers | RankFlags.CanInvitePlayer | RankFlags.CanSetGuildTitle | RankFlags.CanPromoteDemote ),	//Emmissary
				new RankDefinition( 1062960, 3, RankFlags.Member | RankFlags.ControlWarStatus ),	//Warlord
				new RankDefinition( 1062959, 4, RankFlags.All )	//Leader
			};
		public static RankDefinition Leader{ get{ return Ranks[4]; } }
		public static RankDefinition Member{ get{ return Ranks[1]; } }
		public static RankDefinition Lowest{ get{ return Ranks[0]; } }

		private readonly TextDefinition m_Name;
		private readonly int m_Rank;
		private RankFlags m_Flags;

		public TextDefinition Name{ get{ return m_Name; } }
		public int Rank{ get{ return m_Rank; } }
		public RankFlags Flags{ get{ return m_Flags; } }

		public RankDefinition( TextDefinition name, int rank, RankFlags flags )
		{
			m_Name = name; 
			m_Rank = rank;
			m_Flags = flags;
		}

		public bool GetFlag( RankFlags flag )
		{
			return ( (m_Flags & flag) != 0 );
		}

		public void SetFlag( RankFlags flag, bool value )
		{
			if ( value )
				m_Flags |= flag;
			else
				m_Flags &= ~flag;
		}
	}

	#endregion

	#region Alliances
	public class AllianceInfo
	{
		private static readonly Dictionary<string, AllianceInfo> m_Alliances = new Dictionary<string, AllianceInfo>();

		public static Dictionary<string, AllianceInfo> Alliances
		{
			get{ return m_Alliances; }
		}

		private readonly string m_Name;
		private Guild m_Leader;
		private readonly List<Guild> m_Members;
		private readonly List<Guild> m_PendingMembers;
		
		public string Name
		{
			get{ return m_Name; }
		}

		public void CalculateAllianceLeader()
		{
			m_Leader = ((m_Members.Count >= 2) ? m_Members[Utility.Random( m_Members.Count )] : null);
		}

		public void CheckLeader()
		{
			if( m_Leader == null || m_Leader.Disbanded )
			{
				CalculateAllianceLeader();

				if( m_Leader == null )
					Disband();
			}
		}

		public Guild Leader
		{
			get
			{
				CheckLeader();
				return m_Leader; 
			}
			set
			{
				if( m_Leader != value && value != null )
					AllianceMessage( 1070765, value.Name ); // Your Alliance is now led by ~1_GUILDNAME~

				m_Leader = value;

				if( m_Leader == null )
					CalculateAllianceLeader();
			}
		}

		public bool IsPendingMember( Guild g )
		{
			if( g.Alliance != this )
				return false;

			return m_PendingMembers.Contains( g );
		}

		public bool IsMember( Guild g )
		{
			if( g.Alliance != this )
				return false;

			return m_Members.Contains( g );
		}

		public AllianceInfo( Guild leader, string name, Guild partner )
		{
			m_Leader = leader;
			m_Name = name;

			m_Members = new List<Guild>();
			m_PendingMembers = new List<Guild>();

			leader.Alliance = this;
			partner.Alliance = this;

			if( !m_Alliances.ContainsKey( m_Name.ToLower() ) )
				m_Alliances.Add( m_Name.ToLower(), this );
		}

		public void Serialize( GenericWriter writer )
		{
			writer.Write( 0 );	//Version

			writer.Write( m_Name );
			writer.Write( m_Leader );

			writer.WriteGuildList( m_Members, true );
			writer.WriteGuildList( m_PendingMembers, true );

			if( !m_Alliances.ContainsKey( m_Name.ToLower() ) )
				m_Alliances.Add( m_Name.ToLower(), this );
		}

		public AllianceInfo( GenericReader reader )
		{
			var version = reader.ReadInt();

			switch( version )
			{
				case 0:
				{
					m_Name = reader.ReadString();
					m_Leader = reader.ReadGuild() as Guild;

					m_Members = reader.ReadStrongGuildList<Guild>();
					m_PendingMembers = reader.ReadStrongGuildList<Guild>();

					break;
				}
			}
		}

		public void AddPendingGuild( Guild g )
		{
			if( g.Alliance != this || m_PendingMembers.Contains( g ) || m_Members.Contains( g ) )
				return;

			m_PendingMembers.Add( g );
		}

		public void TurnToMember( Guild g )
		{
			if( g.Alliance != this || !m_PendingMembers.Contains( g ) || m_Members.Contains( g ) )
				return;

			g.GuildMessage( 1070760, Name ); // Your Guild has joined the ~1_ALLIANCENAME~ Alliance.
			AllianceMessage( 1070761, g.Name ); // A new Guild has joined your Alliance: ~1_GUILDNAME~

			m_PendingMembers.Remove( g );
			m_Members.Add( g );
            g.Alliance.InvalidateMemberProperties();
        }

		public void RemoveGuild( Guild g )
		{
			if( m_PendingMembers.Contains( g ) )
			{
				m_PendingMembers.Remove( g );
			}
			
			if( m_Members.Contains( g ) )	//Sanity, just incase someone with a custom script adds a character to BOTH arrays
			{
				m_Members.Remove( g );
				g.InvalidateMemberProperties();

				g.GuildMessage( 1070763, Name ); // Your Guild has been removed from the ~1_ALLIANCENAME~ Alliance.
				AllianceMessage( 1070764, g.Name ); // A Guild has left your Alliance: ~1_GUILDNAME~
			}

			//g.Alliance = null;	//NO G.Alliance call here.  Set the Guild's Alliance to null, if you JUST use RemoveGuild, it removes it from the alliance, but doesn't remove the link from the guild to the alliance.  setting g.Alliance will call this method.
			//to check on OSI: have 3 guilds, make 2 of them a member, one pending.  remove one of the memebers.  alliance still exist?
			//ANSWER: NO

			if( g == m_Leader )
			{
				CalculateAllianceLeader();

				/*
				if( m_Leader == null ) //only when m_members.count < 2
					Disband();
				else
					AllianceMessage( 1070765, m_Leader.Name ); // Your Alliance is now led by ~1_GUILDNAME~
				*/
			}

			if( m_Members.Count < 2 )
				Disband();
		}

		public void Disband()
		{
			AllianceMessage( 1070762 ); // Your Alliance has dissolved.

			for( var i = 0; i < m_PendingMembers.Count; i++ )
				m_PendingMembers[i].Alliance = null;

			for( var i =0; i < m_Members.Count; i++ )
				m_Members[i].Alliance = null;


			AllianceInfo aInfo;

			m_Alliances.TryGetValue( m_Name.ToLower(), out aInfo );

			if( aInfo == this )
				m_Alliances.Remove( m_Name.ToLower() );
		}

		public void InvalidateMemberProperties()
		{
			InvalidateMemberProperties( false );
		}

		public void InvalidateMemberProperties( bool onlyOPL )
		{
			for ( var i = 0; i < m_Members.Count; i++ )
			{
				var g = m_Members[i];

				g.InvalidateMemberProperties( onlyOPL );
			}
		}

		public void InvalidateMemberNotoriety()
		{
			for ( var i = 0; i < m_Members.Count; i++ )
				m_Members[i].InvalidateMemberNotoriety();
		}

		#region Alliance[Text]Message(...)
		public void AllianceMessage( int num, bool append, string format, params object[] args )
		{
			AllianceMessage( num, append, String.Format( format, args) );
		}
		public void AllianceMessage( int number )
		{
			for ( var i = 0; i < m_Members.Count; ++i )
				m_Members[i].GuildMessage( number );
		}
		public void AllianceMessage( int number, string args )
		{
			AllianceMessage( number, args, 0x3B2 );
		}
		public void AllianceMessage( int number, string args, int hue )
		{
			for ( var i = 0; i < m_Members.Count; ++i )
				m_Members[i].GuildMessage( number, args, hue );
		}
		public void AllianceMessage( int number, bool append, string affix )
		{
			AllianceMessage( number, append, affix, "", 0x3B2 );
		}
		public void AllianceMessage( int number, bool append, string affix, string args )
		{
			AllianceMessage( number, append, affix, args, 0x3B2 );
		}
		public void AllianceMessage( int number, bool append, string affix, string args, int hue )
		{
			for ( var i = 0; i < m_Members.Count; ++i )
				m_Members[i].GuildMessage( number, append, affix, args, hue );
		}

		public void AllianceTextMessage( string text )
		{
			AllianceTextMessage( 0x3B2, text );
		}
		public void AllianceTextMessage( string format, params object[] args )
		{
			AllianceTextMessage( 0x3B2, String.Format( format, args ) );
		}
		public void AllianceTextMessage( int hue, string text )
		{
			for( var i = 0; i < m_Members.Count; ++i )
				m_Members[i].GuildTextMessage( hue, text );
		}
		public void AllianceTextMessage( int hue, string format, params object[] args )
		{
			AllianceTextMessage( hue, String.Format( format, args ) );
		}

		public void AllianceChat( Mobile from, int hue, string text )
		{
			Packet p = null;
			for( var i = 0; i < m_Members.Count; i++ )
			{
				var g = m_Members[i];

				for( var j = 0; j < g.Members.Count; j++ )
				{
					var m = g.Members[j];

					var state = m.NetState;

					if( state != null )
					{
						if( p == null )
							p = Packet.Acquire( new UnicodeMessage( from.Serial, from.Body, MessageType.Alliance, hue, 3, from.Language, from.Name, text ) );

						state.Send( p );
					}
				}
			}

			Packet.Release( p );
		}

		public void AllianceChat( Mobile from, string text )
		{
			var pm = from as PlayerMobile;

			AllianceChat( from, (pm == null) ? 0x3B2 : pm.AllianceMessageHue, text );
		}
		#endregion

		public class AllianceRosterGump : GuildDiplomacyGump
		{
			protected override bool AllowAdvancedSearch{ get{ return false; } }

			private readonly AllianceInfo m_Alliance;

			public AllianceRosterGump( PlayerMobile pm, Guild g, AllianceInfo alliance ): base( pm, g, true, "", 0, alliance.m_Members, alliance.Name )
			{
				m_Alliance = alliance;
			}

			public AllianceRosterGump( PlayerMobile pm, Guild g, AllianceInfo alliance, IComparer<Guild> currentComparer, bool ascending, string filter, int startNumber ) : base( pm, g, currentComparer, ascending, filter, startNumber, alliance.m_Members, alliance.Name )
			{
				m_Alliance = alliance;
			}

			public override Gump GetResentGump( PlayerMobile pm, Guild g, IComparer<Guild> comparer, bool ascending, string filter, int startNumber )
			{
				return new AllianceRosterGump( pm, g, m_Alliance, comparer, ascending, filter, startNumber );
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				if( info.ButtonID != 8 ) //So that they can't get to the AdvancedSearch button
					base.OnResponse( sender, info );
			}
		}
	}
	#endregion

	#region Wars
	public enum WarStatus
	{
		InProgress = -1,
		Win,
		Lose,
		Draw,
		Pending
	}

	public class WarDeclaration
	{
		private int m_Kills;
		private int m_MaxKills;

		private TimeSpan m_WarLength;
		private DateTime m_WarBeginning;

		private readonly Guild m_Guild;
		private readonly Guild m_Opponent;

		private bool m_WarRequester;

		public int Kills
		{
			get{ return m_Kills; } 
			set{ m_Kills = value; }
		}
		public int MaxKills
		{
			get{ return m_MaxKills; }
			set{ m_MaxKills = value; }
		}
		public TimeSpan WarLength
		{
			get{ return m_WarLength; }
			set{ m_WarLength = value; }
		}
		public Guild Opponent
		{
			get{ return m_Opponent; }
		}
		public Guild Guild
		{
			get{ return m_Guild; }
		}
		public DateTime WarBeginning
		{
			get{ return m_WarBeginning; } 
			set{ m_WarBeginning = value; }
		}
		public bool WarRequester
		{
			get{ return m_WarRequester; }
			set{ m_WarRequester = value; }
		}

		public WarDeclaration( Guild g, Guild opponent, int maxKills, TimeSpan warLength, bool warRequester )
		{
			m_Guild = g;
			m_MaxKills = maxKills;
			m_Opponent = opponent;
			m_WarLength = warLength;
			m_WarRequester = warRequester;
		}

		public WarDeclaration( GenericReader reader )
		{
			var version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_Kills = reader.ReadInt();
					m_MaxKills = reader.ReadInt();

					m_WarLength = reader.ReadTimeSpan();
					m_WarBeginning = reader.ReadDateTime();

					m_Guild = reader.ReadGuild() as Guild;
					m_Opponent = reader.ReadGuild() as Guild;

					m_WarRequester = reader.ReadBool();

					break;
				}
			}
		}

		public void Serialize( GenericWriter writer )
		{
			writer.Write( 0 );	//version

			writer.Write( m_Kills );
			writer.Write( m_MaxKills );

			writer.Write( m_WarLength );
			writer.Write( m_WarBeginning );

			writer.Write( m_Guild );
			writer.Write( m_Opponent );

			writer.Write( m_WarRequester );
		}

		public WarStatus Status
		{
			get
			{
				if( m_Opponent == null || m_Opponent.Disbanded )
					return WarStatus.Win;

				if( m_Guild == null || m_Guild.Disbanded )
					return WarStatus.Lose;

				var w = m_Opponent.FindActiveWar( m_Guild );

				if( m_Opponent.FindPendingWar( m_Guild ) != null && m_Guild.FindPendingWar( m_Opponent ) != null )
					return WarStatus.Pending;


				if( w == null )
					return WarStatus.Win;

				if( m_WarLength != TimeSpan.Zero && (m_WarBeginning + m_WarLength) < DateTime.UtcNow )
				{
				    if( m_Kills > w.m_Kills )
						return WarStatus.Win;
				    if( m_Kills < w.m_Kills )
				        return WarStatus.Lose;
				    return WarStatus.Draw;
				}
			    if( m_MaxKills > 0 )
			    {
			        if( m_Kills >= m_MaxKills )
			            return WarStatus.Win;
			        if( w.m_Kills >= w.MaxKills )
			            return WarStatus.Lose;
			    }

			    return WarStatus.InProgress;
			}
		}
	}

	public class WarTimer : Timer
	{
		private static readonly TimeSpan InternalDelay = TimeSpan.FromMinutes( 1.0 );

		public static void Initialize()
		{
			if( Guild.NewGuildSystem )
				new WarTimer().Start();
		}

		public WarTimer() : base( InternalDelay, InternalDelay )
		{
			Priority = TimerPriority.FiveSeconds;
		}

		protected override void OnTick() 
		{
			foreach( Guild g in BaseGuild.List.Values )
				g.CheckExpiredWars();
		}
	}

	#endregion

	public class Guild : BaseGuild
	{
		public static void Configure()
		{
			EventSink.CreateGuild += EventSink_CreateGuild;
			EventSink.GuildGumpRequest += EventSink_GuildGumpRequest;

			CommandSystem.Register( "GuildProps", AccessLevel.Counselor, GuildProps_OnCommand );
		}

		#region GuildProps
		[Usage( "GuildProps" )]
		[Description( "Opens a menu where you can view and edit guild properties of a targeted player or guild stone.  If the new Guild system is active, also brings up the guild gump." )]
		private static void GuildProps_OnCommand( CommandEventArgs e )
		{
			var arg = e.ArgString.Trim();
			var from = e.Mobile;

			if( arg.Length == 0 )
			{
				e.Mobile.Target = new GuildPropsTarget();
			}
            else
            {
                Guild g = null;

                int id;

                if (int.TryParse(arg, out id))
                    g = Guild.Find(id) as Guild;

                if (g == null)
                {
                    g = Guild.FindByAbbrev(arg) as Guild;

                    if (g == null)
                        g = Guild.FindByName(arg) as Guild;
                }

                if (g != null)
                {
                    from.SendGump(new PropertiesGump(from, g));

                    if (NewGuildSystem && from.AccessLevel >= AccessLevel.GameMaster && from is PlayerMobile)
                        from.SendGump(new GuildInfoGump((PlayerMobile)from, g));
                }
            }

		}

		private class GuildPropsTarget : Target
		{
			public GuildPropsTarget() : base( -1, true, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if( !BaseCommand.IsAccessible( from, o ) )
				{
					from.SendMessage( "That is not accessible." );
					return;
				}

				Guild g = null;

				if( o is Guildstone )
				{
					var stone = o as Guildstone;
					if( stone.Guild == null || stone.Guild.Disbanded )
					{
						from.SendMessage( "The guild associated with that Guildstone no longer exists" );
						return;
					}
				    g = stone.Guild;
				}
				else if( o is Mobile )
				{
					g = ((Mobile)o).Guild as Guild;
				}

				if( g != null )
				{
					from.SendGump( new PropertiesGump( from, g ) );

					if( NewGuildSystem && from.AccessLevel >= AccessLevel.GameMaster && from is PlayerMobile )
						from.SendGump( new GuildInfoGump( (PlayerMobile)from, g ) );
				}
				else
				{
					from.SendMessage( "That is not in a guild!" );
				}
			}
		}
		#endregion

		#region EventSinks
		public static void EventSink_GuildGumpRequest( GuildGumpRequestArgs args )
		{
			var pm = args.Mobile as PlayerMobile;
			if( !NewGuildSystem || pm == null )
				return;
			
			if( pm.Guild == null )
				pm.SendGump( new CreateGuildGump( pm ) );
			else
				pm.SendGump( new GuildInfoGump( pm, pm.Guild as Guild ) );
		}

		public static BaseGuild EventSink_CreateGuild( CreateGuildEventArgs args )
		{
			return new Guild( args.Id );
		}
		#endregion

		public static bool NewGuildSystem{ get{ return false; } }

		public static readonly int RegistrationFee = 25000;
		public static readonly int AbbrevLimit = 4;
		public static readonly int NameLimit = 40;
		public static readonly int MajorityPercentage = 66;
		public static readonly TimeSpan InactiveTime = TimeSpan.FromDays( 30 );


		#region New Alliances

		public AllianceInfo Alliance
		{
			get
			{
			    if( m_AllianceInfo != null )
					return m_AllianceInfo;
			    if ( m_AllianceLeader != null )
			        return m_AllianceLeader.m_AllianceInfo;
			    return null;
			}
		    set
			{
				var current = Alliance;

				if( value == current )
					return;

				if( current != null )
				{
					current.RemoveGuild( this );
				}

				if( value != null )
				{
					
					if( value.Leader == this )
						m_AllianceInfo = value;
					else
						m_AllianceLeader = value.Leader;

					value.AddPendingGuild( this );
				}
				else
				{
					m_AllianceInfo = null;
					m_AllianceLeader = null;
				}
			}
		}

		[CommandProperty( AccessLevel.Counselor )]
		public string AllianceName
		{
			get
			{
				var al = Alliance;
				if( al != null )
					return al.Name;

				return null;
			}
		}

		[CommandProperty( AccessLevel.Counselor )]
		public Guild AllianceLeader
		{
			get
			{
				var al = Alliance;

				if( al != null )
					return al.Leader;

				return null;
			}
		}

		[CommandProperty( AccessLevel.Counselor )]
		public bool IsAllianceMember
		{
			get
			{
				var al = Alliance;

				if( al != null )
					return al.IsMember( this );

				return false;
			}
		}

		[CommandProperty( AccessLevel.Counselor )]
		public bool IsAlliancePendingMember
		{
			get
			{
				var al = Alliance;

				if( al != null )
					return al.IsPendingMember( this );

				return false;
			}
		}

        public static Guild GetAllianceLeader(Guild g)
        {
            AllianceInfo alliance = g.Alliance;

            if (alliance != null && alliance.Leader != null && alliance.IsMember(g))
                return alliance.Leader;

            return g;
        }

		#endregion

		#region New Wars

		public List<WarDeclaration> PendingWars
		{
			get{ return m_PendingWars; }
		}
		public List<WarDeclaration> AcceptedWars
		{
			get{ return m_AcceptedWars; } 
		}


		public WarDeclaration FindPendingWar( Guild g )
		{
			for( var i = 0; i < PendingWars.Count; i++ )
			{
				var w = PendingWars[i];

				if( w.Opponent == g )
					return w;
			}

			return null;
		}

		public WarDeclaration FindActiveWar( Guild g )
		{
			for( var i = 0; i < AcceptedWars.Count; i++ )
			{
				var w = AcceptedWars[i];

				if( w.Opponent == g )
					return w;
			}

			return null;
		}


		
		public void CheckExpiredWars()
		{
			for( var i = 0; i < AcceptedWars.Count; i++ )
			{
				var w = AcceptedWars[i];
				var g = w.Opponent;

				var status = w.Status;

				if( status != WarStatus.InProgress )
				{
					var myAlliance = Alliance;
					var inAlliance = ( myAlliance != null && myAlliance.IsMember( this ) );
					
					var otherAlliance = ((g != null) ? g.Alliance : null);
					var otherInAlliance = ( otherAlliance != null && otherAlliance.IsMember( this ) );

					if( inAlliance )
					{
						myAlliance.AllianceMessage( 1070739 + (int)status, (g == null) ? "a deleted opponent" : (otherInAlliance ? otherAlliance.Name : g.Name) );
						myAlliance.InvalidateMemberProperties();
					}
					else
					{
						GuildMessage( 1070739 + (int)status, (g == null) ? "a deleted opponent" : (otherInAlliance ? otherAlliance.Name : g.Name) );
						InvalidateMemberProperties();
					}

					AcceptedWars.Remove( w );

					if( g != null )
					{
						if( status != WarStatus.Draw )
							status = (WarStatus)((int)status + 1 % 2);

						if( otherInAlliance )
						{
							otherAlliance.AllianceMessage( 1070739 + (int)status, ( inAlliance ? Alliance.Name : Name ) );
							otherAlliance.InvalidateMemberProperties();
						}
						else
						{
							g.GuildMessage( 1070739 + (int)status, (inAlliance ? Alliance.Name : Name) );
							g.InvalidateMemberProperties();
						}

						g.AcceptedWars.Remove( g.FindActiveWar( this ) );
					}
				}
			}

			for( var i = 0; i < PendingWars.Count; i++ )
			{
				var w = PendingWars[i];
				var g = w.Opponent;

				if( w.Status != WarStatus.Pending )
				{
					//All sanity in here
					PendingWars.Remove( w );

					if( g != null )
					{
						g.PendingWars.Remove( g.FindPendingWar( this ) );
					}
				}
			}
		}

		public static void HandleDeath( Mobile victim )
		{
			HandleDeath( victim, null );
		}

		public static void HandleDeath( Mobile victim, Mobile killer )
		{
			if( !NewGuildSystem )
				return;

			if ( killer == null )
				killer = victim.FindMostRecentDamager( false );

			if( killer == null || victim.Guild == null || killer.Guild == null )
				return;

            Guild victimGuild = GetAllianceLeader(victim.Guild as Guild);
            Guild killerGuild = GetAllianceLeader(killer.Guild as Guild);

            WarDeclaration war = killerGuild.FindActiveWar(victimGuild);

            if (war == null)
                return;

            war.Kills++;

            if (war.Opponent == victimGuild)
                killerGuild.CheckExpiredWars();
            else
                victimGuild.CheckExpiredWars();
		}
		#endregion

		#region Var declarations
		private Mobile m_Leader;

		private string m_Name;
		private string m_Abbreviation;

		private List<Guild> m_Allies;
		private List<Guild> m_Enemies;

		private List<Mobile> m_Members;
		private ArrayList m_Listeners; // staff listening
        private ArrayList m_AllianceListeners; // staff listening
		
		private Item m_Guildstone;
		private Item m_Teleporter;

		private string m_Charter;
		private string m_Website;

		private DateTime m_LastFealty;

		private GuildType m_Type;
		private DateTime m_TypeLastChange;

		private List<Guild> m_AllyDeclarations, m_AllyInvitations;

		private List<Guild> m_WarDeclarations, m_WarInvitations;
		private List<Mobile> m_Candidates, m_Accepted;

		private List<WarDeclaration> m_PendingWars, m_AcceptedWars;

		private AllianceInfo m_AllianceInfo;
		private Guild m_AllianceLeader;
		#endregion

		public Guild( Mobile leader, string name, string abbreviation )
		{
			#region Ctor mumbo-jumbo
			m_Leader = leader;

			m_Members = new List<Mobile>();
			m_Allies = new List<Guild>();
			m_Enemies = new List<Guild>();
			m_WarDeclarations = new List<Guild>();
			m_WarInvitations = new List<Guild>();
			m_AllyDeclarations = new List<Guild>();
			m_AllyInvitations = new List<Guild>();
			m_Candidates = new List<Mobile>();
			m_Accepted = new List<Mobile>();
			m_Listeners = new ArrayList();
            m_AllianceListeners = new ArrayList();
			m_LastFealty = DateTime.UtcNow;

			m_Name = name;
			m_Abbreviation = abbreviation;

			m_TypeLastChange = DateTime.MinValue;

			AddMember( m_Leader );

			if( m_Leader is PlayerMobile )
				((PlayerMobile)m_Leader).GuildRank = RankDefinition.Leader;

			m_AcceptedWars = new List<WarDeclaration>();
			m_PendingWars = new List<WarDeclaration>();
			#endregion
		}

		public Guild( int id ) : base( id )//serialization ctor
		{
		}

		public void InvalidateMemberProperties()
		{
			InvalidateMemberProperties( false );
		}

		public void InvalidateMemberProperties( bool onlyOPL )
		{
			if ( m_Members != null )
			{
				for ( var i = 0; i < m_Members.Count; i++ )
				{
					var m = m_Members[i];
					m.InvalidateProperties();

					if ( !onlyOPL )
						m.Delta( MobileDelta.Noto );
				}
			}
		}

		public void InvalidateMemberNotoriety()
		{
			if ( m_Members != null )
			{
				for (var i=0;i<m_Members.Count;i++)
					m_Members[i].Delta( MobileDelta.Noto );
			}
		}

        public void InvalidateWarNotoriety()
        {
            Guild g = GetAllianceLeader(this);

            if (g.Alliance != null)
                g.Alliance.InvalidateMemberNotoriety();
            else
                g.InvalidateMemberNotoriety();

            if (g.AcceptedWars == null)
                return;

            foreach (WarDeclaration warDec in g.AcceptedWars)
            {
                Guild opponent = warDec.Opponent;

                if (opponent.Alliance != null)
                    opponent.Alliance.InvalidateMemberNotoriety();
                else
                    opponent.InvalidateMemberNotoriety();
            }
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Leader
		{
			get
			{
				if ( m_Leader == null || m_Leader.Deleted || m_Leader.Guild != this )
					CalculateGuildmaster();

				return m_Leader;
			}
			set
			{
				if( value != null )
					AddMember( value ); //Also removes from old guild.

				if( m_Leader is PlayerMobile && m_Leader.Guild == this )
					((PlayerMobile)m_Leader).GuildRank = RankDefinition.Member;

				m_Leader = value;

				if( m_Leader is PlayerMobile )
					((PlayerMobile)m_Leader).GuildRank = RankDefinition.Leader;
			}
		}


		public override bool Disbanded
		{
			get
			{
				return ( m_Leader == null || m_Leader.Deleted );
			}
		}

		public override void OnDelete( Mobile mob )
		{
			RemoveMember( mob );
		}


		public void Disband()
		{
			m_Leader = null;

			List.Remove( Id );

			foreach ( var m in m_Members )
			{
				m.SendLocalizedMessage( 502131 ); // Your guild has disbanded.

				if( m is PlayerMobile )
					((PlayerMobile)m).GuildRank = RankDefinition.Lowest;

				m.Guild = null;
			}

			m_Members.Clear();

			for ( var i = m_Allies.Count - 1; i >= 0; --i )
				if ( i < m_Allies.Count )
					RemoveAlly( m_Allies[i] );

			for ( var i = m_Enemies.Count - 1; i >= 0; --i )
				if ( i < m_Enemies.Count )
					RemoveEnemy( m_Enemies[i] );

			if ( !NewGuildSystem && m_Guildstone != null )
				m_Guildstone.Delete();

			m_Guildstone = null;

			CheckExpiredWars();

			Alliance = null;
		}

		#region Is<something>(...)
		public bool IsMember( Mobile m )
		{
			return m_Members.Contains( m );
		}

		public bool IsAlly( Guild g )
		{
			if( NewGuildSystem )
			{
				return (Alliance != null && Alliance.IsMember( this ) && Alliance.IsMember( g ));
			}

            //Order guilds and chaos guilds can loot eachother
            if (g.m_Type != GuildType.Regular && m_Type != GuildType.Regular && g.m_Type == m_Type)
                return true;

			return m_Allies.Contains( g );
		}

		public bool IsEnemy( Guild g )
		{
			if( NewGuildSystem )
				return IsWar( g );

			if( m_Type != GuildType.Regular && g.m_Type != GuildType.Regular && m_Type != g.m_Type )
				return true;

			return m_Enemies.Contains( g );
		}

		public bool IsWar( Guild g )
		{
			if( g == null )
				return false;

			if( NewGuildSystem )
			{
                Guild guild = GetAllianceLeader(this);
                Guild otherGuild = GetAllianceLeader(g);

                if (guild.FindActiveWar(otherGuild) != null)
					return true;

				return false;
			}


			return m_Enemies.Contains( g );
		}
		#endregion

		#region Serialization
		public override void Serialize( GenericWriter writer )
		{
			if ( LastFealty+TimeSpan.FromDays( 1.0 ) < DateTime.UtcNow )
				CalculateGuildmaster();

			CheckExpiredWars();

			if( Alliance != null )
				Alliance.CheckLeader();

			writer.Write( 7 );//version

            //7
            if (m_AllianceListeners != null)
                writer.WriteMobileList(m_AllianceListeners);
            else
                writer.WriteMobileList(m_AllianceListeners = new ArrayList());

            //6
            if (m_Listeners != null)
                writer.WriteMobileList(m_Listeners);
            else
                writer.WriteMobileList(m_Listeners = new ArrayList());

			#region War Serialization
			writer.Write( m_PendingWars.Count );

			for( var i = 0; i < m_PendingWars.Count; i++ )
			{
				m_PendingWars[i].Serialize( writer );
			}

			writer.Write( m_AcceptedWars.Count );

			for( var i = 0; i < m_AcceptedWars.Count; i++ )
			{
				m_AcceptedWars[i].Serialize( writer );
			}
			#endregion

			#region Alliances

			var isAllianceLeader = (m_AllianceLeader == null && m_AllianceInfo != null );
			writer.Write( isAllianceLeader );

			if( isAllianceLeader )
				m_AllianceInfo.Serialize( writer );
			else
				writer.Write( m_AllianceLeader );

			#endregion

			//

			writer.WriteGuildList( m_AllyDeclarations, true );
			writer.WriteGuildList( m_AllyInvitations, true );

			writer.Write( m_TypeLastChange );

			writer.Write( (int)m_Type );

			writer.Write( m_LastFealty );

			writer.Write( m_Leader );
			writer.Write( m_Name );
			writer.Write( m_Abbreviation );

			writer.WriteGuildList( m_Allies, true );
			writer.WriteGuildList( m_Enemies, true );
			writer.WriteGuildList( m_WarDeclarations, true );
			writer.WriteGuildList( m_WarInvitations, true );

			writer.Write( m_Members, true );
			writer.Write( m_Candidates, true );
			writer.Write( m_Accepted, true );

			writer.Write( m_Guildstone );
			writer.Write( m_Teleporter );

			writer.Write( m_Charter );
			writer.Write( m_Website );
		}

		public override void Deserialize( GenericReader reader )
		{
			var version = reader.ReadInt();

			switch ( version )
			{
                case 7:
                    {
                        if (m_AllianceListeners != null)
                            m_AllianceListeners = reader.ReadMobileList();
                        else
                        {
                            m_AllianceListeners = new ArrayList();
                            m_AllianceListeners = reader.ReadMobileList();
                        }

                        goto case 6;
                    }
                case 6:
		        {
                    if (m_Listeners != null)
                        m_Listeners = reader.ReadMobileList();
                    else
                    {
                        m_Listeners = new ArrayList();
                        m_Listeners = reader.ReadMobileList();
                    }

		            goto case 5;
		        }
				case 5:
				{
					var count = reader.ReadInt();

					m_PendingWars = new List<WarDeclaration>();
					for( var i = 0; i < count; i++ )
					{
						m_PendingWars.Add( new WarDeclaration( reader ) );
					}

					count = reader.ReadInt();
					m_AcceptedWars = new List<WarDeclaration>();
					for( var i = 0; i < count; i++ )
					{
						m_AcceptedWars.Add( new WarDeclaration( reader ) );
					}

					var isAllianceLeader = reader.ReadBool();

					if( isAllianceLeader )
						m_AllianceInfo = new AllianceInfo( reader );
					else
						m_AllianceLeader = reader.ReadGuild() as Guild;


					goto case 4;
				}
				case 4:
				{
					m_AllyDeclarations = reader.ReadStrongGuildList<Guild>();
					m_AllyInvitations = reader.ReadStrongGuildList<Guild>();

					goto case 3;
				}
				case 3:
				{
					m_TypeLastChange = reader.ReadDateTime();

					goto case 2;
				}
				case 2:
				{
					m_Type = (GuildType)reader.ReadInt();

					goto case 1;
				}
				case 1:
				{
					m_LastFealty = reader.ReadDateTime();

					goto case 0;
				}
				case 0:
				{
					m_Leader = reader.ReadMobile();

					if( m_Leader is PlayerMobile )
						((PlayerMobile)m_Leader).GuildRank = RankDefinition.Leader;

					m_Name = reader.ReadString();
					m_Abbreviation = reader.ReadString();

					m_Allies = reader.ReadStrongGuildList<Guild>();
					m_Enemies = reader.ReadStrongGuildList<Guild>();
					m_WarDeclarations = reader.ReadStrongGuildList<Guild>();
					m_WarInvitations = reader.ReadStrongGuildList<Guild>();

					m_Members = reader.ReadStrongMobileList();
					m_Candidates = reader.ReadStrongMobileList();
					m_Accepted = reader.ReadStrongMobileList(); 

					m_Guildstone = reader.ReadItem();
					m_Teleporter = reader.ReadItem();

					m_Charter = reader.ReadString();
					m_Website = reader.ReadString();

					break;
				}
			}

			if ( m_AllyDeclarations == null )
				m_AllyDeclarations = new List<Guild>();

			if ( m_AllyInvitations == null )
				m_AllyInvitations = new List<Guild>();


			if ( m_AcceptedWars == null )
				m_AcceptedWars = new List<WarDeclaration>();

			if ( m_PendingWars == null )
				m_PendingWars = new List<WarDeclaration>();


			/*
			if ( ( !NewGuildSystem && m_Guildstone == null )|| m_Members.Count == 0 )
				Disband();
			*/

			Timer.DelayCall( TimeSpan.Zero, new TimerCallback( VerifyGuild_Callback ) );
		}

		private void VerifyGuild_Callback()
		{
			if( (!NewGuildSystem && m_Guildstone == null) || m_Members.Count == 0 )
				Disband();

			CheckExpiredWars();

			var alliance = Alliance;

			if( alliance != null )
				alliance.CheckLeader();

			alliance = Alliance;	//CheckLeader could possibly change the value of this.Alliance

			if( alliance != null && !alliance.IsMember( this ) && !alliance.IsPendingMember( this ) )	//This block is there to fix a bug in the code in an older version.  
				Alliance = null;	//Will call Alliance.RemoveGuild which will set it null & perform all the pertient checks as far as alliacne disbanding

		}

		#endregion

		#region Add/Remove Member/Old Ally/Old Enemy
		public void AddMember( Mobile m )
		{
			if ( !m_Members.Contains( m ) )
			{
				if ( m.Guild != null && m.Guild != this )
					((Guild)m.Guild).RemoveMember( m );

				m_Members.Add( m );
				m.Guild = this;

				if( !NewGuildSystem )
					m.GuildFealty = m_Leader;
				else
					m.GuildFealty = null;

				if( m is PlayerMobile )
					((PlayerMobile)m).GuildRank = RankDefinition.Lowest;

                Guild guild = m.Guild as Guild;

                if (guild != null)
                    guild.InvalidateWarNotoriety();
			}
		}

		public void RemoveMember( Mobile m )
		{
			RemoveMember( m, 1018028 ); // You have been dismissed from your guild.
		}

		public void RemoveMember( Mobile m, int message )
		{
			if ( m_Members.Contains( m ) )
			{
                //Last remaining member of the guild 
                if (m_Members.Count == 1 && m.Backpack != null)
                    m.AddToBackpack(new GuildDeed());

				m_Members.Remove( m );

                Guild guild = m.Guild as Guild;

				m.Guild = null;
				
				if( m is PlayerMobile )
					((PlayerMobile)m).GuildRank = RankDefinition.Lowest;

				if( message > 0 )
					m.SendLocalizedMessage( message );

				if ( m == m_Leader )
				{
					CalculateGuildmaster();

					if ( m_Leader == null )
						Disband();
				}

				if ( m_Members.Count == 0 )
					Disband();

                if (guild != null)
                    guild.InvalidateWarNotoriety();

                m.Delta(MobileDelta.Noto);
			}
		}

		public void AddAlly( Guild g )
		{
			if ( !m_Allies.Contains( g ) )
			{
				m_Allies.Add( g );

				g.AddAlly( this );
			}
		}

		public void RemoveAlly( Guild g )
		{
			if ( m_Allies.Contains( g ) )
			{
				m_Allies.Remove( g );

				g.RemoveAlly( this );
			}
		}

		public void AddEnemy( Guild g )
		{
			if ( !m_Enemies.Contains( g ) )
			{
				m_Enemies.Add( g );

				g.AddEnemy( this );
			}
		}

		public void RemoveEnemy( Guild g )
		{
			if ( m_Enemies != null && m_Enemies.Contains( g ) )
			{
				m_Enemies.Remove( g );

				g.RemoveEnemy( this );
			}
		}

		#endregion

		#region Guild[Text]Message(...)
		public void GuildMessage( int num, bool append, string format, params object[] args )
		{
			GuildMessage( num, append, String.Format( format, args) );
		}
		public void GuildMessage( int number )
		{
			for ( var i = 0; i < m_Members.Count; ++i )
				m_Members[i].SendLocalizedMessage( number );
		}
		public void GuildMessage( int number, string args )
		{
			GuildMessage( number, args, 0x3B2 );
		}
		public void GuildMessage( int number, string args, int hue )
		{
			for ( var i = 0; i < m_Members.Count; ++i )
				m_Members[i].SendLocalizedMessage( number, args, hue );
		}
		public void GuildMessage( int number, bool append, string affix )
		{
			GuildMessage( number, append, affix, "", 0x3B2 );
		}
		public void GuildMessage( int number, bool append, string affix, string args )
		{
			GuildMessage( number, append, affix, args, 0x3B2 );
		}
		public void GuildMessage( int number, bool append, string affix, string args, int hue )
		{
			for ( var i = 0; i < m_Members.Count; ++i )
				m_Members[i].SendLocalizedMessage( number, append, affix, args, hue );
		}

		public void GuildTextMessage( string text )
		{
			GuildTextMessage( 0x3B2, text );
		}
		public void GuildTextMessage( string format, params object[] args )
		{
			GuildTextMessage( 0x3B2, String.Format( format, args ) );
		}
		public void GuildTextMessage( int hue, string text )
		{
			for( var i = 0; i < m_Members.Count; ++i )
				m_Members[i].SendMessage( hue, text );
		}
		public void GuildTextMessage( int hue, string format, params object[] args )
		{
			GuildTextMessage( hue, String.Format( format, args ) );
		}

		public void GuildChat( Mobile from, int hue, string text )
		{
			Packet p = null;
			for( var i = 0; i < m_Members.Count; i++ )
			{
				var m = m_Members[i];

				var state = m.NetState;

				if( state != null )
				{
					if( p == null )
						p = Packet.Acquire( new UnicodeMessage( from.Serial, from.Body, MessageType.Guild, hue, 3, from.Language, from.Name, text ) );

					state.Send( p );
				}
			}

			Packet.Release( p );
		}

		public void GuildChat( Mobile from, string text )
		{
			var pm = from as PlayerMobile;

			GuildChat( from, (pm == null) ? 0x3B2 : pm.GuildMessageHue, text );
		}
		#endregion

		#region Voting
		public bool CanVote( Mobile m )
		{
			if( NewGuildSystem )
			{
				var pm = m as PlayerMobile;
				if( pm == null || !pm.GuildRank.GetFlag( RankFlags.CanVote ) )
					return false;
			}

			return ( m != null && !m.Deleted && m.Guild == this );
		}
		public bool CanBeVotedFor( Mobile m )
		{
			if( NewGuildSystem )
			{
				var pm = m as PlayerMobile;
				if( pm == null || pm.LastOnline + InactiveTime < DateTime.UtcNow )
					return false;
			}

			return ( m != null && !m.Deleted && m.Guild == this );
		}
		
		public void AddListener(Mobile m){
			m_Listeners.Add(m);
		}
		
		public void RemoveListener(Mobile m){
			m_Listeners.Remove(m);
		}

        public void AddAllianceListener(Mobile m)
        {
            m_AllianceListeners.Add(m);
        }

        public void RemoveAllianceListener(Mobile m)
        {
            m_AllianceListeners.Remove(m);
        }

		public void CalculateGuildmaster()
		{
			var votes = new Dictionary<Mobile, int>();

			var votingMembers = 0;

			for ( var i = 0; m_Members != null && i < m_Members.Count; ++i )
			{
				var memb = m_Members[i];

				if ( !CanVote( memb ) )
					continue;

				var m = memb.GuildFealty;

				if( !CanBeVotedFor( m ) )
				{
					if ( m_Leader != null && !m_Leader.Deleted && m_Leader.Guild == this )
						m = m_Leader;
					else 
						m = memb;
				}

				if ( m == null )
					continue;

				int v;

				if( !votes.TryGetValue( m, out v ) )
					votes[m] = 1;
				else
					votes[m] = v + 1;
			
				votingMembers++;
			}

			Mobile winner = null;
			var highVotes = 0;

			foreach ( var kvp in votes )
			{
				var m = kvp.Key;
				var val = kvp.Value;

				if ( winner == null || val > highVotes )
				{
					winner = m;
					highVotes = val;
				}
			}

			if( NewGuildSystem && (highVotes * 100) / Math.Max( votingMembers, 1 ) < MajorityPercentage && m_Leader != null && winner != m_Leader && !m_Leader.Deleted && m_Leader.Guild == this )
				winner = m_Leader;

			if ( m_Leader != winner && winner != null )
				GuildMessage( 1018015, true, winner.Name ); // Guild Message: Guildmaster changed to:

			Leader = winner;
			m_LastFealty = DateTime.UtcNow;
		}

		#endregion

		#region Getters & Setters
		[CommandProperty( AccessLevel.GameMaster )]
		public Item Guildstone
		{
			get
			{
				return m_Guildstone;
			}
			set
			{
				m_Guildstone = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Item Teleporter
		{
			get
			{
				return m_Teleporter;
			}
			set
			{
				m_Teleporter = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public override string Name
		{
			get
			{
				return m_Name;
			}
			set
			{
				m_Name = value;

				InvalidateMemberProperties( true );

				if ( m_Guildstone != null )
					m_Guildstone.InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string Website
		{
			get
			{
				return m_Website;
			}
			set
			{
				m_Website = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public override string Abbreviation
		{
			get
			{
				return m_Abbreviation;
			}
			set
			{
				m_Abbreviation = value;

				InvalidateMemberProperties( true );

				if( m_Guildstone != null )
					m_Guildstone.InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string Charter
		{
			get
			{
				return m_Charter;
			}
			set
			{
				m_Charter = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public override GuildType Type
		{
			get
			{
				return m_Type;
			}
			set
			{
				if ( m_Type != value )
				{
					m_Type = value;
					m_TypeLastChange = DateTime.UtcNow;

					InvalidateMemberProperties();
				}
			}
		}


		
		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime LastFealty
		{
			get
			{
				return m_LastFealty;
			}
			set
			{
				m_LastFealty = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime TypeLastChange
		{
			get
			{
				return m_TypeLastChange;
			}
		}

		
		public List<Guild> Allies
		{
			get
			{
				return m_Allies;
			}
		}

		public List<Guild> Enemies
		{
			get
			{
				return m_Enemies;
			}
		}

		public List<Guild> AllyDeclarations
		{
			get
			{
				return m_AllyDeclarations;
			}
		}

		public List<Guild> AllyInvitations
		{
			get
			{
				return m_AllyInvitations;
			}
		}

		public List<Guild> WarDeclarations
		{
			get
			{
				return m_WarDeclarations;
			}
		}

		public List<Guild> WarInvitations
		{
			get
			{
				return m_WarInvitations;
			}
		}

		public List<Mobile> Candidates
		{
			get
			{
				return m_Candidates;
			}
		}

		public List<Mobile> Accepted
		{
			get
			{
				return m_Accepted;
			}
		}

		public List<Mobile> Members
		{
			get
			{
				return m_Members;
			}
		}
		
		public ArrayList Listeners
		{
			get
			{
				return m_Listeners;
			}
            set { m_Listeners = value; }
		}

        public ArrayList AllianceListeners
        {
            get
            {
                return m_AllianceListeners;
            }
            set { m_AllianceListeners = value; }
        }

		#endregion

	}
}
