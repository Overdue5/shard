using Server.Items;
using Server.Mobiles;
using Server.Regions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Custom.Zed
{
    public partial class BtceKeyStone
    {
        private class BtceRegion : GuardedRegion
        {
            private static Map m_Map = Map.Felucca;
            private static Timer m_CloseDoorTimer;
            private static List<BtceGuards> m_GateKeepers = new List<BtceGuards>();
            private static List<Item> m_Items = new List<Item>();
            private static TimeSpan m_GateKeeperMsgDelay = TimeSpan.FromSeconds(30);
            private static DateTime m_GateKeeperNextMsg = DateTime.MinValue;
            private static Point3D m_GateKeeperLoc = new Point3D(1523, 1456, 15);

            public BtceRegion() : base("BTCastleRegion", m_Map, 1, new Rectangle2D(1505, 1405, 40, 55))
            {
            }

            public override void OnRegister()
            {
                base.OnRegister();

                var m = new BtceGuards(1, false);
                m.Map = m_Map;
                m.Name = Utility.RandomList(new List<string>() { "Vulkan", "Eisenhorn", "Mephiston", "Shadowsun" });
                m.Location = m_GateKeeperLoc;
                m.Frozen = true;
                m.Direction = Direction.South;
                m.Fame = 1000;
                m.Karma = 1000;
                m.FightMode = FightMode.None;
                m_GateKeepers.Add(m);
                var shield = m.FindItemOnLayer(Layer.TwoHanded);
                if (shield != null && shield is BaseShield bs)
                    bs.Resource = CraftResource.Gold;

            }

            public void TeleportPlayers(Mobile mobile)
            {
                mobile.Location = BtceSettings.RetreatLocation;
                mobile.Warmode = false;
                mobile.Poison = null;
                Effects.SendLocationEffect(mobile.Location, mobile.Map, 0x372A, 3);
                mobile.PlaySound(0x1FE);
                if (mobile is PlayerMobile pl)
                {
                    mobile.Hits = mobile.HitsMax;
                    foreach (var mob in pl.AllFollowers.Where(m => m.Alive && m.Map == m_Map && m.Region == this))
                    {
                        Effects.SendLocationEffect(mobile.Location, mobile.Map, 0x372A, 3);
                        mobile.PlaySound(0x1FE);
                        mob.Location = BtceSettings.RetreatLocation;
                        mob.Poison = null;
                        mob.Warmode = false;
                    }
                }
            }

            private void CreateDoors()
            {
                for (int i = 0; i < 3; i++)
                {
                    var wall = new VirtualItem(0x93)
                    {
                        Location = new Point3D(1522 + i, 1455, 0),
                        Map = m_Map,
                        Hue = 1152,
                        Name = "castle gate"
                    };
                    m_Items.Add(wall);
                }
            }

            public void CloseDoors()
            {
                CreateDoors();
                m_CloseDoorTimer = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromMilliseconds(500), 15, () =>
                {
                    foreach (var item in m_Items.Where(x => !x.Deleted))
                    {
                        item.Z++;
                        if (item.Z <= 1)
                        {
                            foreach (var mobile in GetPlayers().Where(x => x.X == item.X && x.Y == item.Y && x.Alive))
                            {
                                TeleportPlayers(mobile);
                            }
                        }
                    }
                });
            }

            public void OpenDoors()
            {
                if (m_CloseDoorTimer != null && m_CloseDoorTimer.Running)
                    m_CloseDoorTimer.Stop();
                Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromMilliseconds(500), 25, () =>
                {
                    foreach (var item in m_Items.Where(x => !x.Deleted))
                    {
                        item.Z--;
                    }
                });
            }

            public override void OnUnregister()
            {
                foreach (var mob in m_GateKeepers)
                    mob.Delete();
                foreach (var i in m_Items)
                    i.Delete();
                base.OnUnregister();
            }

            public override bool OnDamage(Mobile mobile, ref int damage)
            {
                if (m_GateKeepers.Contains(mobile))
                    return false;
                if (mobile != null && mobile.Alive && mobile.Hits - damage <= 1)
                {
                    if (mobile is BaseCreature bs)
                    {
                        if (bs.LastOwner != null && !bs.Summoned)
                        {
                            TeleportPlayers(mobile);
                            return false;
                        }
                        if (bs.Summoned)
                        {
                            bs.Map = Map.Internal;
                            Effects.SendLocationEffect(mobile.Location, mobile.Map, 0x372A, 3);
                            bs.Delete();
                            return false;
                        }
                    }
                    
                    if (mobile is PlayerMobile pm && pm.Hits - damage <= 1)
                    {
                        TeleportPlayers(mobile);
                        return false;
                    }
                    if (mobile is BtceGuards)
                    {
                        Effects.SendLocationEffect(mobile.Location, mobile.Map, 0x372A, 3);
                        var attacker = mobile.FindMostRecentDamager(false);
                        if (attacker != null)
                        {
                            if (attacker is BaseCreature bc && bc.Alive && bc.LastOwner != null && bc.LastOwner is PlayerMobile apm)
                            {
                                attacker = apm;
                            }
                            BtStatistic.AddKill(attacker);
                        }
                        mobile.Map = Map.Internal;
                        mobile.Location = Point3D.Zero;
                        mobile.Delete();
                        return false;
                    }
                    BtLogs.Write($"Strange mobile for kill {mobile}, Id :{mobile.Body.BodyID}, type:{mobile.GetType()}");
                }
                return base.OnDamage(mobile, ref damage);
            }

            public override TimeSpan GetLogoutDelay(Mobile m)
            {
                return TimeSpan.FromSeconds(5);
            }

            public override void OnEnter(Mobile m)
            {
                if (BtceTimer.Started && !(m is BtceGuards) && m.AccessLevel < AccessLevel.Counselor && !(m is BaseCreature bs && bs.Summoned))
                {
                    m.Location = BtceSettings.RetreatLocation;
                    m.Map = m_Map;
                    if (m_GateKeeperNextMsg <= DateTime.UtcNow)
                    {
                        Utility.RandomList(m_GateKeepers).Say("Hear ye! The Lord Blackthorne's guard is engaged in intensive training sessions within the castle walls" +
                                                          ". Entry is strictly prohibited to ensure the utmost focus and security.");
                        m_GateKeeperNextMsg = DateTime.UtcNow + m_GateKeeperMsgDelay;
                    }
                    return;
                }

                base.OnEnter(m);
            }

            public override void OnExit(Mobile m)
            {
                BtceTimer.RemovePlayer(m);
                base.OnExit(m);
            }

        }
    }
}
