namespace Server.Items
{
	public class WaterloggedBoots : BaseShoes
	{		
		public override int LabelNumber => 1074364; // Waterlogged boots
		
		[Constructable]
		public WaterloggedBoots() : base( 0x1711 )
		{
			if ( Utility.RandomBool() )
			{	
				// thigh boots
				ItemID = 0x1711;
				Weight = 4.0;
			}
			else
			{
				// boots
				ItemID = 0x170B;
				Weight = 3.0;
			}
		}

		public WaterloggedBoots( Serial serial ) : base( serial )
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
