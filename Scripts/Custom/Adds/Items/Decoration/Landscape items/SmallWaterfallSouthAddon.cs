/////////////////////////////////////////////////
//                                             //
// Automatically generated by the              //
// AddonGenerator script by Arya               //
//                                             //
/////////////////////////////////////////////////
namespace Server.Items
{
	public class SmallWaterfallSouthAddon : BaseAddon
	{
		public override BaseAddonDeed Deed => new SmallWaterfallSouthAddonDeed();

        [ Constructable ]
		public SmallWaterfallSouthAddon()
		{
			AddonComponent ac;
			ac = new AddonComponent( 6010 );
			ac.Hue = 1885;
			AddComponent( ac, -1, -2, 8 );
			ac = new AddonComponent( 4956 );
			AddComponent( ac, -1, -2, 8 );
			ac = new AddonComponent( 6002 );
			AddComponent( ac, -1, -1, 3 );
			ac = new AddonComponent( 13460 );
			AddComponent( ac, -1, 1, 4 );
			ac = new AddonComponent( 13460 );
			AddComponent( ac, -1, 2, 4 );
			ac = new AddonComponent( 6039 );
			AddComponent( ac, -1, 0, 4 );
			ac = new AddonComponent( 4950 );
			AddComponent( ac, -1, 0, 5 );
			ac = new AddonComponent( 4949 );
			AddComponent( ac, -1, 1, 5 );
			ac = new AddonComponent( 4967 );
			AddComponent( ac, -1, 3, 7 );
			ac = new AddonComponent( 4970 );
			AddComponent( ac, -1, 3, 5 );
			ac = new AddonComponent( 4961 );
			ac.Hue = 1885;
			AddComponent( ac, -2, 2, 5 );
			ac = new AddonComponent( 4962 );
			ac.Hue = 1885;
			AddComponent( ac, -1, 2, 5 );
			ac = new AddonComponent( 4963 );
			AddComponent( ac, 3, 1, 4 );
			ac = new AddonComponent( 13603 );
			AddComponent( ac, 0, 0, 4 );
			ac = new AddonComponent( 4951 );
			AddComponent( ac, 2, 0, 4 );
			ac = new AddonComponent( 4953 );
			AddComponent( ac, 3, -1, 4 );
			ac = new AddonComponent( 4967 );
			AddComponent( ac, 3, 1, 8 );
			ac = new AddonComponent( 6001 );
			AddComponent( ac, 1, 0, 28 );
			ac = new AddonComponent( 13585 );
			AddComponent( ac, 0, -1, 4 );
			ac = new AddonComponent( 4967 );
			ac.Hue = 1891;
			AddComponent( ac, 1, -1, 21 );
			ac = new AddonComponent( 4963 );
			AddComponent( ac, 2, -1, 18 );
			ac = new AddonComponent( 4963 );
			AddComponent( ac, 0, 3, 5 );
			ac = new AddonComponent( 4960 );
			AddComponent( ac, 1, 3, 5 );
			ac = new AddonComponent( 13603 );
			AddComponent( ac, 1, 0, 4 );
			ac = new AddonComponent( 4961 );
			AddComponent( ac, 1, 3, 4 );
			ac = new AddonComponent( 4962 );
			AddComponent( ac, 2, 3, 4 );
			ac = new AddonComponent( 4959 );
			AddComponent( ac, 0, 3, 5 );
			ac = new AddonComponent( 13603 );
			AddComponent( ac, 2, 0, 4 );
			ac = new AddonComponent( 4958 );
			AddComponent( ac, 0, -3, 8 );
			ac = new AddonComponent( 4949 );
			AddComponent( ac, 3, 3, 8 );
			ac = new AddonComponent( 4950 );
			AddComponent( ac, 3, 2, 8 );
			ac = new AddonComponent( 4952 );
			AddComponent( ac, 3, 0, 4 );
			ac = new AddonComponent( 13585 );
			AddComponent( ac, 1, -1, 4 );
			ac = new AddonComponent( 4967 );
			AddComponent( ac, 3, -1, 21 );
			ac = new AddonComponent( 4970 );
			AddComponent( ac, 1, -1, 17 );
			ac = new AddonComponent( 13585 );
			AddComponent( ac, 2, -1, 4 );
			ac = new AddonComponent( 13460 );
			AddComponent( ac, 2, 1, 4 );
			ac = new AddonComponent( 13460 );
			AddComponent( ac, 1, 1, 4 );
			ac = new AddonComponent( 13460 );
			AddComponent( ac, 2, 2, 4 );
			ac = new AddonComponent( 13460 );
			AddComponent( ac, 1, 2, 4 );
			ac = new AddonComponent( 3246 );
			AddComponent( ac, 2, -1, 21 );
			ac = new AddonComponent( 13460 );
			AddComponent( ac, 0, 2, 4 );
			ac = new AddonComponent( 13460 );
			AddComponent( ac, 0, 1, 4 );
			ac = new AddonComponent( 3231 );
			AddComponent( ac, 3, 3, 13 );
			ac = new AddonComponent( 13484 );
			AddComponent( ac, 1, 2, 11 );
			ac = new AddonComponent( 4943 );
			AddComponent( ac, 2, -2, 0 );
			ac = new AddonComponent( 4945 );
			AddComponent( ac, 3, -3, 0 );
			ac = new AddonComponent( 4944 );
			AddComponent( ac, 3, -2, 0 );
			ac = new AddonComponent( 4961 );
			AddComponent( ac, 0, -3, 3 );
			ac = new AddonComponent( 4962 );
			AddComponent( ac, 1, -3, 3 );
			ac = new AddonComponent( 6007 );
			AddComponent( ac, 1, 3, 10 );
			ac = new AddonComponent( 6006 );
			AddComponent( ac, 0, -3, 0 );

		}

		public SmallWaterfallSouthAddon( Serial serial ) : base( serial )
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

	public class SmallWaterfallSouthAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon => new SmallWaterfallSouthAddon();

        [Constructable]
		public SmallWaterfallSouthAddonDeed()
		{
			Name = "SmallWaterfallSouth";
		}

		public SmallWaterfallSouthAddonDeed( Serial serial ) : base( serial )
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