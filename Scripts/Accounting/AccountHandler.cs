//Rev 187

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Server.Accounting;
using Server.Commands;
using Server.Engines.Help;
using Server.Logging;
using Server.Mobiles;
using Server.Network;
using Server.Regions;

namespace Server.Misc
{
	public enum PasswordProtection
	{
		None,
		Crypt,
		NewCrypt
	}

	public class AccountHandler
	{
		private static int MaxAccountsPerIP = 100;
		private static bool AutoAccountCreation = true;
		private static readonly bool RestrictDeletion = !TestCenter.Enabled;
		private static readonly TimeSpan DeleteDelay = TimeSpan.FromDays( 7.0 );

		public static PasswordProtection ProtectPasswords = PasswordProtection.NewCrypt;

		private static AccessLevel m_LockdownLevel;

		public static AccessLevel LockdownLevel
		{
			get => m_LockdownLevel;
            set => m_LockdownLevel = value;
        }

        private static readonly CityInfo[] StartingCities = new CityInfo[]
			{
				new CityInfo( "New Haven",	"New Haven Bank",	1150168, 3667,	2625,	0  ),
				new CityInfo( "Yew",		"The Empath Abbey",	1075072, 633,	858,	0  ),
				new CityInfo( "Minoc",		"The Barnacle",		1075073, 2476,	413,	15 ),
				new CityInfo( "Britain",	"The Wayfarer's Inn",	1075074, 1602,	1591,	20 ),
				new CityInfo( "Moonglow",	"The Scholars Inn",	1075075, 4408,	1168,	0  ),
				new CityInfo( "Trinsic",	"The Traveler's Inn",	1075076, 1845,	2745,	0  ),
				new CityInfo( "Jhelom",		"The Mercenary Inn",	1075078, 1374,	3826,	0  ),
				new CityInfo( "Skara Brae",	"The Falconer's Inn",	1075079, 618,	2234,	0  ),
				new CityInfo( "Vesper",		"The Ironwood Inn",	1075080, 2771,	976,	0  )
			};

        /* Old Haven/Magincia Locations
            new CityInfo( "Britain", "Sweet Dreams Inn", 1496, 1628, 10 );
            // ..
            // Trinsic
            new CityInfo( "Magincia", "The Great Horns Tavern", 3734, 2222, 20 ),
            // Jhelom
            // ..
            new CityInfo( "Haven", "Buckler's Hideaway", 3667, 2625, 0 )

            if ( Core.AOS )
            {
                //CityInfo haven = new CityInfo( "Haven", "Uzeraan's Mansion", 3618, 2591, 0 );
                CityInfo haven = new CityInfo( "Haven", "Uzeraan's Mansion", 3503, 2574, 14 );
                StartingCities[StartingCities.Length - 1] = haven;
            }
        */

		private static bool PasswordCommandEnabled = true;

		public static void Initialize()
		{
			EventSink.DeleteRequest += EventSink_DeleteRequest;
			EventSink.AccountLogin += EventSink_AccountLogin;
			EventSink.GameLogin += EventSink_GameLogin;

			if ( PasswordCommandEnabled )
				CommandSystem.Register( "Password", AccessLevel.Player, Password_OnCommand );

            if (!Directory.Exists("Logs"))
                Directory.CreateDirectory("Logs");
            if (!Directory.Exists("Logs/LoginLogout"))
                Directory.CreateDirectory("Logs/LoginLogout");
		}

		[Usage( "Password <newPassword> <repeatPassword>" )]
		[Description( "Changes the password of the commanding players account. Requires the same C-class IP address as the account's creator." )]
		public static void Password_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			Account acct = from.Account as Account;

			if ( acct == null )
				return;

			IPAddress[] accessList = acct.LoginIPs;

			if ( accessList.Length == 0 )
				return;

			NetState ns = from.NetState;

			if ( ns == null )
				return;

			if ( e.Length == 0 )
			{
				from.SendMessage( "You must specify the new password." );
				return;
			}
			else if ( e.Length == 1 )
			{
				from.SendMessage( "To prevent potential typing mistakes, you must type the password twice. Use the format:" );
				from.SendMessage( "Password \"(newPassword)\" \"(repeated)\"" );
				return;
			}

			string pass = e.GetString( 0 );
			string pass2 = e.GetString( 1 );

			if ( pass != pass2 )
			{
				from.SendMessage( "The passwords do not match." );
				return;
			}

			bool isSafe = true;

			for ( int i = 0; isSafe && i < pass.Length; ++i )
				isSafe = ( pass[i] >= 0x20 && pass[i] < 0x80 );

			if ( !isSafe )
			{
				from.SendMessage( "That is not a valid password." );
				return;
			}

			try
			{
				IPAddress ipAddress = ns.Address;

				if ( Utility.IPMatchClassC( accessList[0], ipAddress ) )
				{
					acct.SetPassword( pass );
					from.SendMessage( "The password to your account has changed." );
				}
				else
				{
					PageEntry entry = PageQueue.GetEntry( from );

					if ( entry != null )
					{
						if ( entry.Message.StartsWith( "[Automated: Change Password]" ) )
							from.SendMessage( "You already have a password change request in the help system queue." );
						else
							from.SendMessage( "Your IP address does not match that which created this account." );
					}
					else if ( PageQueue.CheckAllowedToPage( from ) )
					{
						from.SendMessage( "Your IP address does not match that which created this account.  A page has been entered into the help system on your behalf." );

						from.SendLocalizedMessage( 501234, "", 0x35 ); /* The next available Counselor/Game Master will respond as soon as possible.
																	    * Please check your Journal for messages every few minutes.
																	    */

						PageQueue.Enqueue( new PageEntry( from, String.Format( "[Automated: Change Password]<br>Current IP address: {0}<br>Account IP address: {1}", ipAddress, accessList[0] ), PageType.Account ) );
					}

				}
			}
			catch
			{
			}
		}

		private static void EventSink_DeleteRequest( DeleteRequestEventArgs e )
		{
			NetState state = e.State;
			int index = e.Index;

			Account acct = state.Account as Account;
		    IPAddress[] accessList = new IPAddress[] {};
		    if ( acct != null )
                accessList = acct.LoginIPs;

			if ( acct == null )
			{
				state.Dispose();
			}

			else if ( index < 0 || index >= acct.Length )
			{
				state.Send( new DeleteResult( DeleteResultType.BadRequest ) );
				state.Send( new CharacterListUpdate( acct ) );
			}
			else
			{
			    try
			    {
			        Mobile m = acct[index];
                    IPAddress ipAddress = state.Address;
			        PlayerMobile pm = null;
			        if (m != null && m.Player)
                        pm = (PlayerMobile)m;
			        if (m == null)
			        {
			            state.Send(new DeleteResult(DeleteResultType.CharNotExist));
			            state.Send(new CharacterListUpdate(acct));
			        }
			        else if (m.NetState != null)
			        {
			            state.Send(new DeleteResult(DeleteResultType.CharBeingPlayed));
			            state.Send(new CharacterListUpdate(acct));
			        }
			        else if (RestrictDeletion && DateTime.UtcNow < (m.CreationTime + DeleteDelay))
			        {
			            state.Send(new DeleteResult(DeleteResultType.CharTooYoung));
			            state.Send(new CharacterListUpdate(acct));
			        }
			        else if (m.AccessLevel == AccessLevel.Player && Region.Find(m.LogoutLocation, m.LogoutMap).GetRegion(typeof (Jail)) != null) //Don't need to check current location, if netstate is null, they're logged out
			        {
			            state.Send(new DeleteResult(DeleteResultType.BadRequest));
			            state.Send(new CharacterListUpdate(acct));
			        }/*
                    else if ( (pm != null && !pm.AllowDelete) && !Utility.IPMatchClassC(accessList[0], ipAddress))
                    {
                        ConsoleLog.Write.Information("Client: {0}: Trying to delete character {1} (0x{2:X})", state, index, m.Serial.Value);
                        state.Send(new DeleteResult(DeleteResultType.BadRequest));
                        state.Send(new CharacterListUpdate(acct));
                    }*/
                    else
                    {
						ConsoleLog.Write.Information($@"Client: {state}: Deleting character {index} (0x{m.Serial.Value:X})");

                        acct.Comments.Add(new AccountComment("System", String.Format("Character #{0} {1} deleted by {2}", index + 1, m, state)));

                        m.Delete();
                        state.Send(new CharacterListUpdate(acct));
                    }
			    }
                catch
			    {
			    }
			}
		}

		public static bool CanCreate( IPAddress ip )
		{
			if ( !IPTable.ContainsKey( ip ) )
				return true;

			return ( IPTable[ip] < MaxAccountsPerIP );
		}

		private static Dictionary<IPAddress, Int32> m_IPTable;

		public static Dictionary<IPAddress, Int32> IPTable
		{
			get
			{
				if ( m_IPTable == null )
				{
					m_IPTable = new Dictionary<IPAddress, Int32>();

					foreach ( Account a in Accounts.GetAccounts() )
						if ( a.LoginIPs.Length > 0 )
						{
							IPAddress ip = a.LoginIPs[0];

							if ( m_IPTable.ContainsKey( ip ) )
								m_IPTable[ip]++;
							else
								m_IPTable[ip] = 1;
						}
				}

				return m_IPTable;
			}
		}

        private static readonly char[] m_ForbiddenChars = new char[]
		{
			'<', '>', ':', '"', '/', '\\', '|', '?', '*'
		};

        private static bool IsForbiddenChar(char c)
        {
            for (int i = 0; i < m_ForbiddenChars.Length; ++i)
                if (c == m_ForbiddenChars[i])
                    return true;

            return false;
        }

		private static Account CreateAccount( NetState state, string un, string pw )
		{
			if ( un.Length == 0 || pw.Length == 0 )
				return null;

            bool isSafe = !(un.StartsWith(" ") || un.EndsWith(" ") || un.EndsWith("."));

			for ( int i = 0; isSafe && i < un.Length; ++i )
                isSafe = (un[i] >= 0x20 && un[i] < 0x80 && !IsForbiddenChar(un[i]));

			for ( int i = 0; isSafe && i < pw.Length; ++i )
				isSafe = ( pw[i] >= 0x20 && pw[i] < 0x80 );

			if ( !isSafe )
				return null;

			if ( !CanCreate( state.Address ) )
			{
				ConsoleLog.Write.Warning($"Login: {state}: Account '{un}' not created, ip already has {MaxAccountsPerIP} account{(MaxAccountsPerIP == 1 ? "" : "s")}." );
				return null;
			}

			ConsoleLog.Write.Information($"Login: {state}: Creating new account '{un}'" );

			Account a = new Account( un, pw );

			return a;
		}

		public static void EventSink_AccountLogin( AccountLoginEventArgs e )
		{
			if ( !IPLimiter.SocketBlock && !IPLimiter.Verify( e.State.Address ) )
			{
				e.Accepted = false;
				e.RejectReason = ALRReason.InUse;

				ConsoleLog.Write.Warning($"Login: {e.State}: Past IP limit threshold" );

				using ( StreamWriter op = new StreamWriter( "ipLimits.log", true ) )
					op.WriteLine( "{0}\tPast IP limit threshold\t{1}", e.State, DateTime.UtcNow );

				return;
			}

			string un = e.Username;
			string pw = e.Password;

			e.Accepted = false;
			Account acct = Accounts.GetAccount( un ) as Account;

			if ( acct == null )
			{
				if ( AutoAccountCreation && un.Trim().Length > 0 )	//To prevent someone from making an account of just '' or a bunch of meaningless spaces 
				{
					e.State.Account = acct = CreateAccount( e.State, un, pw );
					e.Accepted = acct == null ? false : acct.CheckAccess( e.State );

					if ( !e.Accepted )
						e.RejectReason = ALRReason.BadComm;
				}
				else
				{
					ConsoleLog.Write.Warning($"Login: {e.State}: Invalid username '{un}'" );
					e.RejectReason = ALRReason.Invalid;
				}
			}
			else if ( !acct.HasAccess( e.State ) )
			{
				ConsoleLog.Write.Warning($"Login: {e.State}: Access denied for '{un}'" );
				e.RejectReason = ( m_LockdownLevel > AccessLevel.Player ? ALRReason.BadComm : ALRReason.BadPass );
			}
			else if ( !acct.CheckPassword( pw ) )
			{
				ConsoleLog.Write.Warning($"Login: {e.State}: Invalid password for '{un}'" );
				e.RejectReason = ALRReason.BadPass;
                //*****Logging attempt*****
                try
                {
                    Stream fileStream = File.Open("Logs/LoginLogout/" + un + ".log", FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    StreamWriter writeAdapter = new StreamWriter(fileStream);
                    writeAdapter.WriteLine($"{e.State}: Invalid password for {un} on {DateTime.UtcNow}");
                    writeAdapter.Close();
                }
                catch
                {
					ConsoleLog.Write.Error($"Record Error... {un} Login");
                }
                //**************************
			}
			else if ( acct.Banned )
			{
				ConsoleLog.Write.Warning($"Login: {e.State}: Banned account '{un}'" );
				e.RejectReason = ALRReason.Blocked;
                //*****Logging attempt*****
                try
                {
                    Stream fileStream = File.Open("Logs/LoginLogout/" + un + ".log", FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    StreamWriter writeAdapter = new StreamWriter(fileStream);
                    writeAdapter.WriteLine(String.Format("{0}: Banned account: {1} on {2}", e.State, un, DateTime.UtcNow));
                    writeAdapter.Close();
                }
                catch
                {
					ConsoleLog.Write.Error($"Record Error... {un} Login");
                }
                //**************************
			}
			else
			{
				ConsoleLog.Write.Information($"Login: {e.State}: Valid credentials for '{un}'" );
				e.State.Account = acct;
				e.Accepted = true;

				acct.LogAccess( e.State );
			}

			if ( !e.Accepted )
				AccountAttackLimiter.RegisterInvalidAccess( e.State );
		}

		public static void EventSink_GameLogin( GameLoginEventArgs e )
		{
			if ( !IPLimiter.SocketBlock && !IPLimiter.Verify( e.State.Address ) )
			{
				e.Accepted = false;

				ConsoleLog.Write.Warning($"Login: {e.State}: Past IP limit threshold" );

				using ( StreamWriter op = new StreamWriter( "ipLimits.log", true ) )
					op.WriteLine( "{0}\tPast IP limit threshold\t{1}", e.State, DateTime.UtcNow );

				return;
			}

			string un = e.Username;
			string pw = e.Password;

			Account acct = Accounts.GetAccount( un ) as Account;

			if ( acct == null )
			{
				e.Accepted = false;
			}
			else if ( !acct.HasAccess( e.State ) )
			{
				ConsoleLog.Write.Warning($"Login: {e.State}: Access denied for '{un}'" );
				e.Accepted = false;
			}
			else if ( !acct.CheckPassword( pw ) )
			{
				ConsoleLog.Write.Warning($"Login: {e.State}: Invalid password for '{un}'" );
				e.Accepted = false;
                //*****Logging attempt*****
                try
                {
                    Stream fileStream = File.Open("Logs/LoginLogout/" + un + ".log", FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    StreamWriter writeAdapter = new StreamWriter(fileStream);
                    writeAdapter.WriteLine(String.Format("{0}: Invalid password for {1} on {2}", e.State, un, DateTime.UtcNow));
                    writeAdapter.Close();
                }
                catch
                {
					ConsoleLog.Write.Error($"Record Error... {un} Login");
                }
                //**************************
			}
			else if ( acct.Banned )
			{
				ConsoleLog.Write.Warning($"Login: {e.State}: Banned account '{un}'" );
				e.Accepted = false;
                //*****Logging attempt*****
                try
                {
                    Stream fileStream = File.Open("Logs/LoginLogout/" + un + ".log", FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    StreamWriter writeAdapter = new StreamWriter(fileStream);
                    writeAdapter.WriteLine(String.Format("{0}: Banned account: {1} on {2}", e.State, un, DateTime.UtcNow));
                    writeAdapter.Close();
                }
                catch
                {
					ConsoleLog.Write.Error($"Record Error... {un} Login");
                }
                //**************************
			}
			else
			{
				acct.LogAccess( e.State );

				ConsoleLog.Write.Information($"Login: {e.State}: Account '{un}' at character list" );
				e.State.Account = acct;
				e.Accepted = true;
				e.CityInfo = StartingCities;
			}

			if ( !e.Accepted )
				AccountAttackLimiter.RegisterInvalidAccess( e.State );
		}
	}
}