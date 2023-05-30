using System;
using System.Collections.Generic;
using Scripts.SpecialSystems;
using Server.Mobiles;

namespace Server
{
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

	public class PvXPointSystem
	{
		#region Public Fields

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

		#endregion Public Fields

		#region Private Fields

		private static readonly bool EnablePvMPointSystem = true;

		// Enables Point System
		private static readonly bool EnablePvPPointSystem = true;

		// Enables Res Kill Timer
		private static readonly bool EnableResKillTimer = true;

		private static readonly TimeSpan ResKillPvMTime = TimeSpan.FromMinutes(
					Config.Get(@"PvXsystem.ResKillPvMTime", 3));

		// Enables Res Kill Time
		private static readonly TimeSpan ResKillPvPTime = TimeSpan.FromMinutes(
			Config.Get(@"PvXsystem.ResKillPvPTime", 3));

		#endregion Private Fields

		#region Public Methods

		public static void CalculateStat(double winPoint, double loosePoint, out int winChange, out int looseChange)
		{
			if (winPoint < 0)
				winPoint = 0;
			if (loosePoint < 0)
				loosePoint = 0;
			double w = (loosePoint - winPoint) / 400;
			winChange = Convert.ToInt32(GetEloK(winPoint) * (1 - 1 / (1 + Math.Pow(10, w))));
			looseChange = Convert.ToInt32(GetEloK(loosePoint) * (1 / (1 + Math.Pow(10, -w))));
		}

		private static int GetEloK(double point)
		{
			if (point > 2400)
				return 10;
			else if (point > 1800)
				return 20;
			return 40;
		}

		public static void CalculatePvMStat(Mobile attacker, Mobile defender)
		{
			if (!EnablePvMPointSystem)
				return;
			if (attacker is PlayerMobile pa)
			{
				var attStat = PvXData.GetPvMStat(pa);
				var defPoint = Utility.LimitMinMax(1, defender.HitsMax/3, 3000);
				int bonus = Utility.LimitMinMax(0, defender.HitsMax / 5000 - 1, 2);
				int winChange = 0;
				int looseChange = 0;
				CalculateStat(attStat.TotalPoints / 50, defPoint, out winChange, out looseChange);
				winChange = Utility.LimitMinMax(0, winChange + bonus, 40);
				if (winChange > 0)
					pa.SendMessage($"PvM rating increased by {winChange}");
				attStat.TotalWins += winChange;
				attStat.ResKillTime = DateTime.UtcNow;
				attStat.LastChangeTime = DateTime.UtcNow;
				UpdatePvMRank(pa);
			}
			else if (defender is PlayerMobile pd)
			{
				var defStat = PvXData.GetPvMStat(pd);
				var attackPoint = Utility.LimitMinMax(1, defender.HitsMax / 3, 3000);
				int winChange = 0;
				int looseChange = 0;
				CalculateStat(attackPoint, defStat.TotalPoints / 50, out winChange, out looseChange);
				if (defStat.LastKiller.ContainsKey(attacker.Serial.Value))
				{
					looseChange++;
					defStat.TotalResKilled += 1;
				}
				looseChange = Utility.LimitMinMax(0, looseChange, 40);
				pd.SendMessage($"PvM rating decreased by {looseChange}");
				defStat.LastKiller[attacker.Serial.Value] = DateTime.UtcNow;
				defStat.TotalLoses += looseChange;
				defStat.LastChangeTime = DateTime.UtcNow;
				UpdatePvMRank(pd);
			}
		}

		public static void CalculatePvPStat(PlayerMobile attacker, PlayerMobile defender)
		{
			if (!EnablePvPPointSystem)
				return;
			var defStat = PvXData.GetPvPStat(defender);
			var attStat = PvXData.GetPvPStat(attacker);
			int winChange = 0;
			int looseChange = 0;
			CalculateStat(attStat.TotalPoints / 10, defStat.TotalPoints / 10, out winChange, out looseChange);
            Logs.PvXLog.WriteLine($"PvPStat. Attacker:{attacker}- winChange {winChange}, Defender: {defender} - looseChange {looseChange}");
			if (defStat.TotalPoints >= 0)
			{
				defStat.TotalPointsLost += looseChange;
			}
			if (EnableResKillTimer == true)
			{
				if (!attStat.LastKilled.ContainsKey(defender.Serial.Value))
				{
					defStat.TotalLoses += looseChange;
					attStat.TotalWins += winChange;
					attacker.SendMessage($"You have gained {winChange} pvp point from {defender.Name}");
					defender.SendMessage($"You have lost {looseChange} pvp point");
					defStat.ResKillTime = DateTime.UtcNow;
				}
				else
				{
					if (Convert.ToDateTime(attStat.LastKilled[defender.Serial.Value]) + ResKillPvPTime < DateTime.UtcNow)
					{
						attStat.TotalResKills += 1;
						attStat.TotalWins += winChange;
						defStat.TotalLoses += looseChange;
						defStat.TotalResKilled += 1;
						attacker.SendMessage($"You have gained {winChange} pvp point from {defender.Name}");
						defender.SendMessage($"You have lost {looseChange} pvp point");
					}
					else
					{
						defStat.TotalLoses += looseChange;
						attStat.TotalWins += winChange;
						attacker.SendMessage($"You have gained {winChange} pvp point from {defender.Name}");
						defender.SendMessage($"You have lost {looseChange} pvp point");
					}
				}
				defStat.ResKillTime = DateTime.UtcNow;
			}
			else
			{
				defStat.TotalLoses += looseChange;
				attStat.TotalWins += winChange;
				attacker.SendMessage($"You have gained {winChange} pvp point from {defender.Name}");
				defender.SendMessage($"You have lost {looseChange} pvp point");
			}

			attStat.LastKilled[defender.Serial.Value] = DateTime.UtcNow;
			defStat.LastKiller[attacker.Serial.Value] = DateTime.UtcNow;
			attStat.LastChangeTime = DateTime.UtcNow;
			defStat.LastChangeTime = DateTime.UtcNow;
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
			EventSink_CalculateStats(def.FindMostRecentDamager(false), def);
		}

		public static void EventSink_CalculateStats(Mobile attacker, Mobile defender)
		{
			if (attacker == null || defender == null || (!attacker.Player && !defender.Player) ||
				!attacker.Alive || attacker is BaseGuard || defender is BaseGuard || attacker.Deleted)
				return;

			if ((attacker is BaseCreature && ((BaseCreature)attacker).IsSummonedOrTamed()) ||
				(defender is BaseCreature && ((BaseCreature)defender).IsSummonedOrTamed()))
				return;

			if (attacker.Player && defender.Player)
			{
				if (attacker.AccessLevel==AccessLevel.Player && defender.AccessLevel == AccessLevel.Player)
				    BaseDiscord.Bot.SendToDiscord(BaseDiscord.Channel.PvP, Utility.RandomList(m_DeathMessage).Replace(m_WinName, attacker.Name).Replace(m_LooseName, defender.Name));
				CalculatePvPStat(attacker as PlayerMobile, defender as PlayerMobile);
            }
			else
			{
				CalculatePvMStat(attacker, defender);
			}
		}

        private static string m_WinName = "$Player1$";
        private static string m_LooseName = "$Player2$";

        private static string[] m_DeathMessage = new[]
        {
            "$Player1$ strikes a fatal blow, defeating $Player2$ in a fierce battle.",
            "With a swift and precise strike, $Player1$ vanquishes $Player2$ from existence.",
            "$Player1$ emerges victorious as $Player2$'s life force is extinguished in a clash of blades.",
            "$Player1$ claims dominance by defeating $Player2$, leaving no chance for retaliation.",
            "In a display of superior skill, $Player1$ delivers the final blow, ending $Player2$'s journey.",
            "Clash of warriors,\n$Player1$'s might conquers all,\n$Player2$ meets end.",
            "Battle's fierce dance,\n$Player1$ claims victory's song,\n$Player2$ falls, vanquished.",
            "Steel against steel clash,\n$Player1$'s valor prevails,\n$Player2$'s fate sealed.",
            "Silent shadows weep,\n$Player1$'s blade finds its mark,\n$Player2$'s journey ends.",
            "$Player1$'s victory dance leaves $Player2$ questioning their life choices and contemplating a career change to farming.",
            "The forces of destiny intertwine as $Player1$'s blade claims the spirit of $Player2$.",
            "A prophecy fulfilled: $Player1$, the harbinger of fate, extinguishes the essence of $Player2$.",
            "$Player1$, an embodiment of ethereal might, banishes $Player2$'s soul to realms beyond mortal reach.",
            "The stars whisper secrets of $Player1$'s triumph, as $Player2$'s existence fades into the eternal night."
        };

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

		#endregion Public Methods
	}
}