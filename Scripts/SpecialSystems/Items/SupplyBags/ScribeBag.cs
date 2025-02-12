namespace Server.Items
{
	public class ScribeBag : Bag
	{
		public override string DefaultName => "a Scribe Kit";

        [Constructable]
		public ScribeBag() : this( 1 )
		{
			Movable = true;
			Hue = 0x105;
		}

		[Constructable]
		public ScribeBag( int amount )
		{
			DropItem( new BagOfReagents( 5000 ) );
			DropItem( new BlankScroll( 500 ) );
		}

		public ScribeBag( Serial serial ) : base( serial )
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