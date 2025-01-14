/////////////////////////////////////////////////
//
// Automatically generated by the
// AddonGenerator script by Arya
//
/////////////////////////////////////////////////

namespace Server.Items
{
	public class ShowerEastAddon : BaseAddon
	{
		public override BaseAddonDeed Deed => new ShowerEastAddonDeed();

        [ Constructable ]
		public ShowerEastAddon()
		{
			AddonComponent ac = null;
			ac = new AddonComponent( 13573 );
			AddComponent( ac, 0, 0, 0 );
			ac = new AddonComponent( 13573 );
			AddComponent( ac, 0, -1, 0 );
			ac = new AddonComponent( 6196 );
			AddComponent( ac, -2, 0, 5 );
			ac = new AddonComponent( 6039 );
			AddComponent( ac, 1, 0, 0 );
			ac = new AddonComponent( 6039 );
			AddComponent( ac, 1, -1, 0 );
			ac = new AddonComponent( 13555 );
			AddComponent( ac, -1, -1, 2 );
			ac = new AddonComponent( 6039 );
			AddComponent( ac, 0, -1, 0 );
			ac = new AddonComponent( 6039 );
			AddComponent( ac, 0, 0, 0 );
			ac = new AddonComponent( 6039 );
			AddComponent( ac, -1, 0, 0 );
			ac = new AddonComponent( 6039 );
			AddComponent( ac, -1, -1, 0 );
			ac = new AddonComponent( 5645 );
			AddComponent( ac, 2, -1, 0 );
			ac = new AddonComponent( 5645 );
			AddComponent( ac, 2, 0, 0 );
			ac = new AddonComponent( 13555 );
			AddComponent( ac, -1, 0, 1 );
			ac = new AddonComponent( 5646 );
			AddComponent( ac, 0, 2, 0 );
			ac = new AddonComponent( 6039 );
			AddComponent( ac, 1, 1, 0 );
			ac = new AddonComponent( 6039 );
			AddComponent( ac, 0, 1, 0 );
			ac = new AddonComponent( 6039 );
			AddComponent( ac, -1, 1, 0 );
			ac = new AddonComponent( 5645 );
			AddComponent( ac, 2, 1, 0 );
			ac = new AddonComponent( 5646 );
			AddComponent( ac, 1, 2, 0 );
			ac = new AddonComponent( 5646 );
			AddComponent( ac, -1, 2, 0 );

		}

		public ShowerEastAddon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class ShowerEastAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon => new ShowerEastAddon();

        [Constructable]
		public ShowerEastAddonDeed()
		{
			Name = "Shower East";
		}

		public ShowerEastAddonDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void	Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}