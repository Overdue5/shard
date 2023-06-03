/////////////////////////////////////////////////
//
// Automatically generated by the
// AddonGenerator script by Arya
//
/////////////////////////////////////////////////
namespace Server.Items
{
	public class FirePitLargeAddon : BaseAddon
	{
		public override BaseAddonDeed Deed => new FirePitLargeAddonDeed();

        [ Constructable ]
		public FirePitLargeAddon()
		{
			AddonComponent ac = null;
			ac = new AddonComponent( 3119 );
			AddComponent( ac, 0, 1, 0 );
			ac = new AddonComponent( 3118 );
			AddComponent( ac, 0, 0, 0 );
			ac = new AddonComponent( 3117 );
			AddComponent( ac, 1, 1, 0 );
			ac = new AddonComponent( 6012 );
			AddComponent( ac, -1, 2, 0 );
			ac = new AddonComponent( 4970 );
			AddComponent( ac, 0, 2, 0 );
			ac = new AddonComponent( 4966 );
			AddComponent( ac, 1, 2, 0 );
			ac = new AddonComponent( 4973 );
			AddComponent( ac, 1, -1, 0 );
			ac = new AddonComponent( 4972 );
			AddComponent( ac, 1, -1, 5 );
			ac = new AddonComponent( 4966 );
			AddComponent( ac, 0, -1, 3 );
			ac = new AddonComponent( 4971 );
			AddComponent( ac, -1, -1, 0 );
			ac = new AddonComponent( 4966 );
			AddComponent( ac, -1, 1, 3 );
			ac = new AddonComponent( 4969 );
			AddComponent( ac, -1, 0, 2 );
			ac = new AddonComponent( 3120 );
			AddComponent( ac, 1, 0, 0 );
			ac = new AddonComponent( 3562 );
			AddComponent( ac, -1, 1, 0 );
			ac = new AddonComponent( 3562 );
			AddComponent( ac, -1, 0, 0 );
			ac = new AddonComponent( 3562 );
			AddComponent( ac, 1, 1, 0 );
			ac = new AddonComponent( 3561 );
			ac.Light = LightType.ArchedWindowEast;
			AddComponent( ac, 1, 0, 1 );
			ac = new AddonComponent( 3561 );
			ac.Light = LightType.ArchedWindowEast;
			AddComponent( ac, 0, 1, 1 );
			ac = new AddonComponent( 3561 );
			ac.Light = LightType.ArchedWindowEast;
			AddComponent( ac, 0, 0, 1 );
			ac = new AddonComponent( 4972 );
			AddComponent( ac, 2, 2, 3 );
			ac = new AddonComponent( 4966 );
			AddComponent( ac, 2, 1, 0 );
			ac = new AddonComponent( 4965 );
			AddComponent( ac, 2, 0, 0 );
			ac = new AddonComponent( 4972 );
			AddComponent( ac, 2, -1, 0 );

		}

		public FirePitLargeAddon( Serial serial ) : base( serial )
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

	public class FirePitLargeAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon => new FirePitLargeAddon();

        [Constructable]
		public FirePitLargeAddonDeed()
		{
			Name = "FirePitLarge";
		}

		public FirePitLargeAddonDeed( Serial serial ) : base( serial )
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