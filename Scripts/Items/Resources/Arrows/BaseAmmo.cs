namespace Server.Items
{
	public abstract class BaseAmmo : Item, ICommodity
	{
		#region Protected Fields

		protected Poison m_Poison;

		#endregion Protected Fields

		#region Public Constructors

		public BaseAmmo(int itemID) : base(itemID)
		{
		}

		public BaseAmmo(Serial serial) : base(serial)
		{
		}

		#endregion Public Constructors

		#region Public Properties

		int ICommodity.DescriptionNumber => LabelNumber;

		bool ICommodity.IsDeedable => true;

		[CommandProperty(AccessLevel.GameMaster)]
		public Poison Poison
		{
			get => m_Poison;
			set
			{
				m_Poison = value;
				InvalidateProperties();
			}
		}

		#endregion Public Properties

		#region Public Methods

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			switch (version)
			{
				case 0:
					{
						reader.Info = "S";
						break;
					}
				case 1:
					{
						m_Poison = Poison.Deserialize(reader);
						break;
					}

			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(1); // version
			Poison.Serialize(m_Poison, writer);
		}

		#endregion Public Methods
	}
}