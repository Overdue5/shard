namespace Server.Items
{
	public class RedDartFish : BaseFish
	{		
		public override int LabelNumber => 1073834; // A Red Dart Fish 
		
		[Constructable]
		public RedDartFish() : base( 0x3B00 )
		{
		}

		public RedDartFish( Serial serial ) : base( serial )
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
