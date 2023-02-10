using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Server
{
    public class BaseDiscord
    {
#if (!DEBUG)
		private static bool Enabled = false;
#else
        private static bool Enabled = false;
#endif
        private static int MsgLength;
        private static BaseDiscord m_Discord;
        private static DiscordSocketClient discord;
        private static Dictionary<Channel, ChannelsInfo> Channels = new Dictionary<Channel, ChannelsInfo>();
        public static Timer DTimer;

        public static BaseDiscord Bot
        {
            get { return m_Discord ?? new BaseDiscord(); }
            set { m_Discord = value; }
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
            foreach (Channel ch in (Channel[]) Enum.GetValues(typeof(Channel)))
            {
                Channels[ch] = new ChannelsInfo();
            }
        }

        public enum Channel : ulong
        {
            //Test = 876407848941281320,
			None = 0,

#if (!DEBUG)
			Console = 876407848941281320,
			GlobalChat = 871460287021207552
#else
	        Console = 877330007947612270,
			GlobalChat = 877330103317708841
#endif
		}

        async public static Task StopAsync()
        {
            if (discord != null)
                await discord.StopAsync();
            if (DTimer != null)
                DTimer.Stop();
        }

        async public static Task MainAsync()
        {
            if (!Enabled)
                return;
            var token = Config.Get("Discord.Token", "");
            MsgLength = Config.Get("Discord.MsgLength", 1000);
            discord = new DiscordSocketClient();
            discord.MessageReceived += Bot.CommandsHandler;
            discord.Log += Bot.Log;
            await discord.LoginAsync(TokenType.Bot, token);
            await discord.StartAsync();
            DTimer = Timer.DelayCall(TimeSpan.FromSeconds(5), ()=>BaseDiscord.Bot.SendMsg());
        }

        async public static void CheckAndRestart()
        {
	        if (discord == null || discord.ConnectionState != ConnectionState.Connected)
	        {
		        await StopAsync();
		        await MainAsync();
	        }
        }

        async public virtual void SendToDiscord(Channel ch, string msg)
        {
            if (!Enabled)
                return;
            if (ch == Channel.None)
            {
	            Utility.ConsoleWriteLine(Utility.ConsoleMsgType.Error, $"!Send to None channel.{new StackTrace()}");
	            return;
            }

            try
            {
                lock (Channels[ch].Msg)
                {
                    Channels[ch].Msg.Add(msg);
                }

                await Task.Run(SendMsg);
            }
            catch (Exception e)
            {
                Utility.ConsoleWriteLine(Utility.ConsoleMsgType.Error,
                    $"Discord Error: {e.ToString()} for channel {ch}");
            }
        }

        public virtual void SendMsg()
        {
            if (discord == null)
                return;
            lock (Channels)
            {
                foreach (var ch in Channels.Keys.Where(ch => !Channels[ch].Sending))
                {
                    SendAsync(ch, Channels[ch]);
                }
            }
        }

        async private Task SendAsync(Channel ch, ChannelsInfo info)
        {
            try
            {
                info.Sending = true;
                while (info.Msg.Count > 0)
                {
                    if (info.DChannel == null)
                        info.DChannel = (IMessageChannel) discord.GetChannel((ulong) ch);
                    if (info.DChannel == null)
                        return;

                    StringBuilder toSend = new StringBuilder();
                    string[] msgArray;
                    lock (info.Msg)
                    {
                        msgArray = info.Msg.ToArray();
                    }

                    toSend.AppendLine("");
                    var mcount = 0;
                    foreach (var mInfo in msgArray)
                    {
                        if (toSend.Length + mInfo.Length > MsgLength)
                        {
                            if (toSend.Length < 10 )
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
                        Utility.ConsoleWriteLine(Utility.ConsoleMsgType.Error, 
	                        $"!Error send messages to discord.Len:{toSend.Length}.fcount:{fcount}\n" +
	                        $"sendtask.Result:{sendTask.Result}");
                    }
                }
            }
            catch (Exception e)
            {
                Utility.ConsoleWriteLine(Utility.ConsoleMsgType.Error, $"Discord sending critical error: {e.Message}");
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

        protected virtual Task Log(LogMessage msg)
        {
            Utility.ConsoleWriteLine(Utility.ConsoleMsgType.Info, $"!{msg.ToString()}");
            return Task.CompletedTask;
        }
	}
}