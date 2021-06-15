using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class BetaBag : Bag
	{
		public override string DefaultName
		{
			get { return "a Beta Tester bag"; }
		}

		[Constructable]
		public BetaBag() : this( 1 )
		{
			Movable = true;
			Hue = 2022;
		}

		[Constructable]
		public BetaBag( int amount )
		{
			DropItem( new GreaterHealPotion( 25 ) );
			DropItem( new TotalManaPotion( 25 ) );
			DropItem( new Bandage( 150 ) );
			DropItem( new PlateArms() );
                        DropItem( new PlateChest() );
			DropItem( new PlateGloves() );
			DropItem( new PlateGorget() );
			DropItem( new PlateHelm() );
			DropItem( new PlateLegs() );
			DropItem( new FlamestrikeScroll( 25 ) );
			DropItem( new LightningScroll( 25 ) );
                        DropItem( new MagicReflectScroll( 25 ) );
                        DropItem( new GreaterHealScroll( 25 ) );
			DropItem( new HellsHalberd() );
			DropItem( new JudgementHammer() );
			DropItem( new GoblinClooba() );
			DropItem( new Gold( 30000 ) );
                        DropItem( new ShrinkPotion( 5 ) );
                        DropItem( new RareDyeTub( 1 ) );
		}
		
		public BetaBag( Serial serial ) : base( serial )
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