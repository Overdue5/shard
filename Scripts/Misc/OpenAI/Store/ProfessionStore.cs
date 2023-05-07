using System.Collections.Generic;
using System;

namespace Server.Mobiles.AI.OpenAI
{
	public class ProfessionStore
	{
		public readonly string Profession;

		private Dictionary<string, List<string>> QADictStore;

		private const int MaxAnswers = 10;

		private readonly object StoreLock = new object();

		public ProfessionStore(string profession)
		{
			Profession = profession;

			QADictStore = new Dictionary<string, List<string>>();
		}

		public void AddQuestionAnswer(string question, string answer)
		{
			lock (StoreLock)
			{
				if (!QADictStore.ContainsKey(question))
				{
					QADictStore.Add(question, new List<string> { answer });
				}
				else
				{
					var answers = QADictStore[question];

					if (answers.Count < MaxAnswers && !answers.Contains(answer))
					{
						answers.Add(answer);
					}
				}
			}
		}

		public void RemoveQuestion(string question)
		{
			lock (StoreLock)
			{
				QADictStore.Remove(question);
			}
		}

		public void RemoveAnswer(string question, string answer)
		{
			lock (StoreLock)
			{
				var answers = QADictStore[question];

				answers.Remove(answer);

				if (answers.Count == 0)
				{
					QADictStore.Remove(question);
				}
			}
		}

		public void EditAnswer(string question, string oldAnswer, string newAnswer)
		{
			lock (StoreLock)
			{
				var answers = QADictStore[question];

				var index = answers.IndexOf(oldAnswer);

				if (index >= 0)
				{
					answers[index] = newAnswer;
				}
			}
		}

		public void Reset()
		{
			lock (StoreLock)
			{
				QADictStore.Clear();
			}
		}

		public Dictionary<string, List<string>> GetQuestionsAnswers()
		{
			lock (StoreLock)
			{
				return new Dictionary<string, List<string>>(QADictStore);
			}
		}

		public string FindAnswer(string question)
		{
			lock (StoreLock)
			{
				if (QADictStore.ContainsKey(question))
				{
					var answers = QADictStore[question];

					if (answers.Count == 1)
					{
						return answers[0];
					}
					else
					{
						var random = new Random();

						var index = random.Next(answers.Count);

						return answers[index];
					}
				}
				else
				{
					return null;
				}
			}
		}

		public bool HasQuestion(string question)
		{
			if (!QADictStore.ContainsKey(question)) return false;
			return QADictStore.ContainsKey(question);
		}

		public bool IsMaxAnswers(string question)
		{
			if (!QADictStore.ContainsKey(question)) return false;
			return QADictStore[question].Count >= MaxAnswers;
		}

		public void Serialize(GenericWriter writer)
		{
			writer.Write(Profession);

			writer.Write(QADictStore.Count);

			foreach (var kvp in QADictStore)
			{
				writer.Write(kvp.Key);

				writer.Write(kvp.Value.Count);

				foreach (var answer in kvp.Value)
				{
					writer.Write(answer);
				}
			}
		}

		public static ProfessionStore Deserialize(GenericReader reader)
		{
			var profession = reader.ReadString();

			var count = reader.ReadInt();

			var profQAStore = new ProfessionStore(profession);

			for (var i = 0; i < count; i++)
			{
				var key = reader.ReadString();

				var answerCount = reader.ReadInt();

				var answers = new List<string>();

				for (var j = 0; j < answerCount; j++)
				{
					answers.Add(reader.ReadString());
				}

				profQAStore.QADictStore.Add(key, answers);
			}

			return profQAStore;
		}
	}
}
