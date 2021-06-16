namespace Server.Items.MusicBox
{
    public class LordBritishsCastleSong : MusicBoxTrack
    {
        [Constructable]
        public LordBritishsCastleSong()
            : base(1075148)
        {
            Song = MusicName.LBCastle;
            //Name = "Lord British's Castle";
        }

        public LordBritishsCastleSong(Serial s)
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


