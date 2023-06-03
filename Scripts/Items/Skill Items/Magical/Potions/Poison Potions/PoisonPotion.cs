namespace Server.Items
{
	public class PoisonPotion : BasePoisonPotion
	{
		public override Poison Poison => Poison.Regular;

        public override double MinPoisoningSkill => 30.0;
        public override double MaxPoisoningSkill => 70.0;

        [Constructable]
		public PoisonPotion() : base( PotionEffect.Poison )
		{
            Name = "Regular Poison Potion";
		}

		public PoisonPotion( Serial serial ) : base( serial )
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