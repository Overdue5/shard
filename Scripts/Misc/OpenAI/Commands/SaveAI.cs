using Server.Commands;
using System;

namespace Server.Mobiles.AI.OpenAI
{
	internal class SaveAI
	{
		public static void Initialize()
		{
			CommandSystem.Register("~SaveAI", AccessLevel.Administrator, SaveAI_OnCommand);
		}

		[Usage("~SaveAI")]
		[Description("Save UO OpenAI: QAStore to File..")]
		private static void SaveAI_OnCommand(CommandEventArgs e)
		{
			QAStore.SaveToFile();

			e.Mobile.SendMessage("Exported AI Directory!");

			if (UOOpenAI.InDebugMode)
			{
				var msg = "Exported AI Directory!";

				UOOpenAIUtility.SendToConsole(msg, ConsoleColor.Green, ConsoleColor.Red, UOOpenAI.InDebugMode);
			}
		}
	}
}
