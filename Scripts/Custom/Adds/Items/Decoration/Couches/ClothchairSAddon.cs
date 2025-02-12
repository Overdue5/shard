/////////////////////////////////////////////////
//
// Automatically generated by the
// AddonGenerator script by Arya
//
/////////////////////////////////////////////////
namespace Server.Items
{
	public class ClothchairSAddon : BaseAddon
	{
		public override BaseAddonDeed Deed => new ClothchairSAddonDeed();

        [ Constructable ]
		public ClothchairSAddon()
		{
			AddonComponent ac = null;
			ac = new AddonComponent( 1113 );
			ac.Hue = 46;
			AddComponent( ac, 0, 1, 0 );
			ac = new AddonComponent( 1113 );
			ac.Hue = 46;
			AddComponent( ac, 1, 1, 0 );
			ac = new AddonComponent( 10386 );
			ac.Hue = 46;
			AddComponent( ac, 0, 0, 0 );
			ac = new AddonComponent( 10386 );
			ac.Hue = 46;
			AddComponent( ac, 1, 0, 0 );
			ac = new AddonComponent( 10382 );
			ac.Hue = 46;
			AddComponent( ac, -1, 1, 1 );
			ac = new AddonComponent( 10382 );
			ac.Hue = 46;
			AddComponent( ac, 1, 1, 1 );
			ac = new AddonComponent( 10380 );
			ac.Hue = 46;
			AddComponent( ac, -1, 0, 0 );
			ac = new AddonComponent( 5015 );
			ac.Hue = 46;
			AddComponent( ac, 1, 1, 3 );
			ac = new AddonComponent( 5031 );
			ac.Hue = 46;
			AddComponent( ac, 1, 1, 4 );

		}

		public ClothchairSAddon( Serial serial ) : base( serial )
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

	public class ClothchairSAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon => new ClothchairSAddon();

        [Constructable]
		public ClothchairSAddonDeed()
		{
			Name = "ClothchairS";
		}

		public ClothchairSAddonDeed( Serial serial ) : base( serial )
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