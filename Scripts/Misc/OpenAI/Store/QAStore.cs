using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Server.Mobiles.AI.OpenAI
{
	public static class QAStore
	{
		static internal Dictionary<string, ProfessionStore> QADictionary;

		private static readonly string FilePath = Path.Combine("Saves", "UOOpenAI.bin");

		private static readonly object QAStoreLock = new object();

		static QAStore()
		{
			QADictionary = new Dictionary<string, ProfessionStore>();
		}

		public static void Configure()
		{
			EventSink.WorldSave += OnSave;

			EventSink.WorldLoad += OnLoad;

			if (UOOpenAI.InDebugMode)
			{
				var msg = "[ QA Directory Created ]";

				UOOpenAIUtility.SendToConsole(msg, ConsoleColor.DarkCyan, ConsoleColor.DarkRed, UOOpenAI.UsingAI);
			}
		}

		public static void OnSave(WorldSaveEventArgs e)
		{
			Persistence.Serialize(FilePath, OnSerialize);
		}

		public static void OnLoad()
		{
			Persistence.Deserialize(FilePath, OnDeserialize);
		}

		public static int QuestionTotal()
		{
			lock (QAStoreLock)
			{
				return QADictionary.Values.Sum(x => x.GetQuestionsAnswers().Count);
			}
		}

		public static int AnswerTotal()
		{
			var count = 0;

			foreach (var kvp in QADictionary)
			{
				var profession = kvp.Key;

				var qaDict = kvp.Value.GetQuestionsAnswers();

				foreach (var qaKvp in qaDict)
				{
					var answers = qaKvp.Value;

					count += answers.Count;
				}
			}
			return count;
		}

		public static void StoreQuestionAnswer(string profession, string question, string answer)
		{
			lock (QAStoreLock)
			{
				if (!QADictionary.ContainsKey(profession))
				{
					QADictionary.Add(profession, new ProfessionStore(profession));
				}

				QADictionary[profession].AddQuestionAnswer(question, answer);
			}
		}

		public static void RemoveQuestion(string profession, string question)
		{
			lock (QAStoreLock)
			{
				if (QADictionary.ContainsKey(profession))
				{
					QADictionary[profession].RemoveQuestion(question);
				}
			}
		}

		public static void RemoveAnswer(string profession, string question, string answer)
		{
			lock (QAStoreLock)
			{
				if (QADictionary.ContainsKey(profession))
				{
					QADictionary[profession].RemoveAnswer(question, answer);
				}
			}
		}

		public static void EditAnswer(string profession, string question, string oldAnswer, string newAnswer)
		{
			lock (QAStoreLock)
			{
				if (QADictionary.ContainsKey(profession))
				{
					QADictionary[profession].EditAnswer(question, oldAnswer, newAnswer);
				}
			}
		}

		public static Dictionary<string, List<string>> GetQuestionsAnswers(string profession)
		{
			lock (QAStoreLock)
			{
				if (QADictionary.ContainsKey(profession))
				{
					return QADictionary[profession].GetQuestionsAnswers();
				}
				else
				{
					return new Dictionary<string, List<string>>();
				}
			}
		}

		public static string FindAnswer(string profession, string question)
		{
			lock (QAStoreLock)
			{
				if (QADictionary.ContainsKey(profession))
				{
					return QADictionary[profession].FindAnswer(question);
				}
				else
				{
					return null;
				}
			}
		}

		public static bool HasQuestion(string profession, string question)
		{
			lock (QAStoreLock)
			{
				if (QADictionary.ContainsKey(profession))
				{
					return QADictionary[profession].HasQuestion(question);
				}
				else
				{
					return false;
				}
			}
		}

		public static bool IsMaxAnswers(string profession, string question)
		{
			lock (QAStoreLock)
			{
				if (QADictionary.ContainsKey(profession))
				{
					return QADictionary[profession].IsMaxAnswers(question);
				}
				else
				{
					return false;
				}
			}
		}

		public static void Reset(string profession)
		{
			lock (QAStoreLock)
			{
				if (QADictionary.ContainsKey(profession))
				{
					QADictionary[profession].Reset();
				}
			}
		}

		public static bool ResetAll()
		{
			lock (QAStoreLock)
			{
				QADictionary.Clear();
			}

			return true;
		}

		public static void SaveToFile()
		{
			lock (QAStoreLock)
			{
				QAFileUtility.SaveToFile();
			}
		}
		public static void LoadFromFile()
		{
			lock (QAStoreLock)
			{
				QADictionary = new Dictionary<string, ProfessionStore>();

				QAFileUtility.LoadFromFile();
			}
		}

		private static void OnSerialize(GenericWriter writer)
		{
			writer.Write(0);

			lock (QAStoreLock)
			{
				writer.Write(QADictionary.Count);

				foreach (var kvp in QADictionary)
				{
					kvp.Value.Serialize(writer);
				}
			}
		}

		private static void OnDeserialize(GenericReader reader)
		{
			reader.ReadInt();

			lock (QAStoreLock)
			{
				var count = reader.ReadInt();

				for (var i = 0; i < count; i++)
				{
					var profQAStore = ProfessionStore.Deserialize(reader);

					QADictionary.Add(profQAStore.Profession, profQAStore);
				}
			}

			var question = "Question Total ( " + QuestionTotal() + " )";

			var answer = "Answer Total ( " + AnswerTotal() + " )";

			var msg = "Using [ " + question + " / " + answer + " ]";

			UOOpenAIUtility.SendToConsole(msg, ConsoleColor.DarkCyan, ConsoleColor.DarkRed, UOOpenAI.UsingAI);
		}
	}
}
