using System;
using System.Collections.Generic;
using Server.Logging;
using Server.Mobiles;
using Server.Scripts.Custom.Adds.System.KillInfo.DatabaseEntries;

namespace Server.Scripts.Custom.Adds.System.KillInfo
{
    public static class KillInfo
    {
        private static List<KillEntry> m_KillEntries = new List<KillEntry>();

        public static void Initialize()
        {
            //Initialize KilLInfo system by inserting all the monster types into the data base
        }

        public static void AddKillInfo(BaseCreature creatureKilled, List<DamageEntry> damageEntries, Dictionary<Mobile, int> mobilesKilled, List<Item> lootItems)
        {
            //m_KillEntries.Add(new KillEntry(creatureKilled, damageEntries, mobilesKilled, lootItems));
        }

        /// <summary>
        /// This should be something like a save method. Will store all the KillEntries to the database and reset the list
        /// </summary>
        public static void PrintResults()
        {
            ConsoleLog.Write.Information(m_KillEntries.Count.ToString());
            try
            {
                foreach (KillEntry entry in m_KillEntries)
                {
                    ConsoleLog.Write.Information(entry.ToString());
                }
            }
            catch (Exception ex)
            {
                ConsoleLog.Write.Error("Error, resetting results.");
                ConsoleLog.Write.Error(ex.StackTrace);
                ResetResults();
            }
        }

        public static void ResetResults()
        {
            m_KillEntries = new List<KillEntry>();
        }
    }
}