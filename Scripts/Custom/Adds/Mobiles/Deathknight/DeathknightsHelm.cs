namespace Server.Items
{
	public class DeathknightsHelm : DragonHelm
	{
		public override int InitMinHits => 75;
        public override int InitMaxHits => 95;
        public override CraftResource DefaultResource => CraftResource.Iron;

        [Constructable]
		public DeathknightsHelm()
		{
			Name = "Crown of the Deathknight";
			Hue = 1174;
		}

		public DeathknightsHelm( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}