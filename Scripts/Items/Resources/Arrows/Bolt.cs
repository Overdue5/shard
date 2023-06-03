namespace Server.Items
{
	[Flipable(7163, 7164, 7165)]
	public class Bolt : BaseAmmo
	{
		#region Public Constructors

		[Constructable]
		public Bolt() : this(1)
		{
		}

		[Constructable]
		public Bolt(int amount) : base(0x1BFB)
		{
			Stackable = true;
			Amount = amount;
		}

		public Bolt(Serial serial) : base(serial)
		{
		}

		#endregion Public Constructors

		#region Public Properties

		public override double DefaultWeight => 0.1;

        #endregion Public Properties

		#region Public Methods

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			if (reader.Info == "S")
			{
				reader.Info = null;
				return;
			}
			reader.ReadInt();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(1); // version
		}

		#endregion Public Methods
	}
}