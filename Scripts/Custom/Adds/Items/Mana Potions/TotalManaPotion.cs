﻿namespace Server.Items
{
    public class TotalManaPotion : BaseManaPotion
    {
        [Constructable]
        public TotalManaPotion() : this(1)
        {
        }

        [Constructable]
        public TotalManaPotion(int amount) : base(PotionEffect.TotalMana)
        {
            Name = "Total Mana Potion";
            ItemID = 3853;
            Hue = 0x388;
            Amount = amount;
        }

        public TotalManaPotion(Serial serial)
            : base(serial)
        {
        }

        public override double PotionDelay => 13.0;

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
                        corpse.DropItem(new TotalManaPotion());
                }
                else
                    base.OnAdded(parent);
            }
            else
                base.OnAdded(parent);
        }
        */
        public override int Mana => 50;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}