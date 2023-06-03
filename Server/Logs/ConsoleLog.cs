namespace Server.Logging;

public static class ConsoleLog
{
    public static ILogger Write => new SerilogLogger(LogFactory.SerilogLogger);
}

public static class DiscordBot
{
    public static DiscordLogger Send => new DiscordLogger(LogFactory.SerilogLogger);
}