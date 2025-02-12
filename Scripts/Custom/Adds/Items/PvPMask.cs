namespace Server.Items
{
    public class PvPMask : BaseHat
    {
        [CommandProperty(AccessLevel.Counselor)]
        public override bool EventItem
        {
            get => base.EventItem;
            set => base.EventItem = value;
        }

        [Constructable]
        public PvPMask(): base(0x1549)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
			Hue = 2958;
			Name = "Iniquitous mask";
        }

        public PvPMask(Serial serial)
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