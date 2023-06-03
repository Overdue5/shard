using System;

namespace Server.Items
{
	public class AgilityPotion : BaseAgilityPotion
	{
		public override int DexOffset => 8;
        public override TimeSpan Duration => TimeSpan.FromMinutes( 2 );

        [Constructable]
		public AgilityPotion() : base( PotionEffect.Agility )
		{
            Name = "Agility Potion";
		}

		public AgilityPotion( Serial serial ) : base( serial )
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