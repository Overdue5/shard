namespace Server.Items
{
    public class SoulBardiche : Bardiche
    {
        [Constructable]
        public SoulBardiche()
        {
            Hue = 1175;
            Name = "Soul Infused Bardiche";
            Weight = 15.0;
            Speed = 302;
            MinDamage = 30;
            MaxDamage = 48;
            AccuracyLevel = WeaponAccuracyLevel.Surpassingly;
            DamageLevel = WeaponDamageLevel.Force;
            DurabilityLevel = WeaponDurabilityLevel.Substantial;
            IsRenamed = true;
            Slayer = SlayerName.OrcSlaying;
        }

        public SoulBardiche(Serial serial)
            : base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, Sphere.ComputeCustomWeaponName(this));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}