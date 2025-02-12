/***************************************************************************
 *                               RelayInfo.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id: RelayInfo.cs 4 2006-06-15 04:28:39Z mark $
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

namespace Server.Gumps
{
	public class TextRelay
	{
		private int m_EntryID;
		private string m_Text;

		public TextRelay( int entryID, string text )
		{
			m_EntryID = entryID;
			m_Text = text;
		}

		public int EntryID => m_EntryID;

        public string Text => m_Text;
    }

	public class RelayInfo
	{
		private int m_ButtonID;
		private int[] m_Switches;
		private TextRelay[] m_TextEntries;

		public RelayInfo( int buttonID, int[] switches, TextRelay[] textEntries )
		{
			m_ButtonID = buttonID;
			m_Switches = switches;
			m_TextEntries = textEntries;
		}

		public int ButtonID => m_ButtonID;

        public int[] Switches => m_Switches;

        public TextRelay[] TextEntries => m_TextEntries;

        public bool IsSwitched( int switchID )
		{
			for ( int i = 0; i < m_Switches.Length; ++i )
			{
				if ( m_Switches[i] == switchID )
				{
					return true;
				}
			}

			return false;
		}

		public TextRelay GetTextEntry( int entryID )
		{
			for ( int i = 0; i < m_TextEntries.Length; ++i )
			{
				if ( m_TextEntries[i].EntryID == entryID )
				{
					return m_TextEntries[i];
				}
			}

			return null;
		}
	}
}