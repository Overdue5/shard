using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Server
{
	public class BaseDiscord
	{
#if (!DEBUG)
		private static bool Enabled = true;
#else
		private static bool Enabled = true;
#endif
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
			public Task SendTask;

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
			Console = 876407848941281320,
			//Logs = 876407848941281320
		}

		async public static Task MainAsync()
		{
			if (!Enabled)
				return;
			var token = Config.Get("Discord.Token", "");
			discord = new DiscordSocketClient();
			discord.MessageReceived += Bot.CommandsHandler;
			discord.Log += Bot.Log;
			await discord.LoginAsync(TokenType.Bot, token);
			await discord.StartAsync();
			DTimer = Timer.DelayCall(TimeSpan.FromSeconds(10), () => Bot.SendMsg());
		}

		async public virtual void SendToDiscord(Channel ch, string msg)
		{
			if (!Enabled)
				return;
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
				Utility.ConsoleWriteLine(Utility.ConsoleMsgType.Error, $"Discord Error: {e.ToString()} for channel {ch}");
			}
		}

		public virtual void SendMsg()
		{
			if (discord == null)
				return;
			lock (Channels)
			{
				foreach (var ch in Channels.Keys)
				{
					if (Channels[ch].Sending)
						continue;

#pragma warning disable 4014
					SendAsync(ch, Channels[ch]);
#pragma warning restore 4014
				}
			}
		}

		async private Task SendAsync(Channel ch, ChannelsInfo info)
		{
			await Task.Run(() =>
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

						toSend.AppendLine("");
						var mcount = 0;
						foreach (var mInfo in msgArray)
						{
							if (toSend.Length + mInfo.Length> 1800)
								break;
							mcount++;
							toSend.AppendLine(mInfo);

							lock (info.Msg)
							{
								info.Msg.Remove(mInfo);
							}
						}
						info.SendTask = info.DChannel.SendMessageAsync(toSend.ToString());
						while (info.SendTask != null && info.SendTask.Status!=TaskStatus.RanToCompletion)
						{
							Task.Delay(100);
							if (info.SendTask != null && info.SendTask.Status == TaskStatus.Faulted)
							{
								info.SendTask = info.DChannel.SendMessageAsync(toSend.ToString());
							}
						}

						Utility.ConsoleWriteLine(Utility.ConsoleMsgType.Info,
							$"!Sending {mcount} messages to {ch.ToString()} channel");
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
			});
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