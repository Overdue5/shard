namespace Server.Items.MusicBox
{
    public class PawsSong : MusicBoxTrack
    {
        [Constructable]
        public PawsSong()
            : base(1075137)
        {
            Song = MusicName.Paws;
            //Name = "PawsSong (U9)";
        }

        public PawsSong(Serial s)
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


