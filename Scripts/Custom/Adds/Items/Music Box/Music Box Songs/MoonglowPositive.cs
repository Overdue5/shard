namespace Server.Items.MusicBox
{
    public class MoonglowPositiveSong : MusicBoxTrack
    {
        [Constructable]
        public MoonglowPositiveSong()
            : base(1075177)
        {
            Song = MusicName.Moonglow;
            //Name = "Moonglow (Positive)";
        }

        public MoonglowPositiveSong(Serial s)
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


