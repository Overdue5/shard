using System;
using System.IO;

namespace Server.Diagnostics
{
    public class ExceptionLogging
	{
		#region Private Fields

		private static StreamWriter _Output;

		#endregion Private Fields

		#region Public Constructors

		static ExceptionLogging()
		{
			var directory = Path.Combine(Core.BaseDirectory, "Logs/Exceptions");

			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);

			LogDirectory = directory;
		}

		#endregion Public Constructors

		#region Public Properties

		public static string LogDirectory { get; set; }

		public static StreamWriter Output
		{
			get
			{
				if (_Output == null)
				{
					_Output = new StreamWriter(Path.Combine(LogDirectory, $"{DateTime.UtcNow.ToLongDateString()}.log"), true)
					{
						AutoFlush = true
					};

					_Output.WriteLine("##############################");
					_Output.WriteLine("Exception log started on {0}", DateTime.UtcNow);
					_Output.WriteLine();
				}

				return _Output;
			}
		}

		#endregion Public Properties

		#region Public Methods

		public static void LogException(Exception e)
		{
			Utility.ConsoleWriteLine(Utility.ConsoleMsgType.Error, $"Caught Exception, ex:{e.ToString()}");
			Output.WriteLine("Exception Caught: {0}", DateTime.UtcNow);
			Output.WriteLine(e);
			Output.WriteLine();
		}

		public static void LogException(Exception e, string arg)
		{
			Utility.ConsoleWriteLine(Utility.ConsoleMsgType.Error, $"Caught Exception, args:{arg} ex:{e.ToString()}");

			Output.WriteLine("Exception Caught: {0}", DateTime.UtcNow);
			Output.WriteLine(e);
			Output.WriteLine();
		}

		#endregion Public Methods
	}
}