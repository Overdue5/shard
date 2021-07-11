using Scripts.SpecialSystems;
using Server.Network;

namespace Server.Gumps
{
    public class PvXScoreBoardHelpGump : Gump
    {
        private PvXType m_SType;
        public PvXScoreBoardHelpGump(PvXType stype) : base(50, 50)
        {
            m_SType = stype;

            Closable = false;
            Disposable = false;
            Dragable = true;
            Resizable = false;
            AddPage(0);
            AddBackground(28, 19, 325, 321, 5120);
            AddImageTiled(39, 27, 305, 265, 5174);
            AddImageTiled(31, 298, 320, 10, 5121);
            AddAlphaRegion(39, 27, 305, 265);
            AddButton(38, 305, 4005, 4006, 1, GumpButtonType.Reply, 0);
            AddLabel(74, 305, 1149, @"Return to score board");
            AddButton(268, 305, 4017, 4018, 2, GumpButtonType.Reply, 0);
            AddLabel(302, 305, 1149, @"Close");
            var boardType = stype.ToString();
            AddHtml(39, 27, 305, 265,
                $"<CENTER><U>{boardType} Score Board Help Menu</U><BR><BR></CENTER><U>Overall Listing</U>" +
                $"<BR>This feature will display all players on the server and their {boardType.ToLower()} record.<BR>" +
                "<BR><U>Most Wins</U><BR>This feature will display the servers top three players with the most wins.<BR>" +
                "<BR><U>Most Loses</U><BR>This feature will display the servers top three players with the most loses.<BR>" +
                "<BR><U>Most Times Res Killed</U>" +
                "<BR>This feature will display the servers top three players with the most times res killed.<BR>" +
                "<BR><U>Most Res Kills</U><BR>This feature will display the servers top three players with the most res kills.<BR>" +
                "<BR><U>Most Pure Wins</U><BR>This feature will display the servers top three players with the most pure wins.<BR>" +
                "<BR><U>Key Legend</U><BR><U>Name</U>Displays the players name.<BR>" +
                "<BR><U>Wins</U><BR>Displays the players total wins.<BR>" +
                "<BR><U>Loses</U><BR>Displays the players total loses.<BR>" +
                "<BR><U>TRK</U><BR>Displays the players total res kills<BR>" +
                "<BR><U>TTRK</U><BR>Displays the players total times res killed.<BR>" +
                "<BR><U>Pure</U><BR>Displays the players pure wins.<BR>" +
                "<BR><U>Guild Abbr</U><BR>Displays the players guild abbreviation.",
                (bool) false, (bool) true);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            if (from == null)
                return;

            if (info.ButtonID == 1) // Return
            {
                from.SendGump(new OverallPvXGump(from, 0, null, null, m_SType));
            }

            if (info.ButtonID == 2) // Close
            {
            }
        }
	}
}