namespace Server.Items.MusicBox
{
    public class BritainPositiveSong : MusicBoxTrack
    {
        [Constructable]
        public BritainPositiveSong()
            : base(1075144)
        {
            Song = MusicName.Britain1;
            //Name = "Britain (Positive)";
        }

        public BritainPositiveSong(Serial s)
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


