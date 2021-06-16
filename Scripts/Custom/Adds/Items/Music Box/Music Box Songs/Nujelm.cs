namespace Server.Items.MusicBox
{
    public class NujelmSong : MusicBoxTrack
    {
        [Constructable]
        public NujelmSong()
            : base(1075174)
        {
            Song = MusicName.Nujelm;
            //Name = "Nujel'm";
        }

        public NujelmSong(Serial s)
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


