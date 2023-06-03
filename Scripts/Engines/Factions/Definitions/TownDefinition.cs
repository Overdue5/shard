namespace Server.Factions
{
	public class TownDefinition
	{
		private readonly int m_Sort;
		private readonly int m_SigilID;

		private readonly string m_Region;

		private readonly string m_FriendlyName;

		private readonly TextDefinition m_TownName;
		private readonly TextDefinition m_FactionTownStoneHeader;
		private readonly TextDefinition m_StrongholdMonolithName;
		private readonly TextDefinition m_TownMonolithName;
		private readonly TextDefinition m_FactionTownStoneName;
		private readonly TextDefinition m_SigilName;
		private readonly TextDefinition m_CorruptedSigilName;

		private readonly Point3D m_Monolith;
		private readonly Point3D m_FactionTownStone;

		public int Sort => m_Sort;
        public int SigilID => m_SigilID;

        public string Region => m_Region;
        public string FriendlyName => m_FriendlyName;

        public TextDefinition TownName => m_TownName;
        public TextDefinition FactionTownStoneHeader => m_FactionTownStoneHeader;
        public TextDefinition StrongholdMonolithName => m_StrongholdMonolithName;
        public TextDefinition TownMonolithName => m_TownMonolithName;
        public TextDefinition FactionTownStoneName => m_FactionTownStoneName;
        public TextDefinition SigilName => m_SigilName;
        public TextDefinition CorruptedSigilName => m_CorruptedSigilName;

        public Point3D Monolith => m_Monolith;
        public Point3D FactionTownStone => m_FactionTownStone;

        public TownDefinition( int sort, int sigilID, string region, string friendlyName, TextDefinition townName, TextDefinition FactionTownStoneHeader, TextDefinition strongholdMonolithName, TextDefinition townMonolithName, TextDefinition FactionTownStoneName, TextDefinition sigilName, TextDefinition corruptedSigilName, Point3D monolith, Point3D FactionTownStone )
		{
			m_Sort = sort;
			m_SigilID = sigilID;
			m_Region = region;
			m_FriendlyName = friendlyName;
			m_TownName = townName;
			m_FactionTownStoneHeader = FactionTownStoneHeader;
			m_StrongholdMonolithName = strongholdMonolithName;
			m_TownMonolithName = townMonolithName;
			m_FactionTownStoneName = FactionTownStoneName;
			m_SigilName = sigilName;
			m_CorruptedSigilName = corruptedSigilName;
			m_Monolith = monolith;
			m_FactionTownStone = FactionTownStone;
		}
	}
}