using System;

namespace Server.Mobiles.AI.OpenAI
{
	internal class UOOpenAICallBack : Timer
	{
		private readonly Mobile Talk;
		private readonly Mobile Listen;
		private readonly string Prof;
		private readonly string Prompt;

		public UOOpenAICallBack(Mobile talk, Mobile listen, string prof, string text) : base(TimeSpan.FromSeconds(3))
		{
			Talk = talk;
			Listen = listen;
			Prof = prof;
			Prompt = text;
		}

		protected override void OnTick()
		{
			Listen.Say(UOOpenAI.OnSpeechAI(Talk, Listen, Prof, Prompt, out _));

			Stop();
		}
	}
}
