using Server.Gumps;
using Server.Network;

namespace Server.Items
{
	public class CustomHueGroup
	{
		private readonly int m_Name;
		private readonly string m_NameString;
		private readonly int[] m_Hues;

		public int Name => m_Name;
        public string NameString => m_NameString;

        public int[] Hues => m_Hues;

        public CustomHueGroup( int name, int[] hues )
		{
			m_Name = name;
			m_Hues = hues;
		}

		public CustomHueGroup( string name, int[] hues )
		{
			m_NameString = name;
			m_Hues = hues;
		}
	}

	public class CustomHuePicker
	{
		private readonly CustomHueGroup[] m_Groups;
		private readonly bool m_DefaultSupported;
		private readonly int m_Title;
		private readonly string m_TitleString;

		public bool DefaultSupported => m_DefaultSupported;
        public CustomHueGroup[] Groups => m_Groups;
        public int Title => m_Title;
        public string TitleString => m_TitleString;

        public CustomHuePicker( CustomHueGroup[] groups, bool defaultSupported )
		{
			m_Groups = groups;
			m_DefaultSupported = defaultSupported;
		}

		public CustomHuePicker( CustomHueGroup[] groups, bool defaultSupported, int title )
		{
			m_Groups = groups;
			m_DefaultSupported = defaultSupported;
			m_Title = title;
		}

		public CustomHuePicker( CustomHueGroup[] groups, bool defaultSupported, string title )
		{
			m_Groups = groups;
			m_DefaultSupported = defaultSupported;
			m_TitleString = title;
		}

		public static readonly CustomHuePicker SpecialDyeTub = new CustomHuePicker( new CustomHueGroup[]
			{
				/* Violet */
				new CustomHueGroup( 1018345, new int[]{ 1230, 1231, 1232, 1233, 1234, 1235 } ),
				/* Tan */
				new CustomHueGroup( 1018346, new int[]{ 1501, 1502, 1503, 1504, 1505, 1506, 1507, 1508 } ),
				/* Brown */
				new CustomHueGroup( 1018347, new int[]{ 2012, 2013, 2014, 2015, 2016, 2017 } ),
				/* Dark Blue */
				new CustomHueGroup( 1018348, new int[]{ 1303, 1304, 1305, 1306, 1307, 1308 } ),
				/* Forest Green */
				new CustomHueGroup( 1018349, new int[]{ 1420, 1421, 1422, 1423, 1424, 1425, 1426 } ),
				/* Pink */
				new CustomHueGroup( 1018350, new int[]{ 1619, 1620, 1621, 1622, 1623, 1624, 1625, 1626 } ),
				/* Red */
				new CustomHueGroup( 1018351, new int[]{ 1640, 1641, 1642, 1643, 1644 } ),
				/* Olive */
				new CustomHueGroup( 1018352, new int[]{ 2001, 2002, 2003, 2004, 2005 } )
			}, false, 1018344 );

		public static readonly CustomHuePicker LeatherDyeTub = new CustomHuePicker( new CustomHueGroup[]
			{
				/* Dull Copper */
				new CustomHueGroup( 1018332, new int[]{ 2419, 2420, 2421, 2422, 2423, 2424 } ),
				/* Shadow Iron */
				new CustomHueGroup( 1018333, new int[]{ 2406, 2407, 2408, 2409, 2410, 2411, 2412 } ),
				/* Copper */
				new CustomHueGroup( 1018334, new int[]{ 2413, 2414, 2415, 2416, 2417, 2418 } ),
				/* Bronze */
				new CustomHueGroup( 1018335, new int[]{ 2414, 2415, 2416, 2417, 2418 } ),
				/* Glden */
				new CustomHueGroup( 1018336, new int[]{ 2213, 2214, 2215, 2216, 2217, 2218 } ),
				/* Agapite */
				new CustomHueGroup( 1018337, new int[]{ 2425, 2426, 2427, 2428, 2429, 2430 } ),
				/* Verite */
				new CustomHueGroup( 1018338, new int[]{ 2207, 2208, 2209, 2210, 2211, 2212 } ),
				/* Valorite */
				new CustomHueGroup( 1018339, new int[]{ 2219, 2220, 2221, 2222, 2223, 2224 } ),
				/* Reds */
				new CustomHueGroup( 1018340, new int[]{ 2113, 2114, 2115, 2116, 2117, 2118 } ),
				/* Blues */
				new CustomHueGroup( 1018341, new int[]{ 2119, 2120, 2121, 2122, 2123, 2124 } ),
				/* Greens */
				new CustomHueGroup( 1018342, new int[]{ 2126, 2127, 2128, 2129, 2130 } ),
				/* Yellows */
				new CustomHueGroup( 1018343, new int[]{ 2213, 2214, 2215, 2216, 2217, 2218 } )
			}, true );

		public static readonly CustomHuePicker RewardDyeTub = new CustomHuePicker( new CustomHueGroup[]
			{
				//White
				new CustomHueGroup( 1060821, new int[]{ 1936, 1156, 1153, 1157, 1953, 1175, 1154 } ),
				//Blues
				new CustomHueGroup( 1018341, new int[]{ 1930, 1935, /*1152 place,*/1165, 1942, 1952, 1161, 1963, 1954, 1928, 1167, 1926, 1947, 1938 } ),
				//Violet
				new CustomHueGroup( 1018345, new int[]{ 1921, 1934, 1163, 1924, 1166, 1966, 1155, 1961, 1922 } ),
				//Reds
				new CustomHueGroup( 1018340, new int[]{ 1168, 1164, 1967, 1929, 1958, 1943, 1955, 1949, 1948, 1945, 1171, 1932, 1176 } ),
				//Greens
				new CustomHueGroup( 1018342, new int[]{ 1162, 1931, 1159, 1941, 1957, 1169, 1927, 1964, 1960 } ),
				//Greens
				new CustomHueGroup( 1018343, new int[]{ 1158, 1939, 1925, 1923, 1959, 1933, 1956, 1946, 1965, 1944, 1962, 1968 } ),
				//Brown
				new CustomHueGroup( 1018347, new int[]{ 1940, 1160, 1951, 1172, 1950 } ),
				//Greys
				new CustomHueGroup( 1075689, new int[]{ 1170, 1173, 1174, 1937, 1 } )
			}, false, 1018344 );
	}

	public delegate void CustomHuePickerCallback( Mobile from, object state, int hue );

	public class CustomHuePickerGump : Gump
	{
		private readonly Mobile m_From;
		private readonly CustomHuePicker m_Definition;
		private readonly CustomHuePickerCallback m_Callback;
		private readonly object m_State;

		private int GetRadioID( int group, int index )
		{
			return (index * m_Definition.Groups.Length) + group;
		}

		private void RenderBackground()
		{
			AddPage( 0 );

			AddBackground( 0, 0, 450, 450, 5054 );
			AddBackground( 10, 10, 430, 430, 3000 );

			if ( m_Definition.TitleString != null )
				AddHtml( 20, 30, 400, 25, m_Definition.TitleString, false, false );
			else if ( m_Definition.Title > 0 )
				AddHtmlLocalized( 20, 30, 400, 25, m_Definition.Title, false, false );

			AddButton( 20, 400, 4005, 4007, 1, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 55, 400, 200, 25, 1011036, false, false ); // OKAY

			//AddButton( 20, 350, 4005, 4007, 3, GumpButtonType.Reply, 0 );
			//AddLabel( 55, 350, 200, "Preview" );

			if ( m_Definition.DefaultSupported )
			{
				AddButton( 200, 400, 4005, 4007, 2, GumpButtonType.Reply, 0 );
				AddLabel( 235, 400, 0, "DEFAULT" );
			}
		}

		private void RenderCategories()
		{
			CustomHueGroup[] groups = m_Definition.Groups;

			for ( int i = 0; i < groups.Length; ++i )
			{
				AddButton( 30, 85 + (i * 25), 5224, 5224, 0, GumpButtonType.Page, 1 + i );

				if ( groups[i].NameString != null )
					AddHtml( 55, 85 + (i * 25), 200, 25, groups[i].NameString, false, false );
				else
					AddHtmlLocalized( 55, 85 + (i * 25), 200, 25, groups[i].Name, false, false );
			}

			for ( int i = 0; i < groups.Length; ++i )
			{
				AddPage( 1 + i );

				int[] hues = groups[i].Hues;

				for ( int j = 0; j < hues.Length; ++j )
				{
					AddRadio( 260, 90 + (j * 25), 210, 211, false, GetRadioID( i, j ) );
					AddLabel( 278, 90 + (j * 25), hues[j] - 1, "*****" );
				}
			}
		}

		public CustomHuePickerGump( Mobile from, CustomHuePicker definition, CustomHuePickerCallback callback, object state ) : base( 50, 50 )
		{
			m_From = from;
			m_Definition = definition;
			m_Callback = callback;
			m_State = state;

			RenderBackground();
			RenderCategories();
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
 			switch ( info.ButtonID )
			{
				case 1: // Okay
				{
					int[] switches = info.Switches;

					if ( switches.Length > 0 )
					{
						int index = switches[0];

						int group = index % m_Definition.Groups.Length;
						index /= m_Definition.Groups.Length;

						if ( group >= 0 && group < m_Definition.Groups.Length )
						{
							int[] hues = m_Definition.Groups[group].Hues;

							if ( index >= 0 && index < hues.Length )
								m_Callback( m_From, m_State, hues[index] );
						}
					}

					break;
				}
				case 2: // Default
				{
					if ( m_Definition.DefaultSupported )
						m_Callback( m_From, m_State, 0 );

					break;
				}
			}
		}
	}
}