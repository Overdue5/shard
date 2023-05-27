using Server.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Server
{
    public class MobInfo
    {
        public string Name;
        public string AccountName;
        public Serial Serial;
        public bool IsPlayer;
        public AccessLevel AccessLevel;

        public MobInfo(Mobile m)
        {
            Name = m.Name;
            Serial = m.Serial;
            IsPlayer = m.Player;
            AccountName = m.Account.Username;
            AccessLevel = m.Account.AccessLevel;
        }
        public MobInfo()
        {
            Name = "System";
            Serial =0;
            IsPlayer = false;
            AccountName = Name;
            AccessLevel = AccessLevel.Player;
        }
    }

    public class LogInfo
    {
        public MobInfo Player;
        public string Log;
        
        public LogInfo(Mobile player, string text)
        {
            if (player == null)
                Player = new MobInfo();
            else
                Player = new MobInfo(player);
            Log = text;
        }
        /// <summary>
        /// For system messages
        /// </summary>
        /// <param name="text"></param>
        public LogInfo(string text)
        {
            Player = new MobInfo();
            Log = text;
        }
    }

    public class BaseLogs
    {
        private string m_baseDir;
        protected bool m_Enabled = true;
        public int MaxLogCount = 2000;
        public bool Enabled { get { return m_Enabled; } set { m_Enabled = value; } }
        protected string LogsDirName { get; set; }
        protected string LogsName { get; set; }
        protected Dictionary<Serial, List<LogInfo>> LogDict;
        protected List<LogInfo> LogData;
        private static HashSet<Action> allLogs = new HashSet<Action>();

        public static void Initialize()
        {
            CommandSystem.Register("SaveLogs", AccessLevel.GameMaster, new CommandEventHandler(SaveLogs_OnCommand));
        }

        private static void SaveLogs_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendAsciiMessage("SaveLogs Started");
            
            foreach (var action in allLogs)
            {
                action.Invoke();
            }
            e.Mobile.SendAsciiMessage("SaveLogs Completed");
        }

        protected string LogFileName => $"{DateTime.UtcNow.ToString($"[yyyy-MM-dd]")}_{LogsName}.log";

        public BaseLogs(string dirLocation, string logName)
        {
            LogsDirName = dirLocation;
            LogsName = logName;
            LogDict = new Dictionary<Serial, List<LogInfo>>();
            LogData = new List<LogInfo>();
            m_baseDir = Path.Combine(Core.BaseDirectory, LogsDirName);
            m_baseDir = Path.Combine(m_baseDir, LogsName);

            if (!Directory.Exists(m_baseDir))
                Directory.CreateDirectory(m_baseDir);
            try
            {
                using (StreamWriter sw = new StreamWriter(Path.Combine(m_baseDir, LogFileName), true, Core.ASCIIEncoding))
                {
                    sw.WriteLine("##############################");
                    sw.WriteLine($"Log started on {DateTime.UtcNow}");
                    sw.WriteLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error init log for {LogsName}. Ex:{e.Message}");
            }
            EventSink.WorldSave += new WorldSaveEventHandler(SaveLog);
            EventSink.Crashed += new CrashedEventHandler(SaveLog);
            allLogs.Add(SaveLog);
        }

        public virtual void SaveLog(EventArgs e)
        {
            SaveLog();
        }

        public virtual void SaveLog()
        {
            using (StreamWriter sw = new StreamWriter(Path.Combine(m_baseDir, LogFileName), true, Core.ASCIIEncoding))
            {
                foreach (var log in LogData)
                {
                    sw.WriteLine(log.Log);
                }
            }

            var SortedList = LogData.OrderBy(o => o.Player.Serial).ToList();
            StreamWriter swSort = null;
            int oldValue = -1;
            foreach (var logInfo in SortedList)
            {
                if (oldValue != logInfo.Player.Serial)
                {
                    if (swSort != null)
                    {
                        swSort.Dispose();
                        swSort = null;
                    }
                    oldValue = logInfo.Player.Serial;
                }
                if (swSort == null)
                {
                    string path = m_baseDir;
                    var name = logInfo.Player.IsPlayer ? logInfo.Player.AccountName : logInfo.Player.Name;
                    AppendPath(ref path, logInfo.Player.AccessLevel.ToString());
                    path = Path.Combine(path, String.Format("{0}.log", name));
                    swSort = new StreamWriter(path, true, Core.ASCIIEncoding);
                }
                swSort.WriteLine(logInfo.Log);
            }
            if (swSort != null)
            {
                swSort.Dispose();
                swSort.Close();
                swSort = null;
            }
            LogData.Clear();
        }

        public virtual void WriteLine(Mobile from, string text)
        {
            if (!m_Enabled)
                return;
            LogData.Add(new LogInfo(from, $"{DateTime.UtcNow}: {@from.Name}_{@from.NetState}: {text}"));
            if (LogData.Count > MaxLogCount)
            {
                SaveLog();
            }
        }

        public virtual void WriteLine(string text)
        {
            if (!m_Enabled)
                return;
            LogData.Add(new LogInfo($"{DateTime.UtcNow}: SystemMsg: {text}"));
            if (LogData.Count > MaxLogCount)
            {
                SaveLog();
            }
        }

        private void AppendPath(ref string path, string toAppend)
        {
            path = Path.Combine(path, toAppend);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

    }
}