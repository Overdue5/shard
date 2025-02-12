namespace Server.Items
{
	public class Shell : Item
	{		
		public override int LabelNumber => 1074598; // A shell
		
		[Constructable]
		public Shell() : base( Utility.RandomList( 0x3B12, 0x3B13 ) )
		{
			Weight = 1;
		}

		public Shell( Serial serial ) : base( serial )
		{		
		}
		
		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			
			list.Add( 1073634 ); // An aquarium decoration
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
