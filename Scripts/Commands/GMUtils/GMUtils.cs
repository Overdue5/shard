using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using Discord;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Engines.Harvest;
using Server.Items;
using Server.Misc;
using Server.Targeting;
using CommandEventArgs = Server.Commands.CommandEventArgs;
using CommandEventHandler = Server.Commands.CommandEventHandler;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.Diagnostics;
using System.IO;
using static Server.Games.PaintBall.PBGameItem;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using Server.Network;
using MessageType = Server.Network.MessageType;

namespace Scripts.Commands
{
    public class GMUtils
    {
        private static Hashtable _PreviousAccessLevel = null;
        private static bool m_RestartFlag = false;

        public static void Initialize()
        {
            CommandSystem.Register("TurnAccessLevel", AccessLevel.Player, new CommandEventHandler(TurnAccessLevel_OnCommand));
            CommandSystem.Register("TAL", AccessLevel.Player, new CommandEventHandler(TurnAccessLevel_OnCommand));
            CommandSystem.Register("anim", AccessLevel.GameMaster, new CommandEventHandler(Animation_OnCommand));
            CommandSystem.Register("status", AccessLevel.Counselor, new CommandEventHandler(Status_OnCommand));
            CommandSystem.Register("discord", AccessLevel.Counselor, new CommandEventHandler(Discord_OnCommand));
            CommandSystem.Register("online", AccessLevel.Counselor, new CommandEventHandler(Online_OnCommand));
            CommandSystem.Register("restart", AccessLevel.GameMaster, new CommandEventHandler(Restart_OnCommand));
            CommandSystem.Register("update", AccessLevel.GameMaster, new CommandEventHandler(Update_OnCommand));
            CommandSystem.Register("shutdown", AccessLevel.GameMaster, new CommandEventHandler(ShutDown_OnCommand));
            EventSink.WorldSave += args => { BaseDiscord.Bot.SendToDiscord(BaseDiscord.Channel.WorldChat, GetOnlineReport()); };
#if DEBUG
            CommandSystem.Register("harvestStat", AccessLevel.Player, new CommandEventHandler(CheckHarvestStat_OnCommand));
            CommandSystem.Register("saycheck", AccessLevel.Player, new CommandEventHandler(Say_OnCommand));
#endif

        }

        private static string GetOnlineReport()
        {
            int count = World.Mobiles.Values.Count(x => x.NetState != null && x is PlayerMobile pl && pl.AccessLevel == AccessLevel.Player);
            if (count == 0)
                return $"No any avatars in Britannia";
            if (count == 1)
                return $"Now only {count} avatar in Britannia";
            return $"Now {count} avatars in Britannia";
        }
        [Usage("update")]
        [Description("Update server")]
        private static void Update_OnCommand(CommandEventArgs e)
        {
            Restart_part(e, restart: false, update:true);
        }

        [Usage("ShutDown")]
        [Description("Shutdown server")]
        private static void ShutDown_OnCommand(CommandEventArgs e)
        {
            Restart_part(e, restart: false);
        }

        [Usage("Restart")]
        [Description("Restart server")]
        private static void Restart_OnCommand(CommandEventArgs e)
        {
            Restart_part(e, restart: true);
        }

        private static void Restart_part(CommandEventArgs e, bool restart, bool update=false)
        {
            if (m_RestartFlag)
            {
                e.Mobile.SendAsciiMessage($"Restart already activated");
                return;
            }

            var msg = new List<string>();
            msg.Add("Portals to Britannia close,\nAvatars pause,\nJourney's end beckons.");
            msg.Add("Travelers heed,\nMagic fades,\nBritannia's doors slowly closing.");
            msg.Add("Mystic currents shift,\nGates to Britannia soon to seal,\nFarewell, dear avatars.");
            msg.Add("Crystal moon above,\nBritannia's gate will soon close,\nTravelers, rest well.");
            msg.Add("Darkness fills the sky,\nBritannia's doors soon will shut,\nJourney's end for now.");
            msg.Add("Sunset on the shore,\nBritannia bids farewell,\nRest, weary traveler.");
            msg.Add("Majestic mountains,\nBritannia soon sleeps,\nTravelers dream of dawn.");
            msg.Add("Nightfall in Britannia,\nJourneys pause, gates close,\nRest, brave adventurer.");
            msg.Add("The gates of Britannia will soon be barred to all weary travelers.");
            msg.Add("The land of Britannia shall soon close its doors to all wandering souls.");
            msg.Add("The time approaches when Britannia shall deny entry to all who seek it.");
            msg.Add("The way to Britannia will soon be sealed to all those who would enter.");
            msg.Add("A warning to all who seek Britannia: soon its gates shall be shut tight.");
            msg.Add("As the arcane energies shift, Britannia's paths close to all but the initiated.");
            msg.Add("The winds of magic warn of Britannia's impending closure to mortal passage.");
            msg.Add("The enchanted realm of Britannia will soon bar its gates to wandering feet.");
            msg.Add("The veil between worlds grows thin, soon Britannia will close its mystical doors.");
            World.Broadcast(33, false, $"{msg[Utility.Random(msg.Count)]}");
            int delay = 5;
            var restartTime = DateTime.UtcNow;
            try
            {
                if (e.Length >= 1)
                    delay = e.GetInt32(0);
                restartTime += TimeSpan.FromMinutes(delay);
            }
            catch (Exception exception)
            {
                e.Mobile.SendAsciiMessage($"Error restart: {exception.Message}");
                return;
            }

            m_RestartFlag = true;
            Timer.DelayCall(TimeSpan.Zero, () => { RestartPrepare(restartTime, restart, update); });
            AutoSave.Save(false);

        }

        private static void RestartPrepare(DateTime restartTime, bool restart, bool update=false)
        {
            var time = (restartTime - DateTime.UtcNow).TotalSeconds;
            if (!m_RestartFlag)
            {
                return;
            }

            if (time > 60)
            {
                World.Broadcast(33, true, $"Britannia will be unavailable for travelers in {Math.Ceiling(time / 60)} minutes");
                if (time>90)
                    Timer.DelayCall(TimeSpan.FromMinutes(1), () => { RestartPrepare(restartTime, restart, update); });
                else
                    Timer.DelayCall(TimeSpan.FromSeconds(40), () => { RestartPrepare(restartTime, restart, update); });
                return;
            }
            else if (time < 60 && time > 0)
            {
                World.Broadcast(33, true, $"Britannia will be unavailable for travelers in {Math.Ceiling(time)} seconds");
                if (time>20)
                    Timer.DelayCall(TimeSpan.FromSeconds(10), () => { RestartPrepare(restartTime, restart, update); });
                else
                    Timer.DelayCall(TimeSpan.FromSeconds(1), () => { RestartPrepare(restartTime, restart, update); });
            }
            else if (time <=0 )
            {
                AutoSave.Save(false);
                Core.Kill(restart, update);
            }

        }

        [Usage("harvestStat")]
        [Description("CheckHarvestStat")]
        private static void CheckHarvestStat_OnCommand(CommandEventArgs e)
        {
            var loc = new Point3D(2481, 46, 0);
            var target = new StaticTarget(loc, 1343);
            var tileID = 17727;
            HarvestDefinition def = Mining.System.GetDefinition(tileID);
            HarvestBank bank = def.GetBank(Map.Felucca, loc.X, loc.Y);
            var tool = new Pickaxe();
            var exist = new Dictionary<Serial, int>();
            foreach (var item in e.Mobile.Backpack.Items.ToArray())
            {
                if (item is BaseOre bo)
                {
                    bo.Delete();
                    continue;
                }

                exist[item.Serial] = item.Amount;
            }

            Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
            {
                for (int i = 0; i < 100; i++)
                {
                    bank.ResetRespawnTimer();
                    Mining.System.FinishHarvesting(e.Mobile, tool, def, target, null);
                }
            });

            Timer.DelayCall(TimeSpan.FromSeconds(5), () =>
            {
                var loot = new Dictionary<Serial, int>();
                var stat = new Dictionary<string, float>();
                var oreTotal = 0;
                foreach (Item item in e.Mobile.Backpack.Items.ToArray())
                {
                    if (item.Deleted)
                        continue;
                    if (exist.Keys.Contains(item.Serial))
                    {
                        if (exist[item.Serial] == item.Amount)
                            continue;
                        loot[item.Serial] = exist[item.Serial] - item.Amount;
                    }

                    loot[item.Serial] = item.Amount;
                    if (item is BaseOre)
                        oreTotal += item.Amount;
                    stat[item.GetType().ToString()] = item.Amount;
                }

                foreach (string statKey in stat.Keys)
                {
                    int p = (int)(100 * stat[statKey] / oreTotal);
                    e.Mobile.SendAsciiMessage($"{statKey}:{(int)stat[statKey]}:{p}");
                }
                e.Mobile.SendAsciiMessage($"Total Ore:{oreTotal}");
                tool.Delete();
            });

        }
        
        [Usage("online")]
        [Description("Print server online status")]
        private static void Online_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendAsciiMessage(GetOnlineReport());
        }

        [Usage("status")]
        [Description("Print server status ans short performance info")]
        private static void Status_OnCommand(CommandEventArgs e)
        {
	        e.Mobile.SendMessage($"\nTotal Items:{World.Items.Count}; Total Mobile:{World.Mobiles.Count}; " +
	                             $"Total Players:{World.Mobiles.Values.Count(x => x.Player)}; " +
	                             $"Online:{World.Mobiles.Values.Count(x => x.NetState != null)};\n" +
	                             $"Performance:CyclesPerSecond:{Core.CyclesPerSecond:0.00};AverageCPS:{Core.AverageCPS:0.00};\n" +
	                             $"Timers:{Timer.TimerThread.GetTimersShortInfo()}");
        }

        [Usage("discord")]
        [Description("Restart discord")]
        private static void Discord_OnCommand(CommandEventArgs e)
        {
	        try
            {
                _ = BaseDiscord.StopAsync();
                Timer.DelayCall(TimeSpan.FromSeconds(5), () => _ = BaseDiscord.MainAsync());
	        }
	        catch
	        {
		        Utility.ConsoleWriteLine(Utility.ConsoleMsgType.Error, $"Restart discord finished with error.{e}");
	        }
        }

		[Usage("TurnAccessLevel")]
        [Description("Switch access level to player and back")]
        private static void TurnAccessLevel_OnCommand(CommandEventArgs e)
        {
            if (!(e.Mobile is PlayerMobile) || (e.Mobile.Account == null))
                return;

            if (_PreviousAccessLevel == null)
            {
                _PreviousAccessLevel = new Hashtable(8);
                EventSink.WorldBeforeSave += args => {
                    var ide = _PreviousAccessLevel.GetEnumerator();
                    while (ide.MoveNext())
                    {
                        var m = World.FindMobile((Serial)ide.Key);
                        if (m != null && m.Account != null && !m.Deleted)
                        {
                            e.Mobile.AccessLevel = (AccessLevel)ide.Value;
                            e.Mobile.Account.AccessLevel = (AccessLevel)ide.Value;
                        }
                    }
                };
                EventSink.WorldSave += args => {
                    var ide = _PreviousAccessLevel.GetEnumerator();
                    while (ide.MoveNext())
                    {
                        var m = World.FindMobile((Serial)ide.Key);
                        if (m != null && m.Account != null && !m.Deleted)
                        {
                            e.Mobile.AccessLevel = AccessLevel.Player;
                            e.Mobile.Account.AccessLevel = AccessLevel.Player;
                        }
                    }
                };
            }

            var access = (AccessLevel)Math.Max((int)e.Mobile.AccessLevel, (int)e.Mobile.Account.AccessLevel);
            if (access > AccessLevel.Player)
            {
                _PreviousAccessLevel.Add(e.Mobile.Serial, access);
                e.Mobile.AccessLevel = AccessLevel.Player;
                e.Mobile.Account.AccessLevel = AccessLevel.Player;
            }
            else if (_PreviousAccessLevel.Contains(e.Mobile.Serial))
            {
                access = (AccessLevel)_PreviousAccessLevel[e.Mobile.Serial];
                _PreviousAccessLevel.Remove(e.Mobile.Serial);
                e.Mobile.AccessLevel = access;
                e.Mobile.Account.AccessLevel = access;
            }
        }

        [Usage("anim")]
        [Description("Test animation play.  anim AnimationType action")]
        private static void Animation_OnCommand(CommandEventArgs e)
        {
            //public virtual void Animate( int action, int frameCount, int repeatCount, bool forward, bool repeat, int delay )
            try
            {
                if (e.Length >= 2)
                {
                    var animstr = e.GetString(0);
                    var actionstr = e.GetString(1);
                    int repeatCount = 1;
                    bool forward = false;
                    bool repeat = false;
                    int delay = 1;
                    if (e.Length >= 3)
                        repeatCount = e.GetInt32(2);
                    if (e.Length >= 4)
                        forward = e.GetBoolean(3);
                    if (e.Length >= 5)
                        repeat = e.GetBoolean(4);
                    if (e.Length >= 6)
                        delay = e.GetInt32(5);
                    var anim = 0;
                    var action = 0;
                    if (int.TryParse(animstr, out anim) && int.TryParse(actionstr, out action))
                    {
                        e.Mobile.Animate(anim, action, repeatCount, forward, repeat, delay);
                        return;
                    }
                    //else if (Enum.TryParse<AnimationType>(animstr, out AnimationType animEnum) && int.TryParse(actionstr, out action))
                    //{
                    //    e.Mobile.Animate((int)(AnimationType)Enum.ToObject(typeof(AnimationType), animEnum), action, repeatCount, forward, repeat, delay);
                    //    return;
                    //}
                }
            }
            catch
            {
                e.Mobile.SendAsciiMessage("Incorrect input, usage 'anim AnimationType action framcount repeatcount '");
            }

        }

        [Usage("SayCheck")]
        [Description("Test say font, hues and etc for ascii message")]
        private static void Say_OnCommand(CommandEventArgs e)
        {
            var t = MessageType.Emote;
            bool ascii = true;
            int hueS = 10;
            int hueE = 11;
            int font = 3;
            for (int i = 0; i < e.Length; i++)
            {
                var attr = e.GetString(i).ToLower();
                if (attr == "u")
                {
                    ascii = false;
                }

                if (attr.StartsWith("f"))
                {
                    attr = attr.Replace("f", "");
                    font = Convert.ToInt32(attr);
                    continue;
                }
                if (attr.StartsWith("m"))
                {
                    attr = attr.Replace("m", "");
                    t = (MessageType)Enum.Parse(typeof(MessageType), attr);
                    continue;
                }

                if (attr.StartsWith("h"))
                {
                    if (attr.Contains("-"))
                    {
                        attr = attr.Replace("h", "");
                        hueS = Convert.ToInt32(attr.Split('-')[0]);
                        hueE = Convert.ToInt32(attr.Split('-')[1]);
                    }
                    else
                    {
                        attr = attr.Replace("h", "");
                        hueS = Convert.ToInt32(attr);
                        hueE = hueS;
                    }
                    continue;
                }
            }
            var mob = e.Mobile;
            for (int h = hueS; h <= hueE; h++)
            {
                if (ascii)
                {
                    Packet p = new AsciiMessage(mob.Serial, mob.Body, t, h, font, "Test", $"T:{t};H:{h};F:{font}. Pack my box with five dozen liquor jugs");
                    p.Acquire();
                    mob.NetState.Send(p);
                }
                else
                {
                    Packet p = new UnicodeMessage(mob.Serial, mob.Body, t, h, font, "","Test", $"T:{(int)t};H:{h};F:{font}. Pack my box with five dozen liquor jugs");
                    p.Acquire();
                    mob.NetState.Send(p);
                }
            }
        }
    }

}