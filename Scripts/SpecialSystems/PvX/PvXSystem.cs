using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Server;
using Server.Mobiles;

namespace Scripts.SpecialSystems
{
    /// <summary>
    /// All types of PvX
    /// </summary>
    public enum PvXType
    {
        PVP = 0,
        PVM = 1
    }

    /// <summary>
    /// All counters for statistics 
    /// </summary>
    public enum PvXCounterType
    {
        MaxTotalWins = 0,
        MaxTotalPoints = 1,
        MaxTotalLoses,
        TotalResKills,
        TotalResKilled
    }

    public class PvXStatistic
    {
        #region Fields

        public List<PvXSystem> MaxTotalWins;
        public List<PvXSystem> MaxTotalPoints;
        public List<PvXSystem> MaxTotalLoses;
        public List<PvXSystem> TotalResKills;
        public List<PvXSystem> TotalResKilled;

        #endregion

        /// <summary>
        /// Dictionary 
        /// </summary>
        public Dictionary<PvXCounterType, List<PvXSystem>> StatTypesDict;

        public PvXStatistic()
        {
            MaxTotalWins = new List<PvXSystem>();
            MaxTotalPoints = new List<PvXSystem>();
            MaxTotalLoses = new List<PvXSystem>();
            TotalResKills = new List<PvXSystem>();
            TotalResKilled = new List<PvXSystem>();
            StatTypesDict = new Dictionary<PvXCounterType, List<PvXSystem>>();
        }
    }

    public class PvXSystem
    {
        #region Fields

        private PlayerMobile m_Owner;
        private int m_TotalWins;
        private int m_TotalLoses;
        private int m_TotalResKills;
        private int m_TotalResKilled;
        private Hashtable m_LastKiller;
        private Hashtable m_LastKilled;
        private DateTime m_ResKillTime;
        private DateTime m_LastChangeTime;
        private int m_TotalPointsLost;
        private int m_TotalPointsSpent;
        private string m_RankName;
        private PvXType m_type;

        #endregion

        #region Properties

        public PlayerMobile Owner
        {
            get { return m_Owner; }
            set { m_Owner = value; }
        }

        /// <summary>
        /// Calculated value, TotalWins - TotalLoses - TotalResKills
        /// </summary>
        [CommandProperty(AccessLevel.GameMaster)]
        public int TotalPoints
        {
            get { return m_TotalWins - m_TotalLoses - m_TotalResKills; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TotalWins
        {
            get { return m_TotalWins; }
            set { m_TotalWins = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TotalLoses
        {
            get { return m_TotalLoses; }
            set { m_TotalLoses = value; }
        }

        /// <summary>
        /// Killed several times in a row one character 
        /// </summary>
        [CommandProperty(AccessLevel.GameMaster)]
        public int TotalResKills
        {
            get { return m_TotalResKills; }
            set { m_TotalResKills = value; }
        }

        /// <summary>
        /// Was killed in a row by one character 
        /// </summary>
        [CommandProperty(AccessLevel.GameMaster)]
        public int TotalResKilled
        {
            get { return m_TotalResKilled; }
            set { m_TotalResKilled = value; }
        }

        /// <summary>
        /// The last killer 
        /// </summary>
        [CommandProperty(AccessLevel.GameMaster)]
        public Hashtable LastKiller
        {
            get { return m_LastKiller; }
            set { m_LastKiller = value; }
        }

        /// <summary>
        /// The last killed
        /// </summary>
        [CommandProperty(AccessLevel.GameMaster)]
        public Hashtable LastKilled
        {
            get { return m_LastKilled; }
            set { m_LastKilled = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime ResKillTime
        {
            get { return m_ResKillTime; }
            set { m_ResKillTime = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastChangeTime
        {
            get { return m_LastChangeTime; }
            set { m_LastChangeTime = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TotalPointsLost
        {
            get { return m_TotalPointsLost; }
            set { m_TotalPointsLost = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TotalPointsSpent
        {
            get { return m_TotalPointsSpent; }
            set { m_TotalPointsSpent = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string RankName
        {
            get { return m_RankName; }
            set { m_RankName = value; }
        }

        public PvXType PvXType
        {
            get { return m_type; }
            set { m_type = value; }
        }

        #endregion

        #region Constructors
        public PvXSystem()
        {
        }
        public PvXSystem(PvXType _type)
        {
            m_type = _type;
            m_LastChangeTime = DateTime.UtcNow;
        }

        public PvXSystem(PvXType _type, PlayerMobile owner) : this(_type)
        {
            m_Owner = owner;
        }

        #endregion
    }

    public class PvXData
    {
        private const int MaxCountCalculate = 5;
        public static string FilePath = Path.Combine("Saves/Special", "PvXSystem.bin");

        /// <summary>
        /// Full info about all mobiles
        /// </summary>
        private static Dictionary<PvXType, SortedDictionary<int, PvXSystem>> PvXDataDict;

        /// <summary>
        /// Full statistic info for PvXBoards
        /// </summary>
        public static Dictionary<PvXType, PvXStatistic> PvXStatistics = new Dictionary<PvXType, PvXStatistic>();

        public static SortedDictionary<int, PvXSystem> GetPvXData(PvXType xtype)
        {
            return PvXDataDict[xtype];
        }

        public static void Configure()
        {
            EventSink.WorldSave += OnSave;
            EventSink.WorldLoad += OnLoad;
        }

        public static PvXSystem GetPvPStat(PlayerMobile m)
        {
            return GetStat(PvXType.PVP, m);
        }

        public static PvXSystem GetPvMStat(PlayerMobile m)
        {
            return GetStat(PvXType.PVM, m);
        }

        private static PvXSystem GetStat(PvXType xtype, PlayerMobile m)
        {
            if (PvXDataDict[xtype].ContainsKey(m.Serial.Value))
                return PvXDataDict[xtype][m.Serial.Value];
            PvXDataDict[xtype][m.Serial.Value] = new PvXSystem(xtype, m);
            return PvXDataDict[xtype][m.Serial.Value];
        }

        static PvXData()
        {
            PvXDataDict = new Dictionary<PvXType, SortedDictionary<int, PvXSystem>>();
            foreach (PvXType pvx in (PvXType[]) Enum.GetValues(typeof(PvXType)))
            {
                PvXDataDict[pvx] = new SortedDictionary<int, PvXSystem>();
                PvXStatistics[pvx] = new PvXStatistic();
            }
        }

        public static void CalculateStat(PvXType x_type)
        {
            List<PvXSystem> list = PvXDataDict[x_type].Values.ToList();
            if (x_type == PvXType.PVP)
            {
                PvXStatistics[x_type].MaxTotalWins = list.OrderByDescending(x => x.TotalWins)
                    .Where(x => x.Owner.AccessLevel == AccessLevel.Player).ToList();
                PvXStatistics[x_type].MaxTotalLoses = list.OrderByDescending(x => x.TotalLoses)
                    .Where(x => x.Owner.AccessLevel == AccessLevel.Player).ToList();

                PvXStatistics[x_type].TotalResKills = list.OrderByDescending(x => x.TotalResKills)
                    .Where(x => x.Owner.AccessLevel == AccessLevel.Player).ToList();
                PvXStatistics[x_type].TotalResKilled = list.OrderByDescending(x => x.TotalResKilled)
                    .Where(x => x.Owner.AccessLevel == AccessLevel.Player).ToList();

                PvXStatistics[x_type].MaxTotalPoints = list.OrderByDescending(x => x.TotalPoints)
                    .Where(x => x.Owner.AccessLevel == AccessLevel.Player).ToList();

                PvXStatistics[x_type].TotalResKills = ListTrimm(PvXStatistics[x_type].TotalResKills);
                PvXStatistics[x_type].StatTypesDict[PvXCounterType.TotalResKills] = PvXStatistics[x_type].TotalResKills;
            }
            else
            {
                PvXStatistics[x_type].MaxTotalWins =
                    list.OrderByDescending(x => x.TotalWins)
                        .Where(x => x.Owner.AccessLevel == AccessLevel.Player).ToList();
                PvXStatistics[x_type].MaxTotalLoses =
                    list.OrderByDescending(x => x.TotalLoses)
                        .Where(x => x.Owner.AccessLevel == AccessLevel.Player).ToList();
                PvXStatistics[x_type].TotalResKilled =
                    list.OrderByDescending(x => x.TotalResKilled)
                        .Where(x => x.Owner.AccessLevel == AccessLevel.Player).ToList();
                PvXStatistics[x_type].MaxTotalPoints =
                    list.OrderByDescending(x => x.TotalPoints)
                        .Where(x => x.Owner.AccessLevel == AccessLevel.Player).ToList();
            }

            PvXStatistics[x_type].MaxTotalWins = ListTrimm(PvXStatistics[x_type].MaxTotalWins);
            PvXStatistics[x_type].MaxTotalLoses = ListTrimm(PvXStatistics[x_type].MaxTotalLoses);
            PvXStatistics[x_type].MaxTotalPoints = ListTrimm(PvXStatistics[x_type].MaxTotalPoints);
            PvXStatistics[x_type].TotalResKilled = ListTrimm(PvXStatistics[x_type].TotalResKilled);


            PvXStatistics[x_type].StatTypesDict[PvXCounterType.MaxTotalWins] = PvXStatistics[x_type].MaxTotalWins;
            PvXStatistics[x_type].StatTypesDict[PvXCounterType.MaxTotalLoses] = PvXStatistics[x_type].MaxTotalLoses;
            PvXStatistics[x_type].StatTypesDict[PvXCounterType.MaxTotalPoints] = PvXStatistics[x_type].MaxTotalPoints;
            PvXStatistics[x_type].StatTypesDict[PvXCounterType.TotalResKilled] = PvXStatistics[x_type].TotalResKilled;
        }

        private static List<PvXSystem> ListTrimm(List<PvXSystem> value)
        {
            if (value != null && value.Count > MaxCountCalculate)
            {
                value.RemoveRange(MaxCountCalculate, value.Count - MaxCountCalculate);
            }

            return value;
        }

        private static void CheckStatBeforeSave()
        {
            foreach (var xtype in PvXDataDict.Keys.ToList())
            {
                foreach (var serial in PvXDataDict[xtype].Keys.ToList())
                {
                    if (!World.Mobiles.ContainsKey(serial))
                    {
                        Logs.PvXLog.WriteLine($"Serial:0x{serial.ToString("X")} not found, deleted");
                        PvXDataDict[xtype].Remove(serial);
                        continue;
                    }

                    if (PvXDataDict[xtype][serial].LastChangeTime + TimeSpan.FromDays(30) < DateTime.UtcNow)
                    {
                        int minus = Utility.LimitMinMax(1,
                            Convert.ToInt32(PvXDataDict[xtype][serial].TotalPoints * 0.05), 500);
                        int before = PvXDataDict[xtype][serial].TotalPoints;
                        PvXDataDict[xtype][serial].TotalLoses += minus;
                        Logs.PvXLog.WriteLine($"For Account:{World.Mobiles[serial].Account.Username} " +
                                              $"and name {World.Mobiles[serial].Name} TotalLoses increased by {minus}. " +
                                              $"Total_Points before:{before}; " +
                                              $"Total_Points after:{PvXDataDict[xtype][serial].TotalPoints}");
                        if (PvXDataDict[xtype][serial].TotalPoints <= 0)
                        {
                            Logs.PvXLog.WriteLine($"Account:{World.Mobiles[serial].Account.Username} deleted, TotalPoints<=0");
                            PvXDataDict[xtype].Remove(serial);
                            continue;
                        }
                        PvXDataDict[xtype][serial].LastChangeTime = DateTime.UtcNow;
                    }
                }
            }
        }

        private static void OnSave(WorldSaveEventArgs e)
        {
            var watch = Stopwatch.StartNew();
            CheckStatBeforeSave();
            Persistence.Serialize(FilePath, writer =>
            {
                writer.Write(0); //version
                foreach (var xtype in PvXDataDict.Keys)
                {
                    writer.Write((int) xtype);
                    writer.Write(PvXDataDict[xtype].Count);

                    foreach (var serial in PvXDataDict[xtype].Keys)
                    {
                        writer.Write(serial);
                        writer.Write(PvXDataDict[xtype][serial].TotalWins);
                        writer.Write(PvXDataDict[xtype][serial].TotalLoses);
                        writer.Write(PvXDataDict[xtype][serial].TotalResKilled);
                        writer.Write(PvXDataDict[xtype][serial].TotalResKills);
                        writer.Write(PvXDataDict[xtype][serial].TotalPointsLost);
                        writer.Write(PvXDataDict[xtype][serial].TotalPointsSpent);
                        writer.Write(PvXDataDict[xtype][serial].LastChangeTime);
                        writer.Write(PvXDataDict[xtype][serial].Owner);
                        PvXDataDict[xtype][serial].LastKilled = new Hashtable();
                        PvXDataDict[xtype][serial].LastKiller = new Hashtable();
                    }
                }
            });
            OnCalculateStat();
            watch.Stop();
            Console.WriteLine($"PvX stats save complete, duration:{watch.Elapsed.TotalMilliseconds} ms");
        }

        private static void OnLoad()
        {
            Persistence.Deserialize(FilePath, reader =>
            {
                reader.ReadInt(); // version
                for (int i = 0; i < 2; i++)
                {
                    var xtype = (PvXType)reader.ReadInt();
                    int xcount = reader.ReadInt();
                    for (int x = 0; x < xcount; x++)
                    {
                        int serial = reader.ReadInt();
                        PvXDataDict[xtype][serial] = new PvXSystem();
                        PvXDataDict[xtype][serial].TotalWins = reader.ReadInt();
                        PvXDataDict[xtype][serial].TotalLoses = reader.ReadInt();
                        PvXDataDict[xtype][serial].TotalResKilled = reader.ReadInt();
                        PvXDataDict[xtype][serial].TotalResKills = reader.ReadInt();
                        PvXDataDict[xtype][serial].TotalPointsLost = reader.ReadInt();
                        PvXDataDict[xtype][serial].TotalPointsSpent = reader.ReadInt();
                        PvXDataDict[xtype][serial].LastChangeTime = reader.ReadDateTime();
                        PvXDataDict[xtype][serial].Owner = reader.ReadMobile<PlayerMobile>();
                        PvXDataDict[xtype][serial].LastKilled = new Hashtable();
                        PvXDataDict[xtype][serial].LastKiller = new Hashtable();
                    }   
                }
            });
            OnCalculateStat();
        }

        private static void OnCalculateStat()
        {
            CalculateStat(PvXType.PVM);
            CalculateStat(PvXType.PVP);
        }
    }
}
