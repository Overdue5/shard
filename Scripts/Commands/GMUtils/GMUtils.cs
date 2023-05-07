using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Engines.Harvest;
using Server.Items;
using Server.Targeting;
using CommandEventArgs = Server.Commands.CommandEventArgs;
using CommandEventHandler = Server.Commands.CommandEventHandler;

namespace Scripts.Commands
{
    public class GMUtils
    {
        private static Hashtable _PreviousAccessLevel = null;

        public static void Initialize()
        {
            CommandSystem.Register("TurnAccessLevel", AccessLevel.Player, new CommandEventHandler(TurnAccessLevel_OnCommand));
            CommandSystem.Register("TAL", AccessLevel.Player, new CommandEventHandler(TurnAccessLevel_OnCommand));
            CommandSystem.Register("anim", AccessLevel.GameMaster, new CommandEventHandler(Animation_OnCommand));
            CommandSystem.Register("status", AccessLevel.Counselor, new CommandEventHandler(Status_OnCommand));
            CommandSystem.Register("discord", AccessLevel.Counselor, new CommandEventHandler(Discord_OnCommand));
            CommandSystem.Register("online", AccessLevel.Counselor, new CommandEventHandler(Online_OnCommand));
            EventSink.WorldSave += args => { BaseDiscord.Bot.SendToDiscord(BaseDiscord.Channel.WorldChat, GetOnlineReport()); };
#if DEBUG
            CommandSystem.Register("harvestStat", AccessLevel.Player, new CommandEventHandler(CheckHarvestStat_OnCommand));
#endif

        }

        private static string GetOnlineReport()
        {
            int count = World.Mobiles.Values.Count(x => x.NetState != null && x is PlayerMobile pl && pl.AccessLevel == AccessLevel.Player);
            if (count == 0)
                return $"No any avatars in Britain";
            if (count == 1)
                return $"Now only {count} avatar in Britain";
            return $"Now {count} avatars in Britain";
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
                BaseDiscord.StopAsync();
		        Timer.DelayCall(TimeSpan.FromSeconds(5),  ()=>BaseDiscord.MainAsync());
	        }
	        catch (Exception exception)
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
            catch (Exception exception)
            {
                e.Mobile.SendAsciiMessage("Incorrect input, usage 'anim AnimationType action framcount repeatcount '");
            }

        }
    }

}