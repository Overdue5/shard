using Server.Mobiles;
using Server.Engines.VeteranRewards;

namespace Server.Items
{    
	public class CharacterStatueMaker : Item, IRewardItem
	{
		public override int LabelNumber => 1076173; // Character Statue Maker
	
		private bool m_IsRewardItem;
		private StatueType m_Type;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsRewardItem
		{
			get => m_IsRewardItem;
            set{ m_IsRewardItem = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public StatueType StatueType
		{
			get => m_Type;
            set{ m_Type = value; InvalidateHue(); }
		}
		
		public CharacterStatueMaker( StatueType type ) : base( 0x32F0 )
		{
			m_Type = type;
			
			InvalidateHue();
		
			LootType = LootType.Blessed;
			Weight = 5.0;
		}

		public CharacterStatueMaker( Serial serial ) : base( serial )
		{
		}    	
		
		public override void OnDoubleClick( Mobile from )
		{
			if ( m_IsRewardItem && !RewardSystem.CheckIsUsableBy( from, this, new object[] { m_Type } ) )
				return;
				
			if ( IsChildOf( from.Backpack ) )
			{
				if ( !from.IsBodyMod )
				{
					from.SendLocalizedMessage( 1076194 ); // Select a place where you would like to put your statue.
					from.Target = new CharacterStatueTarget( this, m_Type );
				}
				else
					from.SendLocalizedMessage( 1073648 ); // You may only proceed while in your original state...
			}
			else
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
		}
		
		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			
			if ( m_IsRewardItem )
				list.Add( 1076222 ); // 6th Year Veteran Reward					
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
			
			writer.Write( (bool) m_IsRewardItem );
			writer.Write( (int) m_Type );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_IsRewardItem = reader.ReadBool();
			m_Type = (StatueType) reader.ReadInt();
		}
		
		public void InvalidateHue()
		{
			Hue = 0xB8F + (int) m_Type * 4;
		}
	}
	
	public class MarbleStatueMaker : CharacterStatueMaker
	{
		[Constructable]
		public MarbleStatueMaker() : base( StatueType.Marble )
		{
		}
		
		public MarbleStatueMaker( Serial serial ) : base( serial )
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
	
	public class JadeStatueMaker : CharacterStatueMaker
	{
		[Constructable]
		public JadeStatueMaker() : base( StatueType.Jade )
		{
		}
		
		public JadeStatueMaker( Serial serial ) : base( serial )
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
	
	public class BronzeStatueMaker : CharacterStatueMaker
	{
		[Constructable]
		public BronzeStatueMaker() : base( StatueType.Bronze )
		{
		}
		
		public BronzeStatueMaker( Serial serial ) : base( serial )
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
