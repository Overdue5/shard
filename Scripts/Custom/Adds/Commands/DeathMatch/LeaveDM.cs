using Server.Commands;
using Server.Custom.PvpToolkit;

namespace Server.Scripts.Custom.Adds.Commands.DeathMatch
{
    public class LeaveDM
    {
        public static void Initialize()
        {
            CommandSystem.Register("LeaveDM", AccessLevel.Counselor, Execute);
            CommandSystem.Register("LDM", AccessLevel.Counselor, Execute);
        }

        [Usage("LeaveDM")]
        [Description("Exits the death match, without the chance for a prize.")]
        private static void Execute(CommandEventArgs e)
        {
            PvpCore.TryLeaveDM(e.Mobile, false);
        }
    }
}