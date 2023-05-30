using Server.Commands;
using Server.Commands.Generic;
using Server.Items;
using Server.Spells;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Mobiles;

namespace Server.Custom.Zed
{
    public static class CurseOfGods
    {
        private class SetCurse : BaseCommand
        {
            public SetCurse()
            {
                AccessLevel = AccessLevel.Owner;
                Supports = CommandSupport.Simple;
                Commands = new string[] { "SetCurse"};
                ObjectTypes = ObjectTypes.Mobiles;
                Usage = "SetCurse";
                Description = "Curse bad avatars";
            }

            public override void Execute(CommandEventArgs e, object obj)
            {
                IPoint3D p = obj as IPoint3D;

                if (p == null)
                    return;

                if (obj is PlayerMobile pl)
                {
                    CurseOfGods.AddMobileWithLimit(pl);
                }
            }
        }

        private class RemoveCurse : BaseCommand
        {
            public RemoveCurse()
            {
                AccessLevel = AccessLevel.Owner;
                Supports = CommandSupport.Simple;
                Commands = new string[] { "DelCurse" };
                ObjectTypes = ObjectTypes.Mobiles;
                Usage = "DelCurse";
                Description = "Remove the curse from a reformed avatars";
            }

            public override void Execute(CommandEventArgs e, object obj)
            {
                IPoint3D p = obj as IPoint3D;

                if (p == null)
                    return;

                if (obj is PlayerMobile pl )
                {
                    CurseOfGods.RemoveMobile(pl);
                }
            }
        }

        public static readonly string StatisticFileName = "CurseOfGods.json";
        private class TimeLimits
        {
            public DateTime EndCurseTime;
            public DateTime NextAnnouncement;
            private static TimeSpan m_AnnouncementLimet = TimeSpan.FromDays(1);

            public TimeLimits()
            {
                EndCurseTime = DateTime.UtcNow;
                NextAnnouncement = DateTime.MinValue;
            }

            public void UpdAnnouncement()
            {
                NextAnnouncement = DateTime.UtcNow + m_AnnouncementLimet;
            }

            public bool IsCursed => EndCurseTime > DateTime.UtcNow;

        }

        private static Dictionary<Mobile, TimeLimits> m_MobileList = new Dictionary<Mobile, TimeLimits>();
        private static List<string> m_CurseMessages;

        public static void Initialize()
        {
            TargetCommands.Register(new SetCurse());
            TargetCommands.Register(new RemoveCurse());
            //CommandSystem.Register("CoGAdd", AccessLevel.Owner, CoGAdd_OnCommand);
            EventSink.WorldSave += SaveStat;
            LoadStat();
            m_CurseMessages = new List<string>()
            {
                "A shiver runs down your spine, as if unseen spirits whisper tales of ill omens in your ear.",
                "A chill breeze sweeps past, causing your hair to stand on end, as if the specters of the departed brush against your soul.",
                "A sense of unease washes over you, as if the ethereal realm intersects with the mortal plane, sending a shudder through your being.",
                "A haunting sensation fills the air, as if restless souls reach out to touch the living, leaving an unsettling presence in their wake.",
                "A ghostly specter passes through your consciousness, leaving behind an eerie sensation, as if the veil between life and death has momentarily thinned.",
                "An ominous hush befalls the surroundings, as if the unseen forces of the netherworld pause to acknowledge your presence.",
                "A faint whisper echoes in the air, carrying the weight of forgotten souls, causing a shudder to course through your veins.",
                "The ambient temperature drops suddenly, as if the ethereal realm manifests its presence, sending a shiver down your spine.",
                "A spectral mist drifts by, enveloping you in a chilling embrace, as if the spirits of the departed reach out to touch the living.",
                "The sound of distant lamentations fills your ears, an eerie melody that sends a ripple of unease through your core, as if the realm of the dead calls out to you."
            };
        }

        private static void LoadStat()
        {
            string path = Path.Combine(World.CustomDataPath, CurseOfGods.StatisticFileName);
            if (File.Exists(path))
            {
                string readText = File.ReadAllText(path);
                var temp = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, TimeLimits>>(readText).Where(x=>x.Value.EndCurseTime>DateTime.UtcNow).ToDictionary(x=>x.Key, x=>x.Value);
                foreach (string tempKey in temp.Keys)
                {
                    if (temp[tempKey].EndCurseTime<DateTime.UtcNow)
                        continue;
                    var serial = Convert.ToInt32(tempKey.Split(' ')[0], 16);
                    m_MobileList[World.FindMobile(serial)] = temp[tempKey];
                }
            }

        }

        private static void SaveStat(WorldSaveEventArgs e)
        {
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(m_MobileList);
            string path = Path.Combine(World.CustomDataPath, CurseOfGods.StatisticFileName);
            File.WriteAllText(path, jsonString);
        }


        private static void SendAnnouncement(Mobile mob)
        {
            if (m_MobileList.ContainsKey(mob) && m_MobileList[mob].NextAnnouncement <DateTime.UtcNow)
            {
                mob.SendAsciiMessage(34, Utility.RandomList(m_CurseMessages));
                m_MobileList[mob].UpdAnnouncement();
            }
        }

        public static void AddMobile(Mobile mob, TimeSpan curseLength)
        {
            if (!m_MobileList.ContainsKey(mob))
                m_MobileList[mob] = new TimeLimits();
            else if (m_MobileList[mob].EndCurseTime < DateTime.UtcNow)
                m_MobileList[mob].EndCurseTime = DateTime.UtcNow;
            m_MobileList[mob].EndCurseTime += curseLength;
            SendAnnouncement(mob);
        }

        public static void AddMobile(Mobile mob)
        {
            AddMobile(mob, TimeSpan.FromDays(1));
        }

        public static void AddMobileWithLimit(Mobile mob)
        {
            AddMobileWithLimit(mob, TimeSpan.FromDays(1));
        }

        public static void AddMobileWithLimit(Mobile mob, TimeSpan limitCurseLength)
        {
            if (!m_MobileList.ContainsKey(mob))
                m_MobileList[mob] = new TimeLimits();
            if (m_MobileList[mob].EndCurseTime < DateTime.UtcNow + limitCurseLength)
                m_MobileList[mob].EndCurseTime = DateTime.UtcNow + limitCurseLength;
            SendAnnouncement(mob);
        }

        public static void RemoveMobile(Mobile mob)
        {
            if (m_MobileList.ContainsKey(mob))
            {
                m_MobileList.Remove(mob);
                mob.FixedParticles(0x376A, 10, 15, 5007, EffectLayer.Waist);
            }
        }

        public static bool IsCursed(this Mobile mob)
        {
            return m_MobileList.ContainsKey(mob) && m_MobileList[mob].IsCursed;
        }

        public static double GetCursedSkillBase(this Mobile mob, SkillName skillName)
        {
            if (mob.IsCursed())
                return mob.Skills[skillName].Base/Utility.Random(2,4);
            return mob.Skills[skillName].Base;
        }

        public static double GetCursedSkillValue(this Mobile mob, SkillName skillName)
        {
            if (mob.IsCursed())
                return mob.Skills[skillName].Value / Utility.Random(2, 4);
            return mob.Skills[skillName].Value;
        }
    }
}
