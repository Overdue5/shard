/***************************************************************************
 *                               MultiTarget.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id: MultiTarget.cs 644 2010-12-23 09:18:45Z asayre $
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

using Server.Network;

namespace Server.Targeting
{
	public abstract class MultiTarget : Target
	{
		private int m_MultiID;
		private Point3D m_Offset;

		public int MultiID
		{
			get => m_MultiID;
            set => m_MultiID = value;
        }

		public Point3D Offset
		{
			get => m_Offset;
            set => m_Offset = value;
        }

        protected MultiTarget(int multiID, Point3D offset)
            : this(multiID, offset, 10, true, TargetFlags.None)
        {
		}

        protected MultiTarget(int multiID, Point3D offset, int range, bool allowGround, TargetFlags flags)
            : base(range, allowGround, flags)
        {
			m_MultiID = multiID;
			m_Offset = offset;
		}

        public override Packet GetPacketFor(NetState ns)
        {
            if (ns.HighSeas)
                return new MultiTargetReqHS(this);
            else
                return new MultiTargetReq(this);
        }
	}
}