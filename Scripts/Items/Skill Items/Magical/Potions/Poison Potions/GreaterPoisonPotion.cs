namespace Server.Items
{
	public class GreaterPoisonPotion : BasePoisonPotion
	{
		public override Poison Poison => Poison.Greater;

        public override double MinPoisoningSkill => 60.0;
        public override double MaxPoisoningSkill => 100.0;

        [Constructable]
		public GreaterPoisonPotion() : base( PotionEffect.PoisonGreater )
		{
            Name = "Greater Poison Potion";
		}

		public GreaterPoisonPotion( Serial serial ) : base( serial )
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