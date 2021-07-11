using System;
using System.Collections.Generic;
using System.Linq;
using Scripts.SpecialSystems;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server
{
    public enum PvPRank
    {
        Ultra_Newb = 0,
        Newbie,
        Novice_Pvper,
        Adempt_Pvper,
        Expert_Pvper,
        Roxxor,
        Hardcore_Roxxor,
        Duelist,
        Novice_Duelist,
        Adempt_Duelist,
        Expert_Duelist,
        Master_Duelist,
        Grandmaster_Duelist,
        Elder_Duelist,
        Legendary_Duelist
    }
    public enum PvMRank
    {
        Butcher = 0,
        Inquisitor,
        Novice_Pvmer,
        Adempt_Pvmer,
        Expert_Pvmer,
        Giant_Slayer,
        Hardcore_Giant_Slayer,
        Slayer,
        Novice_Slayer,
        Adempt_Slayer,
        Expert_Slayer,
        Master_Slayer,
        Duke_of_Britain,
        Elder_Duke_of_Britain,
        Protector_of_Britain
    }

    public class PvXPointSystem
    {
        private static Dictionary<PvPRank, int> PvPRankTable = new Dictionary<PvPRank, int>
        {
            [PvPRank.Ultra_Newb] = 3,
            [PvPRank.Newbie] = 10,
            [PvPRank.Novice_Pvper] = 30,
            [PvPRank.Adempt_Pvper] = 60,
            [PvPRank.Expert_Pvper] = 150,
            [PvPRank.Roxxor] = 250,
            [PvPRank.Hardcore_Roxxor] = 400,
            [PvPRank.Duelist] = 600,
            [PvPRank.Novice_Duelist] = 1000,
            [PvPRank.Adempt_Duelist] = 1500,
            [PvPRank.Expert_Duelist] = 2500,
            [PvPRank.Master_Duelist] = 4000,
            [PvPRank.Grandmaster_Duelist] = 6000,
            [PvPRank.Elder_Duelist] = 10000,
            [PvPRank.Legendary_Duelist] = Int32.MaxValue
        };

        private static Dictionary<PvMRank, int> PvMRankTable = new Dictionary<PvMRank, int>
        {
            [PvMRank.Butcher] = 100,
            [PvMRank.Inquisitor] = 500,
            [PvMRank.Novice_Pvmer] = 1000,
            [PvMRank.Adempt_Pvmer] = 2500,
            [PvMRank.Expert_Pvmer] = 5000,
            [PvMRank.Giant_Slayer] = 9000,
            [PvMRank.Hardcore_Giant_Slayer] = 1500,
            [PvMRank.Slayer] = 22000,
            [PvMRank.Novice_Slayer] = 30000,
            [PvMRank.Adempt_Slayer] = 40000,
            [PvMRank.Expert_Slayer] = 55000,
            [PvMRank.Master_Slayer] = 70000,
            [PvMRank.Duke_of_Britain] = 90000,
            [PvMRank.Elder_Duke_of_Britain] = 120000,
            [PvMRank.Protector_of_Britain] = Int32.MaxValue
        };

        // Enables Point System
        private static readonly bool EnablePvPPointSystem = true;
        private static readonly bool EnablePvMPointSystem = true;

        // Enables Res Kill Timer
        private static readonly bool EnableResKillTimer = true;

        // Enables Res Kill Time
        private static readonly TimeSpan ResKillPvPTime = TimeSpan.FromMinutes(
            Config.Get(@"PvXsystem.ResKillPvPTime",3));
        private static readonly TimeSpan ResKillPvMTime = TimeSpan.FromMinutes(
            Config.Get(@"PvXsystem.ResKillPvMTime", 3));

        // Enables Rank System
        //private static readonly bool EnableRankSystem = true;

        public static PvPRank GetPvPRank(PlayerMobile mobile)
        {
            foreach (var key in PvPRankTable)
            {
                if (key.Value > mobile.PVPStat.TotalPoints)
                {
                    return key.Key;
                }
            }
            Utility.ConsoleWriteLine(Utility.ConsoleMsgType.Error,
                $@"Something wrong with GetPvPRank for {mobile.Name}");
            return 0;
        }

        public static PvMRank GetPvMRank(PlayerMobile mobile)
        {
            foreach (var key in PvMRankTable)
            {
                if (key.Value > mobile.PVMStat.TotalPoints)
                {
                    return key.Key;
                }
            }
            Utility.ConsoleWriteLine(Utility.ConsoleMsgType.Error,
                $@"Something wrong with GetPvMRank for {mobile.Name}");
            return 0;
        }

        private static void UpdatePvMRank(PlayerMobile mobile)
        {
            mobile.PVMStat.RankName = GetPvMRank(mobile).ToString().Replace("_"," ");
        }

        private static void UpdatePvPRank(PlayerMobile mobile)
        {
            mobile.PVPStat.RankName = GetPvPRank(mobile).ToString().Replace("_", " ");
        }

        public static void UpdatePvXRank(PlayerMobile mobile)
        {
            UpdatePvMRank(mobile);
            UpdatePvPRank(mobile);
        }

        public static void CalculatePvPStat(PlayerMobile attacker, PlayerMobile defender)
        {
            if (!EnablePvPPointSystem)
                return;
            if (defender.PVPStat.TotalPoints >= 0)
            {
                defender.PVPStat.TotalPointsLost += 1;
            }
            if (EnableResKillTimer == true)
            {
                if (defender.LastKiller != defender.PVPStat.LastKiller)
                {
                    defender.PVPStat.TotalLoses += 1;
                    attacker.PVPStat.TotalWins += 1;
                    attacker.SendMessage($"You have gained one pvp point from {defender.Name}");
                    defender.PVPStat.ResKillTime = DateTime.UtcNow;
                }
                else
                {
                    if (defender.PVPStat.ResKillTime - DateTime.UtcNow < ResKillPvPTime)
                    {
                        attacker.PVPStat.TotalResKills += 1;
                        attacker.PVPStat.TotalWins += 1;
                        defender.PVPStat.TotalLoses += 1;
                        defender.PVPStat.TotalResKilled += 1;
                    }
                    else
                    {
                        defender.PVPStat.TotalLoses += 1;
                        attacker.PVPStat.TotalWins += 1;
                        attacker.SendMessage($"You have gained one pvp point from {defender.Name}");
                    }
                }
                defender.PVPStat.ResKillTime = DateTime.UtcNow;
            }
            else
            {
                defender.PVPStat.TotalLoses += 1;
                attacker.PVPStat.TotalWins += 1;
                attacker.SendMessage($"You have gained one pvp point from {defender.Name}");
            }

            attacker.PVPStat.LastKilled = defender;
            defender.PVPStat.LastKiller = attacker;
        }

        public static void CalculatePvMStat(Mobile attacker, Mobile defender)
        {
            if (!EnablePvMPointSystem)
                return;
            if (attacker is PlayerMobile pa)
            {
                var maxPoint = Utility.LimitMinMax(0, defender.HitsMax / 100, 10);
                if (defender.HitsMax > pa.PVPStat.TotalPoints)
                    maxPoint += 1;
                var rank = (int)GetPvMRank(pa);
                if (DateTime.UtcNow - pa.PVMStat.ResKillTime > TimeSpan.FromSeconds(3))
                {
                    var bonus = Utility.LimitMinMax(1, maxPoint - rank, 5);
                    if (defender.HitsMax > pa.PVMStat.TotalWins)
                        bonus += 1;
                    maxPoint = Utility.LimitMinMax(0, maxPoint * (1 + Utility.Random(bonus)), 30);
                }
                pa.SendMessage($"PvM rating increased by {maxPoint}");
                pa.PVMStat.TotalWins += maxPoint;
                pa.PVMStat.ResKillTime = DateTime.UtcNow;
                UpdatePvMRank(pa);
            }
            else if (defender is PlayerMobile pd)
            {
                var maxPoint = Utility.LimitMinMax(0, attacker.HitsMax / 100, 10);
                var rank = (int)GetPvMRank(pd);
                var bonus = 1;
                if (rank > maxPoint)
                    bonus = Utility.LimitMinMax(0, rank - maxPoint, 5);
                if (pd.LastKiller == pd.PVMStat.LastKiller)
                {
                    bonus++;
                    pd.PVMStat.TotalResKilled += 1;
                }

                maxPoint = maxPoint * Utility.LimitMinMax(1, Utility.Random(bonus), 5);
                pd.SendMessage($"PvM rating decreased by {maxPoint}");
                pd.PVMStat.LastKiller = attacker;
                pd.PVMStat.TotalLoses += maxPoint;
                UpdatePvMRank(pd);
            }
        }

        public static void CalculateStats(PlayerDeathEventArgs w)
        {
            CalculateStats(w.Mobile);
        }

        public static void CalculateStats(Mobile def)
        {
            foreach (var damageEntry in def.DamageEntries)
            {
                if (damageEntry.Damager is BaseGuard)
                    return;
            }
            CalculateStats(def.FindMostRecentDamager(false),def);
        }

        public static void CalculateStats(Mobile attacker, Mobile defender)
        {
            if (attacker == null || defender == null || (!attacker.Player && !defender.Player) || 
                !attacker.Alive || attacker is BaseGuard || defender is BaseGuard || attacker.Deleted)
                return;

            if ((attacker is BaseCreature && ((BaseCreature) attacker).IsSummonedOrTamed()) ||
                (defender is BaseCreature && ((BaseCreature)defender).IsSummonedOrTamed()))
                return;

            if (attacker.Player && defender.Player)
            {
                CalculatePvPStat(attacker as PlayerMobile, defender as PlayerMobile);
            }
            else
            {
                CalculatePvMStat(attacker, defender);
            }
        }
    }
}