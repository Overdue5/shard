using Server;

namespace Knives.TownHouses
{
    public delegate void TownHouseCommandHandler(CommandInfo info);

    public class CommandInfo
    {
        private readonly Mobile c_Mobile;
        private readonly string c_Command;
        private readonly string c_ArgString;
        private readonly string[] c_Arguments;

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