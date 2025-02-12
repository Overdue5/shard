using System;
using Server.Mobiles;

namespace Server.Items
{
	public class Torch : BaseEquipableLight
	{
		public override int LitItemID => 0xA12;
        public override int UnlitItemID => 0xF6B;

        public override int LitSound => 0x54;
        public override int UnlitSound => 0x4BB;

        [Constructable]
		public Torch() : base( 0xF6B )
		{
			if ( Burnout )
				Duration = TimeSpan.FromMinutes( 30 );
			else
				Duration = TimeSpan.Zero;

			Burning = false;
			Light = LightType.Circle300;
			Weight = 1.0;
		}

		public override void OnAdded( object parent )
		{
			base.OnAdded( parent );

			if ( parent is Mobile && Burning )
				MeerMage.StopEffect( (Mobile)parent, true );
		}

		public override void Ignite()
		{
			base.Ignite();

			if ( Parent is Mobile && Burning )
				MeerMage.StopEffect( (Mobile)Parent, true );
		}

		public Torch( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( Weight == 2.0 )
				Weight = 1.0;
		}
	}
}