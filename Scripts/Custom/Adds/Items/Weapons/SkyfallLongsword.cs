namespace Server.Items
{
    public class SkyfallLongsword : Longsword
    {
        [Constructable]
        public SkyfallLongsword()
        {
            Hue = 1952;
            Name = "Skyfall Longsword";
            Weight = 6.0;
            Speed = 400;
            MinDamage = 34;
            MaxDamage = 48;
            AccuracyLevel = WeaponAccuracyLevel.Eminently;
            DamageLevel = WeaponDamageLevel.Force;
            DurabilityLevel = WeaponDurabilityLevel.Substantial;
            IsRenamed = true;
            Slayer = SlayerName.Repond;
        }

        public SkyfallLongsword(Serial serial)
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