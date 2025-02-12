/////////////////////////////////////////////////
//
// Automatically generated by the
// AddonGenerator script by Arya
//
/////////////////////////////////////////////////

namespace Server.Items
{
	public class BBQSouthAddon : BaseAddon
	{
		public override BaseAddonDeed Deed => new BBQSouthAddonDeed();

        [ Constructable ]
		public BBQSouthAddon()
		{
			AddonComponent ac = null;
			ac = new AddonComponent( 52 );
			ac.Hue = 2067;
			AddComponent( ac, -1, 0, 0 );
			ac = new AddonComponent( 52 );
			ac.Hue = 2067;
			AddComponent( ac, 0, 0, 0 );
			ac = new AddonComponent( 52 );
			ac.Hue = 2067;
			AddComponent( ac, 1, 0, 0 );
			ac = new AddonComponent( 52 );
			ac.Hue = 2067;
			AddComponent( ac, 2, 0, 0 );
			ac = new AddonComponent( 52 );
			ac.Hue = 2067;
			AddComponent( ac, 3, 0, 0 );
			ac = new AddonComponent( 61 );
			ac.Hue = 2067;
			AddComponent( ac, 3, 1, 0 );
			ac = new AddonComponent( 61 );
			ac.Hue = 2067;
			AddComponent( ac, -1, 1, 0 );
			ac = new AddonComponent( 63 );
			ac.Hue = 2067;
			AddComponent( ac, 2, 1, 0 );
			ac = new AddonComponent( 63 );
			ac.Hue = 2067;
			AddComponent( ac, -2, 1, 0 );
			ac = new AddonComponent( 54 );
			ac.Hue = 2067;
			AddComponent( ac, -2, 0, 0 );
			ac = new AddonComponent( 66 );
			ac.Hue = 2067;
			AddComponent( ac, 0, 1, 0 );
			ac = new AddonComponent( 66 );
			ac.Hue = 2067;
			AddComponent( ac, 1, 1, 0 );
			ac = new AddonComponent( 66 );
			ac.Hue = 2067;
			AddComponent( ac, 2, 1, 0 );
			ac = new AddonComponent( 1173 );
			AddComponent( ac, 3, 1, 10 );
			ac = new AddonComponent( 1173 );
			AddComponent( ac, -1, 1, 10 );
			ac = new AddonComponent( 1301 );
			AddComponent( ac, 0, 1, 0 );
			ac = new AddonComponent( 1301 );
			AddComponent( ac, 1, 1, 0 );
			ac = new AddonComponent( 1301 );
			AddComponent( ac, 2, 1, 0 );
			ac = new AddonComponent( 14732 );
			ac.Light = LightType.ArchedWindowEast;
			AddComponent( ac, 0, 1, 0 );
			ac = new AddonComponent( 14732 );
			ac.Light = LightType.ArchedWindowEast;
			AddComponent( ac, 1, 1, 0 );
			ac = new AddonComponent( 14732 );
			ac.Light = LightType.ArchedWindowEast;
			AddComponent( ac, 2, 1, 0 );
			ac = new AddonComponent( 7386 );
			ac.Hue = 961;
			AddComponent( ac, 0, 1, 7 );
			ac = new AddonComponent( 7386 );
			ac.Hue = 961;
			AddComponent( ac, 1, 1, 7 );
			ac = new AddonComponent( 7386 );
			ac.Hue = 961;
			AddComponent( ac, 2, 1, 7 );
			ac = new AddonComponent( 2431 );
			AddComponent( ac, 0, 1, 7 );
			ac = new AddonComponent( 2541 );
			AddComponent( ac, -1, 1, 10 );
			ac = new AddonComponent( 7829 );
			AddComponent( ac, 1, 1, 7 );
			ac = new AddonComponent( 2416 );
			AddComponent( ac, -1, 1, 16 );
			ac = new AddonComponent( 2450 );
			AddComponent( ac, 3, 1, 10 );

		}

		public BBQSouthAddon( Serial serial ) : base( serial )
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

	public class BBQSouthAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon => new BBQSouthAddon();

        [Constructable]
		public BBQSouthAddonDeed()
		{
			Name = "BBQ South";
		}

		public BBQSouthAddonDeed( Serial serial ) : base( serial )
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