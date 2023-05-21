using System;
using System.IO;
using System.Net;
using Server.Misc;

namespace Server
{
	public class AccessRestrictions
	{
		public static void Initialize()
		{
			EventSink.SocketConnect += EventSink_SocketConnect;
		}
			
		private static void EventSink_SocketConnect( SocketConnectEventArgs e )
		{
			try
			{
				IPAddress ip = ((IPEndPoint)e.Socket.RemoteEndPoint).Address;

				if ( Firewall.IsBlocked( ip ) )
				{
					Utility.ConsoleWriteLine(Utility.ConsoleMsgType.Warning, $"Client: {ip}: Firewall blocked connection attempt." );
					e.AllowConnection = false;
					return;
				}
				else if ( IPLimiter.SocketBlock && !IPLimiter.Verify( ip ) )
				{
					Utility.ConsoleWriteLine(Utility.ConsoleMsgType.Warning, $"Client: {ip}: Past IP limit threshold" );

					using ( StreamWriter op = new StreamWriter( "ipLimits.log", true ) )
						op.WriteLine( "{0}\tPast IP limit threshold\t{1}", ip, DateTime.UtcNow );
	
					e.AllowConnection = false;
					return;
				}
			}
			catch
			{
				e.AllowConnection = false;
			}
		}
	}
}