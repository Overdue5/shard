using System;
using System.Linq;
using System.Collections;
using Server;
using Server.Commands;
using Server.Mobiles;

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