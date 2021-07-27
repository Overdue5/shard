using Server.Gumps;
using System;
using Server;
using Server.Commands;
using Server.Commands.Generic;
using Server.Mobiles;
using Server.Network;

namespace Scripts.SpecialSystems.PvX
{
    public class PvXCommand
    {
        public static void Initialize()
        {
            //CommandSystem.Register("PvXAdmin", AccessLevel.Developer, new CommandEventHandler(PvXAdmin_Command));
            TargetCommands.Register(new PvXEditCommand());
        }

        public static void PvXAdmin_Command(CommandEventArgs e)
        {
            e.Mobile.SendGump(new PvXGumpList());
        }
    }

    public class PvXEditCommand : BaseCommand
    {
        public PvXEditCommand()
        {
            AccessLevel = AccessLevel.GameMaster;
            Supports = CommandSupport.Simple;
            Commands = new[] { "PvXEdit" };
            ObjectTypes = ObjectTypes.Mobiles;
            Usage = "PvXEdit";
            Description = "Modify PvX stats";
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            if (obj is PlayerMobile pl)
            {
                e.Mobile.SendGump(new PvXGumpEdit(pl, PvXType.PVM));
                e.Mobile.SendGump(new PvXGumpEdit(pl, PvXType.PVP));
            }
        }
    }

    public class PvXGumpList : Gump
    {
        public PvXGumpList() : base(100, 100)
        {
        }
    }

    public class PvXGumpEdit : Gump
    {
        private PvXSystem m_stat;

        public PvXGumpEdit(PlayerMobile mobile, PvXType stype) : base(100, 100)
        {
            m_stat = stype == PvXType.PVM ? PvXData.GetPvMStat(mobile) : PvXData.GetPvPStat(mobile);
            Dragable = true;
            Closable = true;
            Resizable = false;
            Disposable = false;
            AddPage(0);
            AddBackground(100, 100, 400, 300, 9200);

            AddLabel(120, 120, 0, @"TotalWin:");
            AddTextEntry(250, 120, 107, 20, 0, 0, m_stat.TotalWins.ToString());

            AddLabel(120, 140, 0, @"TotalLose:");
            AddTextEntry(250, 140, 107, 20, 0, 1, m_stat.TotalLoses.ToString());

            AddLabel(120, 160, 0, $"TotalResKilled:");
            AddTextEntry(250, 160, 107, 20, 0, 2, m_stat.TotalResKilled.ToString());

            AddLabel(120, 180, 0, $"TotalResKills:");
            AddTextEntry(250, 180, 107, 20, 0, 3, m_stat.TotalResKills.ToString());

            AddLabel(120, 200, 0, $"PvXType:{m_stat.PvXType.ToString()}");
            AddLabel(120, 220, 0, $"Owner:{m_stat.Owner.Name}");
            AddLabel(120, 240, 0, $"TotalPoints:{m_stat.TotalPoints.ToString()}");
            AddLabel(120, 260, 0, $"RankName:{m_stat.RankName}");

            AddButton(370, 360, 247, 248, 100, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 0: // Closed or Cancel
                {
                    return;
                }
                case 100:
                {
                    m_stat.TotalWins = Utility.LimitMinMax(0, Convert.ToInt32(info.TextEntries[0].Text), 100000);
                    m_stat.TotalLoses = Utility.LimitMinMax(0, Convert.ToInt32(info.TextEntries[1].Text), 100000);
                    m_stat.TotalResKilled = Utility.LimitMinMax(0, Convert.ToInt32(info.TextEntries[2].Text), 100000);
                    m_stat.TotalResKills = Utility.LimitMinMax(0, Convert.ToInt32(info.TextEntries[3].Text), 100000);
                    m_stat.LastChangeTime = DateTime.UtcNow;
                    return;
                }
            }

            base.OnResponse(sender, info);
        }
    }
}
