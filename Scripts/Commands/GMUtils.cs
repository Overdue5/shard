using System;
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
            Register("TurnAccessLevel", AccessLevel.Player, new CommandEventHandler(TurnAccessLevel_OnCommand));
        }

        public static void Register(string command, AccessLevel access, CommandEventHandler handler)
        {
            CommandSystem.Register(command, access, handler);
        }

        [Usage("TurnAccessLevel")]
        [Description("Переключает права доступа на Player и обратно.")]
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

            var access = (AccessLevel)Math.Min((int)e.Mobile.AccessLevel, (int)e.Mobile.Account.AccessLevel);
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

    }

}