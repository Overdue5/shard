using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Server.Logging;

namespace Server
{
    public class BaseDiscord
    {
#if (!DEBUG)
		public static bool Enabled = true;
#else
        public static bool Enabled = true;
#endif
        private static int MsgLength;
        private static BaseDiscord m_Discord;
        private static DiscordSocketClient discord;
        private static Dictionary<Channel, ChannelsInfo> Channels = new Dictionary<Channel, ChannelsInfo>();
        public static Timer DTimer;

        public static BaseDiscord Bot
        {
            get => m_Discord ?? new BaseDiscord();
            set => m_Discord = value;
        }

        public class ChannelsInfo
        {
            public List<string> Msg;
            public IMessageChannel DChannel;
            public bool Sending;

            public ChannelsInfo()
            {
                Msg = new List<string>();
                DChannel = null;
                Sending = false;
            }
        }

        static BaseDiscord()
        {
            foreach (Channel ch in (Channel[])Enum.GetValues(typeof(Channel)))
            {
                Channels[ch] = new ChannelsInfo();
            }
        }

        public enum Channel : ulong
        {
            //Test = 876407848941281320,
            None = 0,

#if (!DEBUG)
			Console = 1104380028558508187,
			GlobalChat = 1104380028558508187,
            Announcement = 1092018919151243295,
            WorldChat = 1104418812427763864,
            ConsoleImportant = 1104746301393997864,
            TradeChannel = 1104746301393997864,
            PvP = 1092018746018779136
#else
            Console = 877330007947612270,
            GlobalChat = 877330103317708841,
            Announcement = 877330103317708841,
            WorldChat = 877330103317708841,
            ConsoleImportant = 877330103317708841,
            TradeChannel = 877330007947612270,
            PvP = 877330007947612270

#endif
        }

        public static async Task StopAsync()
        {
            if (discord != null)
                await discord.StopAsync();
            if (DTimer != null)
                DTimer.Stop();
        }

        public static async Task MainAsync()
        {
            try
            {
                if (!Enabled)
                    return;
                var token = Config.Get("Discord.Token", "");
                MsgLength = Config.Get("Discord.MsgLength", 1000);
                discord = new DiscordSocketClient();
                discord.MessageReceived += Bot.CommandsHandler;
                await discord.LoginAsync(TokenType.Bot, token);
                await discord.StartAsync();
                DTimer = Timer.DelayCall(TimeSpan.FromSeconds(5), () => Bot.StartSending());
            }
            catch (Exception e)
            {
                ConsoleLog.Write.Warning("Discord exceptin:", e);
            }
        }

        public static async void CheckAndRestart()
        {
            if (discord == null || discord.ConnectionState != ConnectionState.Connected)
            {
                await StopAsync();
                await MainAsync();
            }
        }

        internal virtual async void SendMessageToDiscord(Channel ch, string msg)
        {
            if (!Enabled)
                return;
            if (ch == Channel.None)
            {
                ConsoleLog.Write.Error($"!Send to None channel.{new StackTrace()}");
                return;
            }

            try
            {
                lock (Channels[ch].Msg)
                {
                    Channels[ch].Msg.Add(msg);
                }

                await Task.Run(StartSending);
            }
            catch (Exception e)
            {
                ConsoleLog.Write.Error(
                    $"Discord Error: {e.ToString()} for channel {ch}");
            }
        }

        public virtual void StartSending()
        {
            if (discord == null)
                return;
            lock (Channels)
            {
                foreach (var ch in Channels.Keys.Where(ch => !Channels[ch].Sending))
                {
                    _ = SendAsync(ch, Channels[ch]);
                }
            }
        }

        private async Task SendAsync(Channel ch, ChannelsInfo info)
        {
            try
            {
                info.Sending = true;
                while (info.Msg.Count > 0)
                {
                    if (info.DChannel == null)
                        info.DChannel = (IMessageChannel)discord.GetChannel((ulong)ch);
                    if (info.DChannel == null)
                        return;

                    StringBuilder toSend = new StringBuilder();
                    string[] msgArray;
                    lock (info.Msg)
                    {
                        msgArray = info.Msg.ToArray();
                    }

                    toSend.AppendLine("\n");
                    var mcount = 0;
                    foreach (var mInfo in msgArray)
                    {
                        if (toSend.Length + mInfo.Length > MsgLength)
                        {
                            if (toSend.Length < 10)
                            {
                                mcount++;
                                toSend.AppendLine(mInfo.Substring(0, MsgLength));
                                lock (info.Msg)
                                {
                                    info.Msg.Remove(mInfo);
                                }
                            }

                            break;
                        }

                        mcount++;
                        toSend.AppendLine(mInfo);

                        lock (info.Msg)
                        {
                            info.Msg.Remove(mInfo);
                        }
                    }

                    int count = 0;
                    int fcount = 0;
                    var sendTask = info.DChannel.SendMessageAsync(toSend.ToString());
                    while ((sendTask != null && sendTask.Status != TaskStatus.RanToCompletion) && count < 5)
                    {
                        await Task.Delay(1000);
                        if (sendTask.Status == TaskStatus.Faulted)
                        {
                            sendTask = info.DChannel.SendMessageAsync(toSend.ToString());
                            fcount++;
                        }

                        count++;
                    }

                    if (count >= 5)
                    {
                        ConsoleLog.Write.Error($"!Error send messages to discord.Len:{toSend.Length}.fcount:{fcount} sendtask.Result:{sendTask.Result}", sendToDiscord:false);
                    }
                }
            }
            catch (Exception e)
            {
                ConsoleLog.Write.Error($"Discord sending critical error:", e);
            }
            finally
            {
                info.Sending = false;
            }
        }

        protected virtual Task CommandsHandler(SocketMessage msg)
        {
            return Task.CompletedTask;
        }

        //protected virtual Task Log(LogMessage msg)
        //{
        //    ConsoleLog.Write.Information($"!{msg.ToString()}");
        //    return Task.CompletedTask;
        //}
    }
}