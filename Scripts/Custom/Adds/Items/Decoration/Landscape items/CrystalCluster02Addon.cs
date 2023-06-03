/////////////////////////////////////////////////
//
// Automatically generated by the
// AddonGenerator script by Arya
//
/////////////////////////////////////////////////
namespace Server.Items
{
	public class CrystalCluster02Addon : BaseAddon
	{
		public override BaseAddonDeed Deed => new CrystalCluster02AddonDeed();

        [ Constructable ]
		public CrystalCluster02Addon()
		{
			AddonComponent ac = null;
			ac = new AddonComponent( 730 );
			AddComponent( ac, 1, -2, 0 );
			ac = new AddonComponent( 8712 );
			AddComponent( ac, 1, 1, 0 );
			ac = new AddonComponent( 8765 );
			AddComponent( ac, 2, 1, 0 );
			ac = new AddonComponent( 8762 );
			AddComponent( ac, 1, -1, 0 );
			ac = new AddonComponent( 8763 );
			AddComponent( ac, -1, 0, 0 );
			ac = new AddonComponent( 8766 );
			AddComponent( ac, 1, 3, 0 );
			ac = new AddonComponent( 8742 );
			AddComponent( ac, 0, 0, 0 );
			ac = new AddonComponent( 8732 );
			AddComponent( ac, 0, 1, 0 );
			ac = new AddonComponent( 8774 );
			AddComponent( ac, 0, 2, 0 );
			ac = new AddonComponent( 8775 );
			AddComponent( ac, 0, -1, 0 );
			ac = new AddonComponent( 8776 );
			AddComponent( ac, 2, 2, 0 );
			ac = new AddonComponent( 8777 );
			AddComponent( ac, -1, 3, 0 );
			ac = new AddonComponent( 12263 );
			AddComponent( ac, 1, 0, 0 );
			ac = new AddonComponent( 8732 );
			AddComponent( ac, 2, 0, 0 );

		}

		public CrystalCluster02Addon( Serial serial ) : base( serial )
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

	public class CrystalCluster02AddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon => new CrystalCluster02Addon();

        [Constructable]
		public CrystalCluster02AddonDeed()
		{
			Name = "CrystalCluster02";
		}

		public CrystalCluster02AddonDeed( Serial serial ) : base( serial )
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