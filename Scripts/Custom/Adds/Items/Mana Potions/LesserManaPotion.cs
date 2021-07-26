namespace Server.Items
{
    public class LesserManaPotion : BaseManaPotion
	{
		[Constructable]
		public LesserManaPotion() : this(1)
		{
		}

        [Constructable]
        public LesserManaPotion(int amount) : base(PotionEffect.Mana)
        {
            Name = "Lesser Mana Potion";
            Hue = 0x388;
            Amount = amount;
        }

		public LesserManaPotion( Serial serial ) : base( serial )
		{
		}
        /*
        //Maka - unstacking pots on death, stacking on resurrection
        public override void OnAdded(object parent)
        {
            int amount = Amount;
            if (amount != 1 && parent is Corpse)
            {
                Corpse corpse = (Corpse)parent;
                if (corpse.Owner is PlayerMobile)
                {
                    Delete();
                    for (int i = 0; i < amount; i++)
                        corpse.DropItem(new LesserManaPotion());
                }
                else
                    base.OnAdded(parent);
            }
            else
                base.OnAdded(parent);
        }
        */
		public override int Mana
		{
			get { return 9; }
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