namespace Server.Items.MusicBox
{
    public class BritainSong : MusicBoxTrack
    {
        [Constructable]
        public BritainSong()
            : base(1075145)
        {
            Song = MusicName.Britain2;
            //Name = "BritainSong";
        }

        public BritainSong(Serial s)
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


