namespace Server.Items.MusicBox
{
    public class GoodEndGameSong : MusicBoxTrack
    {
        [Constructable]
        public GoodEndGameSong()
            : base(1075132)
        {
            Song = MusicName.GoodEndGame;
            //Name = "Good End Game (U9)";
        }

        public GoodEndGameSong(Serial s)
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


