namespace Server.Items
{
	public class BetaStone : Item
	{
		public override string DefaultName => "a Beta Tester Supply Stone";

        [Constructable]
		public BetaStone() : base( 0xED4 )
		{
			Movable = false;
			Hue = 2022;
		}

		public override void OnDoubleClick( Mobile from )
		{
			BetaBag betabag = new BetaBag();

			if ( !from.AddToBackpack( betabag ) )
				betabag.Delete();
		}

		public BetaStone( Serial serial ) : base( serial )
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