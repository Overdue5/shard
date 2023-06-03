
namespace Server.Items
{
	[Flipable(3903, 3904, 3905)]
	public class Arrow : BaseAmmo
	{
		#region Public Constructors

		[Constructable]
		public Arrow() : this(1)
		{
		}

		[Constructable]
		public Arrow(int amount) : base(0xF3F)
		{
			Stackable = true;
			Amount = amount;
		}

		public Arrow(Serial serial) : base(serial)
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

			writer.Write(0); // version
		}

		#endregion Public Methods
	}
}