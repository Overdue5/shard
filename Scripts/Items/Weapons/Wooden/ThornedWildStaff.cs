namespace Server.Items
{
	public class ThornedWildStaff : WildStaff
	{
		public override int LabelNumber => 1073551; // thorned wild staff

		[Constructable]
		public ThornedWildStaff()
		{
			Attributes.ReflectPhysical = 12;
		}

		public ThornedWildStaff( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}
