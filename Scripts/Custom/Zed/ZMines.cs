using Server.Commands;
using Server.Engines.Harvest;
using Server.Items.Construction.Chairs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Custom.Zed
{
    public class BaseMineFloor : Item
    {
        public static readonly int TileId = 220;

        [Constructable]
        public BaseMineFloor() : base(0x53B)
        {
            Weight = 0.0;
            Movable = false;
        }

        public BaseMineFloor(Serial serial) : base(serial)
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

    public class BaseMineWall : Item
    {
        public static readonly int TileId = 220;
        private static int[] m_N_Wall = new int[] { 0x25D, 0x25F,0x261, 0x265, 0x269};
        private static int[] m_S_Wall = new int[] { 0x27E, 0x280};
        private static int[] m_W_Wall = new int[] { 0x25C, 0x25E,0x260, 0x266, 0x267, 0x26A };
        private static int[] m_E_Wall = new int[] { 0x27F, 0x27D };
        private static int[] m_NW_Wall = new int[] { 0x262, 0x263, 0x268 };

        [Constructable]
        public BaseMineWall() : base(Utility.RandomList(m_W_Wall))
        {
            Weight = 0.0;
            Movable = false;
        }

        public void Set_N_Wall()
        {
            ItemID = Get_N_Wall();
        }

        public static int Get_N_Wall()
        {
            return Utility.RandomList(m_N_Wall);
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

        public BaseMineWall(Serial serial) : base(serial)
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
            var size = 7;
            var depth = 30;
            var d = new HarvestDefinition();
            var center = e.Mobile.Location;
            center.Z -= depth;
            for (int x = -size; x <= size; x++)
            {
                for (int y = -size; y <= size; y++)
                {
                    Item m = null;
                    if (y == -size && x == -size)
                    {
                        m = new BaseMineWall();
                        m.ItemID = BaseMineWall.Get_NW_Wall();
                    }
                    else if (y == -size)
                    {
                        m = new BaseMineWall();
                        m.ItemID = BaseMineWall.Get_N_Wall();
                    }
                    else if (x == -size)
                    {
                        m = new BaseMineWall();
                    }
                    else
                        m = new BaseMineFloor();
                    m.Map = e.Mobile.Map;
                    m.X = center.X + x;
                    m.Y = center.Y + y;
                    m.Z = center.Z;
                }
            }
            e.Mobile.Z -= depth;
        }
    }
}
