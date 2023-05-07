using System;

namespace Server.Mobiles.AI.OpenAI
{
	internal class UOOpenAIUtility
	{
		public static void SendToConsole(string msg, ConsoleColor mainC, ConsoleColor SubC, bool arg)
		{
			msg = "Engine: UO OpenAI => " + msg;

			Console.ForegroundColor = arg ? mainC : SubC;

			Console.WriteLine(msg);

			Console.ResetColor();
		}
	}
}
