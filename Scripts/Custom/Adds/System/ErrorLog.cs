using System;
using System.IO;
using Server.Logging;

namespace Server.Misc.ErrorLog
{
    class ErrorLog
    {
        public static string runUOPath = "E:\\INX\\RunUO 2 RC1";
        public Exception exception;

        public static void StartLogging(Exception e)
        {
            try
            {
                ErrorLog eL = new ErrorLog();
                eL.exception = e;
                ConsoleLog.Write.Error("Error log created.\n");
                eL.Log();
            }
            catch(Exception inter)
            {
                ConsoleLog.Write.Information(inter + "\n");
            }
        }

        public void Log()
        {
            string filename = "\\CustomLog ";
            filename += DateTime.UtcNow.ToString();
            filename += ".txt";

            FileInfo fI = new FileInfo(runUOPath + filename);
            fI.Create();

            StreamWriter sW = new StreamWriter(fI.FullName);

            sW.Write("Crash log report: \n\n");
            sW.Write(exception.ToString());

            sW.Flush();
            sW.Close();
        }
    }
}