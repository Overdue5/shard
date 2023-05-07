using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenAI_API; // https://github.com/JoePanes/OpenAI-API-dotnet
using System.Text;

namespace Server.Mobiles.AI.OpenAI
{
	public static class UOOpenAI
	{
		// OkGoDolt Lib API
		private static readonly OpenAIAPI API_Davinci = UOOpenAIAPI.OkGoDoltAPI_Davinci;

		public static bool UsingAI { get; set; } = true;

		public static bool InDebugMode { get; set; } = false;

		static UOOpenAI()
		{
			Console.ForegroundColor = UOOpenAI.UsingAI ? ConsoleColor.DarkCyan : ConsoleColor.DarkRed;

			var IsRunning = UOOpenAI.UsingAI ? "Running..." : "Stopped!";

			var msg = IsRunning;

			UOOpenAIUtility.SendToConsole(msg, ConsoleColor.DarkCyan, ConsoleColor.DarkRed, UOOpenAI.UsingAI);
		}

		// This is auto filled by the Players Text In Game!
		// This is a string that represents the input text that the model will use as a
		// starting point for generating text. The model will try to generate text that
		// is coherent and similar to the prompt.
		private static string InPrompt;

		private static readonly int InMaxTokens = UOOpenAISettings.InMaxTokens;

		private static readonly double InTemp = UOOpenAISettings.InTemp;

		private static readonly double InTopP = UOOpenAISettings.InTopP;

		private static readonly int InNumOutputs = UOOpenAISettings.InNumOutputs;

		private static readonly double InPresencePenelty = UOOpenAISettings.InPresencePenelty;

		private static readonly double InFreqencyPenelty = UOOpenAISettings.InFreqencyPenelty;

		private static readonly int InLogProbs = UOOpenAISettings.InLogProbs;

		private static readonly bool InEcho = UOOpenAISettings.InEcho;

		private static readonly string[] InStopSequences = UOOpenAISettings.InStopSequences;

		// Custom OnSpeech Method
		public static string OnSpeechAI(Mobile talker, Mobile listener, string prof, string inputText, out bool newPrompt)
		{
			if (talker is PlayerMobile pm)
			{
				if (prof == null) 
					prof = String.Empty;

				if (pm.AccessLevel >= AccessLevel.Administrator)
				{
					var IsCommand = UOOpenAIControl.AdminControl(pm, inputText);

					if (IsCommand)
					{
						newPrompt = false;

						return "I have done your bidding, Master " + pm.Name;
					}
				}

				var StoreQuestion = inputText.EndsWith("?");

				var FormattedTxt = inputText.ToLower().TrimEnd('?');

				if (QAStore.IsMaxAnswers(prof, FormattedTxt))
				{
					newPrompt = false;

					return QAStore.FindAnswer(prof, FormattedTxt);
				}
				else
				{
					if (IsQuestion(FormattedTxt, StoreQuestion))
						InPrompt = UOOpenAIPrompt.GetSetupPrompt(prof) + FormattedTxt + "?\n\n"; // Rebuild prompt
					else
						InPrompt = UOOpenAIPrompt.GetSetupPrompt(prof) + FormattedTxt + ".\n\n"; // Rebuild prompt

					try
					{
						var Request = new CompletionRequest(
											InPrompt,
											InMaxTokens,
											InTemp,
											InTopP,
											InNumOutputs,
											InPresencePenelty,
											InFreqencyPenelty,
											InLogProbs,
											InEcho,
											InStopSequences
											);

						if (UsingAI)
						{
							// Using Davinci-003 : Send Request Async
							_ = SendApiRequestAsync(Request, prof, FormattedTxt);
						}

						if (QAStore.HasQuestion(prof, FormattedTxt))
						{
							newPrompt = false;

							return QAStore.FindAnswer(prof, FormattedTxt);
						}
						else
						{
							newPrompt = true;

							return UOOpenAIPrompt.RandomReply();
						}
					}
					catch
					{
						newPrompt = false;

						return "I lost my mind, call a dev!";
					}
				}
			}

			newPrompt = false;

			return String.Empty;
		}

		private static bool IsQuestion(string inputTxt, bool bypass)
		{
			if (bypass)
				return bypass;

			var questionList = new List<string>() { "what", "where", "why", "how", "when", "who"};

			foreach (var question in questionList)
			{
				if (inputTxt.Contains(question))
					return true;
			}

			return false;
		}

		private static readonly object SimpleLock = new object();

		private static async Task SendApiRequestAsync(CompletionRequest Request, string prof, string OrgAsk)
		{
			var sb = new StringBuilder();

			Task<CompletionResult> completionTask;

			lock (SimpleLock)
			{
				completionTask = API_Davinci.Completions.CreateCompletionAsync( new CompletionRequest(Request));
			}

			var result = await completionTask;

			if (result.Completions.Count > 0)
			{
				for (var i = 0; i < result.Completions.Count; i++)
				{
					if (result.Completions[i].Text.Length > 2)
					{
						sb.Append(result.Completions[i].ToString());

						break;
					}
				}
			}

			var CleanedReply = CleanupResponseText(sb.ToString());

			if (InDebugMode)
			{
				var msg = "Reply = " + CleanedReply;

				UOOpenAIUtility.SendToConsole(msg, ConsoleColor.Cyan, ConsoleColor.Red, true);
			}

			var FormatReply = CleanedReply.ToLower().TrimEnd('.');

			if (FormatReply != OrgAsk)
			{
				if (CleanedReply != "")
				{
					QAStore.StoreQuestionAnswer(prof, OrgAsk, CleanedReply);
				}
			}
		}

		private static string CleanupResponseText(string responseText)
		{
			var lines = responseText.Split(new[] { "\n" }, StringSplitOptions.None);

			foreach (var line in lines)
			{
				if (line.Contains(":"))
				{
					var parts = line.Split(new[] { ":" }, 2, StringSplitOptions.None);

					if (parts[1].Any(c => Char.IsLetterOrDigit(c)))
					{
						responseText = parts[1].TrimStart(' ');
						break;
					}
				}
				else if (!String.IsNullOrEmpty(line))
				{
					if (line.Any(c => Char.IsLetterOrDigit(c)))
					{
						responseText = line.TrimStart(' ');
						break;
					}
				}
			}

			responseText = new string(responseText.Where(c => Char.IsLetterOrDigit(c) || Char.IsWhiteSpace(c)).ToArray());

			if (!responseText.EndsWith(".") && responseText.Length > 0)
			{
				responseText += "!";
			}

			if (responseText.Length < 3)
			{
				responseText = "";
			}

			return responseText;
		}
	}
}
