using System.Text;

namespace Server.Mobiles.AI.OpenAI
{
	static internal class UOOpenAIPrompt
	{
		public static string GetSetupPrompt(string prof)
		{
			var sb = new StringBuilder();

			sb.Append("Thou art now to act the role of a ");

			if (prof != "Player")
			{
				sb.Append(RandomMood() + " " + prof + " NPC");
			}
			else
			{
				sb.Append(RandomMood() + " " + RandomPlayer() + " Player");
			}

			sb.Append(" in the fantastical realm of Ultima Online. ");

			sb.Append("Speak thy response in a single, grandiose sentence, if thou wouldst oblige: ");

			return sb.ToString();
		}

		public static string RandomReply()
		{
			switch (Utility.RandomMinMax(0, 13))
			{
				case 0: return "I must needs ponder upon this.";
				case 1: return "I am uncertain, give me leave to contemplate.";
				case 2: return "I require a moment to consider.";
				case 3: return "I shall have to give this due contemplation.";
				case 4: return "I am unsure, permit me a moment to think.";
				case 5: return "I must needs think on this further.";
				case 6: return "I must weigh my options.";
				case 7: return "I am unsure, let me reflect.";
				case 8: return "I must consider my choices.";
				case 9: return "I must needs meditate on this.";
				case 10: return "I am not certain, let me think.";
				case 11: return "I must needs give this consideration.";
				case 12: return "I must weigh the pros and cons.";
				default: return "I must think on this!";
			}
		}

		private static string RandomPlayer()
		{
			switch (Utility.RandomMinMax(0, 13))
			{
				case 0: return "New";
				case 1: return "Average";
				case 2: return "Experienced";
				case 3: return "Noob";
				case 4: return "Ganker";
				case 5: return "Pro";
				case 6: return "Expert";
				case 7: return "Rookie";
				case 8: return "Veteran";
				case 9: return "Master";
				case 10: return "Elite";
				case 11: return "Beginner";
				case 12: return "Adept";
				default: return "Skilled";
			}
		}

		private static string RandomMood()
		{
			switch (Utility.RandomMinMax(0, 13))
			{
				case 0: return "Sad";
				case 1: return "Mad";
				case 2: return "Glad";
				case 3: return "Joyful";
				case 4: return "Miserable";
				case 5: return "Proud";
				case 6: return "Embarrassed";
				case 7: return "Shy";
				case 8: return "Bold";
				case 9: return "Confident";
				case 10: return "Surprised";
				case 11: return "Excited";
				case 12: return "Calm";
				default: return "Happy";
			}
		}
	}
}
