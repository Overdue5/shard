/////////////////////////////////////////////////
//                                             //
// Automatically generated by the              //
// AddonGenerator script by Arya               //
//                                             //
/////////////////////////////////////////////////

namespace Server.Items
{
	public class SwimmingPoolAddon : BaseAddon
	{
		public override BaseAddonDeed Deed => new SwimmingPoolAddonDeed();

        [ Constructable ]
		public SwimmingPoolAddon()
		{
			AddComponent( new AddonComponent( 1811 ), 4, 1, 2 );
			AddComponent( new AddonComponent( 1803 ), 4, 0, 2 );
			AddComponent( new AddonComponent( 1813 ), 4, -1, 2 );
			AddComponent( new AddonComponent( 4554 ), 3, 3, 18 );
			AddComponent( new AddonComponent( 4554 ), 3, -3, 18 );
			AddComponent( new AddonComponent( 4554 ), -4, -3, 18 );
			AddComponent( new AddonComponent( 4554 ), -4, 3, 18 );
			AddonComponent ac;
			ac = new AddonComponent( 4643 );
			AddComponent( ac, -4, 3, 9 );
			ac = new AddonComponent( 4643 );
			AddComponent( ac, -4, -3, 9 );
			ac = new AddonComponent( 7385 );
			ac.Hue = 90;
			ac.Name = "Water";
			AddComponent( ac, -3, 2, 5 );
			ac = new AddonComponent( 7385 );
			ac.Hue = 90;
			ac.Name = "Water";
			AddComponent( ac, -2, 2, 5 );
			ac = new AddonComponent( 7385 );
			ac.Hue = 90;
			ac.Name = "Water";
			AddComponent( ac, -1, 2, 5 );
			ac = new AddonComponent( 7385 );
			ac.Hue = 90;
			ac.Name = "Water";
			AddComponent( ac, -1, -2, 5 );
			ac = new AddonComponent( 7385 );
			ac.Hue = 90;
			ac.Name = "Water";
			AddComponent( ac, -2, -2, 5 );
			ac = new AddonComponent( 7385 );
			ac.Hue = 90;
			ac.Name = "Water";
			AddComponent( ac, -3, -2, 5 );
			ac = new AddonComponent( 7385 );
			ac.Hue = 90;
			ac.Name = "Water";
			AddComponent( ac, -1, 1, 5 );
			ac = new AddonComponent( 7385 );
			ac.Hue = 90;
			ac.Name = "Water";
			AddComponent( ac, -1, 0, 5 );
			ac = new AddonComponent( 7385 );
			ac.Hue = 90;
			ac.Name = "Water";
			AddComponent( ac, -1, -1, 5 );
			ac = new AddonComponent( 7385 );
			ac.Hue = 90;
			ac.Name = "Water";
			AddComponent( ac, -2, 1, 5 );
			ac = new AddonComponent( 7385 );
			ac.Hue = 90;
			ac.Name = "Water";
			AddComponent( ac, -2, 0, 5 );
			ac = new AddonComponent( 7385 );
			ac.Hue = 90;
			ac.Name = "Water";
			AddComponent( ac, -2, -1, 5 );
			ac = new AddonComponent( 7385 );
			ac.Hue = 90;
			ac.Name = "Water";
			AddComponent( ac, -3, 0, 5 );
			ac = new AddonComponent( 7385 );
			ac.Hue = 90;
			ac.Name = "Water";
			AddComponent( ac, -3, 1, 5 );
			ac = new AddonComponent( 7385 );
			ac.Hue = 90;
			ac.Name = "Water";
			AddComponent( ac, -3, -1, 5 );
			ac = new AddonComponent( 1173 );
			ac.Hue = 1153;
			AddComponent( ac, -1, 2, 2 );
			ac = new AddonComponent( 1173 );
			ac.Hue = 1153;
			AddComponent( ac, -2, -2, 2 );
			ac = new AddonComponent( 1173 );
			ac.Hue = 1153;
			AddComponent( ac, -3, 2, 2 );
			ac = new AddonComponent( 1173 );
			ac.Hue = 1153;
			AddComponent( ac, -3, 1, 2 );
			ac = new AddonComponent( 1173 );
			ac.Hue = 1153;
			AddComponent( ac, -3, 0, 2 );
			ac = new AddonComponent( 1173 );
			ac.Hue = 1153;
			AddComponent( ac, -3, -1, 2 );
			ac = new AddonComponent( 1173 );
			AddComponent( ac, -3, -2, 2 );
			ac = new AddonComponent( 1173 );
			ac.Hue = 1153;
			AddComponent( ac, -1, -1, 2 );
			ac = new AddonComponent( 1173 );
			ac.Hue = 1153;
			AddComponent( ac, -1, 0, 2 );
			ac = new AddonComponent( 1173 );
			ac.Hue = 1153;
			AddComponent( ac, -1, 1, 2 );
			ac = new AddonComponent( 1173 );
			ac.Hue = 1153;
			AddComponent( ac, -1, -2, 2 );
			ac = new AddonComponent( 1173 );
			ac.Hue = 1153;
			AddComponent( ac, -2, 2, 2 );
			ac = new AddonComponent( 1173 );
			ac.Hue = 1153;
			AddComponent( ac, -2, 1, 2 );
			ac = new AddonComponent( 1173 );
			ac.Hue = 1153;
			AddComponent( ac, -2, 0, 2 );
			ac = new AddonComponent( 1173 );
			ac.Hue = 1153;
			AddComponent( ac, -2, -1, 2 );
			ac = new AddonComponent( 1819 );
			ac.Hue = 1153;
			AddComponent( ac, -3, -2, 0 );
			ac = new AddonComponent( 1820 );
			ac.Hue = 1153;
			AddComponent( ac, -3, 2, 0 );
			ac = new AddonComponent( 1801 );
			AddComponent( ac, -4, -2, 2 );
			ac = new AddonComponent( 1801 );
			AddComponent( ac, -4, -3, 2 );
			ac = new AddonComponent( 1801 );
			AddComponent( ac, -3, -3, 2 );
			ac = new AddonComponent( 1801 );
			AddComponent( ac, -2, -3, 2 );
			ac = new AddonComponent( 1801 );
			AddComponent( ac, -1, -3, 2 );
			ac = new AddonComponent( 1801 );
			AddComponent( ac, -1, 3, 2 );
			ac = new AddonComponent( 1801 );
			AddComponent( ac, -2, 3, 2 );
			ac = new AddonComponent( 1801 );
			AddComponent( ac, -3, 3, 2 );
			ac = new AddonComponent( 1801 );
			AddComponent( ac, -4, 3, 2 );
			ac = new AddonComponent( 1801 );
			AddComponent( ac, -4, 2, 2 );
			ac = new AddonComponent( 1801 );
			AddComponent( ac, -4, 1, 2 );
			ac = new AddonComponent( 1801 );
			AddComponent( ac, -4, 0, 2 );
			ac = new AddonComponent( 1801 );
			AddComponent( ac, -4, -1, 2 );
            ac = new AddonComponent(1801);
            AddComponent(ac, 0, 3, 2);
            ac = new AddonComponent(1801);
            AddComponent(ac, 1, 3, 2);
            ac = new AddonComponent(1801);
            AddComponent(ac, 2, 3, 2);
            ac = new AddonComponent(1801);
            AddComponent(ac, 3, 3, 2);
			ac = new AddonComponent( 4643 );
			AddComponent( ac, 3, -3, 9 );
			ac = new AddonComponent( 4643 );
			AddComponent( ac, 3, 3, 9 );
			ac = new AddonComponent( 7385 );
			ac.Hue = 90;
            ac.Name = "Water";
			AddComponent( ac, 0, 2, 5 );
			ac = new AddonComponent( 7385 );
			ac.Hue = 90;
            ac.Name = "Water";
			AddComponent( ac, 2, 2, 5 );
			ac = new AddonComponent( 7385 );
			ac.Hue = 90;
            ac.Name = "Water";
			AddComponent( ac, 2, -2, 5 );
			ac = new AddonComponent( 7385 );
			ac.Hue = 90;
            ac.Name = "Water";
			AddComponent( ac, 1, -2, 5 );
			ac = new AddonComponent( 7385 );
			ac.Hue = 90;
            ac.Name = "Water";
			AddComponent( ac, 0, -2, 5 );
			ac = new AddonComponent( 7385 );
			ac.Hue = 90;
            ac.Name = "Water";
			AddComponent( ac, 1, 2, 5 );
			ac = new AddonComponent( 7385 );
			ac.Hue = 90;
            ac.Name = "Water";
			AddComponent( ac, 2, 1, 5 );
			ac = new AddonComponent( 7385 );
			ac.Hue = 90;
            ac.Name = "Water";
            AddComponent( ac, 2, 0, 5 );
			ac = new AddonComponent( 7385 );
			ac.Hue = 90;
            ac.Name = "Water";
			AddComponent( ac, 2, -1, 5 );
			ac = new AddonComponent( 7385 );
			ac.Hue = 90;
            ac.Name = "Water";
			AddComponent( ac, 1, 1, 5 );
			ac = new AddonComponent( 7385 );
			ac.Hue = 90;
            ac.Name = "Water";
			AddComponent( ac, 1, 0, 5 );
			ac = new AddonComponent( 7385 );
			ac.Hue = 90;
            ac.Name = "Water";
			AddComponent( ac, 1, -1, 5 );
			ac = new AddonComponent( 7385 );
			ac.Hue = 90;
            ac.Name = "Water";
			AddComponent( ac, 0, 1, 5 );
			ac = new AddonComponent( 7385 );
			ac.Hue = 90;
            ac.Name = "Water";
			AddComponent( ac, 0, 0, 5 );
			ac = new AddonComponent( 7385 );
			ac.Hue = 90;
            ac.Name = "Water";
			AddComponent( ac, 0, -1, 5 );
			ac = new AddonComponent( 1173 );
			ac.Hue = 1153;
			AddComponent( ac, 0, 0, 2 );
			ac = new AddonComponent( 1173 );
			ac.Hue = 1153;
			AddComponent( ac, 1, 0, 2 );
			ac = new AddonComponent( 1173 );
			ac.Hue = 1153;
			AddComponent( ac, 1, 1, 2 );
			ac = new AddonComponent( 1173 );
			ac.Hue = 1153;
			AddComponent( ac, 2, 1, 2 );
			ac = new AddonComponent( 1173 );
			AddComponent( ac, 2, 2, 2 );
			ac = new AddonComponent( 1173 );
			ac.Hue = 1153;
			AddComponent( ac, 0, 2, 2 );
			ac = new AddonComponent( 1173 );
			ac.Hue = 1153;
			AddComponent( ac, 0, 1, 2 );
			ac = new AddonComponent( 1173 );
			ac.Hue = 1153;
			AddComponent( ac, 1, -2, 2 );
			ac = new AddonComponent( 1173 );
			ac.Hue = 1153;
			AddComponent( ac, 1, -1, 2 );
			ac = new AddonComponent( 1173 );
			ac.Hue = 1153;
			AddComponent( ac, 2, -2, 2 );
			ac = new AddonComponent( 1173 );
			ac.Hue = 1153;
			AddComponent( ac, 1, 2, 2 );
			ac = new AddonComponent( 1173 );
			ac.Hue = 1153;
			AddComponent( ac, 2, -1, 2 );
			ac = new AddonComponent( 1173 );
			ac.Hue = 1153;
			AddComponent( ac, 2, 0, 2 );
			ac = new AddonComponent( 1818 );
			ac.Hue = 1153;
			AddComponent( ac, 2, 2, 0 );
			ac = new AddonComponent( 1173 );
			ac.Hue = 1153;
			AddComponent( ac, 0, -2, 2 );
			ac = new AddonComponent( 1173 );
			ac.Hue = 1153;
			AddComponent( ac, 0, -1, 2 );
			ac = new AddonComponent( 1821 );
			ac.Hue = 1153;
			AddComponent( ac, 2, -2, 0 );
			ac = new AddonComponent( 1801 );
			AddComponent( ac, 0, -3, 2 );
			ac = new AddonComponent( 1801 );
			AddComponent( ac, 1, -3, 2 );
			ac = new AddonComponent( 1801 );
			AddComponent( ac, 2, -3, 2 );
			ac = new AddonComponent( 1801 );
			AddComponent( ac, 3, -2, 2 );
			ac = new AddonComponent( 1801 );
			AddComponent( ac, 3, -3, 2 );
			ac = new AddonComponent( 1801 );
			AddComponent( ac, 3, -1, 2 );
			ac = new AddonComponent( 1801 );
			AddComponent( ac, 3, 0, 2 );
			ac = new AddonComponent( 1801 );
			AddComponent( ac, 3, 2, 2 );
			ac = new AddonComponent( 1801 );
			AddComponent( ac, 3, 1, 2 );
		}

		public SwimmingPoolAddon( Serial serial ) : base( serial )
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

	public class SwimmingPoolAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon => new SwimmingPoolAddon();

        [Constructable]
		public SwimmingPoolAddonDeed()
		{
			Name = "Swimming Pool";
		}

		public SwimmingPoolAddonDeed( Serial serial ) : base( serial )
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