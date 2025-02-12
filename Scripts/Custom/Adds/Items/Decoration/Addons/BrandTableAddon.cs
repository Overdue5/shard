/////////////////////////////////////////////////
//
// Automatically generated by the
// AddonGenerator script by Arya
//
/////////////////////////////////////////////////

namespace Server.Items
{
	public class BrandTableAddon : BaseAddon
	{
		public override BaseAddonDeed Deed => new BrandTableAddonDeed();

        [ Constructable ]
		public BrandTableAddon()
		{
			AddonComponent ac = null;
			ac = new AddonComponent( 7621 );
			//ac.Hue = 2006;
			AddComponent( ac, -1, 0, 0 );
			ac = new AddonComponent( 7620 );
			//ac.Hue = 2006;
			AddComponent( ac, 1, 0, 0 );
			ac = new AddonComponent( 7620 );
			//ac.Hue = 2006;
			AddComponent( ac, 1, 1, 0 );
			ac = new AddonComponent( 7622 );
			//ac.Hue = 2006;
			AddComponent( ac, 0, 0, 0 );
			ac = new AddonComponent( 7622 );
			//ac.Hue = 2006;
			AddComponent( ac, 0, 1, 0 );
			ac = new AddonComponent( 2519 );
			//ac.Hue = 1996;
			AddComponent( ac, -1, 0, 6 );
			ac = new AddonComponent( 2519 );
			//ac.Hue = 1996;
			AddComponent( ac, 1, 0, 6 );
			ac = new AddonComponent( 2519 );
			//ac.Hue = 1996;
			AddComponent( ac, 1, 1, 6 );
			ac = new AddonComponent( 2519 );
			//ac.Hue = 1996;
			AddComponent( ac, -1, 1, 6 );
			ac = new AddonComponent( 7621 );
			//ac.Hue = 2006;
			AddComponent( ac, -1, 1, 0 );
			ac = new AddonComponent( 2517 );
			//ac.Hue = 1993;
			AddComponent( ac, 1, 1, 6 );
			ac = new AddonComponent( 2517 );
			//ac.Hue = 1993;
			AddComponent( ac, 1, 0, 6 );
			ac = new AddonComponent( 2493 );
			//ac.Hue = 1993;
			AddComponent( ac, -1, 1, 7 );
			ac = new AddonComponent( 2493 );
			//ac.Hue = 1993;
			AddComponent( ac, -1, 0, 7 );
			ac = new AddonComponent( 5638 );
			AddComponent( ac, 0, 0, 6 );
			ac = new AddonComponent( 2449 );
			//ac.Hue = 1993;
			AddComponent( ac, 0, 1, 6 );
			ac = new AddonComponent( 7829 );
			AddComponent( ac, 0, 1, 7 );
			ac = new AddonComponent( 2894 );
			//ac.Hue = 1994;
			AddComponent( ac, -2, 0, 0 );
			ac = new AddonComponent( 2894 );
			//ac.Hue = 1994;
			AddComponent( ac, -2, 1, 0 );
			ac = new AddonComponent( 2897 );
			//ac.Hue = 1994;
			AddComponent( ac, 2, 0, 0 );
			ac = new AddonComponent( 2897 );
			//ac.Hue = 1994;
			AddComponent( ac, 2, 1, 0 );

		}

		public BrandTableAddon( Serial serial ) : base( serial )
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

	public class BrandTableAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon => new BrandTableAddon();

        [Constructable]
		public BrandTableAddonDeed()
		{
			Name = "Brand Table";
		}

		public BrandTableAddonDeed( Serial serial ) : base( serial )
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