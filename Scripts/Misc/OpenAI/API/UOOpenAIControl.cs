using System;
using System.Linq;

namespace Server.Mobiles.AI.OpenAI
{
	public static class UOOpenAIControl
	{
		public static bool AdminControl(PlayerMobile admin, string inputText)
		{
			var inputTXT = inputText.ToLower();

			if (inputTXT.StartsWith("~add") && inputTXT.Contains(':'))
			{
				var AddQAValues = inputText.Split(':');

				if (AddQAValues.Count() == 4)
				{
					QAStore.StoreQuestionAnswer(AddQAValues[1], AddQAValues[2], AddQAValues[3]);

					admin.SendMessage("Added : " + inputText.Substring(5));
				}
				else
				{
					admin.SendMessage("Error : ~Add:profession:question:answer");
				}

				return true;
			}

			if (inputTXT.StartsWith("~edit") && inputTXT.Contains(':'))
			{
				var AddQAValues = inputText.Split(':');

				if (AddQAValues.Count() == 5)
				{
					QAStore.EditAnswer(AddQAValues[1], AddQAValues[2], AddQAValues[3], AddQAValues[4]);

					admin.SendMessage("Edited : " + inputText.Substring(6));
				}
				else
				{
					admin.SendMessage("Error : ~Edit:profession:question:old_answer:new_answer");
				}

				return true;
			}

			if (inputTXT.StartsWith("~removeq") && inputTXT.Contains(':'))
			{
				var AddQAValues = inputText.Split(':');

				if (AddQAValues.Count() == 3)
				{
					QAStore.RemoveQuestion(AddQAValues[1], AddQAValues[2]);

					admin.SendMessage("Removed : " + inputText.Substring(9));
				}
				else
				{
					admin.SendMessage("Error : ~RemoveQ:profession:question");
				}

				return true;
			}

			if (inputTXT.StartsWith("~removea") && inputTXT.Contains(':'))
			{
				var AddQAValues = inputText.Split(':');

				if (AddQAValues.Count() == 4)
				{
					QAStore.RemoveAnswer(AddQAValues[1], AddQAValues[2], AddQAValues[3]);

					admin.SendMessage("Removed : " + inputText.Substring(9));
				}
				else
				{
					admin.SendMessage("Error : ~RemoveA:profession:question:answer");
				}

				return true;
			}

			if (inputTXT.StartsWith("~reset") && inputTXT.Contains(':'))
			{
				var AddQAValues = inputText.Split(':');

				if (AddQAValues.Count() == 2)
				{
					QAStore.Reset(AddQAValues[1]);

					admin.SendMessage("Reset : " + inputText.Substring(7));

					if (UOOpenAI.InDebugMode)
					{
						var msg = "Reset Profession : " + AddQAValues[1].ToString();

						UOOpenAIUtility.SendToConsole(msg, ConsoleColor.DarkCyan, ConsoleColor.Red, true);
					}
				}
				else
				{
					admin.SendMessage("Error : ~Reset:profession");
				}

				return true;
			}

			return false;
		}
	}
}
