namespace Server.Items
{
    public class Hood : BaseHat
    {
        public override int BasePhysicalResistance => 5;

        public override int InitMinHits => 220;
        public override int InitMaxHits => 330;

        [Constructable]
        public Hood()
            : base(0x3907)
        {
            Name = "Hood";
            Weight = 1.0;
        }

        public Hood(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
