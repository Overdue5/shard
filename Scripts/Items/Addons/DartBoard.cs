using Server.Network;

namespace Server.Items
{
	public class DartBoard : AddonComponent
	{
		public override bool NeedsWall => true;
        public override Point3D WallPosition => East ? new Point3D( -1, 0, 0 ) : new Point3D( 0, -1, 0 );

        public bool East => ItemID == 0x1E2F;

        [Constructable]
		public DartBoard() : this( true )
		{
		}

		[Constructable]
		public DartBoard( bool east ) : base( east ? 0x1E2F : 0x1E2E )
		{
		}

		public DartBoard( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			Direction dir;
			if ( from.Location != Location )
				dir = from.GetDirectionTo( this );
			else if ( East )
				dir = Direction.West;
			else
				dir = Direction.North;

			from.Direction = dir;

			bool canThrow = true;

			if ( !from.InRange( this, 4 ) || !from.InLOS( this ) )
				canThrow = false;
			else if ( East )
				canThrow = ( dir == Direction.Left || dir == Direction.West || dir == Direction.Up );
			else
				canThrow = ( dir == Direction.Up || dir == Direction.North || dir == Direction.Right );

			if ( canThrow )
				Throw( from );
			else
				from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
		}

		public void Throw( Mobile from )
		{
			BaseKnife knife = from.Weapon as BaseKnife;

			if ( knife == null )
			{
				from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 500751 ); // Try holding a knife...
				return;
			}

			from.Animate( from.Mounted ? 26 : 9, 7, 1, true, false, 0 );
			from.MovingEffect( this, knife.ItemID, 7, 1, false, false );
			from.PlaySound( 0x238 );

			double rand = Utility.RandomDouble();

			int message;
			if ( rand < 0.05 )
				message = 500752; // BULLSEYE! 50 Points!
			else if ( rand < 0.20 )
				message = 500753; // Just missed the center! 20 points.
			else if ( rand < 0.45 )
				message = 500754; // 10 point shot.
			else if ( rand < 0.70 )
				message = 500755; // 5 pointer.
			else if ( rand < 0.85 )
				message = 500756; // 1 point.  Bad throw.
			else
				message = 500757; // Missed.

			PublicOverheadMessage( MessageType.Regular, 0x3B2, message );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class DartBoardEastAddon : BaseAddon
	{
		public override BaseAddonDeed Deed => new DartBoardEastDeed();

        public DartBoardEastAddon()
		{
			AddComponent( new DartBoard( true ), 0, 0, 0 );
		}

		public DartBoardEastAddon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class DartBoardEastDeed : BaseAddonDeed
	{
		public override BaseAddon Addon => new DartBoardEastAddon();

        public override int LabelNumber => 1044326; // dartboard (east)

		[Constructable]
		public DartBoardEastDeed()
		{
		}

		public DartBoardEastDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class DartBoardSouthAddon : BaseAddon
	{
		public override BaseAddonDeed Deed => new DartBoardSouthDeed();

        public DartBoardSouthAddon()
		{
			AddComponent( new DartBoard( false ), 0, 0, 0 );
		}

		public DartBoardSouthAddon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class DartBoardSouthDeed : BaseAddonDeed
	{
		public override BaseAddon Addon => new DartBoardSouthAddon();

        public override int LabelNumber => 1044325; // dartboard (south)

		[Constructable]
		public DartBoardSouthDeed()
		{
		}

		public DartBoardSouthDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}