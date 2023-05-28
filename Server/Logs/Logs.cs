namespace Server
{
    public class Logs:BaseLogs
    {
        private static BaseLogs m_pvxlog;
        public static BaseLogs PvXLog
        {
            get
            {
                if (m_pvxlog == null)
                {
                    m_pvxlog = new BaseLogs("Logs", "PvXSystem");
                    m_pvxlog.Enabled = true;
                }

                return m_pvxlog;
            }
        }

        private static BaseLogs m_QuestsLogs;
        public static BaseLogs QuestLog
        {
            get
            {
                if (m_QuestsLogs == null)
                {
                    m_QuestsLogs = new BaseLogs("Logs", "QuestLog");
                    m_QuestsLogs.Enabled = true;
                }

                return m_QuestsLogs;
            }
        }

        private static BaseLogs m_EventsLogs;
        public static BaseLogs EventsLog
        {
            get
            {
                if (m_EventsLogs == null)
                {
                    m_EventsLogs = new BaseLogs("Logs", "EventsLog");
                    m_EventsLogs.Enabled = true;
                }

                return m_EventsLogs;
            }
        }

        private Logs(string dirLocation, string logName) : base(dirLocation, logName)
        {
        }
    }
}
