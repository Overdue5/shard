using Server.Items;
using Server.Mobiles.AI.OpenAI;

namespace Server.Mobiles
{
    public class GwennoBard : BaseCreature
	{
		[Constructable]
        public GwennoBard() : base(AIType.AI_Melee, FightMode.None, 10, 1, 0.2, 0.4)
		{
			Name = "Gwenno";

			Title = "the Bard";

			Body = 0x190;

			InitStats(31, 41, 51);

            SpeechHue = Utility.RandomBrightHue();

            Hue = Utility.RandomSkinHue();

			SetWearable(new LongPants(), Utility.RandomBrightHue(), 1);

			SetWearable(new FancyShirt(), Utility.RandomBrightHue(), 1);

			SetWearable(new Boots(), Utility.RandomBrightHue(), 1);

            Utility.AssignRandomHair(this);

            Container pack = new Backpack();

            pack.DropItem(new Gold(250, 300));

            SetWearable(pack);
        }

        public GwennoBard(Serial serial) : base(serial)
        {
		}

		// UO OpenAI Start : This can go into BaseCreature as well to use this on all npcs!
		public bool AIActive = true;

		public override bool HandlesOnSpeech(Mobile from)
		{
			if (AIActive && from.InRange(Location, 5))
			{
				Direction = GetDirectionTo(from.Location);

				return true;
			}
			else
			{
				return base.HandlesOnSpeech(from);
			}
		}

		private Timer NewPromptCB;

		public override void OnSpeech(SpeechEventArgs e)
		{
			if (NewPromptCB != null && NewPromptCB.Running)
			{
				Say("...");

				return;
			}

			if (AIActive && e.Mobile is PlayerMobile pm)
			{
				var profession = "Player"; // Default Profession : Player

				if (Title != null && Title.StartsWith("the"))
				{
					profession = Title.ToString().Substring(4);
				}

				if (e.Speech.ToLower().StartsWith("what") && e.Speech.ToLower().EndsWith("name"))
				{
					Say("My name is " + Name + "!");
				}
				else if (e.Speech.Contains("play my song") || e.Speech.Contains("RESETAI"))
				{
					Say("Ok, Linking Castles, Enjoy!");

					PlaySong(pm);
				}
				else
				{
					Say(UOOpenAI.OnSpeechAI(pm, this, profession, e.Speech, out var newPrompt));

					if (newPrompt)
					{
						NewPromptCB = new UOOpenAICallBack(pm, this, profession, e.Speech);

						NewPromptCB.Start();
					}
				}

				base.OnSpeech(e);
			}
			else
			{
				base.OnSpeech(e);
			}
		}

		// Not needed for ai
		private void PlaySong(Mobile from)
		{
			//from.LaunchBrowser("https://youtu.be/BQ4tAbwi31I");
		}
		// UO OpenAI End : Besides the Serialize/Deserialize edits

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version 

			writer.Write(AIActive); // Save AI Toggle
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

			_ = reader.ReadInt(); // version

			AIActive = reader.ReadBool(); // Load AI Toggle
		}
    }
}
