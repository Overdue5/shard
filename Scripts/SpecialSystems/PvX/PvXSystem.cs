using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public List<PlayerMobile> MaxTotalWins;
        public List<PlayerMobile> MaxTotalPoints;
        public List<PlayerMobile> MaxTotalLoses;
        public List<PlayerMobile> TotalResKills;
        public List<PlayerMobile> TotalResKilled;
        /// <summary>
        /// Dictionary 
        /// </summary>
        public Dictionary<PvXCounterType, List<PlayerMobile>> StatTypesDict;

        public PvXStatistic()
        {
            MaxTotalWins = new List<PlayerMobile>();
            MaxTotalPoints = new List<PlayerMobile>();
            MaxTotalLoses = new List<PlayerMobile>();
            TotalResKills = new List<PlayerMobile>();
            TotalResKilled = new List<PlayerMobile>();
            StatTypesDict = new Dictionary<PvXCounterType, List<PlayerMobile>>();
        }
    }

    public class PvXSystem
    {
        #region Fields

        private int m_TotalWins;
        private int m_TotalLoses;
        private int m_TotalResKills;
        private int m_TotalResKilled;
        private Mobile m_LastKiller;
        private Mobile m_LastKilled;
        private DateTime m_ResKillTime;
        private int m_TotalPointsLost;
        private int m_TotalPointsSpent;
        private string m_RankName;

        #endregion

        /// <summary>
        /// Dictionary with statistics for each PvXtype
        /// </summary>
        public static Dictionary<PvXType, PvXStatistic> StatDict = new Dictionary<PvXType, PvXStatistic>();

        public static Dictionary<PvXType, PvXSystem> PlayerInit()
        {
            var result = new Dictionary<PvXType, PvXSystem>();
            foreach (PvXType pvx in (PvXType[]) Enum.GetValues(typeof(PvXType)))
            {
                result[pvx] = new PvXSystem();
            }

            return result;
        }

        #region Properties

        [CommandProperty(AccessLevel.GameMaster)]
        public static int MaxCountCalculate { get; set; }

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
        public Mobile LastKiller
        {
            get { return m_LastKiller; }
            set { m_LastKiller = value; }
        }

        /// <summary>
        /// The last killed
        /// </summary>
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile LastKilled
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

        #endregion

        #region Constructors

        public PvXSystem()
        {
        }

        #endregion

        public static void Initialize()
        {
            foreach (PvXType pvx in (PvXType[]) Enum.GetValues(typeof(PvXType)))
            {
                StatDict[pvx] = new PvXStatistic();
            }

            MaxCountCalculate = 5;
            OnCalculateMaxValues(null);
            EventSink.WorldSave += OnCalculateMaxValues;
            EventSink.PlayerDeath += PvXPointSystem.CalculateStats;
        }

        public static void OnCalculateMaxValues(WorldSaveEventArgs w)
        {
            foreach (PvXType pvx in (PvXType[])Enum.GetValues(typeof(PvXType)))
            {
                CalculateMaxValues(pvx);
            }
        }

        private static void CalculateMaxValues(PvXType type)
        {
            var mobs = World.PlayerMobiles;
            var stat = StatDict[type];
            if (type == PvXType.PVP)
            {
                StatDict[type].MaxTotalWins =
                    mobs.Values.Cast<PlayerMobile>().OrderByDescending(x => x.PVPStat.TotalWins)
                        .Where(x => x.AccessLevel == AccessLevel.Player).ToList();

                StatDict[type].MaxTotalLoses =
                    mobs.Values.Cast<PlayerMobile>().OrderByDescending(x => x.PVPStat.TotalLoses)
                        .Where(x => x.AccessLevel == AccessLevel.Player).ToList();

                StatDict[type].TotalResKills =
                    mobs.Values.Cast<PlayerMobile>().OrderByDescending(x => x.PVPStat.TotalResKills)
                        .Where(x => x.AccessLevel == AccessLevel.Player).ToList();

                StatDict[type].TotalResKilled =
                    mobs.Values.Cast<PlayerMobile>().OrderByDescending(x => x.PVPStat.TotalResKilled)
                        .Where(x => x.AccessLevel == AccessLevel.Player).ToList();

                StatDict[type].MaxTotalPoints =
                    mobs.Values.Cast<PlayerMobile>().OrderByDescending(x =>
                            (x.PVPStat.TotalWins - x.PVPStat.TotalLoses - x.PVPStat.TotalResKills))
                        .Where(x => x.AccessLevel == AccessLevel.Player).ToList();

                stat.TotalResKills = ListTrimm(stat.TotalResKills);
                PvXSystem.StatDict[type].StatTypesDict[PvXCounterType.TotalResKills] = stat.TotalResKills;
            }
            else
            {
                stat.MaxTotalWins =
                    mobs.Values.Cast<PlayerMobile>().OrderByDescending(x => x.PVMStat.TotalWins)
                        .Where(x => x.AccessLevel == AccessLevel.Player).ToList();
                stat.MaxTotalLoses =
                    mobs.Values.Cast<PlayerMobile>().OrderByDescending(x => x.PVMStat.TotalLoses)
                        .Where(x => x.AccessLevel == AccessLevel.Player).ToList();
                stat.TotalResKilled =
                    mobs.Values.Cast<PlayerMobile>().OrderByDescending(x => x.PVMStat.TotalResKilled)
                        .Where(x => x.AccessLevel == AccessLevel.Player).ToList();
                stat.MaxTotalPoints =
                    mobs.Values.Cast<PlayerMobile>().OrderByDescending(x =>
                            (x.PVMStat.TotalWins - x.PVMStat.TotalLoses - x.PVMStat.TotalResKills))
                        .Where(x => x.AccessLevel == AccessLevel.Player).ToList();
            }

            stat.MaxTotalWins = ListTrimm(stat.MaxTotalWins);
            stat.MaxTotalLoses = ListTrimm(stat.MaxTotalLoses);
            stat.MaxTotalPoints = ListTrimm(stat.MaxTotalPoints);
            stat.TotalResKilled = ListTrimm(stat.TotalResKilled);
            

            PvXSystem.StatDict[type].StatTypesDict[PvXCounterType.MaxTotalWins] = stat.MaxTotalWins;
            PvXSystem.StatDict[type].StatTypesDict[PvXCounterType.MaxTotalLoses] = stat.MaxTotalLoses;
            PvXSystem.StatDict[type].StatTypesDict[PvXCounterType.MaxTotalPoints] = stat.MaxTotalPoints;
            PvXSystem.StatDict[type].StatTypesDict[PvXCounterType.TotalResKilled] = stat.TotalResKilled;
        }

        private static List<PlayerMobile> ListTrimm(List<PlayerMobile> value)
        {
            if (value != null && value.Count > MaxCountCalculate)
            {
                value.RemoveRange(5, value.Count - MaxCountCalculate);
            }

            return value;
        }
    }
}
