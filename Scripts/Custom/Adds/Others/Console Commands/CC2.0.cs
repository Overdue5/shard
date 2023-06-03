using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Server.Accounting;
using Server.Engines.Help;
using Server.Logging;
using Server.Network;

namespace Server.Misc
{
	internal class ServerConsole
	{
		private static PageEntry[] m_List;
		public static bool paging;

		#region Event Handlers
		public static void EventSink_ServerStarted()
		{
			ThreadPool.QueueUserWorkItem( new WaitCallback( ConsoleListen ) );
			ConsoleLog.Write.Information("CC initialized..." );
		}

		#endregion

		public static void Initialize()
		{
			EventSink.ServerStarted += new ServerStartedEventHandler( EventSink_ServerStarted );
		}

		public static void ConsoleListen( Object stateInfo )
		{
			if( !paging )
			{
				string input = Console.ReadLine();
				Next( input );
			}
		}

		public static void PageResp( object obj )
		{
			paging = true;
			object[] objects = (object[])obj;
			int w = (int)objects[0];
			int pag = (int)objects[1];
			int paG;
			if( w == 1 )
			{
				up:
				try
				{
					paG = Convert.ToInt32( Console.ReadLine() );
				}
				catch
				{
					ConsoleLog.Write.Error("Thats not a number,try again." );
					goto up;
				}
				ConsoleLog.Write.Information("Type your response" );
				object[] ob = new object[] { 2, paG };
				ThreadPool.QueueUserWorkItem( new WaitCallback( PageResp ), ob );
			}
			else
			{
				string resp = Console.ReadLine();
				ArrayList list = PageQueue.List;
				m_List = (PageEntry[])list.ToArray( typeof( PageEntry ) );
				if( m_List.Length > 0 )
					if( pag > m_List.Length )
						ConsoleLog.Write.Warning("Error: Not a valid page number" );
					else
						for( int i = 0; i < m_List.Length; ++i )
						{
							PageEntry e = m_List[i];
							if( i == pag )
							{
								e.Sender.SendGump( new MessageSentGump( e.Sender, "Admin", resp, true ) );
								PageQueue.Remove( e );
								ConsoleLog.Write.Information("Message Sent..." );
							}
						}
				else
					ConsoleLog.Write.Information("There are no pages to display." );
			}
			paging = false;
			ThreadPool.QueueUserWorkItem( new WaitCallback( ConsoleListen ) );
		}

		public static void BroadcastMessage( AccessLevel ac, int hue, string message )
		{
			foreach( NetState state in NetState.Instances )
			{
				Mobile m = state.Mobile;
				if( m != null && m.AccessLevel >= ac )
					m.SendMessage( hue, message );
			}
		}

		public static void Next( string nput )
		{
			string input = nput.ToLower();
			if( input.StartsWith( "bc" ) )
			{
				string imput = input.Replace( "bc", "" );
				BroadcastMessage( AccessLevel.Player, 0x35, String.Format( "[Admin] {0}", imput ) );
				ConsoleLog.Write.Information( "Players will see: {0}", imput );
			}
			else if( input.StartsWith( "sc" ) )
			{
				string imput = input.Replace( "staff", "" );
				BroadcastMessage( AccessLevel.Counselor, 0x32, String.Format( "[Admin] {0}", imput ) );
				ConsoleLog.Write.Information( "Staff will see: {0}", imput );
			}
			else if( input.StartsWith( "ban" ) )
			{
				string imput = input.Replace( "ban", "" );
				List<NetState> states = NetState.Instances;
				if( states.Count == 0 )
					ConsoleLog.Write.Information( "There are no players online." );
				for( int i = 0; i < states.Count; ++i )
				{
					Account a = states[i].Account as Account;
					if( a == null )
						continue;
					Mobile m = states[i].Mobile;
					if( m == null )
						continue;
					string innput = imput.ToLower();
					if( m.Name.ToLower() == innput.Trim() )
					{
						NetState m_ns = m.NetState;
						ConsoleLog.Write.Information( "Mobile name: '{0}' Account name: '{1}'", m.Name, a.Username );
						a.Banned = true;
						m_ns.Dispose();
						ConsoleLog.Write.Information( "Banning complete." );
					}
				}
			}
			else if( input.StartsWith( "kick" ) )
			{
				string imput = input.Replace( "kick", "" );
				List<NetState> states = NetState.Instances;
				if( states.Count == 0 )
					ConsoleLog.Write.Information( "There are no players online." );
				for( int i = 0; i < states.Count; ++i )
				{
					Account a = states[i].Account as Account;
					if( a == null )
						continue;
					Mobile m = states[i].Mobile;
					if( m == null )
						continue;
					string innput = imput.ToLower();
					if( m.Name.ToLower() == innput.Trim() )
					{
						NetState m_ns = m.NetState;
						ConsoleLog.Write.Information( "Mobile name: '{0}' Account name: '{1}'", m.Name, a.Username );
						m_ns.Dispose();
						ConsoleLog.Write.Information( "Kicking complete." );
					}
				}
			}
			else
				switch( input.Trim() )
				{
					case "shutdown nosave":
						Core.Process.Kill();
						break;
					case "restart nosave":
						Process.Start( Core.ExePath, Core.Arguments );
						Core.Process.Kill();
						break;
					case "online":
					{
						List<NetState> states = NetState.Instances;
						if( states.Count == 0 )
							ConsoleLog.Write.Information( "There are no users online at this time." );
						for( int i = 0; i < states.Count; ++i )
						{
							Account a = states[i].Account as Account;
							if( a == null )
								continue;
							Mobile m = states[i].Mobile;
							if( m != null )
								ConsoleLog.Write.Information( "- Account: {0}, Name: {1}, IP: {2}", a.Username, m.Name, states[i] );
						}
						break;
					}
					case "pages":
					{
						paging = true;
						ArrayList list = PageQueue.List;
						PageEntry e;
						for( int i = 0; i < list.Count; )
						{
							e = (PageEntry)list[i];
							if( e.Sender.Deleted || e.Sender.NetState == null )
							{
								e.AddResponse( e.Sender, "[Logout]" );
								PageQueue.Remove( e );
							}
							else
								++i;
						}
						m_List = (PageEntry[])list.ToArray( typeof( PageEntry ) );
						if( m_List.Length > 0 )
						{
							for( int i = 0; i < m_List.Length; ++i )
							{
								e = m_List[i];
								string type = PageQueue.GetPageTypeName( e.Type );
								ConsoleLog.Write.Information( "--------------Page Number: " + i + " --------------------" );
								ConsoleLog.Write.Information( "Player   :" + e.Sender.Name );
								ConsoleLog.Write.Information( "Catagory :" + type );
								ConsoleLog.Write.Information( "Message  :" + e.Message );
							}
							ConsoleLog.Write.Information( "Type the number of the page to respond to." );
							object[] oj = new object[] { 1, 2 };
							ThreadPool.QueueUserWorkItem( new WaitCallback( PageResp ), oj );
						}
						else
						{
							ConsoleLog.Write.Information( "No pages to display." );
							paging = false;
						}
						break;
					}
                    case "save":
                        AutoSave.Save();
                        break;
                    case "help":
					case "list": //Credit to HomeDaddy for this wonderful list!
					default:
					{
						ConsoleLog.Write.Information( " " );
						ConsoleLog.Write.Information( "Commands:" );
						ConsoleLog.Write.Information( "shutdown nosave - Shuts down the server without saving." );
						ConsoleLog.Write.Information( "restart nosave  - Restarts the server without saving." );
						ConsoleLog.Write.Information( "online          - Shows a list of every person online:" );
                        ConsoleLog.Write.Information( "save            - Performs a forced save.");
                        ConsoleLog.Write.Information( "                      Account, Char Name, IP." );
						ConsoleLog.Write.Information( "bc <message>    - Type this command and your message after it. It will then be" );
						ConsoleLog.Write.Information( "                      sent to all players." );
						ConsoleLog.Write.Information( "sc <message>    - Type this command and your message after it.It will then be " );
						ConsoleLog.Write.Information( "                      sent to all staff." );
						ConsoleLog.Write.Information( "pages           - Shows all the pages in the page queue,you type the page" );
						ConsoleLog.Write.Information( "                      number ,then you type your response to the player." );
						ConsoleLog.Write.Information( "ban <playername>- Kicks and bans the users account." );
						ConsoleLog.Write.Information( "kick <playername>- Kicks the user." );
						ConsoleLog.Write.Information( "list or help    - Shows this list." );
						ConsoleLog.Write.Information( " " );
						break;
					}
				}
			if( !paging )
				ThreadPool.QueueUserWorkItem( new WaitCallback( ConsoleListen ) );
		}
	}
}