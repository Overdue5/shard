using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using Server.Accounting;
using Server.Logging;
using Server.Network;

namespace Server.Misc
{
	public class CrashGuard
	{
		private static bool Enabled = true;
		private static bool SaveBackup = true;
		private static bool RestartServer = true;
		private static bool GenerateReport = true;

		public static void Initialize()
		{
			if ( Enabled ) // If enabled, register our crash event handler
				EventSink.Crashed += CrashGuard_OnCrash;
		}

		public static void CrashGuard_OnCrash( CrashedEventArgs e )
		{
            if (GenerateReport)
                GenerateCrashReport(e);

            World.WaitForWriteCompletion();

			if ( SaveBackup )
				Backup();

            /*if ( Core.Service )
                e.Close = true;
            else */
            if (RestartServer)
                Restart(e);
		}

		private static void SendEmail( string filePath )
		{
			Console.Write( "Crash: Sending email..." );

			MailMessage message = new MailMessage( "server@in-x.org", Email.CrashAddresses );

            message.Subject = "Automated magine Nation: Xtreme Crash Report";

			message.Body = "Automated Imagine Nation: Xtreme Crash Report. See attachment for details.";

			message.Attachments.Add( new Attachment( filePath ) );

			if ( Email.Send( message ) )
				ConsoleLog.Write.Information( "done" );
			else
				ConsoleLog.Write.Information( "failed" );
		}

		private static string GetRoot()
		{
			try
			{
				return Path.GetDirectoryName( Environment.GetCommandLineArgs()[0] );
			}
			catch
			{
				return "";
			}
		}

		private static string Combine( string path1, string path2 )
		{
			if ( path1.Length == 0 )
				return path2;

			return Path.Combine( path1, path2 );
		}

		private static void Restart( CrashedEventArgs e )
		{
			Console.Write( "Crash: Restarting..." );

			try
            {
                Core.Kill(restart: true, update: false);
                //Process.Start( Core.ExePath, Core.Arguments );
                //ConsoleLog.Write.Information( "done" );

                //e.Close = true;
                //Core.Process.Kill(); //Taran: Tested a fix for server not shutting down properly on crash, but seemed to prohibit finishing crashlog etc.
            }
			catch
			{
				ConsoleLog.Write.Information( "failed" );
			}
		}

		private static void CreateDirectory( string path )
		{
			if ( !Directory.Exists( path ) )
				Directory.CreateDirectory( path );
		}

		private static void CreateDirectory( string path1, string path2 )
		{
			CreateDirectory( Combine( path1, path2 ) );
		}

		private static void CopyFile( string rootOrigin, string rootBackup, string path )
		{
			string originPath = Combine( rootOrigin, path );
			string backupPath = Combine( rootBackup, path );

			try
			{
				if ( File.Exists( originPath ) )
					File.Copy( originPath, backupPath );
			}
			catch
			{
			}
		}

		private static void Backup()
		{
			Console.Write( "Crash: Backing up..." );

			try
			{
				string timeStamp = GetTimeStamp();

				string root = GetRoot();
				string rootBackup = Combine( root, String.Format( "Backups/Crashed/{0}/", timeStamp ) );
				string rootOrigin = Combine( root, String.Format( "Saves/" ) );

				// Create new directories
				CreateDirectory( rootBackup );
				CreateDirectory( rootBackup, "Accounts/" );
				CreateDirectory( rootBackup, "Items/" );
				CreateDirectory( rootBackup, "Mobiles/" );
				CreateDirectory( rootBackup, "Guilds/" );
				CreateDirectory( rootBackup, "Regions/" );

				// Copy files
				CopyFile( rootOrigin, rootBackup, "Accounts/Accounts.xml" );

				CopyFile( rootOrigin, rootBackup, "Items/Items.bin" );
				CopyFile( rootOrigin, rootBackup, "Items/Items.idx" );
				CopyFile( rootOrigin, rootBackup, "Items/Items.tdb" );

				CopyFile( rootOrigin, rootBackup, "Mobiles/Mobiles.bin" );
				CopyFile( rootOrigin, rootBackup, "Mobiles/Mobiles.idx" );
				CopyFile( rootOrigin, rootBackup, "Mobiles/Mobiles.tdb" );

				CopyFile( rootOrigin, rootBackup, "Guilds/Guilds.bin" );
				CopyFile( rootOrigin, rootBackup, "Guilds/Guilds.idx" );

				CopyFile( rootOrigin, rootBackup, "Regions/Regions.bin" );
				CopyFile( rootOrigin, rootBackup, "Regions/Regions.idx" );

				ConsoleLog.Write.Information( "done" );
			}
			catch
			{
				ConsoleLog.Write.Information( "failed" );
			}
		}

		private static void GenerateCrashReport( CrashedEventArgs e )
		{
			Console.Write( "Crash: Generating report..." );

			try
			{
				string fileName = String.Format( "Crash {0}.log", GetTimeStamp() );

				if( !Directory.Exists( "Crash Logs/" ) )
					Directory.CreateDirectory( "Crash Logs/" );

				string filePath = Combine( "Crash Logs/", fileName );

				using ( StreamWriter op = new StreamWriter( filePath ) )
				{
					Version ver = Core.Assembly.GetName().Version;

					op.WriteLine( "Server Crash Report" );
					op.WriteLine( "===================" );
					op.WriteLine();
					op.WriteLine( "RunUO Version {0}.{1}, Build {2}.{3}", ver.Major, ver.Minor, ver.Build, ver.Revision );
					op.WriteLine( "Operating System: {0}", Environment.OSVersion );
					op.WriteLine( ".NET Framework: {0}", Environment.Version );
					op.WriteLine( "Time: {0}", DateTime.UtcNow );

					try { op.WriteLine( "Mobiles: {0}", World.Mobiles.Count ); }
					catch {}

					try { op.WriteLine( "Items: {0}", World.Items.Count ); }
					catch {}

					op.WriteLine( "Exception:" );
					op.WriteLine( e.Exception );
					op.WriteLine();

					op.WriteLine( "Clients:" );

					try
					{
						List<NetState> states = NetState.Instances;

						op.WriteLine( "- Count: {0}", states.Count );

						for ( int i = 0; i < states.Count; ++i )
						{
							NetState state = states[i];

							op.Write( "+ {0}:", state );

							Account a = state.Account as Account;

							if ( a != null )
								op.Write( " (account = {0})", a.Username );

							Mobile m = state.Mobile;

							if ( m != null )
								op.Write( $" (mobile = 0x{m.Serial}, {m.Name}, X:{m.X},Y:{m.Y},Z:{m.Z}',M:{m.Map})");

							op.WriteLine();
						}
					}
					catch
					{
						op.WriteLine( "- Failed" );
					}
				}

				ConsoleLog.Write.Information( "done" );

				if ( Email.CrashAddresses != null )
					SendEmail( filePath );
			}
			catch
			{
				ConsoleLog.Write.Information( "failed" );
			}
		}

		private static string GetTimeStamp()
		{
			DateTime now = DateTime.UtcNow;

			return String.Format( "{0}-{1}-{2}-{3}-{4}-{5}",
					now.Day,
					now.Month,
					now.Year,
					now.Hour,
					now.Minute,
					now.Second
				);
		}
	}
}