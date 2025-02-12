namespace Server.Items
{
	public class BatWing : BaseReagent, ICommodity
	{
        int ICommodity.DescriptionNumber => LabelNumber;
        bool ICommodity.IsDeedable => true;

        //Misc pots
        public override int PotionGroupIndex => 8;

        //Shrinks
        public override int PotionIndex => 2;

        [Constructable]
		public BatWing() : this( 1 )
		{
		}

		[Constructable]
		public BatWing( int amount ) : base( 0xF78, amount )
		{
		}

		public BatWing( Serial serial ) : base( serial )
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