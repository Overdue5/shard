using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class LPvPBag : Bag
	{
		public override string DefaultName
		{
			get { return "a large combat bag"; }
		}

		[Constructable]
		public LPvPBag() : this( 1 )
		{
			Movable = true;
			Hue = 1176;
		}

		[Constructable]
		public LPvPBag( int amount )
		{
			DropItem( new TotalManaPotion( 15 ) );
			DropItem( new GreaterHealPotion( 10 ) );
			DropItem( new FlamestrikeScroll( 15 ) );
			DropItem( new GreaterHealScroll( 15 ) );
			DropItem( new LightningScroll( 15 ) );
			DropItem( new MagicReflectScroll( 15 ) );
			DropItem( new Bandage( 85 ) );
		}

		public LPvPBag( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}