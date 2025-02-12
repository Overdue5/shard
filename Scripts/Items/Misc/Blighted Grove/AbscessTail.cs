namespace Server.Items
{
	public class AbscessTail : Item
	{
		public override int LabelNumber => 1074231; // Abscess' Tail

		[Constructable]
		public AbscessTail() : base( 0x1A9D )
		{
			LootType = LootType.Blessed;
			Hue = 0x51D; // TODO check
		}

		public AbscessTail( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}

