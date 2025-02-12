namespace Server.Items
{
	public class LesserPoisonPotion : BasePoisonPotion
	{
		public override Poison Poison => Poison.Lesser;

        public override double MinPoisoningSkill => 0.0;
        public override double MaxPoisoningSkill => 60.0;

        [Constructable]
		public LesserPoisonPotion() : base( PotionEffect.PoisonLesser )
		{
            Name = "Lesser Poison Potion";
		}

		public LesserPoisonPotion( Serial serial ) : base( serial )
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