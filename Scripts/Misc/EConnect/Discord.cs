using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Server.Commands;
using Server.Logging;

namespace Server.Misc.EConnect
{
	public class Discord : BaseDiscord
	{
		public static void Initialize()
        {
            if (!Enabled)
                return;
			Bot = new Discord();
			try
			{
				Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromMinutes(1), CheckAndRestart);
			}
			catch (Exception e)
			{
				ConsoleLog.Write.Error($"Error discord init:{e.Message}");
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
									CommandSystem.Handle(DiscordConsoleMobile.GetMobile(msg.Author.Username, Channel.Console), msg.Content);
								}
								catch (Exception e)
								{
									msg.Channel.SendMessageAsync($"Error run command:{e.Message}");
								}
							}
							break;
						}
				}

			if (!msg.Author.IsBot && msg.Channel.Id == (ulong)Channel.WorldChat)
			{
				try
				{
					CommandSystem.Handle(DiscordChatMobile.GetMobile(msg.Author.Username, Channel.GlobalChat),
						$".c {msg.Content}");
				}
				catch (Exception e)
				{
					msg.Channel.SendMessageAsync($"Error run command:{e.Message}");
				}
			}

            //if (!msg.Author.IsBot && msg.Channel.Id == (ulong)Channel.TradeChannel)
            //{
            //    try
            //    {
            //        VendorItems.    
            //    }
            //    catch (Exception e)
            //    {
            //        msg.Channel.SendMessageAsync($"Error run command:{e.Message}");
            //    }
            //}

            return Task.CompletedTask;
		}

	}

	public class DiscordConsoleMobile : Mobile
	{
		private static DiscordConsoleMobile m_Bot;
		private BaseDiscord.Channel m_Channel;
		private DiscordConsoleMobile()
		{
			m_Channel = BaseDiscord.Channel.Console;
			this.AccessLevel = AccessLevel.Owner;
		}

		public DiscordConsoleMobile(Serial serial)
		{
		}

		public static DiscordConsoleMobile GetMobile(string name, BaseDiscord.Channel ch)
		{
			if (m_Bot == null)
			{
				m_Bot = new DiscordConsoleMobile();
			}
			m_Bot.Name = "Discord_" + name;
			m_Bot.m_Channel =ch;
			return m_Bot;
		}

		public override void SendAsciiMessage(string text)
		{
			DiscordBot.Send.ToCh(m_Channel, text);
		}

		public override void SendAsciiMessage(int hue, string text)
		{
            DiscordBot.Send.ToCh(m_Channel, text);
        }

		public override void SendMessage(string text)
		{
            DiscordBot.Send.ToCh(m_Channel, text);
        }

		public override void SendMessage(int hue, string text)
		{
            DiscordBot.Send.ToCh(m_Channel, text);
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

	public class DiscordChatMobile : Mobile
	{
		private static DiscordChatMobile m_Bot;
		private BaseDiscord.Channel m_Channel;
		private DiscordChatMobile()
		{
			m_Channel = BaseDiscord.Channel.GlobalChat;
		}

		public DiscordChatMobile(Serial serial)
		{
		}

		public static DiscordChatMobile GetMobile(string name, BaseDiscord.Channel ch)
		{
			if (m_Bot == null)
			{
				m_Bot = new DiscordChatMobile();
			}
			m_Bot.Name = "<Discord>" + name;
			m_Bot.m_Channel = ch;
			return m_Bot;
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
