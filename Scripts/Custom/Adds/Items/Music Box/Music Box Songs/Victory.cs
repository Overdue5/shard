namespace Server.Items.MusicBox
{
    public class VictorySong : MusicBoxTrack
    {
        [Constructable]
        public VictorySong()
            : base(1075172)
        {
            Song = MusicName.Victory;
            //Name = "VictorySong";
        }

        public VictorySong(Serial s)
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


