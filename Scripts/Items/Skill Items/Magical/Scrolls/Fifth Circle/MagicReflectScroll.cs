//needed for custom scrolls

namespace Server.Items
{
    public class MagicReflectScroll : SpellScroll
    {
        public override int ManaCost => 19; //Loki edit: was 16

        [Constructable]
        public MagicReflectScroll()
            : this(1)
        {
        }

        [Constructable]
        public MagicReflectScroll(int amount)
            : base(35, 0x1F50, amount)
        {
            //Name = "Magic Reflect Scroll";
        }

        public MagicReflectScroll(Serial serial)
            : base(serial)
        {
        }

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