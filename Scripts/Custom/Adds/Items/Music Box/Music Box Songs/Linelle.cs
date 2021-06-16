namespace Server.Items.MusicBox
{
    public class LinelleSong : MusicBoxTrack
    {
        [Constructable]
        public LinelleSong()
            : base(1075185)
        {
            Song = MusicName.Linelle;
            //Name = "LinelleSong";
        }

        public LinelleSong(Serial s)
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


