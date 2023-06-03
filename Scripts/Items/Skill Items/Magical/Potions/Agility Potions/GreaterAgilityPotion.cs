using System;

namespace Server.Items
{
	public class GreaterAgilityPotion : BaseAgilityPotion
	{
		public override int DexOffset => 15;
        public override double PotionDelay => 30;
        public override TimeSpan Duration => TimeSpan.FromMinutes( 2 );

        [Constructable]
		public GreaterAgilityPotion() : base( PotionEffect.AgilityGreater )
		{
            Name = "Greater Agility Potion";
		}

		public GreaterAgilityPotion( Serial serial ) : base( serial )
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