namespace Server.Items
{
	public class GoldenBroadtail : BaseFish
	{		
		public override int LabelNumber => 1073828; // A Golden Broadtail
		
		[Constructable]
		public GoldenBroadtail() : base( 0x3B03 )
		{
		}

		public GoldenBroadtail( Serial serial ) : base( serial )
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
