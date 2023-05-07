using Server.Commands;
using System;

namespace Server.Mobiles.AI.OpenAI
{
	internal class AIActivation
	{
		public static void Initialize()
		{
			CommandSystem.Register("~AI", AccessLevel.Administrator, AI_OnCommand);
		}

		[Usage("~AI")]
		[Description("UO OpenAI: Toggle On/Off.")]
		private static void AI_OnCommand(CommandEventArgs e)
		{
			var ai = !UOOpenAI.UsingAI;

			UOOpenAI.UsingAI = ai;

			e.Mobile.SendMessage(ai ? 62 : 42, $"{e.Mobile.Name}, AI On => {ai}");

			if (UOOpenAI.InDebugMode)
			{
				var msg = "AI " + (UOOpenAI.UsingAI ? "Started ..." : "Ended!");

				UOOpenAIUtility.SendToConsole(msg, ConsoleColor.Green, ConsoleColor.Red, UOOpenAI.InDebugMode);
			}
		}
	}
}
