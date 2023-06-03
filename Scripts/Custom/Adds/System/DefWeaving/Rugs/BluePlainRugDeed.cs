namespace Server.Items
{
	public class BluePlainRugAddon : BaseAddon
	{
		public override BaseAddonDeed Deed => new BluePlainRugDeed();

        [Constructable]
		public BluePlainRugAddon()
		{
			AddComponent( new AddonComponent( 0x0AC3 ), -1, -1, 0 );
			AddComponent( new AddonComponent( 0x0AF7 ),  0, -1, 0 );
			AddComponent( new AddonComponent( 0x0AC5 ),  1, -1, 0 );
			AddComponent( new AddonComponent( 0x0AF6 ), -1,  0, 0 );
			AddComponent( new AddonComponent( 0x0AC1 ),  0,  0, 0 );
			AddComponent( new AddonComponent( 0x0AF8 ),  1,  0, 0 );
			AddComponent( new AddonComponent( 0x0AC4 ), -1,  1, 0 );
			AddComponent( new AddonComponent( 0x0AF9 ),  0,  1, 0 );
			AddComponent( new AddonComponent( 0x0AC2 ),  1,  1, 0 );
		}

		public BluePlainRugAddon( Serial serial ) : base( serial )
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

	public class BluePlainRugDeed : BaseAddonDeed
	{
		public override BaseAddon Addon => new BluePlainRugAddon();
        public override int LabelNumber => 1076585; // BluePlain Rug

		[Constructable]
		public BluePlainRugDeed()
		{
		}

		public BluePlainRugDeed( Serial serial ) : base( serial )
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
