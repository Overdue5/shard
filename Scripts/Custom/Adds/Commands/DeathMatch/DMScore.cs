using Server.Commands;
using Server.Custom.PvpToolkit;

namespace Server.Scripts.Custom.Adds.Commands.DeathMatch
{
    public class DMScore
    {
        public static void Initialize()
        {
            CommandSystem.Register("DMScore", AccessLevel.Counselor, Execute);
            CommandSystem.Register("DMS", AccessLevel.Counselor, Execute);
        }

        [Usage("DMScore")]
        [Description("Shows the score of the death match that you are in.")]
        private static void Execute(CommandEventArgs e)
        {
            PvpCore.TryShowScoreBoard(e.Mobile);
        }
    }
}