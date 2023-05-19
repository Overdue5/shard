using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class SPvPBag : Bag
	{
		public override string DefaultName
		{
			get { return "a small combat bag"; }
		}

		[Constructable]
		public SPvPBag() : this( 1 )
		{
			Movable = true;
			Hue = 2524;
		}

		[Constructable]
		public SPvPBag( int amount )
		{
			DropItem( new TotalManaPotion( 10 ) );
			DropItem( new GreaterHealPotion( 10 ) );
			DropItem( new GreaterAgilityPotion( 3 ) );
			DropItem( new GreaterStrengthPotion( 3 ) );
			DropItem( new FlamestrikeScroll( 8 ) );
			DropItem( new GreaterHealScroll( 8 ) );
			DropItem( new LightningScroll( 8 ) );
			DropItem( new MagicReflectScroll( 8 ) );
			DropItem( new Bandage( 35 ) );
		}

		public SPvPBag( Serial serial ) : base( serial )
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