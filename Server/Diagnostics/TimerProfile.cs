/***************************************************************************
 *                              PacketProfile.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id: TimerProfile.cs 169 2007-04-22 07:31:23Z krrios $
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

using System.Collections.Generic;
using System.IO;

namespace Server.Diagnostics {
	public class TimerProfile : BaseProfile {
		private static Dictionary<string, TimerProfile> _profiles = new Dictionary<string, TimerProfile>();

		public static IEnumerable<TimerProfile> Profiles => _profiles.Values;

        public static TimerProfile Acquire( string name ) {
			if ( !Core.Profiling ) {
				return null;
			}

			TimerProfile prof;

			if ( !_profiles.TryGetValue( name, out prof ) ) {
				_profiles.Add( name, prof = new TimerProfile( name ) );
			}

			return prof;
		}

		private long _created, _started, _stopped;

		public long Created {
			get => _created;
            set => _created = value;
        }

		public long Started {
			get => _started;
            set => _started = value;
        }

		public long Stopped {
			get => _stopped;
            set => _stopped = value;
        }

		public TimerProfile( string name )
			: base( name ) {
		}

		public override void WriteTo( TextWriter op ) {
			base.WriteTo( op );

			op.Write( "\t{0,12:N0} {1,12:N0} {2,-12:N0}", _created, _started, _stopped );
		}
	}
}