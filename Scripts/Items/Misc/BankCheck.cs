using System.Globalization;
using Server.Engines.Quests;
using Server.Engines.Quests.Haven;
using Server.Engines.Quests.Necro;
using Server.Mobiles;
using CashBankCheckObjective=Server.Engines.Quests.Necro.CashBankCheckObjective;

namespace Server.Items
{
	public class BankCheck : Item
	{
		private int m_Worth;

		[CommandProperty( AccessLevel.GameMaster )]
		public int Worth
		{
			get => m_Worth;
            set{ m_Worth = value; InvalidateProperties(); }
		}

		public BankCheck( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); // version

			writer.Write( m_Worth );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			LootType = LootType.Blessed;

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_Worth = reader.ReadInt();
					break;
				}
			}
		}

		[Constructable]
		public BankCheck( int worth ) : base( 0x14F0 )
		{
			Weight = 1.0;
			Hue = 1944;
			LootType = LootType.Blessed;

			m_Worth = worth;
		}

		public override bool DisplayLootType => Core.AOS;

        public override int LabelNumber => 1041361; // A bank check

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            string worth;

            if (Core.ML)
                worth = m_Worth.ToString("N0", CultureInfo.GetCultureInfo("en-US"));
            else
                worth = m_Worth.ToString();

            list.Add(1060738, worth); // value: ~1_val~
        }

		public override void OnSingleClick( Mobile from )
		{
            LabelTo( from, "Bank Check Worth {0} Gold", m_Worth.ToString() );
			//from.Send( new MessageLocalizedAffix( Serial, ItemID, MessageType.Label, 0x3B2, 3, 1041361, "", AffixType.Append, String.Concat( " ", m_Worth.ToString() ), "" ) ); // A bank check:
		}

		public override void OnDoubleClick( Mobile from )
		{
			BankBox box = from.FindBankNoCreate();

			if ( box != null && IsChildOf( box ) )
			{
				Delete();

				int deposited = 0;

				int toAdd = m_Worth;

				Gold gold;

				while ( toAdd > 60000 )
				{
					gold = new Gold( 60000 );

					if ( box.TryDropItem( from, gold, false ) )
					{
						toAdd -= 60000;
						deposited += 60000;
					}
					else
					{
						gold.Delete();

						from.AddToBackpack( new BankCheck( toAdd ) );
						toAdd = 0;

						break;
					}
				}

				if ( toAdd > 0 )
				{
					gold = new Gold( toAdd );

					if ( box.TryDropItem( from, gold, false ) )
					{
						deposited += toAdd;
					}
					else
					{
						gold.Delete();

						from.AddToBackpack( new BankCheck( toAdd ) );
					}
				}

                // Gold was deposited in your account
                from.SendAsciiMessage(string.Format("{0} gold was deposited in your account", deposited));
				PlayerMobile pm = from as PlayerMobile;

				if ( pm != null )
				{
					QuestSystem qs = pm.Quest;

					if ( qs is DarkTidesQuest )
					{
						QuestObjective obj = qs.FindObjective( typeof( CashBankCheckObjective ) );

						if ( obj != null && !obj.Completed )
							obj.Complete();
					}

					if ( qs is UzeraanTurmoilQuest )
					{
						QuestObjective obj = qs.FindObjective( typeof( Engines.Quests.Haven.CashBankCheckObjective ) );

						if ( obj != null && !obj.Completed )
							obj.Complete();
					}
				}
			}
			else
			{
				from.SendLocalizedMessage( 1047026 ); // That must be in your bank box to use it.
			}
		}
	}
}