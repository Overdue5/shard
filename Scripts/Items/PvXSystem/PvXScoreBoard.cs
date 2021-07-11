using System;
using System.Collections.Generic;
using Scripts.SpecialSystems;
using Server; 
using Server.Gumps; 
using Server.Network; 
using Server.Menus; 
using Server.Menus.Questions;
using Server.Mobiles;

namespace Server.Items
{
    [FlipableAttribute(7774, 7775)]
    public class PvPScoreBoard : PvXScoreBoard
    {
        [Constructable]
        public PvPScoreBoard() : base(7774)
        {
            Movable = false;
            Name = "Pvp Score Board";
            boardType = PvXType.PVP;
            Hue = Config.Get(@"PvXsystem.PvPScoreBoard_Hue", 0);
        }

        public PvPScoreBoard(Serial serial) : base(serial)
        {
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            boardType = PvXType.PVP;
        }
    }

    [FlipableAttribute(7774, 7775)]
    public class PvMScoreBoard : PvXScoreBoard
    {
        [Constructable]
        public PvMScoreBoard() : base(7774)
        {
            Movable = false;
            Name = "Pvm Score Board";
            boardType = PvXType.PVM;
            Hue = Config.Get(@"PvXsystem.PvMScoreBoard_Hue", 0);
        }

        public PvMScoreBoard(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version 
        }

        public override void Deserialize(GenericReader reader)
        {
            boardType = PvXType.PVM;
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
    
    public abstract class PvXScoreBoard : Item
    {
        public PvXType boardType;
        protected static Dictionary<PvXType, int> m_textHueId;

        [CommandProperty(AccessLevel.Owner)]
        public int TextHueID
        {
            get { return PvXScoreBoard.m_textHueId[boardType]; }
            set
            {
                m_textHueId[boardType] = value;
                Config.Set($@"PvXsystem.{boardType.ToString()}_Text_Hue", value);
            }
        }

        public static void Initialize()
        {
            m_textHueId = new Dictionary<PvXType, int>();
            m_textHueId[PvXType.PVM] = Config.Get(@"PvXsystem.PVM_Text_Hue", 0);
            m_textHueId[PvXType.PVP] = Config.Get(@"PvXsystem.PVP_Text_Hue", 0);
        }
        
        public override void OnDoubleClick( Mobile from ) 
		{ 
            if ( from.InRange( this.GetWorldLocation(), 2 ) ) 
			{ 
				from.CloseGump( typeof(OverallPvXGump) );
				from.SendGump( new OverallPvXGump( from, 0, null, null, boardType ) );
			} 
		}

        public static int GetTextHueId(PvXType pvxType)
        {
            return m_textHueId[pvxType];
        }

        public PvXScoreBoard(int id) : base(id)
        {
        }

        public PvXScoreBoard(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            //writer.Write((int)0); // version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            //int version = reader.ReadInt();
        }
    } 
} 