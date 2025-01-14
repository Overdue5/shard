using System.Collections;
using Server;

namespace Arya.Chess
{
	public class ChessRegion : Region
	{
		/// <summary>
		/// Specifies whether spectators should be allowed on the BChessboard or not
		/// </summary>
		private bool m_AllowSpectators = false;
		/// <summary>
		/// The game that's being held on the BChessboard
		/// </summary>
		private readonly ChessGame m_Game;
		/// <summary>
		/// The bounds of the region
		/// </summary>
		//private Rectangle2D m_Bounds;
		/// <summary>
		/// The bounds of the BChessboard
		/// </summary>
		private Rectangle3D m_BoardBounds;
		/// <summary>
		/// The height of the BChessboard
		/// </summary>
		private readonly int m_Height;

	    private readonly bool m_SafeZone;

		public bool AllowSpectators
		{
			get => m_AllowSpectators;
            set
			{
				if ( value != m_AllowSpectators )
				{
					m_AllowSpectators = value;

					ForceExpel();
				}
			}
		}

		public ChessRegion( Map map, ChessGame game, bool allowSpectators, bool safeZone, Rectangle3D bounds, int height ) : base( "Chessboard", map, 100, bounds )
		{
			m_Game = game;
			m_AllowSpectators = allowSpectators;
		    m_SafeZone = safeZone;
		    m_BoardBounds = bounds;

		    // Make the region larger so that people can't cast invisibility outside
//			m_Bounds = new Rectangle2D( bounds.X - 12, bounds.Y - 12, bounds.Width + 24, bounds.Height + 24 );
//			m_BoardBounds = bounds;

		    m_Height = height;

            if (!m_AllowSpectators)
                ForceExpel();

//			Coords = new ArrayList();
//			Coords.Add( m_Bounds );
		}

        public override bool AllowHarmful(Mobile from, Mobile target)
        {
            if (m_SafeZone)
            {
                from.SendMessage("You cannot attack here.");
                return false;
            }

            return base.AllowHarmful(from, target);
        }

        public override bool OnDamage(Mobile m, ref int Damage)
        {
            if (m_SafeZone)
            {
                m.SendMessage("You cannot be damaged here.");
                return false;
            }

            return base.OnDamage(m, ref Damage);
        }

        public override bool OnMoveInto(Mobile m, Direction d, Point3D newLocation, Point3D oldLocation)
        {
            if (m_Game == null || m is ChessMobile || m_AllowSpectators || m_Game.IsPlayer(m) || m.AccessLevel > AccessLevel.Player)
                return true;

            m.SendMessage(0x40, "Spectators aren't allowed on the chessboard");
            return false;
        }

		public override bool OnBeginSpellCast(Mobile m, ISpell s)
		{
			if ( s is Server.Spells.Sixth.InvisibilitySpell )
			{
				m.SendMessage( 0x40, "You can't cast that spell when you're close to a chessboard" );
				return false;
			}

            if (m_SafeZone)
            {
                m.SendMessage( 0x40, "You can't cast spells near this chessboard");
                return false;
            }

			return base.OnBeginSpellCast (m, s);
		}


		// Don't announce
		public override void OnEnter(Mobile m)
		{
		}

		public override void OnExit(Mobile m)
		{
		}

		public override bool AllowSpawn()
		{
			return false;
		}

		public override void OnSpeech(SpeechEventArgs args)
		{
			if ( m_Game != null && m_Game.IsPlayer( args.Mobile ) && m_Game.AllowTarget )
			{
				if ( args.Speech.ToLower().IndexOf( ChessConfig.ResetKeyword.ToLower() ) > -1 )
					m_Game.SendAllGumps( null, null );				
			}

			base.OnSpeech( args );
		}

		private void ForceExpel()
		{
			if ( m_Game != null && ! m_AllowSpectators )
			{
                Rectangle2D rect = new Rectangle2D( m_BoardBounds.Start, m_BoardBounds.End );

				IPooledEnumerable en = Map.GetMobilesInBounds(rect);
				ArrayList expel = new ArrayList();

				try
				{
					foreach( Mobile m in en )
					{
						if ( m.Player && !m_Game.IsPlayer( m ) && m.Location.Z > m_BoardBounds.Start.Z && m.Location.Z < m_BoardBounds.End.Z )
						{
							expel.Add( m );
						}
					}
				}
				finally
				{
					en.Free();
				}

				foreach( Mobile m in expel )
				{
					m.SendMessage( 0x40, "Spectators aren't allowed on the chessboard" );
					m.Location = new Point3D( m_BoardBounds.Start.X - 1, m_BoardBounds.Start.Y - 1, m_Height );
				}
			}
		}

		// public override void Register()
		// {
			// base.Register();

			// ForceExpel();
		// }
	}
}
