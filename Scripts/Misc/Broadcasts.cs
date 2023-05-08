namespace Server.Misc
{
	public class Broadcasts
	{
		public static void Configure()
		{
            EventSink.AfterWorldLoad += EventSink_Load;
            EventSink.Crashed += EventSink_Crashed;
			EventSink.Shutdown += EventSink_Shutdown;
		}

        public static void EventSink_Crashed( CrashedEventArgs e )
		{
			try
			{
			    World.Broadcast(0x35, true, "Deluxe: Communication with the Britannia cut off");
                BaseDiscord.Bot.SendToDiscord(Server.BaseDiscord.Channel.ConsoleImportant, $"Deluxe: The server has crashed, attempting to restart automatically...");
            }
			catch
			{
			}
		}

        public static void EventSink_Load()
        {
            try
            {
                World.Broadcast(0x35, true, "Britannia is available again");
                BaseDiscord.Bot.SendToDiscord(Server.BaseDiscord.Channel.Announcement, $"Deluxe: Britannia is available again");
            }
            catch
            {
            }
        }

        public static void EventSink_Shutdown( ShutdownEventArgs e )
		{
			try
			{
				World.Broadcast( 0x35, true, "Deluxe: The Britannia is currently unavailable, be back up shortly...");
			}
			catch
			{
			}
		}
	}
}