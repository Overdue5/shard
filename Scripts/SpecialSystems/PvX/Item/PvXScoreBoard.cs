using System.Collections.Generic;
using Scripts.SpecialSystems;
using Server.Gumps;

namespace Server.Items
{
	[FlipableAttribute(7774, 7775)]
	public class PvMScoreBoard : PvXScoreBoard
	{
		#region Public Constructors

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

		#endregion Public Constructors

		#region Public Methods

		public override void Deserialize(GenericReader reader)
		{
			boardType = PvXType.PVM;
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version
		}

		#endregion Public Methods
	}

	[FlipableAttribute(7774, 7775)]
	public class PvPScoreBoard : PvXScoreBoard
	{
		#region Public Constructors

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

		#endregion Public Constructors

		#region Public Methods

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			boardType = PvXType.PVP;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version
		}

		#endregion Public Methods
	}

	public abstract class PvXScoreBoard : Item
	{
		#region Public Fields

		public PvXType boardType;

		#endregion Public Fields

		#region Protected Fields

		protected static Dictionary<PvXType, int> m_textHueId;

		#endregion Protected Fields

		#region Public Constructors

		public PvXScoreBoard(int id) : base(id)
		{
		}

		public PvXScoreBoard(Serial serial) : base(serial)
		{
		}

		#endregion Public Constructors

		#region Public Properties

		[CommandProperty(AccessLevel.Owner)]
		public int TextHueID
		{
			get => PvXScoreBoard.m_textHueId[boardType];
            set
			{
				m_textHueId[boardType] = value;
				Config.Set($@"PvXsystem.{boardType.ToString()}_Text_Hue", value);
			}
		}

		#endregion Public Properties

		#region Public Methods

		public static int GetTextHueId(PvXType pvxType)
		{
			return m_textHueId[pvxType];
		}

		public static void Initialize()
		{
			m_textHueId = new Dictionary<PvXType, int>();
			m_textHueId[PvXType.PVM] = Config.Get(@"PvXsystem.PVM_Text_Hue", 0);
			m_textHueId[PvXType.PVP] = Config.Get(@"PvXsystem.PVP_Text_Hue", 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			//int version = reader.ReadInt();
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (from.InRange(this.GetWorldLocation(), 2))
			{
				from.CloseGump(typeof(OverallPvXGump));
				from.SendGump(new OverallPvXGump(from, 0, null, null, boardType));
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			//writer.Write((int)0); // version
		}

		#endregion Public Methods
	}
}