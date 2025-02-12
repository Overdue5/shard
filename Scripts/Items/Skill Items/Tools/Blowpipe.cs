using Server.Engines.Craft;

namespace Server.Items
{
	[Flipable( 0xE8A, 0xE89 )]
	public class Blowpipe : BaseTool
	{
		public override CraftSystem CraftSystem => DefGlassblowing.CraftSystem;

        public override int LabelNumber => 1044608; // blow pipe

		[Constructable]
		public Blowpipe() : base( 0xE8A )
		{
			Weight = 4.0;
			Hue = 0x3B9;
		}

		[Constructable]
		public Blowpipe( int uses ) : base( uses, 0xE8A )
		{
			Weight = 4.0;
			Hue = 0x3B9;
		}

        public override bool OnEquip(Mobile from)
        {
            if (from.Skills[SkillName.Tactics].Base < 120.0)
            {
                from.SendAsciiMessage("You cannot equip this");
                return false;
            }
            return base.OnEquip(from);
        }

		public Blowpipe( Serial serial ) : base( serial )
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

			if ( Weight == 2.0 )
				Weight = 4.0;
		}
	}
}