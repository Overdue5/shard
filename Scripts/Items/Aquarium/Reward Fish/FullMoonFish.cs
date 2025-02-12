namespace Server.Items
{
	public class FullMoonFish : BaseFish
	{		
		public override int LabelNumber => 1074597; // A Full Moon Fish
		
		[Constructable]
		public FullMoonFish() : base( 0x3B15 )
		{
		}		

		public FullMoonFish( Serial serial ) : base( serial )
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
