using Server;

namespace Knives.Chat3
{
    public class Public : Channel
    {
        public Public() : base("Public")
        {
            Commands.Add("chat");
            Commands.Add("c");
            NewChars = true;
            DiscordChannel = BaseDiscord.Channel.GlobalChat;
            Register(this);
        }
    }
}