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

        private Logs(string dirLocation, string logName) : base(dirLocation, logName)
        {
        }
    }
}
