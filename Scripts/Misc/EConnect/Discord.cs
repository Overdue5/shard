using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Server.Commands;

namespace Server.Misc.EConnect
{
	public class Discord : BaseDiscord
	{
		public static void Initialize()
		{
			Bot = new Discord();
			try
			{
				Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromMinutes(1), CheckAndRestart);
			}
			catch (Exception e)
			{
				Utility.ConsoleWriteLine(Utility.ConsoleMsgType.Error, $"Error discord init:{e.Message}");
			}
		}

		protected override Task CommandsHandler(SocketMessage msg)
		{
			if (!msg.Author.IsBot && msg.Channel.Id == (ulong)Channel.Console)
				switch (msg.Content)
				{
					default:
						{
							if (msg.Content.StartsWith("."))
							{
								try
								{
									CommandSystem.Handle(DiscordMobile.GetMobile(msg.Author.Username, Channel.Console), msg.Content);
								}
								catch (Exception e)
								{
									msg.Channel.SendMessageAsync($"Error run command:{e.Message}");
								}
							}
							break;
						}
				}
			return Task.CompletedTask;
		}

	}

	public class DiscordMobile : Mobile
	{
		private static DiscordMobile m_Bot;
		private BaseDiscord.Channel m_Channel;
		private DiscordMobile()
		{
			m_Channel = BaseDiscord.Channel.Console;
			this.AccessLevel = AccessLevel.Owner;
		}

		public DiscordMobile(Serial serial)
		{
		}

		public static DiscordMobile GetMobile(string name, BaseDiscord.Channel ch)
		{
			if (m_Bot == null)
			{
				m_Bot = new DiscordMobile();
			}
			m_Bot.Name = "Discord_" + name;
			m_Bot.m_Channel =ch;
			return m_Bot;
		}

		public override void SendAsciiMessage(string text)
		{
			Discord.Bot.SendToDiscord(m_Channel, text);
		}

		public override void SendAsciiMessage(int hue, string text)
		{
			Discord.Bot.SendToDiscord(m_Channel, text);
		}

		public override void SendMessage(string text)
		{
			Discord.Bot.SendToDiscord(m_Channel, text);
		}

		public override void SendMessage(int hue, string text)
		{
			Discord.Bot.SendToDiscord(m_Channel, text);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			m_Bot = this;
		}
	}
}
