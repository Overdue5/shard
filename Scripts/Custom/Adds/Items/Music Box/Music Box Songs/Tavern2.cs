namespace Server.Items.MusicBox
{
    public class Tavern2Song : MusicBoxTrack
    {
        [Constructable]
        public Tavern2Song()
            : base(1075165)
        {
            Song = MusicName.Tavern02;
            //Name = "Tavern 2";
        }

        public Tavern2Song(Serial s)
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


