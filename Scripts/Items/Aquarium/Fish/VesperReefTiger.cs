namespace Server.Items
{
	public class VesperReefTiger : BaseFish
	{		
		public override int LabelNumber => 1073836; // A Vesper Reef Tiger
		
		[Constructable]
		public VesperReefTiger() : base( 0x3B08 )
		{
		}

		public VesperReefTiger( Serial serial ) : base( serial )
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
