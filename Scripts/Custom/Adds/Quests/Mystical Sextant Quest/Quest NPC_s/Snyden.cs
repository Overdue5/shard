using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
	[CorpseName( "Snyden's corpse" )]
	public class Snyden : Mobile
	{
        public override bool ClickTitle => true;
        public virtual bool IsInvulnerable => true;

        [Constructable]
		public Snyden()
		{
			Name = "Snyden";
                        Title = "the Shipmate";
			Body = 0x190;
			Hue = Utility.RandomSkinHue();
			Blessed = true;
			CantWalk = true;
			Direction = Direction.South;

			Boots bt = new Boots();
                        bt.Hue = 0;
                        AddItem( bt );

                        LongPants lp = new LongPants();
                        lp.Hue = 0;
                        AddItem( lp );

		        FancyShirt fs = new FancyShirt();
                        fs.Hue = 0;
                        AddItem( fs );

			TricorneHat th = new TricorneHat();
                        th.Hue = 0;
                        AddItem( th );			

	                Scimitar sc = new Scimitar();
                        AddItem( sc );

			GoldBeadNecklace gn = new GoldBeadNecklace();
			AddItem( gn );

			GoldBracelet gb = new GoldBracelet();
			AddItem( gb );

			GoldEarrings ge = new GoldEarrings();
			AddItem( ge );

			GoldRing gr = new GoldRing();
			AddItem( gr );			
                        
                        AddItem( new PonyTail(1149));
                     
			AddItem( new Vandyke(1149));
			
		}

		public Snyden( Serial serial ) : base( serial )
		{
		}
        public override bool ShowContextMenu => true; //
        public override void GetContextMenuEntries (Mobile from, System.Collections.Generic.List<ContextMenuEntry> list) 
	        { 
	                base.GetContextMenuEntries( from, list ); 
        	        list.Add( new SnydenEntry( from, this ) ); 
	        } 

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}

		public class SnydenEntry : ContextMenuEntry
		{
			private readonly Mobile m_Mobile;
			private Mobile m_Giver;
			
			public SnydenEntry( Mobile from, Mobile giver ) : base( 6146, 3 )
			{
				m_Mobile = from;
				m_Giver = giver;
			}

			public override void OnClick()
			{
				

                          if( !( m_Mobile is PlayerMobile ) )
					return;
				
				PlayerMobile mobile = (PlayerMobile) m_Mobile;

				{
					if ( ! mobile.HasGump( typeof( SnydenGump ) ) )
					{
						mobile.SendGump( new SnydenGump( mobile ));						
					} 
				}
			}
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{          		
         	        Mobile m = from;
			PlayerMobile mobile = m as PlayerMobile;

			if ( mobile != null)
			{
				if( dropped is LetterToSnyden )
				{
					dropped.Delete();					
					mobile.SendGump( new SnydenStartGump( mobile ));
					return true;
				}

				if( dropped is DecayingHead )
				{
					dropped.Delete();
					mobile.AddToBackpack( new ReinforcedHinge() );
					mobile.AddToBackpack( new LetterToBarthus() );
					mobile.SendGump( new SnydenFinishGump( mobile ));
					return true;
				}
				else
					{
						mobile.SendMessage("I have no need for this item.");
					}
				}
			else
				{
					PrivateOverheadMessage( MessageType.Regular, 1153, false, "I have no need for this item.", mobile.NetState );
				}
			return false;
		}
	}
}