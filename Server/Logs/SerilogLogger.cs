using System;
using System.Runtime.CompilerServices;

namespace Server.Logging;

public class SerilogLogger : ILogger
{
    private readonly Serilog.ILogger m_SerilogLogger;

    public SerilogLogger(Serilog.ILogger serilogLogger) =>
        this.m_SerilogLogger = serilogLogger;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Debug(string message, params object[] args)
    {
        m_SerilogLogger.Debug(message, args);
        BaseDiscord.Bot.SendMessageToDiscord(BaseDiscord.Channel.Console, String.Format($"[DBG] {String.Format(message, args)}"));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Debug(Exception exception, string message, params object[] args)
    {
        m_SerilogLogger.Debug(exception, message, args);
        BaseDiscord.Bot.SendMessageToDiscord(BaseDiscord.Channel.ConsoleImportant, String.Format($"[DBG] {String.Format(message, args)}\n{exception}"));
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Information(string message, params object[] args)
    {
        m_SerilogLogger.Information(message, args);
        BaseDiscord.Bot.SendMessageToDiscord(BaseDiscord.Channel.Console, String.Format($"[INF] {String.Format(message, args)}"));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Information(Exception exception, string message, params object[] args)
    {
        m_SerilogLogger.Information(exception, message, args);
        BaseDiscord.Bot.SendMessageToDiscord(BaseDiscord.Channel.ConsoleImportant, String.Format($"[INF] {String.Format(message, args)}\n{exception}"));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Warning(string message, params object[] args)
    {
        m_SerilogLogger.Warning(message, args);
        BaseDiscord.Bot.SendMessageToDiscord(BaseDiscord.Channel.Console, String.Format($"[WRN] {String.Format(message, args)}"));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Warning(Exception exception, string message, params object[] args)
    {
        m_SerilogLogger.Warning(exception, message, args);
        BaseDiscord.Bot.SendMessageToDiscord(BaseDiscord.Channel.ConsoleImportant, String.Format($"[WRN] {String.Format(message, args)}\n{exception}"));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Warning(Exception exception)
    {
        m_SerilogLogger.Warning(exception, "");
        BaseDiscord.Bot.SendMessageToDiscord(BaseDiscord.Channel.ConsoleImportant, String.Format($"[WRN] {exception}"));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Error(string message, bool sendToDiscord, params object[] args)
    {
        m_SerilogLogger.Error(message, args);
        if (sendToDiscord)
            BaseDiscord.Bot.SendMessageToDiscord(BaseDiscord.Channel.ConsoleImportant, String.Format($"[ERR] {String.Format(message, args)}"));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Error(string message, params object[] args)
    {
        m_SerilogLogger.Error(message, args);
        BaseDiscord.Bot.SendMessageToDiscord(BaseDiscord.Channel.ConsoleImportant, String.Format($"[ERR] {String.Format(message, args)}"));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Error(Exception exception, string message, params object[] args)
    {
        m_SerilogLogger.Error(exception, message, args);
        BaseDiscord.Bot.SendMessageToDiscord(BaseDiscord.Channel.ConsoleImportant, String.Format($"[ERR] {String.Format(message, args)}\n{exception}"));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Fatal(string message, params object[] args)
    {
        m_SerilogLogger.Fatal(message, args);
        BaseDiscord.Bot.SendMessageToDiscord(BaseDiscord.Channel.ConsoleImportant, String.Format($"[ERR] {String.Format(message, args)}"));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Fatal(Exception exception, string message, params object[] args)
    {
        m_SerilogLogger.Fatal(exception, message, args);
        BaseDiscord.Bot.SendMessageToDiscord(BaseDiscord.Channel.ConsoleImportant, String.Format($"[ERR] {String.Format(message, args)}\n{exception}"));
    }
}

public class DiscordLogger
{
    private readonly Serilog.ILogger m_SerilogLogger;

    public DiscordLogger(Serilog.ILogger serilogLogger) =>
        this.m_SerilogLogger = serilogLogger;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ToCh(BaseDiscord.Channel ch, string message, params object[] args)
    {
        if (ch == BaseDiscord.Channel.Announcement)
            Announcement(message, args);
        else if (ch == BaseDiscord.Channel.Console)
            Console(message, args);
        else if (ch == BaseDiscord.Channel.ConsoleImportant)
            ConsoleImportant(message, args);
        else if (ch == BaseDiscord.Channel.PvP)
            PvP(message, args);
        else if (ch == BaseDiscord.Channel.WorldChat)
            WoldChat(message, args);
        else if (ch == BaseDiscord.Channel.TradeChannel)
            Trade(message, args);
        else         Console(message, args);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Console(string message, params object[] args)
    {
        m_SerilogLogger.Information($"[c] {message}", args);
        BaseDiscord.Bot.SendMessageToDiscord(BaseDiscord.Channel.Console, String.Format($"{String.Format(message, args)}"));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ConsoleImportant(string message, params object[] args)
    {
        m_SerilogLogger.Information($"[{BaseDiscord.Channel.ConsoleImportant}] {message}", args);
        BaseDiscord.Bot.SendMessageToDiscord(BaseDiscord.Channel.ConsoleImportant, String.Format($"{String.Format(message, args)}"));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WoldChat(string message, params object[] args)
    {
        m_SerilogLogger.Information($"[{BaseDiscord.Channel.WorldChat}] {message}", args);
        BaseDiscord.Bot.SendMessageToDiscord(BaseDiscord.Channel.WorldChat, String.Format($"{String.Format(message, args)}"));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PvP(string message, params object[] args)
    {
        m_SerilogLogger.Information($"[{BaseDiscord.Channel.PvP}] {message}", args);
        BaseDiscord.Bot.SendMessageToDiscord(BaseDiscord.Channel.PvP, String.Format($"{String.Format(message, args)}"));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Announcement(string message, params object[] args)
    {
        m_SerilogLogger.Information($"[{BaseDiscord.Channel.Announcement}] {message}", args);
        BaseDiscord.Bot.SendMessageToDiscord(BaseDiscord.Channel.Announcement, String.Format($"{String.Format(message, args)}"));
    }

    public void Trade(string message, params object[] args)
    {
        m_SerilogLogger.Information($"[{BaseDiscord.Channel.TradeChannel}] {message}", args);
        BaseDiscord.Bot.SendMessageToDiscord(BaseDiscord.Channel.TradeChannel, String.Format($"{String.Format(message, args)}"));
    }
}
