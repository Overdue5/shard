using System.Collections.Generic;
using System.IO;

namespace Server.Mobiles.AI.OpenAI
{
	public static class QAFileUtility
	{
		public static void SaveToFile()
		{
			foreach (var kvp in QAStore.QADictionary)
			{
				var profession = kvp.Key;

				if (!Directory.Exists(Path.Combine("Data", "UOOpenAI")))
				{
					Directory.CreateDirectory(Path.Combine("Data", "UOOpenAI"));
				}

				var professionFilePath = Path.Combine("Data", "UOOpenAI", profession + ".txt");

				using (var writer = new StreamWriter(professionFilePath))
				{
					var qaDict = kvp.Value.GetQuestionsAnswers();

					foreach (var qaKvp in qaDict)
					{
						var question = qaKvp.Key;

						var answers = qaKvp.Value;

						foreach (var answer in answers)
						{
							writer.WriteLine("P: " + profession + " Q: " + question + " A: " + answer);
						}
					}
				}
			}
		}

		public static void LoadFromFile()
		{
			var result = new Dictionary<string, ProfessionStore>();

			var filePaths = Directory.GetFiles(@"Data\UOOpenAI", "*.txt");

			foreach (var filePath in filePaths)
			{
				using (var reader = new StreamReader(filePath))
				{
					string line;

					while ((line = reader.ReadLine()) != null)
					{
						var parts = line.Split(':');

						if (parts.Length < 4)
						{
							continue;
						}

						var profession = parts[1].TrimEnd('Q').Trim();

						var question = parts[2].TrimEnd('A').Trim();

						var answer = parts[3].Trim();

						if (!result.ContainsKey(profession))
						{
							result.Add(profession, new ProfessionStore(profession));
						}

						result[profession].AddQuestionAnswer(question, answer);
					}
				}
			}

			QAStore.QADictionary = result;
		}
	}
}
