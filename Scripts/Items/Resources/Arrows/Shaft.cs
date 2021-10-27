namespace Server.Items
{
	public class Shaft : Item, ICommodity
	{
		#region Public Constructors

		[Constructable]
		public Shaft() : this(1)
		{
		}

		[Constructable]
		public Shaft(int amount) : base(0x1BD4)
		{
			Stackable = true;
			Amount = amount;
		}

		public Shaft(Serial serial) : base(serial)
		{
		}

		#endregion Public Constructors

		#region Public Properties

		public override double DefaultWeight
		{
			get { return 0.1; }
		}

		int ICommodity.DescriptionNumber { get { return LabelNumber; } }
		bool ICommodity.IsDeedable { get { return true; } }

		#endregion Public Properties

		#region Public Methods

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		#endregion Public Methods
	}
}