using Server;

namespace Knives.Chat3
{
    public delegate void ChatCommandHandler(CommandInfo info);

    public class CommandInfo
    {
        private Mobile c_Mobile;
        private string c_Command;
        private string c_ArgString;
        private string[] c_Arguments;

        public Mobile Mobile => c_Mobile;
        public string Command => c_Command;
        public string ArgString => c_ArgString;
        public string[] Arguments => c_Arguments;

        public CommandInfo(Mobile m, string com, string args, string[] arglist)
        {
            c_Mobile = m;
            c_Command = com;
            c_ArgString = args;
            c_Arguments = arglist;
        }

        public string GetString(int num)
        {
            if (c_Arguments.Length > num)
                return c_Arguments[num];

            return "";
        }
    }
}