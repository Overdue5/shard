namespace Server.Items
{
    public class TrainingWand : BaseBashing
	{

        public override int AosStrengthReq => 40;
        public override int AosMinDamage => 11;
        public override int AosMaxDamage => 13;
        public override int AosSpeed => 44;

        public override int OldStrengthReq => 10;
        public override int OldMinDamage => 1;
        public override int OldMaxDamage => 5;
        public override int OldSpeed => 312;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 45;

        [Constructable]
		public TrainingWand() : base( 0xDF5 )
		{
			Weight = 3.0;
            Name = "Training wand";
			Layer = Layer.OneHanded;
		}

		public TrainingWand( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}