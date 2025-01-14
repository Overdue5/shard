namespace Server.Engines.BulkOrders
{
	public class BOBLargeEntry
	{
		private readonly bool m_RequireExceptional;
		private readonly BODType m_DeedType;
		private readonly BulkMaterialType m_Material;
		private readonly int m_AmountMax;
		private int m_Price;
		private readonly BOBLargeSubEntry[] m_Entries;

		public bool RequireExceptional => m_RequireExceptional;
        public BODType DeedType => m_DeedType;
        public BulkMaterialType Material => m_Material;
        public int AmountMax => m_AmountMax;
        public int Price{ get => m_Price;
            set => m_Price = value;
        }
		public BOBLargeSubEntry[] Entries => m_Entries;

        public Item Reconstruct()
		{
			LargeBOD bod = null;

			if ( m_DeedType == BODType.Smith )
				bod = new LargeSmithBOD( m_AmountMax, m_RequireExceptional, m_Material, ReconstructEntries() );
			else if ( m_DeedType == BODType.Tailor )
				bod = new LargeTailorBOD( m_AmountMax, m_RequireExceptional, m_Material, ReconstructEntries() );

			for ( int i = 0; bod != null && i < bod.Entries.Length; ++i )
				bod.Entries[i].Owner = bod;

			return bod;
		}

		private LargeBulkEntry[] ReconstructEntries()
		{
			LargeBulkEntry[] entries = new LargeBulkEntry[m_Entries.Length];

			for ( int i = 0; i < m_Entries.Length; ++i )
			{
				entries[i] = new LargeBulkEntry( null, new SmallBulkEntry( m_Entries[i].ItemType, m_Entries[i].Number, m_Entries[i].Graphic ) );
				entries[i].Amount = m_Entries[i].AmountCur;
			}

			return entries;
		}

		public BOBLargeEntry( LargeBOD bod )
		{
			m_RequireExceptional = bod.RequireExceptional;

			if ( bod is LargeTailorBOD )
				m_DeedType = BODType.Tailor;
			else if ( bod is LargeSmithBOD )
				m_DeedType = BODType.Smith;

			m_Material = bod.Material;
			m_AmountMax = bod.AmountMax;

			m_Entries = new BOBLargeSubEntry[bod.Entries.Length];

			for ( int i = 0; i < m_Entries.Length; ++i )
				m_Entries[i] = new BOBLargeSubEntry( bod.Entries[i] );
		}

		public BOBLargeEntry( GenericReader reader )
		{
			int version = reader.ReadEncodedInt();

			switch ( version )
			{
				case 0:
				{
					m_RequireExceptional = reader.ReadBool();

					m_DeedType = (BODType)reader.ReadEncodedInt();

					m_Material = (BulkMaterialType)reader.ReadEncodedInt();
					m_AmountMax = reader.ReadEncodedInt();
					m_Price = reader.ReadEncodedInt();

					m_Entries = new BOBLargeSubEntry[reader.ReadEncodedInt()];

					for ( int i = 0; i < m_Entries.Length; ++i )
						m_Entries[i] = new BOBLargeSubEntry( reader );

					break;
				}
			}
		}

		public void Serialize( GenericWriter writer )
		{
			writer.WriteEncodedInt( 0 ); // version

			writer.Write( m_RequireExceptional );

			writer.WriteEncodedInt( (int) m_DeedType );
			writer.WriteEncodedInt( (int) m_Material );
			writer.WriteEncodedInt( m_AmountMax );
			writer.WriteEncodedInt( m_Price );

			writer.WriteEncodedInt( m_Entries.Length );

			for ( int i = 0; i < m_Entries.Length; ++i )
				m_Entries[i].Serialize( writer );
		}
	}
}