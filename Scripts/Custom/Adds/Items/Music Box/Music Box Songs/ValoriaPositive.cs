namespace Server.Items.MusicBox
{
    public class ValoriaPositiveSong : MusicBoxTrack
    {
        [Constructable]
        public ValoriaPositiveSong()
            : base(1075151)
        {
            Song = MusicName.Ocllo;
            //Name = "Valoria (Positive)";
        }

        public ValoriaPositiveSong(Serial s)
            : base(s)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}


