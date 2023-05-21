using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Engines.Craft;
using Server.Items;

namespace Server.Custom
{
    public class RoofSupport:Item
    {
        public static HashSet<RoofSupport> AllRoofSupport = new HashSet<RoofSupport>();
        public static void Initialize()
        {
            var index = DefCarpentry.CraftSystem.AddCraft(typeof(DogHouse), 1044294, "roof support", 30, 50, typeof(Log), 1044041, 10, 1044351);
            DefCarpentry.CraftSystem.AddSkill(index, SkillName.Tinkering, 20.0, 40.0);
        }

        [Constructable]
        public RoofSupport() : base(0x0093)
        {
            Weight = 20.0;
            Movable = true;
            Name = "roof support";
            AllRoofSupport.Add(this);
        }

        public RoofSupport(Serial serial) : base(serial)
        {
        }

        public override void OnDelete()
        {
            AllRoofSupport.Remove(this);
            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            AllRoofSupport.Add(this);
        }
    }
}
