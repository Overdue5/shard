using Server.Network;

namespace Server.Items
{
	public class RewardCake : Item
	{
		public override int LabelNumber => 1049786; // Happy Birthday!  ...

		[Constructable]
		public RewardCake() : base( 0x9e9 )
		{
			Stackable = false;
			Weight = 1.0;
			Hue = Utility.RandomList(0x135, 0xcd, 0x38, 0x3b, 0x42, 0x4f, 0x11e, 0x60, 0x317, 0x10, 0x136, 0x1f9, 0x1a, 0xeb, 0x86, 0x2e);
			LootType = LootType.Blessed;
		}

		public RewardCake( Serial serial ) : base( serial )
		{
		}

		public override bool DisplayLootType => false;

        public override void OnDoubleClick( Mobile from )
		{
			if (! from.InRange( GetWorldLocation(), 1 ))
			{
				from.LocalOverheadMessage( MessageType.Regular, 906, 1019045 ); // I can't reach that.
			}	
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			LootType = LootType.Blessed;

			int version = reader.ReadInt();
		}
	}
}