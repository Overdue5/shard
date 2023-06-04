using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Server.Commands;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom.Zed
{
    public static class BtLogs
    {
        public static void Write(string text)
        {
            Logs.QuestLog.WriteLine($"BlackthornGT: {text}");
        }
    }

    public class BtStatistic
    {
        public static Dictionary<Mobile, BtStatistic> BtStatistics = new Dictionary<Mobile, BtStatistic>();
        public int Wins;
        public int Attempts;
        public int Kills;
        public int PointsSpend;
        public long TotalGold;

        public BtStatistic()
        {
            Wins = 0;
            Attempts = 0;
            Kills = 0;
            PointsSpend = 0;
            TotalGold = 0;
        }

        private static bool IsNotPlayer(Mobile mob)
        {
            return !(mob is PlayerMobile);
        }

        public static void AddGold(Mobile mob, int amount)
        {
            if (IsNotPlayer(mob)) return;
            if (!BtStatistics.ContainsKey(mob))
                BtStatistics[mob] = new BtStatistic();
            BtStatistics[mob].TotalGold += amount;
        }

        public static void AddKill(Mobile mob, int amount = 1)
        {
            if (IsNotPlayer(mob)) return;
            if (!BtStatistics.ContainsKey(mob))
                BtStatistics[mob] = new BtStatistic();
            BtStatistics[mob].Kills += amount;
        }

        public static void AddAttempt(Mobile mob)
        {
            if (IsNotPlayer(mob)) return;
            if (!BtStatistics.ContainsKey(mob))
                BtStatistics[mob] = new BtStatistic();
            BtStatistics[mob].Attempts += 1;
        }

        public static void AddAttempt(List<Mobile> mobs)
        {
            foreach (var mob in mobs)
                AddAttempt(mob);
        }

        public static void AddSpendPoints(Mobile mob, int amount)
        {
            if (IsNotPlayer(mob)) return;
            if (!BtStatistics.ContainsKey(mob))
                BtStatistics[mob] = new BtStatistic();
            BtStatistics[mob].PointsSpend += amount;
        }

        public static void AddWin(Mobile mob)
        {
            if (IsNotPlayer(mob)) return;
            if (!BtStatistics.ContainsKey(mob))
                BtStatistics[mob] = new BtStatistic();
            BtStatistics[mob].Wins += 1;
        }

        public static void AddWin(List<Mobile> mobs)
        {
            foreach (var mob in mobs)
                AddWin(mob);
        }
    }

    public static class BtceSettings
    {
        public static int BroadCastHue = 34;
        
        public static Point3D RetreatLocation = new Point3D(1523, 1462, 15);
#if DEBUG
        public static TimeSpan AnnounceTimer = TimeSpan.FromSeconds(15);
        public static TimeSpan EventDuration = TimeSpan.FromMinutes(5);
        public static TimeSpan WaveAnnouncementTimer = TimeSpan.FromSeconds(15);
        public static TimeSpan WaveDuaration = TimeSpan.FromSeconds(60);
#else
        public static TimeSpan AnnounceTimer = TimeSpan.FromMinutes(15);
        public static TimeSpan EventDuration = TimeSpan.FromMinutes(30);
        public static TimeSpan WaveAnnouncementTimer = TimeSpan.FromSeconds(30);
        public static TimeSpan WaveDuaration = TimeSpan.FromMinutes(7);
#endif
        //public static TimeSpan WaveDuaration = TimeSpan.FromMinutes(5);

        [CommandProperty(AccessLevel.GameMaster)]
        public static int NumberGuardsInWave { get; set; } = 15;

        public static int GuardFindDistance = 10;
        public static int GuardSayHelpDistance = 6;
        public static int GoldPerGuard = 500;
        public static int MaxGoldPerPlayer = 30000;
        public static int MinGoldPerPLayer = 200;
        public static int ValorBonus = 5;
        public static readonly string StatisticFileName = "BTEStatistics.json";
        public class GuardsWaves
        {
            public int WaveNumber;
            public double Difficult;
            public int GuardsCount;
            public TimeSpan WaveDuration;
            
            public GuardsWaves(int waveNumber, double difficult, int guardsCount, TimeSpan waveDuration)
            {
                WaveNumber = waveNumber;
                Difficult = difficult;
                GuardsCount = guardsCount;
                WaveDuration = waveDuration;
            }
        }
        
        public static List<GuardsWaves> Waves = new List<GuardsWaves>()
        {
            new GuardsWaves(1, 1, NumberGuardsInWave, WaveDuaration),
            new GuardsWaves(2, 1.5, NumberGuardsInWave, WaveDuaration),
            new GuardsWaves(3, 2, NumberGuardsInWave, WaveDuaration)
        };
        public static List<Point3D> SpawnLocation = new List<Point3D>()
        {
            new Point3D(1525, 1423, 15), new Point3D(1527, 1425, 15), new Point3D(1527, 1427, 15), new Point3D(1527, 1429, 15),
            new Point3D(1527, 1431, 15), new Point3D(1523, 1425, 15), new Point3D(1523, 1427, 15), new Point3D(1523, 1429, 15),
            new Point3D(1523, 1431, 15), new Point3D(1518, 1426, 15), new Point3D(1516, 1429, 15), new Point3D(1534, 1417, 15),
            new Point3D(1534, 1427, 15), new Point3D(1534, 1435, 15), new Point3D(1512, 1445, 15), new Point3D(1523, 1445, 15),
            new Point3D(1531, 1445, 15), new Point3D(1531, 1436, 15), new Point3D(1531, 1426, 15), new Point3D(1525, 1419, 15),
            new Point3D(1525, 1418, 35), new Point3D(1532, 1419, 35), new Point3D(1534, 1419, 35), new Point3D(1535, 1417, 35),
            new Point3D(1535, 1415, 35), new Point3D(1525, 1418, 35), new Point3D(1533, 1413, 35), new Point3D(1531, 1413, 35),
            new Point3D(1530, 1415, 35), new Point3D(1530, 1417, 35), new Point3D(1536, 1424, 35), new Point3D(1536, 1430, 35),
            new Point3D(1536, 1435, 35), new Point3D(1523, 1435, 35), new Point3D(1523, 1430, 35), new Point3D(1523, 1424, 35),
            new Point3D(1522, 1414, 56), new Point3D(1523, 1423, 56), new Point3D(1523, 1427, 56), new Point3D(1523, 1432, 56),
            new Point3D(1526, 1414, 56), new Point3D(1526, 1423, 56), new Point3D(1526, 1427, 56), new Point3D(1526, 1432, 56),
            new Point3D(1530, 1414, 56), new Point3D(1530, 1423, 56), new Point3D(1530, 1427, 56), new Point3D(1530, 1432, 56),
            new Point3D(1536, 1414, 56), new Point3D(1536, 1423, 56), new Point3D(1536, 1427, 56), new Point3D(1536, 1432, 56),
        };

    }

    public partial class BtceKeyStone : Item
    {
        public BtceKeyStone() : base(0x2FD4)
        {
            Movable = false;
            Visible = false;
        }

        public BtceKeyStone(Serial serial) : base(serial)
        {
        }


      

        public static void Initialize()
        {
            CommandSystem.Register("BTCStart", AccessLevel.Developer, BTCStart_OnCommand);
            CommandSystem.Register("BTCKill", AccessLevel.Developer, BTCKill_OnCommand);
            EventSink.WorldSave += SaveStat;
            LoadStat();
        }

        private static void LoadStat()
        {
            string path = Path.Combine(World.CustomDataPath, BtceSettings.StatisticFileName);
            if (File.Exists(path))
            {
                string readText = File.ReadAllText(path);
                var temp = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, BtStatistic>>(readText);
                foreach (string tempKey in temp.Keys)
                {
                    var serial = Convert.ToInt32(tempKey.Split(' ')[0], 16);
                    BtStatistic.BtStatistics[World.FindMobile(serial)] = temp[tempKey];
                }
            }

        }

        private static void SaveStat(WorldSaveEventArgs e)
        {
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(BtStatistic.BtStatistics);
            string path = Path.Combine(World.CustomDataPath, BtceSettings.StatisticFileName);
            File.WriteAllText(path, jsonString);
        }

        [Usage("BTCStart")]
        [Description("Start event")]
        private static void BTCStart_OnCommand(CommandEventArgs e)
        {
            var timer = BtceTimer.GetTimer();
            timer.Start();
        }

        [Usage("BTCKill")]
        [Description("Start event")]
        private static void BTCKill_OnCommand(CommandEventArgs e)
        {
            foreach (var guard in BtceTimer.Guards.ToArray())
                guard.Delete();
            BtceTimer.Guards.Clear();
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendGump(new BtceInfoGump());
        }

        public static bool PlayerInEvent(Mobile mob)
        {
            return BtceTimer.PlayerExist(mob);
        }

        private class BtceInfoGump : Gump
        {
            public BtceInfoGump() : base(100, 100)
            {
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                base.OnResponse(sender, info);
            }

        }

       
    }
    
}
