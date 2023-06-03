using System;

namespace Server.Items
{
	public class GreaterStrengthPotion : BaseStrengthPotion
	{
		public override int StrOffset => 15;
        public override double PotionDelay => 30;
        public override TimeSpan Duration => TimeSpan.FromMinutes( 2 );

        [Constructable]
		public GreaterStrengthPotion() : base( PotionEffect.StrengthGreater )
		{
            Name = "Greater Strength Potion";
		}

		public GreaterStrengthPotion( Serial serial ) : base( serial )
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