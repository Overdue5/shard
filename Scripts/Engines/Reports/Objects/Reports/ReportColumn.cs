namespace Server.Engines.Reports
{
	public class ReportColumn : PersistableObject
	{
		#region Type Identification
		public static readonly PersistableType ThisTypeID = new PersistableType( "rc", Construct );

		private static PersistableObject Construct()
		{
			return new ReportColumn();
		}

		public override PersistableType TypeID => ThisTypeID;

        #endregion

		private string m_Width;
		private string m_Align;
		private string m_Name;

		public string Width{ get => m_Width;
            set => m_Width = value;
        }
		public string Align{ get => m_Align;
            set => m_Align = value;
        }
		public string Name{ get => m_Name;
            set => m_Name = value;
        }

		private ReportColumn()
		{
		}

		public ReportColumn( string width, string align ) : this( width, align, null )
		{
		}

		public ReportColumn( string width, string align, string name )
		{
			m_Width = width;
			m_Align = align;
			m_Name = name;
		}

		public override void SerializeAttributes( PersistanceWriter op )
		{
			op.SetString( "w", m_Width );
			op.SetString( "a", m_Align );
			op.SetString( "n", m_Name );
		}

		public override void DeserializeAttributes( PersistanceReader ip )
		{
			m_Width = Utility.Intern( ip.GetString( "w" ) );
			m_Align = Utility.Intern( ip.GetString( "a" ) );
			m_Name = Utility.Intern( ip.GetString( "n" ) );
		}
	}
}