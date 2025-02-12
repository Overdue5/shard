﻿//////////////////////////////////
//				                //
//				                //
//				                //
//    Created by Lord Talon	    //
//     www.uohelmsdeep.com	    //
//				                //
//		                        //
//		                        //
//////////////////////////////////	

/* DESCRIPTION: A book when opened displays the playerguide gump.
 */

namespace Server.Items
{
    public class PlayerGuide : Item
    {
        [Constructable]
        public PlayerGuide() : base(0xFF2)
        {
            Name = "Beginner's Guides - Read me!";
            Movable = true;
            Hue = 1925;
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.LaunchBrowser("https://discord.gg/C2kMaERQCG");
            //from.CloseGump(typeof(PlayerGuidegump));
            //from.SendGump(new PlayerGuidegump());
        }
    
        public PlayerGuide( Serial serial ) : base( serial )
            {
            }

        public override void Serialize( GenericWriter writer )
		    {
			base.Serialize( writer );
			writer.WriteEncodedInt( (int) 0 ); // version
		    }

		public override void Deserialize( GenericReader reader )
		    {
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		    }
	    }
    }
