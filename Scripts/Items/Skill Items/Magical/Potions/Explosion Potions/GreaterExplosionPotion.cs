namespace Server.Items
{
	public class GreaterExplosionPotion : BaseExplosionPotion
	{
		public override int MinDamage => Core.AOS ? 20 : 25;
        public override int MaxDamage => Core.AOS ? 40 : 30;

        [Constructable]
		public GreaterExplosionPotion() : base( PotionEffect.ExplosionGreater )
		{
            Name = "Greater Explosion Potion";
		}

		public GreaterExplosionPotion( Serial serial ) : base( serial )
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