namespace Server.Items
{
	public class RedRugAddon : BaseAddon
	{
		public override BaseAddonDeed Deed => new RedRugDeed();

        [Constructable]
		public RedRugAddon() 
		{
			AddComponent( new AddonComponent( 0x0ACA ), -1, -1, 0 );
			AddComponent( new AddonComponent( 0x0ACE ),  0, -1, 0 );
			AddComponent( new AddonComponent( 0x0ACC ),  1, -1, 0 );
			AddComponent( new AddonComponent( 0x0ACD ), -1,  0, 0 );
			AddComponent( new AddonComponent( 0x0AC8 ),  0,  0, 0 );
			AddComponent( new AddonComponent( 0x0ACF ),  1,  0, 0 );
			AddComponent( new AddonComponent( 0x0ACB ), -1,  1, 0 );
			AddComponent( new AddonComponent( 0x0AD0 ),  0,  1, 0 );
			AddComponent( new AddonComponent( 0x0AC9 ),  1,  1, 0 );
		}

		public RedRugAddon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class RedRugDeed : BaseAddonDeed
	{
		public override BaseAddon Addon => new RedRugAddon();

        [Constructable]
		public RedRugDeed()
		{
			Name = "Red rug";
		}

		public RedRugDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
