using Server.Commands.GMUtils;

namespace Server.Items
{
	public class RightArm : Item
	{
	    [CommandProperty(AccessLevel.GameMaster)]
	    public Mobile Owner { get; set; }

	    [Constructable]
        public RightArm()
            : base(7397)
		{
		}

		public RightArm( Serial serial ) : base( serial )
		{
		}

        public override void OnDoubleClick(Mobile from)
        {
            // Fill the Mobile with FillFactor
            if (Food.FillHunger(from, 4))
            {
                // Play a random "eat" sound
                from.PlaySound(Utility.Random(0x3A, 3));

                if (from.Body.IsHuman && !from.Mounted)
                    from.Animate(34, 5, 1, true, false, 0);

                if (Owner != null)
                    from.SayAction(GMExtendMethods.EmotionalTextHue.StrangeAction, $"You see {from.Name} eat some {Name}");
//from.PublicOverheadMessage(MessageType.Emote, 0x22, true, string.Format("*You see {0} eat some {1}*", from.Name, Name));

                Consume();
            }
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); // version

            //Ver 0
            writer.Write(Owner);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            Owner = reader.ReadMobile();
		}
	}
}