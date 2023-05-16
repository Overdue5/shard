using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Engines.XmlSpawner2;
using Server.Ethics;
using Server.Mobiles;
using Server.Network;

namespace Server.Commands.GMUtils
{
    public static class GMExtendMethods
    {
        public enum EmotionalTextHue
        {
            NormalAction = 33,
            StrangeAction = 1500,
            HeroicAction = 1700,
            PainFeel = 25,
            HappyFeel = 26,
            BoringFeel = 27
        }

        public static void SayAction(this Mobile mob, EmotionalTextHue emote, string text)
        {
            if (!mob.Hidden && mob.AccessLevel < AccessLevel.Counselor)
            {
                string name = "Action";
                if (emote == EmotionalTextHue.BoringFeel || emote == EmotionalTextHue.PainFeel || emote == EmotionalTextHue.HappyFeel)
                    name = "Emote";
                mob.PublicOverheadMessageLos(MessageType.Emote, (int)emote, name, $"** {text} **", true);
            }
        }

        public static void PublicOverheadMessageLos(this Mobile mob, MessageType type, int hue, string name, string text, bool noLineOfSight)
        {
            if (mob.Map != null)
            {
                Packet p = null;


                IPooledEnumerable eable = mob.Map.GetClientsInRange(mob.Location);

                foreach (NetState state in eable)
                {
                    if (state.Mobile!=mob && state.Mobile.CanSee(mob) && mob.InLOS(state.Mobile))
                    {
                        if (p == null)
                        {
                            p = new UnicodeMessage(mob.Serial, mob.Body, type, hue, 3, "", name, text);
                            p.Acquire();
                        }

                        state.Send(p);
                    }
                }

                Packet.Release(p);

                eable.Free();
            }
        }
    }
}
