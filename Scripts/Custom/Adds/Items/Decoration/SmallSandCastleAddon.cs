
////////////////////////////////////////
//                                    //
//   Generated by CEO's YAAAG - V1.2  //
// (Yet Another Arya Addon Generator) //
//                                    //
////////////////////////////////////////

namespace Server.Items
{
	public class SmallSandCastleAddon : BaseAddon
	{
         
            
		public override BaseAddonDeed Deed => new SmallSandCastleAddonDeed();

        [ Constructable ]
		public SmallSandCastleAddon()
		{



			AddComplexComponent( (BaseAddon) this, 194, -1, 1, 0, 0, -1, "sandbox", 1);// 1
			AddComplexComponent( (BaseAddon) this, 193, 1, 1, 0, 0, -1, "sandbox", 1);// 2
			AddComplexComponent( (BaseAddon) this, 194, 1, 1, 0, 0, -1, "sandbox", 1);// 3
			AddComplexComponent( (BaseAddon) this, 1341, 1, 1, 0, 51, -1, "sand", 1);// 4
			AddComplexComponent( (BaseAddon) this, 1342, 1, 0, 0, 51, -1, "sand", 1);// 5
			AddComplexComponent( (BaseAddon) this, 193, 0, 1, 0, 0, -1, "sandbox", 1);// 6
			AddComplexComponent( (BaseAddon) this, 1339, 0, 1, 0, 51, -1, "sand", 1);// 7
			AddComplexComponent( (BaseAddon) this, 194, -1, 0, 0, 0, -1, "sandbox", 1);// 8
			AddComplexComponent( (BaseAddon) this, 1340, 0, 0, 0, 51, -1, "sand", 1);// 9
			AddComplexComponent( (BaseAddon) this, 193, 1, -1, 0, 0, -1, "sandbox", 1);// 10
			AddComplexComponent( (BaseAddon) this, 194, 1, 0, 0, 0, -1, "sandbox", 1);// 11
			AddComplexComponent( (BaseAddon) this, 193, 0, -1, 0, 0, -1, "sandbox", 1);// 12
			AddComplexComponent( (BaseAddon) this, 8949, 1, 1, 7, 51, -1, "small sand castle", 1);// 13

		}

		public SmallSandCastleAddon( Serial serial ) : base( serial )
		{
		}

        private static void AddComplexComponent(BaseAddon addon, int item, int xoffset, int yoffset, int zoffset, int hue, int lightsource)
        {
            AddComplexComponent(addon, item, xoffset, yoffset, zoffset, hue, lightsource, null, 1);
        }

        private static void AddComplexComponent(BaseAddon addon, int item, int xoffset, int yoffset, int zoffset, int hue, int lightsource, string name, int amount)
        {
            AddonComponent ac;
            ac = new AddonComponent(item);
            if (name != null && name.Length > 0)
                ac.Name = name;
            if (hue != 0)
                ac.Hue = hue;
            if (amount > 1)
            {
                ac.Stackable = true;
                ac.Amount = amount;
            }
            if (lightsource != -1)
                ac.Light = (LightType) lightsource;
            addon.AddComponent(ac, xoffset, yoffset, zoffset);
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

	public class SmallSandCastleAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon => new SmallSandCastleAddon();

        [Constructable]
		public SmallSandCastleAddonDeed()
		{
			Name = "Small Sand Castle Deed";
		}

		public SmallSandCastleAddonDeed( Serial serial ) : base( serial )
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