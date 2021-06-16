namespace Server.Items.MusicBox
{
    public class ElfCitySong : MusicBoxTrack
    {
        [Constructable]
        public ElfCitySong()
            : base(1075182)
        {
            Song = MusicName.ElfCity;
            //Name = "Elf City";
        }

        public ElfCitySong(Serial s)
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


