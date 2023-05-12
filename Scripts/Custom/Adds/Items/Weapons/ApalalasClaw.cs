namespace Server.Items
{
    public class ApalalasClaw : ExecutionersAxe
    {
        [Constructable]
        public ApalalasClaw()
        {
            Hue = 1946;
            Name = "Apalalas Claw";
            Weight = 12.0;
            Speed = 450;
            MinDamage = 29;
            MaxDamage = 42;
            AccuracyLevel = WeaponAccuracyLevel.Surpassingly;
            DamageLevel = WeaponDamageLevel.Force;
            DurabilityLevel = WeaponDurabilityLevel.Substantial;
            IsRenamed = true;
        }

        public ApalalasClaw(Serial serial)
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