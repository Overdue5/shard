namespace Server.Items.MusicBox
{
    public class SkaraBraePositiveSong : MusicBoxTrack
    {
        [Constructable]
        public SkaraBraePositiveSong()
            : base(1075154)
        {
            Song = MusicName.Skarabra;
            //Name = "Skara Brae (Positive)";
        }

        public SkaraBraePositiveSong(Serial s)
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


