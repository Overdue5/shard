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
        public static Dictionary<PvPRank, int> PvPRankTable = new Dictionary<PvPRank, int>
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

        public static Dictionary<PvMRank, int> PvMRankTable = new Dictionary<PvMRank, int>
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

        public static void Initialize()
        {
            EventSink.PlayerDeath += PvXPointSystem.EventSink_CalculateStats;
        }

        public static void UpdatePvMRank(PlayerMobile mobile)
        {
            PvXData.GetPvMStat(mobile).CalculateRankName();
        }

        public static void UpdatePvPRank(PlayerMobile mobile)
        {
            PvXData.GetPvPStat(mobile).CalculateRankName();
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
            var defStat = PvXData.GetPvPStat(defender);
            var attStat = PvXData.GetPvPStat(attacker);
            if (defStat.TotalPoints >= 0)
            {
                defStat.TotalPointsLost += 1;
            }
            if (EnableResKillTimer == true)
            {
                if (!attStat.LastKilled.ContainsKey(defender.Serial.Value))
                {
                    defStat.TotalLoses += 1;
                    attStat.TotalWins += 1;
                    attacker.SendMessage($"You have gained one pvp point from {defender.Name}");
                    defStat.ResKillTime = DateTime.UtcNow;
                }
                else
                {
                    if (Convert.ToDateTime(attStat.LastKilled[defender.Serial.Value]) + ResKillPvPTime < DateTime.UtcNow)
                    {
                        attStat.TotalResKills += 1;
                        attStat.TotalWins += 1;
                        defStat.TotalLoses += 1;
                        defStat.TotalResKilled += 1;
                    }
                    else
                    {
                        defStat.TotalLoses += 1;
                        attStat.TotalWins += 1;
                        attacker.SendMessage($"You have gained one pvp point from {defender.Name}");
                    }
                }
                defStat.ResKillTime = DateTime.UtcNow;
            }
            else
            {
                defStat.TotalLoses += 1;
                attStat.TotalWins += 1;
                attacker.SendMessage($"You have gained one pvp point from {defender.Name}");
            }

            attStat.LastKilled[defender.Serial.Value] = DateTime.UtcNow;
            defStat.LastKiller[attacker.Serial.Value] = DateTime.UtcNow;
            attStat.LastChangeTime = DateTime.UtcNow;
            defStat.LastChangeTime = DateTime.UtcNow;
        }

        public static void CalculatePvMStat(Mobile attacker, Mobile defender)
        {
            if (!EnablePvMPointSystem)
                return;
            if (attacker is PlayerMobile pa)
            {
                var attStat = PvXData.GetPvMStat(pa);
                var maxPoint = Utility.LimitMinMax(0, defender.HitsMax / 100, 10);
                if (defender.HitsMax > attStat.TotalPoints)
                    maxPoint += 1;
                var rank = attStat.RankId;
                if (DateTime.UtcNow - attStat.ResKillTime > TimeSpan.FromSeconds(3))
                {
                    var bonus = Utility.LimitMinMax(1, maxPoint - rank, 5);
                    if (defender.HitsMax > attStat.TotalWins)
                        bonus += 1;
                    maxPoint = Utility.LimitMinMax(0, maxPoint * (1 + Utility.Random(bonus)), 30);
                }
                pa.SendMessage($"PvM rating increased by {maxPoint}");
                attStat.TotalWins += maxPoint;
                attStat.ResKillTime = DateTime.UtcNow;
                attStat.LastChangeTime = DateTime.UtcNow;
                UpdatePvMRank(pa);
            }
            else if (defender is PlayerMobile pd)
            {
                var defStat = PvXData.GetPvMStat(pd);
                var maxPoint = Utility.LimitMinMax(0, attacker.HitsMax / 100, 10);
                var rank = defStat.RankId;
                var bonus = 1;
                if (rank > maxPoint)
                    bonus = Utility.LimitMinMax(0, rank - maxPoint, 5);
                if (defStat.LastKiller.ContainsKey(attacker.Serial.Value))
                {
                    bonus++;
                    defStat.TotalResKilled += 1;
                }

                maxPoint = maxPoint * Utility.LimitMinMax(1, Utility.Random(bonus), 5);
                pd.SendMessage($"PvM rating decreased by {maxPoint}");
                defStat.LastKiller[attacker.Serial.Value] = DateTime.UtcNow;
                defStat.TotalLoses += maxPoint;
                defStat.LastChangeTime = DateTime.UtcNow;
                UpdatePvMRank(pd);
            }
        }

        public static void EventSink_CalculateStats(PlayerDeathEventArgs w)
        {
            EventSink_CalculateStats(w.Mobile);
        }

        public static void EventSink_CalculateStats(Mobile def)
        {
            foreach (var damageEntry in def.DamageEntries)
            {
                if (damageEntry.Damager is BaseGuard)
                    return;
            }
            EventSink_CalculateStats(def.FindMostRecentDamager(false),def);
        }

        public static void EventSink_CalculateStats(Mobile attacker, Mobile defender)
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