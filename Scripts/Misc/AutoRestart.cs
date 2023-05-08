using System;
using Server.Commands;

namespace Server.Misc
{
	public class AutoRestart : Timer
	{
		public static bool Enabled = false; // is the script enabled?

		private static readonly TimeSpan RestartTime = TimeSpan.FromHours( 2.0 ); // time of day at which to restart
		private static readonly TimeSpan RestartDelay = TimeSpan.FromMinutes(1.0); // how long the server should remain active before restart (period of 'server wars')

		private static readonly TimeSpan WarningDelay = TimeSpan.FromMinutes( 1.0 ); // at what interval should the shutdown message be displayed?

		private static bool m_Restarting;
		private static DateTime m_RestartTime;
        private bool m_SendToDiscord = true;

		public static bool Restarting
		{
			get{ return m_Restarting; }
		}

		public static void Initialize()
		{
			//CommandSystem.Register( "Restart", AccessLevel.Administrator, Restart_OnCommand );
			if (Enabled)
                new AutoRestart().Start();
		}

		public static void Restart_OnCommand( CommandEventArgs e )
		{
			if ( m_Restarting )
			{
				e.Mobile.SendMessage( "The server is already restarting." );
			}
			else
			{
				Enabled = true;
				new AutoRestart().Start();
                m_RestartTime = DateTime.Now;
                
				e.Mobile.SendMessage($"You have initiated server shutdown in {DateTime.Now + RestartDelay}, after {((m_RestartTime + RestartDelay) - DateTime.Now).TotalSeconds} seconds");
            }
		}

		public AutoRestart() : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
		{
			Priority = TimerPriority.FiveSeconds;

			m_RestartTime = DateTime.Now.Date + RestartTime;

			if ( m_RestartTime < DateTime.Now )
				m_RestartTime += TimeSpan.FromDays( 1.0 );
		}

		private void Warning_Callback()
        {
            var time = Utility.LimitMinMax(0, (m_RestartTime + RestartDelay - DateTime.Now).TotalSeconds, RestartDelay.TotalSeconds);

			World.Broadcast( 0x22, true, $"Britain will soon be out of reach for avatars. {Math.Truncate(time)} seconds remain", m_SendToDiscord);
            m_SendToDiscord = false;
        }

        private void Restart_Callback()
        {
            Core.Kill(true);
        }

		protected override void OnTick()
		{
			if ( m_Restarting || !Enabled )
				return;

			if ( DateTime.Now < m_RestartTime )
				return;

			if ( WarningDelay > TimeSpan.Zero )
			{
				Warning_Callback();
				DelayCall(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), new TimerCallback( Warning_Callback ) );
			}

            AutoSave.Save(false);
            m_Restarting = true;

			DelayCall( RestartDelay, new TimerCallback( Restart_Callback ) );
		}
	}
}