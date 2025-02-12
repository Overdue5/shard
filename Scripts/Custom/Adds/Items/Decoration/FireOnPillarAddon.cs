/////////////////////////////////////////////////
//
// Automatically generated by the
// AddonGenerator script by Arya
//
/////////////////////////////////////////////////
namespace Server.Items
{
	public class FireOnPillarAddon : BaseAddon
	{
		public override BaseAddonDeed Deed => new FireOnPillarAddonDeed();

        [ Constructable ]
		public FireOnPillarAddon()
		{
			AddonComponent ac = null;
			ac = new AddonComponent( 7978 );
			AddComponent( ac, 0, 0, 0 );
			ac = new AddonComponent( 7979 );
			AddComponent( ac, 0, 0, 7 );
			ac = new AddonComponent( 6571 );
			ac.Light = LightType.ArchedWindowEast;
			AddComponent( ac, 0, 0, 10 );

		}

		public FireOnPillarAddon( Serial serial ) : base( serial )
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

	public class FireOnPillarAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon => new FireOnPillarAddon();

        [Constructable]
		public FireOnPillarAddonDeed()
		{
			Name = "FireOnPillar";
		}

		public FireOnPillarAddonDeed( Serial serial ) : base( serial )
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