namespace Server.Items
{		
	public enum WaterState
	{
		Dead,
		Dying,
		Unhealthy,
		Healthy,
		Strong,
	}
	
	public enum FoodState
	{
		Dead,
		Starving,
		Hungry,
		Full,
		Overfed,
	}
	
	[PropertyObject]
	public class AquariumState
	{
		private int m_State;
		private int m_Maintain;
		private int m_Improve;
		private int m_Added;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int State
		{
			get => m_State;
            set => m_State = value;
        }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int Maintain
		{
			get => m_Maintain;
            set => m_Maintain = value;
        }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int Improve
		{
			get => m_Improve;
            set => m_Improve = value;
        }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int Added
		{
			get => m_Added;
            set => m_Added = value;
        }
		
		public AquariumState()
		{
		}
		
		public override string ToString()
		{
			return "...";
		}
		
		public virtual void Serialize( GenericWriter writer )
		{
			writer.Write( 0 ); // version
			
			writer.Write( m_State );
			writer.Write( m_Maintain );
			writer.Write( m_Improve );
			writer.Write( m_Added );
		}
		
		public virtual void Deserialize( GenericReader reader )
		{
			int version = reader.ReadInt();
			
			m_State = reader.ReadInt();
			m_Maintain = reader.ReadInt();
			m_Improve = reader.ReadInt();
			m_Added = reader.ReadInt();
		}
	}
}
