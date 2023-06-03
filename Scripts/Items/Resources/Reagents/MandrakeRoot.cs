namespace Server.Items
{
	public class MandrakeRoot : BaseReagent, ICommodity
	{
        int ICommodity.DescriptionNumber => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public override int PotionGroupIndex => 4;

        [Constructable]
		public MandrakeRoot() : this( 1 )
		{
		}

		[Constructable]
		public MandrakeRoot( int amount ) : base( 0xF86, amount )
		{
            //Name = "Mandrake Root";
		}

		public MandrakeRoot( Serial serial ) : base( serial )
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