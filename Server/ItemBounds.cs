/***************************************************************************
 *                               ItemBounds.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id: ItemBounds.cs 804 2012-01-02 10:24:08Z asayre $
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

using System;
using System.IO;
using Server.Logging;

namespace Server
{
    public static class ItemBounds
    {
		private static Rectangle2D[] m_Bounds;

		public static Rectangle2D[] Table => m_Bounds;

        static ItemBounds()
		{
            m_Bounds = new Rectangle2D[TileData.ItemTable.Length];

			if ( File.Exists( "Data/Binary/Bounds.bin" ) )
			{
				using ( FileStream fs = new FileStream( "Data/Binary/Bounds.bin", FileMode.Open, FileAccess.Read, FileShare.Read ) )
				{
					BinaryReader bin = new BinaryReader( fs );

                    int count = Math.Min(m_Bounds.Length, (int)(fs.Length / 8));

                    for (int i = 0; i < count; ++i)
                    {
						int xMin = bin.ReadInt16();
						int yMin = bin.ReadInt16();
						int xMax = bin.ReadInt16();
						int yMax = bin.ReadInt16();

						m_Bounds[i].Set( xMin, yMin, (xMax - xMin) + 1, (yMax - yMin) + 1 );
					}

					bin.Close();
				}
			}
			else
			{
				ConsoleLog.Write.Warning( "Warning: Data/Binary/Bounds.bin does not exist" );
			}
		}
	}
}