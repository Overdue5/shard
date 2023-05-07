namespace Server.Mobiles.AI.OpenAI
{
	static internal class UOOpenAISettings
	{
		public static readonly string APIKey = "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"; // Your_API_Key_Here -> https://beta.openai.com

		// AI Settings
		/*
		 * The settings in these fields can be adjusted to control the behavior of the 
		 * language generation model and the characteristics of its output. For example, 
		 * increasing the temperature or decreasing the TopP value can increase the 
		 * diversity of the model's output, while decreasing the temperature or increasing 
		 * the TopP value can make the output more predictable and similar to the input text. 
		 * 
		 * Adjusting the presence and frequency penalties can also influence the similarity 
		 * of the output to the input text, as well as the diversity of the output.
		 */

		public static readonly int InMaxTokens = 1024;
		// This is an integer that specifies the maximum number of tokens (i.e., words
		// or word pieces) that the model should generate in its output. The model will
		// stop generating text once it reaches the maximum number of tokens, even if
		// it has not reached the end of a sentence or paragraph.

		public static readonly double InTemp = 0.5;
		// This is a double-precision floating point number that specifies the
		// temperature of the model's output. The temperature determines the randomness
		// of the model's output. A higher temperature will result in more random
		// and diverse output, while a lower temperature will result in output that
		// is more predictable and similar to the input text. The minimum and maximum
		// values for the temperature depend on the implementation of the model, but
		// values between 0 and 1 are common.

		public static readonly double InTopP = 0.3;
		// This is a double-precision floating point number that specifies the
		// proportion of the model's output that should be taken from the most likely
		// tokens (i.e., the tokens with the highest probability). For example, if
		// TopP is set to 0.1, the model will only consider the top 10% of the most
		// likely tokens when generating its output. This can be used to control the
		// diversity and predictability of the model's output. The minimum and maximum
		// values for TopP depend on the implementation of the model, but values
		// between 0 and 1 are common.

		public static readonly int InNumOutputs = 1;
		// This is an integer that specifies the number of output sequences that
		// the model should generate. For example, if NumOutputs is set to 1, the
		// model will only generate a single output sequence. If NumOutputs is set
		// to 2, the model will generate two output sequences, and so on.

		public static readonly double InPresencePenelty = 0.1;
		// This is a double-precision floating point number that specifies a penalty
		// that is applied to tokens that are not present in the input text. The
		// purpose of this penalty is to encourage the model to generate output
		// that is more similar to the input text. The minimum and maximum values
		// for the presence penalty depend on the implementation of the model, but
		// values between 0 and 1 are common.

		public static readonly double InFreqencyPenelty = 0.1;
		// This is a double-precision floating point number that specifies a
		// penalty that is applied to tokens that are more frequent in the input
		// text. The purpose of this penalty is to encourage the model to generate
		// output that is more diverse and less predictable. The minimum and maximum
		// values for the frequency penalty depend on the implementation of the
		// model, but values between 0 and 1 are common.

		public static readonly int InLogProbs = 0;
		// This is an integer that specifies whether the model should output the log
		// probabilities of the generated tokens. If LogProbs is set to 0, the model
		// will not output the log probabilities. If LogProbs is set to 1, the model
		// will output the log probabilities of the generated tokens.

		public static readonly bool InEcho = false;
		// This is a boolean value that specifies whether the model should repeat the
		// input text in its output. If Echo is set to true, the model will repeat the
		// input text in its output. If Echo is set to false, the model will not repeat
		// the input text in its output.

		public static readonly string[] InStopSequences = new string[] { "." };
		// This is an array of strings that specifies sequences of tokens that should
		// cause the model to stop generating text. If the model generates any of the
		// specified stop sequences, it will stop generating text even if it has not
		// reached the maximum number of tokens or the end of a sentence or paragraph.

		// This can be used to control the length and structure of the model's output.

		//The value of the StopSequences field will depend on the specific use case
		//and the desired behavior of the language generation model. A common value
		//for StopSequences might be an array containing sequences of tokens that
		//signal the end of a paragraph or document, such as ["\n", "."] or ["\n\n"].
		//This would cause the model to stop generating text when it generates a
		//newline character or a period, which are often used to mark the end of
		//a sentence or paragraph.

		//Alternatively, StopSequences could be set to an array containing sequences
		//of tokens that indicate the end of a specific type of content or information,
		//such as ["Thank you for your question."] or["The end."]. This would cause the
		//model to stop generating text when it generates one of these sequences, which
		//could be useful if the model is being used to generate responses to specific
		//types of prompts or questions.

		//It's important to note that the StopSequences field is optional, and it may
		//not be necessary to use stop sequences in some cases. If the StopSequences
		//field is not specified, the model will continue generating text until it
		//reaches the maximum number of tokens or the end of a sentence or paragraph.
	}
}
