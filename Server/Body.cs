/***************************************************************************
 *                                  Body.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id: Body.cs 807 2012-01-02 14:21:20Z asayre $
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;
using System.IO;
using Server.Logging;

namespace Server
{
	public enum BodyType : byte
	{
		Empty,
		Monster,
		Sea,
		Animal,
		Human,
		Equipment
	}

	public struct Body
	{
		private int m_BodyID;

		private static BodyType[] m_Types;

		static Body()
		{
			if ( File.Exists( "Data/bodyTable.cfg" ) )
			{
				using ( StreamReader ip = new StreamReader( "Data/bodyTable.cfg" ) )
				{
                    m_Types = new BodyType[0x1000];

					string line;

					while ( (line = ip.ReadLine()) != null )
					{
						if ( line.Length == 0 || line.StartsWith( "#" ) )
							continue;

						string[] split = line.Split( '\t' );
#if Framework_4_0
						BodyType type;
						int bodyID;

						if( int.TryParse( split[0], out bodyID ) && Enum.TryParse( split[1], true, out type ) && bodyID >= 0 && bodyID < m_Types.Length )
						{
							m_Types[bodyID] = type;
						}
						else
						{
							ConsoleLog.Write.Warning( "Warning: Invalid bodyTable entry:" );
							ConsoleLog.Write.Information( line );
						}
#else
                        try
                        {
                            int bodyID = int.Parse(split[0]);
                            BodyType type = (BodyType)Enum.Parse(typeof(BodyType), split[1], true);

                            if (bodyID >= 0 && bodyID < m_Types.Length)
                                m_Types[bodyID] = type;
                        }
                        catch
                        {
                            ConsoleLog.Write.Warning("Warning: Invalid bodyTable entry:");
                            ConsoleLog.Write.Information(line);
                        }
#endif
					}
				}
			}
			else
			{
				ConsoleLog.Write.Warning( "Warning: Data/bodyTable.cfg does not exist" );

				m_Types = new BodyType[0];
			}
		}

		public Body( int bodyID )
		{
			m_BodyID = bodyID;
		}

		public BodyType Type
		{
			get
			{
				if ( m_BodyID >= 0 && m_BodyID < m_Types.Length )
					return m_Types[m_BodyID];
				else
					return BodyType.Empty;
			}
		}

		public bool IsHuman =>
            m_BodyID >= 0
            && m_BodyID < m_Types.Length
            && m_Types[m_BodyID] == BodyType.Human
            && m_BodyID != 402
            && m_BodyID != 403
            && m_BodyID != 607
            && m_BodyID != 608
            && m_BodyID != 970;

        public bool IsMale =>
            m_BodyID == 183
            || m_BodyID == 185
            || m_BodyID == 400
            || m_BodyID == 402
            || m_BodyID == 605
            || m_BodyID == 607
            || m_BodyID == 750;

        public bool IsFemale =>
            m_BodyID == 184
            || m_BodyID == 186
            || m_BodyID == 401
            || m_BodyID == 403
            || m_BodyID == 606
            || m_BodyID == 608
            || m_BodyID == 751;

        public bool IsGhost =>
            m_BodyID == 402
            || m_BodyID == 403
            || m_BodyID == 607
            || m_BodyID == 608
            || m_BodyID == 970;

        public bool IsMonster =>
            m_BodyID >= 0
            && m_BodyID < m_Types.Length
            && m_Types[m_BodyID] == BodyType.Monster;

        public bool IsAnimal =>
            m_BodyID >= 0
            && m_BodyID < m_Types.Length
            && m_Types[m_BodyID] == BodyType.Animal;

        public bool IsEmpty =>
            m_BodyID >= 0
            && m_BodyID < m_Types.Length
            && m_Types[m_BodyID] == BodyType.Empty;

        public bool IsSea =>
            m_BodyID >= 0
            && m_BodyID < m_Types.Length
            && m_Types[m_BodyID] == BodyType.Sea;

        public bool IsEquipment =>
            m_BodyID >= 0
            && m_BodyID < m_Types.Length
            && m_Types[m_BodyID] == BodyType.Equipment;

        public int BodyID => m_BodyID;

        public static implicit operator int( Body a )
		{
			return a.m_BodyID;
		}

		public static implicit operator Body( int a )
		{
			return new Body( a );
		}

		public override string ToString()
		{
			return string.Format( "0x{0:X}", m_BodyID );
		}

		public override int GetHashCode()
		{
			return m_BodyID;
		}

		public override bool Equals( object o )
		{
			if ( o == null || !(o is Body) ) return false;

			return ((Body)o).m_BodyID == m_BodyID;
		}

		public static bool operator == ( Body l, Body r )
		{
			return l.m_BodyID == r.m_BodyID;
		}

		public static bool operator != ( Body l, Body r )
		{
			return l.m_BodyID != r.m_BodyID;
		}

		public static bool operator > ( Body l, Body r )
		{
			return l.m_BodyID > r.m_BodyID;
		}

		public static bool operator >= ( Body l, Body r )
		{
			return l.m_BodyID >= r.m_BodyID;
		}

		public static bool operator < ( Body l, Body r )
		{
			return l.m_BodyID < r.m_BodyID;
		}

		public static bool operator <= ( Body l, Body r )
		{
			return l.m_BodyID <= r.m_BodyID;
		}
	}
}