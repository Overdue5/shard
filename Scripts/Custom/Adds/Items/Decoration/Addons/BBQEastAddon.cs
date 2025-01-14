/////////////////////////////////////////////////
//
// Automatically generated by the
// AddonGenerator script by Arya
//
/////////////////////////////////////////////////

namespace Server.Items
{
	public class BBQEastAddon : BaseAddon
	{
		public override BaseAddonDeed Deed => new BBQEastAddonDeed();

        [ Constructable ]
		public BBQEastAddon()
		{
			AddonComponent ac = null;
			ac = new AddonComponent( 53 );
			ac.Hue = 2067;
			AddComponent( ac, 0, -1, 0 );
			ac = new AddonComponent( 53 );
			ac.Hue = 2067;
			AddComponent( ac, 0, 0, 0 );
			ac = new AddonComponent( 53 );
			ac.Hue = 2067;
			AddComponent( ac, 0, 1, 0 );
			ac = new AddonComponent( 53 );
			ac.Hue = 2067;
			AddComponent( ac, 0, 2, 0 );
			ac = new AddonComponent( 53 );
			ac.Hue = 2067;
			AddComponent( ac, 0, 3, 0 );
			ac = new AddonComponent( 61 );
			ac.Hue = 2067;
			AddComponent( ac, 1, 3, 0 );
			ac = new AddonComponent( 62 );
			ac.Hue = 2067;
			AddComponent( ac, 1, 2, 0 );
			ac = new AddonComponent( 1301 );
			AddComponent( ac, 1, 2, 0 );
			ac = new AddonComponent( 1301 );
			AddComponent( ac, 1, 1, 0 );
			ac = new AddonComponent( 1301 );
			AddComponent( ac, 1, 0, 0 );
			ac = new AddonComponent( 61 );
			ac.Hue = 2067;
			AddComponent( ac, 1, -1, 0 );
			ac = new AddonComponent( 62 );
			ac.Hue = 2067;
			AddComponent( ac, 1, -2, 0 );
			ac = new AddonComponent( 54 );
			ac.Hue = 2067;
			AddComponent( ac, 0, -2, 0 );
			ac = new AddonComponent( 1173 );
			AddComponent( ac, 1, -1, 10 );
			ac = new AddonComponent( 1173 );
			AddComponent( ac, 1, 3, 10 );
			ac = new AddonComponent( 2541 );
			AddComponent( ac, 1, -1, 10 );
			ac = new AddonComponent( 2416 );
			AddComponent( ac, 1, -1, 16 );
			ac = new AddonComponent( 14742 );
			ac.Light = LightType.ArchedWindowEast;
			AddComponent( ac, 1, 0, 0 );
			ac = new AddonComponent( 14742 );
			ac.Light = LightType.ArchedWindowEast;
			AddComponent( ac, 1, 1, 0 );
			ac = new AddonComponent( 14742 );
			ac.Light = LightType.ArchedWindowEast;
			AddComponent( ac, 1, 2, 0 );
			ac = new AddonComponent( 7386 );
			ac.Hue = 961;
			AddComponent( ac, 1, 2, 7 );
			ac = new AddonComponent( 7386 );
			ac.Hue = 961;
			AddComponent( ac, 1, 1, 7 );
			ac = new AddonComponent( 7386 );
			ac.Hue = 961;
			AddComponent( ac, 1, 0, 7 );
			ac = new AddonComponent( 2431 );
			AddComponent( ac, 1, 2, 7 );
			ac = new AddonComponent( 7832 );
			AddComponent( ac, 1, 1, 7 );
			ac = new AddonComponent( 2449 );
			AddComponent( ac, 1, 3, 10 );
			ac = new AddonComponent( 67 );
			ac.Hue = 2067;
			AddComponent( ac, 1, 0, 0 );
			ac = new AddonComponent( 67 );
			ac.Hue = 2067;
			AddComponent( ac, 1, 1, 0 );
			ac = new AddonComponent( 67 );
			ac.Hue = 2067;
			AddComponent( ac, 1, 2, 0 );

		}

		public BBQEastAddon( Serial serial ) : base( serial )
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

	public class BBQEastAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon => new BBQEastAddon();

        [Constructable]
		public BBQEastAddonDeed()
		{
			Name = "BBQ East";
		}

		public BBQEastAddonDeed( Serial serial ) : base( serial )
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