namespace Server.Items
{
	public class SPvMBag : Bag
	{
		public override string DefaultName => "a small hunting bag";

        [Constructable]
		public SPvMBag() : this( 1 )
		{
			Movable = true;
			Hue = 2524;
		}

		[Constructable]
		public SPvMBag( int amount )
		{
			DropItem( new ManaPotion( 8 ) );
			DropItem( new LesserHealPotion( 5 ) );
			DropItem( new EnergyVortexScroll( 15 ) );
			DropItem( new BladeSpiritsScroll( 15 ) );
			DropItem( new Arrow( 85 ) );
			DropItem( new Bolt( 85 ) );
			DropItem( new Bandage( 55 ) );
		}

		public SPvMBag( Serial serial ) : base( serial )
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