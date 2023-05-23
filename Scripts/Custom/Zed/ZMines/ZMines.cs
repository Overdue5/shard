using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Server.Commands;
using Server.Engines;
using Server.Engines.Harvest;
using Server.Engines.Quests.Ambitious;
using Server.Items;
using static Server.Scripts.Custom.Adds.System.TipSystem;

namespace Server.Custom.Zed
{
    public class BaseMine : Item
    {
        public static readonly int TileId = 220;
        private byte r_WarningLevel;
        public BaseMine() : base()
        {
            Weight = 0.0;
            Movable = false;
        }

        public BaseMine(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            if ((DateTime.UtcNow - LastMoved) < UtilityWorldTime.MonthTime)
                r_WarningLevel++;
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }
    }

    public class MineFloor : BaseMine
    {

        [Constructable]
        public MineFloor() : base(0x53B)
        {
            Weight = 0.0;
            Movable = false;
            LastMoved = DateTime.UtcNow;

        }

        public void CheckBorder()
        {
            var items = Map.GetItemsInRange(this.Location, 1);
            var around = new HashSet<Item>();
            foreach (Item item in items)
            {
                if (item.Z == this.Z && item is MineFloor || item is MineWall)
                {
                    around.Add(item);
                }

            }
            items.Free();
            if (around.Count < 9)
            {
                for (int x = -1; x <= 1; x ++)
                {
                    for (int y = -1; y <= 1; y ++)
                    {
                        if (Math.Abs(x) == Math.Abs(y) )
                        {
                            if (x == -1 && y == -1 && around.Count(m => m.X == this.X + x && m.Y == this.Y + y) == 0 && around.Count(m => m.X == this.X + x && m.Y == this.Y - y && m is MineFloor) == 0 && around.Count(m => m.X == this.X  && m.Y == this.Y - 1 && m is MineFloor ) == 0)
                            {
                                var steps = MineWall.WallMap[x + 1][y + 1];
                                foreach (var id in steps)
                                {
                                    var w  = new MineWall
                                    {
                                        ItemID = id(),
                                        Location = new Point3D(X + x, Y + y, Z),
                                        Map = Map,
                                        Name = $"{x}:{y}"
                                    };
                                    around.Add(w);
                                }
                            }

                        }
                        else if (around.Count(m => m.X == this.X +x && m.Y == this.Y + y) == 0)
                        {
                            var steps = MineWall.WallMap[x+1][y+1];
                            foreach (var id in steps)
                            {
                                var w = new MineWall
                                {
                                    ItemID = id(),
                                    Location = new Point3D(X + x, Y + y, Z),
                                    Map = Map,
                                    Name = $"{x}:{y}"
                                };
                                around.Add(w);
                            }
                        }
                    }
                }
            }

            
        }

        public override void OnDoubleClick(Mobile from)
        {
            this.CheckBorder();
        }

        public MineFloor(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }
    }

    public class MineWall : BaseMine
    {
        private static int[] m_N_Wall = new int[] { 0x25D, 0x25F,0x261, 0x265, 0x269};
        private static int[] m_S_Wall = new int[] { 0x27E, 0x280};
        private static int[] m_W_Wall = new int[] { 0x25C, 0x25E,0x260, 0x266, 0x267, 0x26A };
        private static int[] m_E_Wall = new int[] { 0x27F, 0x27D };
        private static int[] m_NW_Wall = new int[] { 0x262, 0x263, 0x268 };
        public delegate int WallDelegate();
        public static List<WallDelegate>[][] WallMap;

        private int m_Damaged;

        public static void Initialize()
        {
            WallMap = new List<WallDelegate>[3][];
            WallMap[0] = new List<WallDelegate>[3];
            WallMap[0][0] = new List<WallDelegate>() { Get_NW_Wall };
            WallMap[0][1] = new List<WallDelegate>() { Get_W_Wall};
            WallMap[0][2] = new List<WallDelegate>() { Get_S_Wall};

            WallMap[1] = new List<WallDelegate>[3];
            WallMap[1][0] = new List<WallDelegate>() { Get_N_Wall };
            WallMap[1][1] = new List<WallDelegate>() { Get_W_Wall };
            WallMap[1][2] = new List<WallDelegate>() { Get_S_Wall};

            WallMap[2] = new List<WallDelegate>[3];
            WallMap[2][0] = new List<WallDelegate>() { Get_E_Wall};
            WallMap[2][1] = new List<WallDelegate>() { Get_E_Wall};
            WallMap[2][2] = new List<WallDelegate>() { Get_S_Wall};
        }
        
        [CommandProperty(AccessLevel.GameMaster)]
        public int GetMaxDrability => Math.Abs(Z) + 30;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Damaged
        {
            get => m_Damaged;
            set => m_Damaged = value;
        }

        [Constructable]
        public MineWall() : base(Utility.RandomList(m_W_Wall))
        {
            Weight = 0.0;
            Movable = false;
        }

        public MineWall(Serial serial) : base(serial)
        {
        }

        
        
        public void Set_N_Wall()
        {
            ItemID = Get_N_Wall();
        }

        public static int Get_N_Wall()
        {
            return Utility.RandomList(m_N_Wall);
        }

        public static int Get_S_Wall()
        {
            return Utility.RandomList(m_S_Wall);
        }

        public void Set_W_Wall()
        {
            ItemID = Get_W_Wall();
        }

        public static int Get_W_Wall()
        {
            return Utility.RandomList(m_W_Wall);
        }

        public void Set_NW_Wall()
        {
            ItemID = Get_NW_Wall();
        }

        public static int Get_NW_Wall()
        {
            return Utility.RandomList(m_NW_Wall);
        }

        public static int Get_E_Wall()
        {
            return Utility.RandomList(m_E_Wall);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }
    }

    public class MineCenter : Item
    {
        
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner
        {
            get { return m_Owner; }
            set { m_Owner = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Teleporter Teleport
        {
            get { return m_Telepor; }
            set { m_Telepor = value; }
        }

        public static Dictionary<MineCenter, Hashtable> m_MineItems = new Dictionary<MineCenter, Hashtable>();
        private Mobile m_Owner;
        private Teleporter m_Telepor;
        [Constructable]
        public MineCenter() : base(0x1183)
        {
            Weight = 0.0;
            Movable = false;
            m_MineItems[this] = new Hashtable();
        }

        public MineCenter(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_Owner == null)
            {
                m_Owner = from;

                var size = 5;
                var depth = 23;
                var center = this.Location;
                center.Z -= depth;
                m_Telepor = new Teleporter();
                m_Telepor.MapDest = Map;
                m_Telepor.Map = Map;
                m_Telepor.Location = center;
                m_Telepor.PointDest = from.Location;
                var floor = new HashSet<MineFloor>();

                for (int x = -size; x <= size; x++)
                {
                    for (int y = -size; y <= size; y++)
                    {
                        var m = new MineFloor();
                        m.Map = from.Map;
                        m.X = center.X + x;
                        m.Y = center.Y + y;
                        m.Z = center.Z;
                        floor.Add(m);
                    }
                }

                for (int x = -1; x <= 1; x+=2)
                {
                    for (int y = -1; y <= 1; y+=2)
                    {
                        _ = new RoofSupport
                        {
                            Location = new Point3D(center.X + x * 3, center.Y + y * 3, center.Z),
                            Map = Map,
                            Movable = false
                        };
                    }
                }

                foreach (var item in floor)
                    item.CheckBorder();
                

                
                from.Location = center;
            }
            else
            {
                from.Location = m_Telepor.Location;
            }
        }

        public void GenerateWall()
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }

        private void GenerateWalls()
        {
        }
    }

    public class ZMines
    {
        public static void Initialize()
        {
            CommandSystem.Register("tmine", AccessLevel.Owner, new CommandEventHandler(CreateMine));
        }

        [Usage("tmine")]
        [Description("Create mine")]
        private static void CreateMine(CommandEventArgs e)
        {
            
        }
    }
}
