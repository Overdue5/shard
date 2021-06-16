namespace Server.Items.MusicBox
{
    public class TaikoSong : MusicBoxTrack
    {
        [Constructable]
        public TaikoSong()
            : base(1075180)
        {
            Song = MusicName.Taiko;
            //Name = "TaikoSong";
        }

        public TaikoSong(Serial s)
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


