namespace Server.Items.MusicBox
{
    public class YewSong : MusicBoxTrack
    {
        [Constructable]
        public YewSong()
            : base(1075157)
        {
            Song = MusicName.Wind;
            //Name = "YewSong";
        }

        public YewSong(Serial s)
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


