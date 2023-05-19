namespace Server.Items
{
    public class PvMMask : BaseHat
    {
        [CommandProperty(AccessLevel.Counselor)]
        public override bool EventItem
        {
            get { return base.EventItem; }
            set { base.EventItem = value; }
        }

        [Constructable]
        public PvMMask(): base(0x1549)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
			Hue = 1325;
			Name = "Marvelous mask";
        }

        public PvMMask(Serial serial)
            : base(serial)
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