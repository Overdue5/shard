namespace Server.Items
{
	public class AquariumFood : Item
	{		
		public override int LabelNumber => 1074819; // Aquarium food
		
		[Constructable]
		public AquariumFood() : base( 0xEFC )
		{
		}

		public AquariumFood( Serial serial ) : base( serial )
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
