/*************************************************************************
 * ModernUO                                                              *
 * Copyright 2019-2022 - ModernUO Development Team                       *
 * Email: hi@modernuo.com                                                *
 * File: LogFactory.cs                                                   *
 *                                                                       *
 * This program is free software: you can redistribute it and/or modify  *
 * it under the terms of the GNU General Public License as published by  *
 * the Free Software Foundation, either version 3 of the License, or     *
 * (at your option) any later version.                                   *
 *                                                                       *
 * You should have received a copy of the GNU General Public License     *
 * along with this program.  If not, see <http://www.gnu.org/licenses/>. *
 *************************************************************************/

using System;
using System.IO;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Server.Logging;

public static class LogFactory
{
    private static string m_ConsolePath;
    private static string m_ConsoleFileName = "Console_.txt";
    private static string GetConsolePath
    {
        get
        {
            if (String.IsNullOrEmpty(m_ConsolePath))
            {
                m_ConsolePath = Path.Combine("Logs", "Console");
                if (!Directory.Exists(m_ConsolePath)) Directory.CreateDirectory(m_ConsolePath);
            }
            return m_ConsolePath;

        }
    }

    public static readonly Serilog.ILogger SerilogLogger = new LoggerConfiguration()
        .Enrich.With(new UtcTimestampEnricher())
        .MinimumLevel.Debug()
        .WriteTo.Async(a => a.Console(
            outputTemplate: "{UtcTimestamp: yyyy-MM-dd HH:mm:ss,fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
        ))
        .WriteTo.File(Path.Combine(GetConsolePath, m_ConsoleFileName), rollingInterval: RollingInterval.Day)
        .CreateLogger();

    public static ILogger GetLogger(Type declaringType) => new SerilogLogger(SerilogLogger.ForContext(declaringType));
}
class UtcTimestampEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory pf)
    {
        logEvent.AddPropertyIfAbsent(pf.CreateProperty("UtcTimestamp", logEvent.Timestamp.UtcDateTime));
    }
}