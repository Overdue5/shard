namespace Server.Items
{
	public class LPvMBag : Bag
	{
		public override string DefaultName => "a large hunting bag";

        [Constructable]
		public LPvMBag() : this( 1 )
		{
			Movable = true;
			Hue = 1176;
		}

		[Constructable]
		public LPvMBag( int amount )
		{
			DropItem( new TotalManaPotion( 10 ) );
			DropItem( new GreaterHealPotion( 8 ) );
			DropItem( new EnergyVortexScroll( 28 ) );
			DropItem( new BladeSpiritsScroll( 28 ) );
			DropItem( new Arrow( 120 ) );
			DropItem( new Bolt( 120 ) );
			DropItem( new Bandage( 85 ) );
		}

		public LPvMBag( Serial serial ) : base( serial )
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