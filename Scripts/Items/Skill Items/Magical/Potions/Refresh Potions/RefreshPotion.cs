namespace Server.Items
{
	public class RefreshPotion : BaseRefreshPotion
	{
		public override int Refresh => 18;

        [Constructable]
		public RefreshPotion() : base( PotionEffect.Refresh )
		{
            Name = "Refresh Potion";
		}

		public RefreshPotion( Serial serial ) : base( serial )
		{
		}

        //public override double PotionDelay { get { return 13.0; } }

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