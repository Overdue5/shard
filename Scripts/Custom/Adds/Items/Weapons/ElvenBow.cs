namespace Server.Items
{
    public class ElvenBow : Bow
    {
        [Constructable]
        public ElvenBow()
        {
            Hue = 1941;
            Name = "Elven Bow";
            Speed = 510;
            MinDamage = 13;
            MaxDamage = 23;
            AccuracyLevel = WeaponAccuracyLevel.Supremely;
            DamageLevel = WeaponDamageLevel.Power;
            DurabilityLevel = WeaponDurabilityLevel.Substantial;
            IsRenamed = true;
        }

        public SerpentsTongue(Serial serial)
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