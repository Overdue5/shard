using System;
using Server.Commands;

namespace Server.Mobiles.AI.OpenAI
{
	internal class DebugAI
	{
		public static void Initialize()
		{
			CommandSystem.Register("DebugAI", AccessLevel.Administrator, DebugAI_OnCommand);
		}

		[Usage("DebugAI")]
		[Description("Debug UO OpenAI: Toggle On/Off.")]
		private static void DebugAI_OnCommand(CommandEventArgs e)
		{
			var debug = !UOOpenAI.InDebugMode;

			UOOpenAI.InDebugMode = debug;

			e.Mobile.SendMessage(debug ? 62 : 42, $"{e.Mobile.Name}, AI Debug On => {debug}");

			if (UOOpenAI.InDebugMode)
			{
				var msg = "Debug " + (UOOpenAI.InDebugMode ? "Started ..." : "Ended!");

				UOOpenAIUtility.SendToConsole(msg, ConsoleColor.Green, ConsoleColor.Red, UOOpenAI.InDebugMode);
			}
		}
	}
}
