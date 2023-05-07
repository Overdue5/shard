using Server.Commands;
using System;

namespace Server.Mobiles.AI.OpenAI
{
	internal class ResetAI
	{
		public static void Initialize()
		{
			CommandSystem.Register("~ResetAI", AccessLevel.Administrator, ResetAI_OnCommand);
		}

		[Usage("~ResetAI")]
		[Description("Reset UO OpenAI: QAStore to File..")]
		private static void ResetAI_OnCommand(CommandEventArgs e)
		{
			QAStore.ResetAll();

			e.Mobile.SendMessage("Reset AI Directory!");

			if (UOOpenAI.InDebugMode)
			{
				var msg = "Reset AI Directory!";

				UOOpenAIUtility.SendToConsole(msg, ConsoleColor.Green, ConsoleColor.Red, UOOpenAI.InDebugMode);
			}
		}
	}
}
