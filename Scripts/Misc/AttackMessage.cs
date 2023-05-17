using System;
using System.Collections.Generic;
using Server.Commands.GMUtils;
using Server.Network;

namespace Server.Misc
{
	public class AttackMessage
	{
		private const string AggressorFormat = "You are attacking {0}!";
		private const string AggressedFormat = "*You see {0} attacking you!*";
		private const int Hue = 0x22;

		private static readonly TimeSpan Delay = TimeSpan.FromMinutes( 1.0 );

		public static void Initialize()
		{
			EventSink.AggressiveAction += EventSink_AggressiveAction;
		}

		public static void EventSink_AggressiveAction( AggressiveActionEventArgs e )
		{
			Mobile aggressor = e.Aggressor;
			Mobile aggressed = e.Aggressed;

            if (aggressor == null || aggressed == null)
                return;

			if ( !CheckAggressions( aggressor, aggressed ) )
			{
				//aggressor.LocalOverheadMessage( MessageType.Regular, Hue, true, String.Format( AggressorFormat, aggressed.Name ) );
                //aggressor.NonlocalOverheadMessage(MessageType.Regular, Hue, true, "IM agressor: " + aggressor.Name);
                //aggressed.NonlocalOverheadMessage(MessageType.Regular, Hue, true, "IM agressor: " + aggressed.Name);


                //Show the 2 players
                aggressed.LocalOverheadMessage(MessageType.Regular, (int)GMExtendMethods.EmotionalTextHue.StrangeAction, false, String.Format(AggressedFormat, aggressor.Name));
                aggressor.LocalOverheadMessage(MessageType.Regular, (int)GMExtendMethods.EmotionalTextHue.StrangeAction, false, String.Format(AggressedFormat, aggressed.Name));
            }
		}

		public static bool CheckAggressions( Mobile m1, Mobile m2 )
		{
			List<AggressorInfo> list = m1.Aggressors;

			for ( int i = 0; i < list.Count; ++i )
			{
				AggressorInfo info = list[i];

				if ( info.Attacker == m2 && DateTime.Now < (info.LastCombatTime + Delay) )
					return true;
			}

			list = m2.Aggressors;

			for ( int i = 0; i < list.Count; ++i )
			{
				AggressorInfo info = list[i];

				if ( info.Attacker == m1 && DateTime.Now < (info.LastCombatTime + Delay) )
					return true;
			}

			return false;
		}
	}
}