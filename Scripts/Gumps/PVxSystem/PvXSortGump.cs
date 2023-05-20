using Server.Network;
using Scripts.SpecialSystems;
using Server.Items;

namespace Server.Gumps
{
    public class PvXSortGump : Gump
    {
        private readonly PvXType m_SType;
        
        public static string GetNameForCounter(PvXCounterType counter)
        {
            switch (counter)
            {
               case PvXCounterType.MaxTotalWins: return "View top players with most overall wins.";
               case PvXCounterType.MaxTotalLoses: return "View top players with most overall loses.";
               case PvXCounterType.MaxTotalPoints: return "View top players with most overall pure wins.";
               case PvXCounterType.TotalResKilled: return "View top players with most overall res kills.";
               case PvXCounterType.TotalResKills: return "View top players with most overall res killed.";
               default:
               {
                   Utility.ConsoleWriteLine(Utility.ConsoleMsgType.Error, "Unknown PvXCounterType in PvXSortGump");
                   return "Unknown pvx counter";
               }
            }
        }

        public PvXSortGump(PvXType stype, PvXCounterType counter) : base(50, 50)
        {
            m_SType = stype;
            var textGumpId = PvXScoreBoard.GetTextHueId(stype);

            Closable = true;
            Disposable = false;
            Dragable = true;
            Resizable = false;
            AddPage(0);
            AddBackground(16, 12, 549, 303, 9300);
            AddImageTiled(26, 21, 531, 26, 5154);
            AddAlphaRegion(26, 21, 531, 26);
            AddButton(28, 50, 4005, 4006, 1, GumpButtonType.Reply, 0);
            AddButton(202, 50, 4005, 4006, 2, GumpButtonType.Reply, 0);
            AddButton(364, 50, 4005, 4006, 3, GumpButtonType.Reply, 0);
            AddButton(28, 75, 4005, 4006, 4, GumpButtonType.Reply, 0);
            if (stype == PvXType.PVP)
            {
                AddButton(202, 75, 4005, 4006, 5, GumpButtonType.Reply, 0);
                AddLabel(237, 75, textGumpId, @"Most Res Kills");
            }
            AddButton(364, 75, 4005, 4006, 6, GumpButtonType.Reply, 0);
            AddImageTiled(20, 101, 543, 10, 5121);
            AddImageTiled(20, 280, 543, 10, 5121);
            AddButton(482, 285, 4017, 4018, 7, GumpButtonType.Reply, 0);
            AddLabel(517, 286, textGumpId, @"Close");
            AddAlphaRegion(26, 110, 199, 23);
            AddAlphaRegion(232, 110, 42, 23);
            AddAlphaRegion(281, 110, 42, 23);
            AddAlphaRegion(330, 110, 42, 23);
            AddAlphaRegion(379, 110, 42, 23);
            AddAlphaRegion(428, 110, 42, 23);
            AddAlphaRegion(477, 110, 81, 23);
            AddLabel(30, 111, textGumpId, @"Name");
            AddLabel(236, 111, textGumpId, @"Wins");
            AddLabel(285, 111, textGumpId, @"Loses");
            AddLabel(334, 111, textGumpId, @"TRK");
            AddLabel(383, 111, textGumpId, @"TTRK");
            AddLabel(433, 111, textGumpId, @"Pure");
            AddLabel(482, 111, textGumpId, @"Guild Abbr");
            AddAlphaRegion(26, 140, 199, 134);
            AddAlphaRegion(232, 140, 42, 134);
            AddAlphaRegion(281, 140, 42, 134);
            AddAlphaRegion(330, 140, 42, 134);
            AddAlphaRegion(379, 140, 42, 134);
            AddAlphaRegion(428, 140, 42, 134);
            AddAlphaRegion(477, 140, 81, 134);
            AddButton(264, 285, 4026, 4006, 8, GumpButtonType.Reply, 0);
            AddLabel(299, 286, textGumpId, @"Help Menu");
            AddLabel(61, 50, textGumpId, @"Overall Listing");
            AddLabel(236, 49, textGumpId, @"Most Wins");
            AddLabel(398, 49, textGumpId, @"Most Loses");
            AddLabel(61, 75, textGumpId, @"Most Times Res Killed");
            AddLabel(397, 75, textGumpId, @"Most Pure Wins");

            string time = GetNameForCounter(counter);
            AddLabel(30, 25, textGumpId, time.ToString());

            int step = 0;
            
            foreach (var stat in PvXData.PvXStatistics[m_SType].StatTypesDict[counter])
            {
                AddLabel(30, 148 + step, textGumpId, stat.Owner.Name.ToString());
                AddLabel(236, 148 + step, textGumpId, stat.TotalWins.ToString());
                AddLabel(285, 148 + step, textGumpId, stat.TotalLoses.ToString());
                AddLabel(383, 148 + step, textGumpId, stat.TotalResKilled.ToString());
                if (stype == PvXType.PVP)
                {
                    AddLabel(334, 148 + step, textGumpId, stat.TotalResKills.ToString());
                }
                else
                {
                    AddLabel(334, 148 + step, textGumpId, "-");
                }
                AddLabel(433, 148 + step, textGumpId, stat.TotalPoints.ToString());

                if (stat.Owner.Guild == null)
                    AddLabel(482, 148 + step, textGumpId, @"No Guild");
                else
                {
                    AddLabel(482, 148 + step, textGumpId, $@"[{(stat.Owner.Guild.Name.Length > 10 ?
                        stat.Owner.Guild.Name.Substring(0, 10) : stat.Owner.Guild.Name)}]");
                }

                step += 20;
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            if (from == null)
                return;

            if (info.ButtonID == 1) // Overall
            {
                from.SendGump(new OverallPvXGump(from, 0, null, null, m_SType));
            }

            if (info.ButtonID == 2) // Most wins
            {
                from.SendGump(new PvXSortGump(m_SType, PvXCounterType.MaxTotalWins));
            }

            if (info.ButtonID == 3) // Most loses
            {
                from.SendGump(new PvXSortGump(m_SType, PvXCounterType.MaxTotalLoses));
            }

            if (info.ButtonID == 4) // Most times res killed
            {
                from.SendGump(new PvXSortGump(m_SType, PvXCounterType.TotalResKilled));
            }

            if (info.ButtonID == 5) // Most reskills
            {
                from.SendGump(new PvXSortGump(m_SType, PvXCounterType.TotalResKills));
            }

            if (info.ButtonID == 6) // Most pure wins
            {
                from.SendGump(new PvXSortGump(m_SType, PvXCounterType.MaxTotalPoints));
            }

            if (info.ButtonID == 7) // Close
            {
            }

            if (info.ButtonID == 8) // Help menu
            {
                from.SendGump(new PvXScoreBoardHelpGump(m_SType));
            }
        }
    }
}