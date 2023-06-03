using System;

namespace Server.Items
{
	public class StrengthPotion : BaseStrengthPotion
	{
		public override int StrOffset => 8;
        public override TimeSpan Duration => TimeSpan.FromMinutes( 2 );

        [Constructable]
		public StrengthPotion() : base( PotionEffect.Strength )
		{
            Name = "Strength Potion";
		}

		public StrengthPotion( Serial serial ) : base( serial )
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