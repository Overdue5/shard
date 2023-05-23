using Server.Network;
using System.Collections;
using Scripts.SpecialSystems;
using Server.Items;

namespace Server.Gumps
{
    public class OverallPvXGump : Gump
    {
        private ArrayList m_List;
        private int m_ListPage;
        private ArrayList m_CountList;
        private PvXType m_SType;
        private int m_CountPlayer;

        public OverallPvXGump(Mobile from, int listPage, ArrayList list, ArrayList count, PvXType xtype) : base(50, 50)
        {
            var textGumpId = PvXScoreBoard.GetTextHueId(xtype);
            m_List = list;
            m_ListPage = listPage;
            m_CountList = count;
            m_SType = xtype;


            Closable = true;
            Disposable = false;
            Dragable = true;
            Resizable = false;
            AddPage(0);
            AddBackground(16, 12, 549, 303, 9300);
			AddBackground(16, 12, 549, 303, 5120);
            AddImageTiled(26, 21, 531, 26, 5154);
            AddAlphaRegion(26, 21, 531, 26);
            AddButton(28, 50, 4005, 4006, 1, GumpButtonType.Reply, 0);
            AddButton(202, 50, 4005, 4006, 2, GumpButtonType.Reply, 0);
            AddButton(364, 50, 4005, 4006, 3, GumpButtonType.Reply, 0);
            AddButton(28, 75, 4005, 4006, 4, GumpButtonType.Reply, 0);
            if (xtype == PvXType.PVP)
            {
                AddButton(202, 75, 4005, 4006, 5, GumpButtonType.Reply, 0);
            }
            AddButton(364, 75, 4005, 4006, 6, GumpButtonType.Reply, 0);
            AddImageTiled(20, 101, 543, 10, 5121);
            AddImageTiled(20, 280, 543, 10, 5121);
            AddButton(482, 285, 4017, 4018, 7, GumpButtonType.Reply, 0);
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
            if (m_List == null)
            {
                m_List = new ArrayList(PvXData.GetPvXData(xtype).Values);
                foreach (var pvXSystem in PvXData.GetPvXData(xtype))
                {
                    if (pvXSystem.Value.Owner.AccessLevel == AccessLevel.Player)
                        m_CountPlayer++;
                }
            }

            if (listPage > 0)
            {
                AddButton(26, 285, 4014, 4015, 9, GumpButtonType.Reply, 0);
                AddLabel(60, 286, textGumpId, @"Last Page");
            }

            if ((listPage + 1) * 6 < m_List.Count)
            {
                AddButton(146, 285, 4005, 4006, 10, GumpButtonType.Reply, 0);
                AddLabel(179, 286, textGumpId, @"Next Page");
            }
            AddLabel(61, 50, textGumpId, @"Overall Listing");
            AddLabel(236, 49, textGumpId, @"Most Wins");
            AddLabel(398, 49, textGumpId, @"Most Loses");
            AddLabel(61, 75, textGumpId, @"Most Times Res Killed");
            if (xtype == PvXType.PVP)
            {
                AddLabel(237, 75, textGumpId, @"Most Res Kills");
            }
            AddLabel(397, 75, textGumpId, @"Most Pure Wins");
            AddLabel(299, 286, textGumpId, @"Help Menu");
            AddLabel(517, 286, textGumpId, @"Close");
            string time = "Viewing " + m_CountPlayer + $" player {xtype.ToString().ToLower()} records.";
            AddLabel(30, 25, textGumpId, time.ToString());

            int k = 0;

            for (int i = 0, j = 0, index = ((listPage * 6) + k);
                i < 6 && index >= 0 && index < m_List.Count && j >= 0;
                ++i, ++j, ++index)
            {
                var stat = m_List[index] as PvXSystem;
                var mob = stat.Owner;

                if (mob != null)
                {
                    int offset = 148 + (i * 20);

                    if (mob.AccessLevel != AccessLevel.Player)
                    {
                        --i;
                    }
                    else
                    {
                        int pure = 0;
                        AddLabel(30, offset, textGumpId, mob.Name.ToString());
                        AddLabel(236, offset, textGumpId, stat.TotalWins.ToString());
                        AddLabel(285, offset, textGumpId, stat.TotalLoses.ToString());
                        AddLabel(383, offset, textGumpId, stat.TotalResKilled.ToString());
                        if (xtype == PvXType.PVP)
                        {
                            AddLabel(334, offset, textGumpId, stat.TotalResKills.ToString());
                            pure = stat.TotalPoints;
                        }
                        else
                        {
                            AddLabel(334, offset, textGumpId, "-");
                            pure = stat.TotalPoints;
                        }

                        AddLabel(433, offset, textGumpId, pure.ToString());
                        if (mob.Guild == null)
                            AddLabel(482, offset, textGumpId, @"No Guild");
                        else
                        {
                            AddLabel(482, offset, textGumpId, $@"[{(mob.Guild.Name.Length > 10 ?
                                mob.Guild.Name.Substring(0, 10) : mob.Guild.Name)}]");
                        }

                    }
                }
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
            if (info.ButtonID == 9) // Previous page
            {
                if (m_ListPage > 0)
                    from.SendGump(new OverallPvXGump(from, m_ListPage - 1, m_List, m_CountList, m_SType));
            }

            if (info.ButtonID == 10) // Next page
            {
                if ((m_ListPage + 1) * 6 < m_List.Count)
                    from.SendGump(new OverallPvXGump(from, m_ListPage + 1, m_List, m_CountList, m_SType));
            }
        }
    }
}