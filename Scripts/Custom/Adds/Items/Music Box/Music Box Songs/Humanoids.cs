namespace Server.Items.MusicBox
{
    public class HumanoidsSong : MusicBoxTrack
    {
        [Constructable]
        public HumanoidsSong()
            : base(1075135)
        {
            Song = MusicName.Humanoids_U9;
            //Name = "HumanoidsSong (U9)";
        }

        public HumanoidsSong(Serial s)
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


