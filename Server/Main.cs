/***************************************************************************
 *                                  Main.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id: Main.cs 804 2012-01-02 10:24:08Z asayre $
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
#if Framework_4_0
using System.Threading.Tasks;
#endif
using Server.Network;
using System.Runtime;
using System.Threading.Tasks;
using Server.Logging;

namespace Server
{
	public delegate void Slice();
	public enum ExitCode
    {
		Success = 0,
		PullAndRebuild = 100
    }

    public static class Core
    {
	    public static readonly string DateFormat = "[MMMM dd HH:mm:ss.f]: ";
        private static readonly ILogger m_Logger = LogFactory.GetLogger(typeof(Core));
        private static bool m_Crashed;
		private static Thread timerThread;
		private static string m_BaseDirectory;
		private static string m_ExePath;
		private static List<string> m_DataDirectories = new List<string>();
		private static Assembly m_Assembly;
		private static Process m_Process;
		private static Thread m_Thread;
		private static bool m_Service;
		private static bool m_Debug;
		private static bool m_Cache = true;
        private static bool m_NoCompile;
		private static bool m_HaltOnWarning;
        private static bool m_VBdotNET;
        private static bool m_IgnoreCache;
		private static MultiTextWriter m_MultiConOut;

		private static bool m_Profiling;
		private static DateTime m_ProfileStart;
		private static TimeSpan m_ProfileTime;

		private static MessagePump m_MessagePump;

		public static MessagePump MessagePump
		{
			get => m_MessagePump;
            set => m_MessagePump = value;
        }

		public static Slice Slice;

		public static bool Profiling
		{
			get => m_Profiling;
            set
			{
				if( m_Profiling == value )
					return;

				m_Profiling = value;

				if( m_ProfileStart > DateTime.MinValue )
					m_ProfileTime += DateTime.UtcNow - m_ProfileStart;

				m_ProfileStart = (m_Profiling ? DateTime.UtcNow : DateTime.MinValue);
			}
		}

		public static TimeSpan ProfileTime
		{
			get
			{
				if( m_ProfileStart > DateTime.MinValue )
					return m_ProfileTime + (DateTime.UtcNow - m_ProfileStart);

				return m_ProfileTime;
			}
		}

		public static bool Service => m_Service;
        public static bool Debug => m_Debug;
        public static bool IgnoreCache => m_IgnoreCache;
        internal static bool HaltOnWarning => m_HaltOnWarning;
        internal static bool VBdotNet => m_VBdotNET;
        public static List<string> DataDirectories => m_DataDirectories;
        public static Assembly Assembly { get => m_Assembly;
            set => m_Assembly = value;
        }
        public static Version Version => m_Assembly.GetName().Version;
        public static Process Process => m_Process;
        public static Thread Thread => m_Thread;
        public static MultiTextWriter MultiConsoleOut => m_MultiConOut;
        public static readonly Encoding ASCIIEncoding = Encoding.GetEncoding(1251);

#if Framework_4_0
		public static readonly bool Is64Bit = Environment.Is64BitProcess;
#else
        public static readonly bool Is64Bit = (IntPtr.Size == 8);	//Returns the size for the current /process/
#endif

		private static bool m_MultiProcessor;
		private static int m_ProcessorCount;

        public static bool MultiProcessor => m_MultiProcessor;
        public static int ProcessorCount => m_ProcessorCount;

        private static bool m_Unix;

        public static bool Unix => m_Unix;

        public static string FindDataFile( string path )
		{
			if( m_DataDirectories.Count == 0 )
				throw new InvalidOperationException( "Attempted to FindDataFile before DataDirectories list has been filled." );

			string fullPath = null;

			for( int i = 0; i < m_DataDirectories.Count; ++i )
			{
				fullPath = Path.Combine( m_DataDirectories[i], path );

				if( File.Exists( fullPath ) )
					break;

				fullPath = null;
			}
            ConsoleLog.Write.Information("Ultima Online directory:" + fullPath);
            return fullPath;
		}

		public static string FindDataFile( string format, params object[] args )
		{
			return FindDataFile( String.Format( format, args ) );
		}

		#region Expansions

		private static Expansion m_Expansion;
		public static Expansion Expansion
		{
			get => m_Expansion;
            set => m_Expansion = value;
        }

        public static bool T2A => m_Expansion >= Expansion.T2A;

        public static bool UOR => m_Expansion >= Expansion.UOR;

        public static bool UOTD => m_Expansion >= Expansion.UOTD;

        public static bool LBR => m_Expansion >= Expansion.LBR;

        public static bool AOS => m_Expansion >= Expansion.AOS;

        public static bool SE => m_Expansion >= Expansion.SE;

        public static bool ML => m_Expansion >= Expansion.ML;

        public static bool SA => m_Expansion >= Expansion.SA;

        #endregion

		public static string ExePath
		{
			get
			{
				if( m_ExePath == null )
				{
					m_ExePath = Assembly.Location;
					//m_ExePath = Process.GetCurrentProcess().MainModule.FileName;
				}

				return m_ExePath;
			}
		}

		public static string BaseDirectory
		{
			get
			{
				if( m_BaseDirectory == null )
				{
					try
					{
						m_BaseDirectory = ExePath;

						if( m_BaseDirectory.Length > 0 )
							m_BaseDirectory = Path.GetDirectoryName( m_BaseDirectory );
					}
					catch
					{
						m_BaseDirectory = "";
					}
				}

				return m_BaseDirectory;
			}
		}

		private static void CurrentDomain_UnhandledException( object sender, UnhandledExceptionEventArgs e )
		{
            ConsoleLog.Write.Warning(e.IsTerminating ? "Error:" : "Warning:", e );
			if( e.IsTerminating )
			{
				m_Crashed = true;

				bool close = false;

				try
				{
					CrashedEventArgs args = new CrashedEventArgs( e.ExceptionObject as Exception );

					EventSink.InvokeCrashed( args );

					close = args.Close;
				}
				catch
				{
				}

				if( !close && !m_Service )
				{
					try
					{
						for( int i = 0; i < m_MessagePump.Listeners.Length; i++ )
						{
							m_MessagePump.Listeners[i].Dispose();
						}
					}
					catch
					{
					}
                    
                    if (m_Service)
                    {
						ConsoleLog.Write.Error("This exception is fatal.");
                    }
                    else
                    {
	                    ConsoleLog.Write.Error("This exception is fatal, press return to exit");
                        Console.ReadLine();
                    }
				}

				m_Closing = true;
			}
		}

		private enum ConsoleEventType
		{
			CTRL_C_EVENT,
			CTRL_BREAK_EVENT,
			CTRL_CLOSE_EVENT,
			CTRL_LOGOFF_EVENT=5,
			CTRL_SHUTDOWN_EVENT
		}

		private delegate bool ConsoleEventHandler( ConsoleEventType type );
		private static ConsoleEventHandler m_ConsoleEventHandler;

		[DllImport( "Kernel32" )]
		private static extern bool SetConsoleCtrlHandler( ConsoleEventHandler callback, bool add );

		private static bool OnConsoleEvent( ConsoleEventType type )
		{
            if (World.Saving || (m_Service && type == ConsoleEventType.CTRL_LOGOFF_EVENT))
                return true;

            Kill();	//Kill -> HandleClosed will hadnle waiting for the completion of flushign to disk

			return true;
		}

		private static void CurrentDomain_ProcessExit( object sender, EventArgs e )
		{
			HandleClosed();
		}

		private static bool m_Closing;
		public static bool Closing => m_Closing;

        private static long m_CycleIndex = 1;
        private static float[] m_CyclesPerSecond = new float[100];

		public static float CyclesPerSecond => m_CyclesPerSecond[(m_CycleIndex - 1) % m_CyclesPerSecond.Length];

        public static float AverageCPS
		{
			get
			{
				float t = 0.0f;
				int c = 0;

				for( int i = 0; i < m_CycleIndex && i < m_CyclesPerSecond.Length; ++i )
				{
					t += m_CyclesPerSecond[i];
					++c;
				}

				return (t / Math.Max( c, 1 ));
			}
		}

		public static void Kill()
		{
			Kill( false );
		}

		public static void Kill( bool restart , bool update=false)
		{
			HandleClosed();

			if ( restart )
				Process.Start( ExePath, Arguments );
			if (update) 
            {
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = "git.exe",
                    Arguments = "pull --progress -v --no-rebase \"origin\"",
                    WorkingDirectory = Directory.GetParent(System.IO.Directory.GetCurrentDirectory())?.ToString(),
                };
                var process = System.Diagnostics.Process.Start(startInfo);
                string output = process.StandardOutput.ReadToEnd();
                ConsoleLog.Write.Information(output);
                process.WaitForExit();
                Environment.Exit((int)ExitCode.PullAndRebuild);
            }
            Environment.Exit((int)ExitCode.Success);
		}

		private static void HandleClosed()
		{
			if( m_Closing )
				return;

			m_Closing = true;

			ConsoleLog.Write.Information("Exiting..." );

            World.WaitForWriteCompletion();

			if( !m_Crashed )
				EventSink.InvokeShutdown( new ShutdownEventArgs() );

			Timer.TimerThread.Set();

			ConsoleLog.Write.Information("done" );
		}

		private static AutoResetEvent m_Signal = new AutoResetEvent( true );
		public static void Set() { m_Signal.Set(); }

		public static void Main( string[] args )
		{
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler( CurrentDomain_UnhandledException );
			AppDomain.CurrentDomain.ProcessExit += new EventHandler( CurrentDomain_ProcessExit );
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
			for ( int i = 0; i < args.Length; ++i )
			{
                if (Insensitive.Equals(args[i], "-debug"))
                    m_Debug = true;
                else if (Insensitive.Equals(args[i], "-service"))
                    m_Service = true;
                else if (Insensitive.Equals(args[i], "-profile"))
                    Profiling = true;
                else if (Insensitive.Equals(args[i], "-nocache"))
                    m_Cache = false;
                else if (Insensitive.Equals(args[i], "-nocompile"))
                    m_NoCompile = true;
                else if (Insensitive.Equals(args[i], "-haltonwarning"))
                    m_HaltOnWarning = true;
                else if (Insensitive.Equals(args[i], "-vb"))
                    m_VBdotNET = true;
                else if (Insensitive.Equals(args[i], "-ignorecache"))
                    m_IgnoreCache = true;
			}

			try
			{
				if (m_Service)
				{
					if (!Directory.Exists("Logs/Console"))
						Directory.CreateDirectory("Logs/Console");

					Console.SetOut(m_MultiConOut = new MultiTextWriter(new FileLogger("Logs/Console")));
				}
				else
				{
					if (!Directory.Exists("Logs/Console"))
						Directory.CreateDirectory("Logs/Console");

					Console.SetOut(m_MultiConOut = new MultiTextWriter(Console.Out));
                    //MultiConsoleOut.Add(new FileLogger("Logs/Console"));
				}
			}
			catch
			{
			}

			m_Thread = Thread.CurrentThread;
			m_Process = Process.GetCurrentProcess();
			m_Assembly = Assembly.GetEntryAssembly();

			if( m_Thread != null )
				m_Thread.Name = "Core Thread";

			if( BaseDirectory.Length > 0 )
				Directory.SetCurrentDirectory( BaseDirectory );

			Timer.TimerThread ttObj = new Timer.TimerThread();
			timerThread = new Thread( new ThreadStart( ttObj.TimerMain ) );
			timerThread.Name = "Timer Thread";

			Version ver = m_Assembly.GetName().Version;

            // Added to help future code support on forums, as a 'check' people can ask for to it see if they recompiled core or not
            ConsoleLog.Write.Information($"RunUO - [www.runuo.com] Version {ver.Major}.{ver.Minor}, Build {ver.Build}.{ver.Revision}" );
			ConsoleLog.Write.Information($"Core: Running on .NET Framework Version {Environment.Version.Major}.{Environment.Version.Minor}.{Environment.Version.Build}" );

			string s = Arguments;

			if( s.Length > 0 )
				ConsoleLog.Write.Information($"Core: Running with arguments: {s}" );

            m_ProcessorCount = Environment.ProcessorCount;

            if (m_ProcessorCount > 1)
                m_MultiProcessor = true;

            if (m_MultiProcessor || Is64Bit)
				ConsoleLog.Write.Information($"Core: Optimizing for {m_ProcessorCount} {(Is64Bit ? "64-bit " : "")}processor{(m_ProcessorCount == 1 ? "" : "s")}");

            int platform = (int)Environment.OSVersion.Platform;
            if (platform == 4 || platform == 128) { // MS 4, MONO 128
                m_Unix = true;
				ConsoleLog.Write.Information("Core: Unix environment detected");
            }
            else {
                m_ConsoleEventHandler = new ConsoleEventHandler(OnConsoleEvent);
                SetConsoleCtrlHandler(m_ConsoleEventHandler, true);
            }

            if (GCSettings.IsServerGC)
				ConsoleLog.Write.Information("Core: Server garbage collection mode enabled");

            if (!m_NoCompile || !ScriptCompiler.LoadPrecompiledAssembly())
            {
                while (!ScriptCompiler.Compile(m_Debug, m_Cache))
                {
					ConsoleLog.Write.Error("Scripts: One or more scripts failed to compile or no script files were found.");

                    if (m_Service)
                        return;

					ConsoleLog.Write.Information(" - Press return to exit, or R to try again.");

                    if (Console.ReadKey(true).Key != ConsoleKey.R)
                        return;
                }
            }

            ScriptCompiler.Invoke("Configure");

            Region.Load();
            World.Load();

            ScriptCompiler.Invoke("Initialize");

            MessagePump messagePump = new MessagePump();

			timerThread.Start();

            //ClilocTranslator.Start();

			for( int i = 0; i < Map.AllMaps.Count; ++i )
				Map.AllMaps[i].Tiles.Force();

			//NetState.Initialize();

			EventSink.InvokeServerStarted();

			try
			{
                DateTime now, last = DateTime.UtcNow;

				const int sampleInterval = 100;
				const float ticksPerSecond = (float)(TimeSpan.TicksPerSecond * sampleInterval);

				long sample = 0;

				while( m_Signal.WaitOne() )
				{
					Mobile.ProcessDeltaQueue();
					Item.ProcessDeltaQueue();

					Timer.Slice();
                    messagePump.Slice();

					NetState.FlushAll();
					NetState.ProcessDisposedQueue();

					if( Slice != null )
						Slice();

					if( (++sample % sampleInterval) == 0 )
					{
                        now = DateTime.UtcNow;
                        m_CyclesPerSecond[m_CycleIndex++ % m_CyclesPerSecond.Length] =
							ticksPerSecond / (now.Ticks - last.Ticks);
						last = now;
					}
				}
			}
			catch( Exception e )
			{
				CurrentDomain_UnhandledException( null, new UnhandledExceptionEventArgs( e, true ) );
			}
		}

		public static string Arguments
		{
			get
			{
				StringBuilder sb = new StringBuilder();

				if( Core.Debug )
					Utility.Separate( sb, "-debug", " " );

				if( Core.Service )
					Utility.Separate( sb, "-service", " " );

				if( Core.Profiling )
					Utility.Separate( sb, "-profile", " " );

				if( !m_Cache )
					Utility.Separate( sb, "-nocache", " " );

				if( m_HaltOnWarning )
					Utility.Separate( sb, "-haltonwarning", " " );

                if (m_VBdotNET)
                    Utility.Separate(sb, "-vb", " ");

                if (m_IgnoreCache)
                    Utility.Separate(sb, "-ignorecache", " ");

				return sb.ToString();
			}
		}

		private static int m_GlobalMaxUpdateRange = 24;

		public static int GlobalMaxUpdateRange
		{
			get => m_GlobalMaxUpdateRange;
            set => m_GlobalMaxUpdateRange = value;
        }

		private static int m_ItemCount, m_MobileCount, m_CustomsCount;

		public static int ScriptItems => m_ItemCount;
        public static int ScriptMobiles => m_MobileCount;
        public static int ScriptCustoms => m_CustomsCount;

        public static void VerifySerialization()
		{
			m_ItemCount = 0;
			m_MobileCount = 0;
            m_CustomsCount = 0;


            VerifySerialization( Assembly.GetCallingAssembly() );

            foreach (var a in ScriptCompiler.Assemblies)
            {
                VerifySerialization(a);
            }
        }

        private static readonly Type[] m_SerialTypeArray = new Type[1] { typeof(Serial) };

        private static void VerifyType(Type t)
        {
            var isItem = t.IsSubclassOf(typeof(Item));

            if (isItem || t.IsSubclassOf(typeof(Mobile)))
            {
                if (isItem)
                {
                    Interlocked.Increment(ref m_ItemCount);
                }
                else
                {
                    Interlocked.Increment(ref m_MobileCount);
                }

                StringBuilder warningSb = null;

                try
                {
                    if (t.GetConstructor(m_SerialTypeArray) == null)
                    {
                        warningSb = new StringBuilder();

                        warningSb.AppendLine("       - No serialization constructor");
                    }

                    if (
                        t.GetMethod(
                            "Serialize",
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly) == null)
                    {
                        if (warningSb == null)
                        {
                            warningSb = new StringBuilder();
                        }

                        warningSb.AppendLine("       - No Serialize() method");
                    }

                    if (
                        t.GetMethod(
                            "Deserialize",
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly) == null)
                    {
                        if (warningSb == null)
                        {
                            warningSb = new StringBuilder();
                        }

                        warningSb.AppendLine("       - No Deserialize() method");
                    }

                    if (warningSb != null && warningSb.Length > 0)
                    {
						ConsoleLog.Write.Warning($"{t}\n{warningSb}");
	                }
                }
                catch
                {
					ConsoleLog.Write.Information($"Exception in serialization verification of type {t}");
                }
            }
        }
        private static void VerifyTypeOld(Type t)
        {
            bool isItem = t.IsSubclassOf(typeof(Item));

            if (isItem || t.IsSubclassOf(typeof(Mobile)))
            {
                if (isItem)
                {
                    //++m_ItemCount;
                    Interlocked.Increment(ref m_ItemCount);
                }
                else
                {
                    //++m_MobileCount;
                    Interlocked.Increment(ref m_MobileCount);
                }

                StringBuilder warningSb = null;

                try
                {
                    /*
                    if( isItem && t.IsPublic && !t.IsAbstract )
                    {
                        ConstructorInfo cInfo = t.GetConstructor( Type.EmptyTypes );

                        if( cInfo == null )
                        {
                            if (warningSb == null)
                                warningSb = new StringBuilder();

                            warningSb.AppendLine("       - No zero paramater constructor");
                        }
                    }*/

                    if (t.GetConstructor(m_SerialTypeArray) == null)
                    {
                        if (warningSb == null)
                            warningSb = new StringBuilder();

                        warningSb.AppendLine("       - No serialization constructor");
                    }

                    if (t.GetMethod("Serialize", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly) == null)
                    {
                        if (warningSb == null)
                            warningSb = new StringBuilder();

                        warningSb.AppendLine("       - No Serialize() method");
                    }

                    if (t.GetMethod("Deserialize", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly) == null)
                    {
                        if (warningSb == null)
                            warningSb = new StringBuilder();

                        warningSb.AppendLine("       - No Deserialize() method");
                    }

                    if (warningSb != null && warningSb.Length > 0)
                    {
						ConsoleLog.Write.Warning($"{t}\n{warningSb.ToString()}");
                    }
                }
                catch
                {
					ConsoleLog.Write.Warning($"Exception in serialization verification of type {t}");
                }
            }
        }

        private static void VerifySerialization(Assembly a)
        {
            if (a != null)
            {
                Parallel.ForEach(a.GetTypes(), VerifyType);
            }
        }
    }

	public class FileLogger : TextWriter, IDisposable
	{
		private bool m_NewLine;
		public const string DateFormat = "[MMMM dd hh:mm:ss.f tt]: ";
		public const string FileNameFormat = "yyyy_MM_dd";

		public string FileName => Path.Combine(LogDir,$"{DateTime.UtcNow.ToString(FileNameFormat)}.log");
		public string LogDir { get; set; }

		public FileLogger( string dir, bool append=false )
		{
			LogDir = dir;
			using ( StreamWriter writer = new StreamWriter( new FileStream(FileName, append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.Read ) ) )
			{
				writer.WriteLine( $">>>Logging started on {DateTime.UtcNow:f}." ); //f = Tuesday, April 10, 2001 3:51 PM 
			}
			m_NewLine = true;
		}

		public override void Write( char ch )
		{
			using ( StreamWriter writer = new StreamWriter( new FileStream(FileName, FileMode.Append, FileAccess.Write, FileShare.Read ) ) )
			{
				if( m_NewLine )
				{
					writer.Write( DateTime.UtcNow.ToString( DateFormat ) );
					m_NewLine = false;
				}
				writer.Write( ch );
			}
		}

		public override void Write( string str )
		{
			using ( StreamWriter writer = new StreamWriter( new FileStream(FileName, FileMode.Append, FileAccess.Write, FileShare.Read ) ) )
			{
				if( m_NewLine )
				{
					writer.Write( DateTime.UtcNow.ToString( DateFormat ) );
					m_NewLine = false;
				}
				writer.Write( str );
			}
		}

		public override void WriteLine( string line )
		{
			using ( StreamWriter writer = new StreamWriter( new FileStream(FileName, FileMode.Append, FileAccess.Write, FileShare.Read ) ) )
			{
				if( m_NewLine )
					writer.Write( DateTime.UtcNow.ToString( DateFormat ) );
				writer.WriteLine( line );
				m_NewLine = true;
			}
		}

		public override System.Text.Encoding Encoding => System.Text.Encoding.Default;
    }

	public class MultiTextWriter : TextWriter
	{
		private List<TextWriter> m_Streams;
		private StringBuilder wline = new StringBuilder();

		public MultiTextWriter( params TextWriter[] streams )
		{
			m_Streams = new List<TextWriter>( streams );

			if( m_Streams.Count < 0 )
				throw new ArgumentException( "You must specify at least one stream." );
		}

		public void Add( TextWriter tw )
		{
			m_Streams.Add( tw );
		}

		public void Remove( TextWriter tw )
		{
			m_Streams.Remove( tw );
		}

		public override void Write( char ch )
		{
			for( int i = 0; i < m_Streams.Count; i++ )
				m_Streams[i].Write( ch );
			wline.Append(ch);
		}

		public override void WriteLine( string line )
		{
			for( int i = 0; i < m_Streams.Count; i++ )
				m_Streams[i].WriteLine( line );

			if (line.StartsWith("!"))
			{
				if (wline.Length > 0)
					wline.Clear();
				return;
			}

			if (wline.Length > 0)
			{
				wline.Append(line);
				BaseDiscord.Bot.SendMessageToDiscord(Server.BaseDiscord.Channel.Console, $"{wline.ToString()}");
				wline.Clear();
			}
			else
			{
				BaseDiscord.Bot.SendMessageToDiscord(Server.BaseDiscord.Channel.Console, $"{line}");
			}
		}

		public override void WriteLine( string line, params object[] args )
		{
			WriteLine( String.Format( line, args ) );
		}

		public override Encoding Encoding => Encoding.Default;
    }
}