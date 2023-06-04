using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using Server.Commands.GMUtils;
using Server.Items;

namespace Server.Custom.Zed
{
    public partial class BtceKeyStone
    {
        private class BtceTimer : Timer
        {
            private DateTime m_Start;
            private BtceRegion m_Region;
            private static Timer m_AnnounceTimer;
            private static List<Mobile> m_Players = new List<Mobile>();
            private static List<Mobile> m_BeginPlayers = new List<Mobile>();
            private static List<Timer> m_WavesTimer = new List<Timer>();
            private static bool m_Started;
            private static BtceTimer m_BtTimer;
            public static int TotalGuardsNumber;
            public static HashSet<BtceGuards> Guards = new HashSet<BtceGuards>();
            public static bool Started => m_Started;
            public Map EventMap => m_Region.Map;

            public static BtceTimer GetTimer()
            {
                if (m_BtTimer == null)
                    m_BtTimer = new BtceTimer();
                return m_BtTimer;
            }

            private BtceTimer() : base(TimeSpan.Zero, TimeSpan.FromSeconds(5))
            {
                m_Region = new BtceRegion();
                m_Start = DateTime.UtcNow;
                m_Started = false;
                m_Region.Disabled = true;
                TotalGuardsNumber = 0;
                m_Region.Register();
            }

            public static void RemovePlayer(Mobile m)
            {
                if (m_Started && m_Players.Contains(m))
                {
                    m_Players.Remove(m);
                    BtLogs.Write(m.Location == BtceSettings.RetreatLocation ? $"{m.Name} was killed on event" : $"{m.Name} left event");
                }
            }

            public static bool PlayerExist(Mobile m)
            {
                return m_Started && m_Players.Contains(m);
            }

            public static int GetPlayersCount()
            {
                return m_Players.Count;
            }

            protected override void OnTick()
            {
                if (m_AnnounceTimer == null) m_AnnounceTimer = RunAnnouncement();

                if (m_AnnounceTimer.Running)
                    return;

                if (!m_Started)
                {
                    if (!m_Region.GetPlayers().Any())
                    {
                        BtLogs.Write("No any players, event stopped");
                        World.Broadcast(BtceSettings.BroadCastHue, true, $"Hear ye! The training session scheduled by the Lord Blackthorne's guard stands canceled, for it seems the turnout has been insufficient.");
                        StopEvent();
                        return;
                    }
                    InitEvent();
                }
                var players = m_Region.GetPlayers().Count(m => m.AccessLevel < AccessLevel.Counselor && m.Alive && m.NetState!=null);

                if (m_Started && players == 0)
                {
                    BtLogs.Write("All players dead, stop event");
                    SendMessagesToAll($"Hear ye! The training has concluded, as the valiant guards successfully repelled the impending attack.");
                    StopEvent();
                    return;
                }

                if (m_Started && Guards.Count == 0 && m_WavesTimer.Count(x=>x.Running) == 0)
                {
                    BtLogs.Write($"All guards dead, {players} live players left: {m_Players.Aggregate("", (current, pl) => current + $"{pl.Name} ({pl.Account?.Username}:{pl.NetState.Address}),")}");
                    SendMessagesToAll($"Hear ye! The training session has come to an end, but unfortunate news follows. The castle has been seized by the cunning attackers.");
                    GetBenefits();
                    StopEvent();
                    return;
                }

                if (m_Started && Guards.Count == 0 && m_WavesTimer.Count(x => x.Running) > 0 && m_WavesTimer.Count(x => !x.Running) > 0 && m_WavesTimer.Count(x => x.Delay == TimeSpan.Zero && x.Running) == 0)
                {
                    var timer = m_WavesTimer.Where(x=>x.Running).OrderBy(x => x.Delay).FirstOrDefault();
                    timer.Delay = TimeSpan.Zero;
                    timer.Stop();
                    timer.Start();
                }

                if (m_Started && Guards.Count != 0 && DateTime.UtcNow - m_Start > BtceSettings.EventDuration)
                {
                    BtLogs.Write($"Not all guards killed, {Guards.Count}, {players} live players left: {m_Players.Aggregate(",", (current, pl) => current + $"{pl.Name} ({pl.Account?.Username})")}");
                    SendMessagesToAll($"Behold! The training hath reached its conclusion, and the castle hath withstood the invaders' grasp.");
                    StopEvent();
                }
            }

            private Timer RunAnnouncement()
            {
                return Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromMinutes(1), () =>
                {
                    var delta = (int)(BtceSettings.AnnounceTimer.TotalMinutes - (DateTime.UtcNow - m_Start).TotalMinutes);
                    if (delta == 0 || delta == 14 || delta % 5 == 0 || delta <=3)
                        if (delta == 0)
                            SendMessagesToLiving($"Hearken, fearless fighters! Equip yourselves, for the combat drills are about to begin.");
                        else
                            World.Broadcast(BtceSettings.BroadCastHue, true, $"Hear ye, brave warriors! Lord Blackthorne's loyal guard summons all who seek combat training. Gather at the castle gates in {delta} minutes hence!");
                    if ((DateTime.UtcNow - m_Start).TotalSeconds >= BtceSettings.AnnounceTimer.TotalSeconds) m_AnnounceTimer.Stop();

                });
            }

            private void InitEvent()
            {
                m_Region.CloseDoors();
                foreach (var ghost in m_Region.GetPlayers().Where(m => m.AccessLevel < AccessLevel.Counselor && !m.Alive).ToList())
                {
                    m_Region.TeleportPlayers(ghost);
                }
                m_Start = DateTime.UtcNow;
                var delay = TimeSpan.Zero;
                m_Players = m_Region.GetPlayers().Where(m => m.AccessLevel < AccessLevel.Counselor && m.Alive).ToList();
                m_BeginPlayers = m_Region.GetPlayers().Where(m => m.AccessLevel < AccessLevel.Counselor).ToList();
                BtStatistic.AddAttempt(m_BeginPlayers);
                foreach (var wave in BtceSettings.Waves)
                {
                    var timer = new BtceWaveTimer(delay, this, wave);
                    m_WavesTimer.Add(timer);
                    timer.Start();
                    delay += wave.WaveDuration;
                }
                BtLogs.Write($"Event started, total {m_Players.Count}: {m_Players.Aggregate(",", (current, pl) => current + $"{pl.Name} ({pl.Account?.Username})")}");
                m_Started = true;
            }

            public void SendMessagesToLiving(string text)
            {
                foreach (var player in m_Region.GetPlayers())
                {
                    player.SendAsciiMessage(BtceSettings.BroadCastHue, text);
                }
            }

            public void SendMessagesToAll(string text)
            {
                foreach (var player in m_Region.GetPlayers())
                {
                    player.SendAsciiMessage(BtceSettings.BroadCastHue, text);
                }
            }

            private void StopEvent()
            {
                Timer.DelayCall(TimeSpan.FromMinutes(1), () =>
                {
                    m_Region.Disabled = false;
                    m_Region.Unregister();
                });
                m_Region.OpenDoors();
                foreach (var timer in m_WavesTimer)
                {
                    timer.Stop();
                }
                foreach (var mobile in Guards.ToArray())
                {
                    mobile.Delete();
                }
                m_WavesTimer.Clear();
                Guards.Clear();
                Stop();
                m_BtTimer = null;
                m_AnnounceTimer = null;
            }

            private void GetBenefits()
            {
                if (m_Players.Count == 0) return;
                    var benefit = Utility.LimitMinMax(BtceSettings.MinGoldPerPLayer, TotalGuardsNumber * BtceSettings.GoldPerGuard / m_Players.Count, BtceSettings.MaxGoldPerPlayer);
                if (m_Players.Count > 1)
                    SendMessagesToLiving("Hello brave avatars! The champions among us are bestowed by Lord Blackthorn himself, who rewards them lavishly for exceptional training.");
                else
                    SendMessagesToLiving("Hello warrior! A gift from Lord Blackthorn for exceptional training.");
                BtLogs.Write($"Benefit +{benefit} gold, Valor + {BtceSettings.ValorBonus}");
                foreach (var player in m_Players)
                {
                    if (player is PlayerMobile pl)
                    {
                        pl.Virtues.Valor += BtceSettings.ValorBonus;
                        pl.AddGoldToBackPack(benefit);
                        pl.Backpack.AddItem(new BagOfReagents());
                        BtStatistic.AddGold(pl, benefit);
                        BtStatistic.AddWin(pl);
                    }
                }
            }
        }

        private class BtceWaveTimer : Timer
        {

            private BtceTimer m_MainTimer;
            private BtceSettings.GuardsWaves m_Wave;
            private int m_Count;
            public BtceWaveTimer(TimeSpan delay, BtceTimer maintimer, BtceSettings.GuardsWaves wave) : base(delay, TimeSpan.FromSeconds(1))
            {
                m_MainTimer = maintimer;
                m_Wave = wave;
                m_Count = 0;
            }
            protected override void OnTick()
            {
                if (m_Count < BtceSettings.WaveAnnouncementTimer.TotalSeconds)
                {
                    var delta = BtceSettings.WaveAnnouncementTimer.TotalSeconds - m_Count++;
                    if (delta % 5 == 0 || delta <5)
                        if (m_Wave.WaveNumber == 1)
                            m_MainTimer.SendMessagesToLiving($"Hear me, noble avatars! Prepare yourselves, for the guards shall arrive in a mere {delta} seconds.");
                        else
                            m_MainTimer.SendMessagesToLiving($"Hear me, avatars! Be vigilant, for the aid to the guards set to arrive in a mere {delta} seconds!");
                }
                else
                {
                    int guardToCreate = m_Wave.GuardsCount;
                    if (BtceTimer.GetPlayersCount() > 10)
                        guardToCreate += 10;
                    else if (BtceTimer.GetPlayersCount() > 5)
                        guardToCreate += 5;
                    BtLogs.Write($"Wave {m_Wave.WaveNumber}, Total players:{BtceTimer.GetPlayersCount()}, Guards To create :{guardToCreate}, Total Guards:{BtceTimer.Guards.Count}");
                    BtceTimer.TotalGuardsNumber += guardToCreate;
                    
                    for (int i = 0; i < guardToCreate; i++)
                    {
                        var m = new BtceGuards(m_Wave.Difficult);
                        m.Map = m_MainTimer.EventMap;
                        m.Location = Utility.RandomList(BtceSettings.SpawnLocation);
                    }
                    Stop();
                }
            }
        }
    }
}
