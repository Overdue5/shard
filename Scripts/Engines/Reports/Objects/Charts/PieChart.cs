namespace Server.Engines.Reports
{
	public class PieChart : Chart
	{
		#region Type Identification
		public static readonly PersistableType ThisTypeID = new PersistableType( "pc", Construct );

		private static PersistableObject Construct()
		{
			return new PieChart();
		}

		public override PersistableType TypeID => ThisTypeID;

        #endregion

		private bool m_ShowPercents;

		public bool ShowPercents{ get => m_ShowPercents;
            set => m_ShowPercents = value;
        }

		public PieChart( string name, string fileName, bool showPercents )
		{
			m_Name = name;
			m_FileName = fileName;
			m_ShowPercents = showPercents;
		}

		private PieChart()
		{
		}

		public override void SerializeAttributes( PersistanceWriter op )
		{
			base.SerializeAttributes( op );

			op.SetBoolean( "p", m_ShowPercents );
		}

		public override void DeserializeAttributes( PersistanceReader ip )
		{
			base.DeserializeAttributes( ip );

			m_ShowPercents = ip.GetBoolean( "p" );
		}
	}
}