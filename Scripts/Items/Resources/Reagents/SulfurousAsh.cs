namespace Server.Items
{
	public class SulfurousAsh : BaseReagent, ICommodity
	{
        int ICommodity.DescriptionNumber => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public override int PotionGroupIndex => 7;

        [Constructable]
		public SulfurousAsh() : this( 1 )
		{
		}

		[Constructable]
		public SulfurousAsh( int amount ) : base( 0xF8C, amount )
		{
            //Name = "Sulfurous Ash";
		}

		public SulfurousAsh( Serial serial ) : base( serial )
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