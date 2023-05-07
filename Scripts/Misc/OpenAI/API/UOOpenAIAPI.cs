using System;
using OpenAI_API; // https://github.com/JoePanes/OpenAI-API-dotnet

namespace Server.Mobiles.AI.OpenAI
{
	public static class UOOpenAIAPI
	{
		//Version
		public static readonly string Version = "1.0.0.011423";

		// Your API key
		private static readonly string apiKey = UOOpenAISettings.APIKey;

		// Override Lib and get Davinci-003
		private static readonly Engine UpdateEngine = GetEngine();

		// OkGoDolt Lib API
		public static readonly OpenAIAPI OkGoDoltAPI_Davinci = new OpenAIAPI(apiKey, UpdateEngine);

		static UOOpenAIAPI()
		{
			var msg = "Version = " + UOOpenAIAPI.Version;

			UOOpenAIUtility.SendToConsole(msg, ConsoleColor.Magenta, ConsoleColor.Red, true);
		}

		private static Engine GetEngine()
		{
			var engine = Engine.Davinci;

			engine.EngineName = "text-davinci-003";

			return engine;
		}
	}
}
