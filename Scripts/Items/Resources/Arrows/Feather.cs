namespace Server.Items
{
	public class Feather : Item, ICommodity
	{
		#region Public Constructors

		[Constructable]
		public Feather() : this(1)
		{
		}

		[Constructable]
		public Feather(int amount) : base(0x1BD1)
		{
			Stackable = true;
			Amount = amount;
		}

		public Feather(Serial serial) : base(serial)
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