namespace Server.Items
{
	public class TotalRefreshPotion : BaseRefreshPotion
	{
		public override int Refresh => 36;

        [Constructable]
		public TotalRefreshPotion() : base( PotionEffect.RefreshTotal )
		{
            Name = "Total Refresh Potion";
		}

		public TotalRefreshPotion( Serial serial ) : base( serial )
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