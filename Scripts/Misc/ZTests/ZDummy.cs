using System;
using Server.Items;
using Server.Mobiles;
using Server.Spells;

namespace Server.Mobiles
{
	public class ZDummy : BaseCreature
	{
		[Constructable]
		public ZDummy() : base(AIType.AI_Archer, FightMode.Aggressor, 10, 1, 0.2, 0.4)
		{
			Body = 400 + Utility.Random(2);
			AddItem(new Backpack());
		}

		public ZDummy(Serial serial)
			: base(serial)
		{

		}

		public override void Serialize(GenericWriter writer)
		{
			
		}

		public override void Deserialize(GenericReader reader)
		{
			
		}
	}
}