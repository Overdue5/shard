using Server.Commands;
using System;

namespace Server.Mobiles.AI.OpenAI
{
	internal class LoadAI
	{
		public static void Initialize()
		{
			CommandSystem.Register("~LoadAI", AccessLevel.Administrator, LoadAI_OnCommand);
		}

		[Usage("~LoadAI")]
		[Description("Load UO OpenAI: QAStore to File..")]
		private static void LoadAI_OnCommand(CommandEventArgs e)
		{
			QAStore.LoadFromFile();

			e.Mobile.SendMessage("Imported AI Directory!");

			if (UOOpenAI.InDebugMode)
			{
				var msg = "Imported AI Directory!";

				UOOpenAIUtility.SendToConsole(msg, ConsoleColor.Green, ConsoleColor.Red, UOOpenAI.InDebugMode);
			}
		}
	}
}
