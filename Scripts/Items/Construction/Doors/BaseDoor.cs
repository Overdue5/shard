using System;
using System.Collections.Generic;
using Server.Commands;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
	public abstract class BaseDoor : Item, ILockable, ITelekinesisable
	{
		private bool m_Open, m_Locked;
		private int m_OpenedID, m_OpenedSound;
		private int m_ClosedID, m_ClosedSound;
		private Point3D m_Offset;
		private BaseDoor m_Link;
		private uint m_KeyValue;

        /*erzse - adding guild functionality*/
        private int m_Guild = -1;

		private Timer m_Timer;

		private static readonly Point3D[] m_Offsets = new Point3D[]
			{
				new Point3D(-1, 1, 0 ),
				new Point3D( 1, 1, 0 ),
				new Point3D(-1, 0, 0 ),
				new Point3D( 1,-1, 0 ),
				new Point3D( 1, 1, 0 ),
				new Point3D( 1,-1, 0 ),
				new Point3D( 0, 0, 0 ),
				new Point3D( 0,-1, 0 ),

				new Point3D( 0, 0, 0 ),
				new Point3D( 0, 0, 0 ),
				new Point3D( 0, 0, 0 ),
				new Point3D( 0, 0, 0 )
			};

		// Called by RunUO
		public static void Initialize()
		{
			EventSink.OpenDoorMacroUsed += EventSink_OpenDoorMacroUsed;

			CommandSystem.Register( "Link", AccessLevel.GameMaster, Link_OnCommand );
			CommandSystem.Register( "ChainLink", AccessLevel.GameMaster, ChainLink_OnCommand );

            //Erzse - Adding Get GuildID Command
            CommandSystem.Register("GetGuildID", AccessLevel.GameMaster, GetGuildID_OnCommand);
		}

        #region Erze - Get Guild Command
        /// <summary> 
        /// Find the GuildID of the targeted mobile with GetGuildID_OnTarget.
        /// </summary>
        /// <param name="e">The targeted mobile.</param>
        [Usage("GetGuildID")]
        [Description("Displays the guild ID for a player")]
        private static void GetGuildID_OnCommand(CommandEventArgs e)
        {
            e.Mobile.BeginTarget(-1, false, TargetFlags.None, GetGuildID_OnTarget);
            e.Mobile.SendMessage("Target a member of a guild to see their GuildID");
        }

        /// <summary>
        /// Checking that the target is a mobile and that its guild is not null to return its GuildID.
        /// </summary>
        /// <param name="from">The command invoker.</param>
        /// <param name="targeted">Whos GuildID we want to view.</param>
        private static void GetGuildID_OnTarget(Mobile from, object targeted)
        {
            if (targeted is Mobile)
            {
                Mobile m = (Mobile)targeted;
                if (m.Guild == null || m.Guild.Disbanded)
                {
                    from.BeginTarget(-1, false, TargetFlags.None, GetGuildID_OnTarget);
                    from.SendMessage("That mobile is not in a guild.");
                }
                else
                {
                    from.SendMessage("GuildID: {0}", m.Guild.Id);
                }
            }
        }
        #endregion

		[Usage( "Link" )]
		[Description( "Links two targeted doors together." )]
		private static void Link_OnCommand( CommandEventArgs e )
		{
			e.Mobile.BeginTarget( -1, false, TargetFlags.None, Link_OnFirstTarget );
			e.Mobile.SendMessage( "Target the first door to link." );
		}

		private static void Link_OnFirstTarget( Mobile from, object targeted )
		{
			BaseDoor door = targeted as BaseDoor;

			if ( door == null )
			{
				from.BeginTarget( -1, false, TargetFlags.None, Link_OnFirstTarget );
				from.SendMessage( "That is not a door. Try again." );
			}
			else
			{
				from.BeginTarget( -1, false, TargetFlags.None, Link_OnSecondTarget, door );
				from.SendMessage( "Target the second door to link." );
			}
		}

		private static void Link_OnSecondTarget( Mobile from, object targeted, object state )
		{
			BaseDoor first = (BaseDoor)state;
			BaseDoor second = targeted as BaseDoor;

			if ( second == null )
			{
				from.BeginTarget( -1, false, TargetFlags.None, Link_OnSecondTarget, first );
				from.SendMessage( "That is not a door. Try again." );
			}
			else
			{
				first.Link = second;
				second.Link = first;
				from.SendMessage( "The doors have been linked." );
			}
		}

		[Usage( "ChainLink" )]
		[Description( "Chain-links two or more targeted doors together." )]
		private static void ChainLink_OnCommand( CommandEventArgs e )
		{
			e.Mobile.BeginTarget( -1, false, TargetFlags.None, ChainLink_OnTarget, new List<BaseDoor>() );
			e.Mobile.SendMessage( "Target the first of a sequence of doors to link." );
		}

		private static void ChainLink_OnTarget( Mobile from, object targeted, object state )
		{
			BaseDoor door = targeted as BaseDoor;

			if ( door == null )
			{
				from.BeginTarget( -1, false, TargetFlags.None, new TargetStateCallback(ChainLink_OnTarget), state );
				from.SendMessage( "That is not a door. Try again." );
			}
			else
			{
				List<BaseDoor> list = (List<BaseDoor>)state;

				if ( list.Count > 0 && list[0] == door )
				{
					if ( list.Count >= 2 )
					{
						for ( int i = 0; i < list.Count; ++i )
							list[i].Link = list[(i + 1) % list.Count];

						from.SendMessage( "The chain of doors have been linked." );
					}
					else
					{
                        from.BeginTarget(-1, false, TargetFlags.None, new TargetStateCallback(ChainLink_OnTarget), state);
                        from.SendMessage("You have not yet targeted two unique doors. Target the second door to link.");
					}
				}
				else if ( list.Contains( door ) )
				{
                    from.BeginTarget(-1, false, TargetFlags.None, new TargetStateCallback(ChainLink_OnTarget), state);
                    from.SendMessage("You have already targeted that door. Target another door, or retarget the first door to complete the chain.");
				}
				else
				{
					list.Add( door );

                    from.BeginTarget(-1, false, TargetFlags.None, new TargetStateCallback(ChainLink_OnTarget), state);

					if ( list.Count == 1 )
						from.SendMessage( "Target the second door to link." );
					else
						from.SendMessage( "Target another door to link. To complete the chain, retarget the first door." );
				}
			}
		}

        private static void EventSink_OpenDoorMacroUsed(OpenDoorMacroEventArgs args)
        {
            Mobile m = args.Mobile;

            if (m.Map != null)
            {
                int x = m.X, y = m.Y;

                switch (m.Direction & Direction.Mask)
                {
                    case Direction.North: --y; break;
                    case Direction.Right: ++x; --y; break;
                    case Direction.East: ++x; break;
                    case Direction.Down: ++x; ++y; break;
                    case Direction.South: ++y; break;
                    case Direction.Left: --x; ++y; break;
                    case Direction.West: --x; break;
                    case Direction.Up: --x; --y; break;
                }

                Sector sector = m.Map.GetSector(x, y);

                foreach (Item item in sector.Items)
                {
                    if (item.Location.X == x && item.Location.Y == y && (item.Z + item.ItemData.Height) > m.Z && (m.Z + 16) > item.Z && item is BaseDoor && m.CanSee(item) && m.InLOS(item))
                    {
                        if (m.CheckAlive())
                        {
                            m.SendLocalizedMessage(500024); // Opening door...
                            item.OnDoubleClick(m);
                        }

                        break;
                    }
                }
            }
        }

		public static Point3D GetOffset( DoorFacing facing )
		{
			return m_Offsets[(int)facing];
		}

		private class InternalTimer : Timer
		{
			private readonly BaseDoor m_Door;

			public InternalTimer( BaseDoor door ) : base( TimeSpan.FromSeconds( 65.0 ), TimeSpan.FromSeconds( 10.0 ) )
			{
				Priority = TimerPriority.OneSecond;
				m_Door = door;
			}

			protected override void OnTick()
			{
				if ( m_Door.Open && m_Door.IsFreeToClose() )
					m_Door.Open = false;
			}
		}

        #region Erzse - GuildID Property
        /// <summary>
        /// Adding an editable GuildID property to doors.
        /// </summary>
        [CommandProperty(AccessLevel.GameMaster)]
        public int GuildID
        {
            get => m_Guild;
            set
            {
                m_Guild = value;
                InvalidateProperties();
            }
        }
        #endregion

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Locked
		{
			get => m_Locked;
            set => m_Locked = value;
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public uint KeyValue
		{
			get => m_KeyValue;
            set => m_KeyValue = value;
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Open
		{
			get => m_Open;
            set
			{
				if ( m_Open != value )
				{
					m_Open = value;

					ItemID = m_Open ? m_OpenedID : m_ClosedID;

					if ( m_Open )
						Location = new Point3D( X + m_Offset.X, Y + m_Offset.Y, Z + m_Offset.Z );
					else
						Location = new Point3D( X - m_Offset.X, Y - m_Offset.Y, Z - m_Offset.Z );

					Effects.PlaySound( this, Map, m_Open ? m_OpenedSound : m_ClosedSound );

					if ( m_Open )
						m_Timer.Start();
					else
						m_Timer.Stop();
				}
			}
		}

		public bool CanClose()
		{
			if ( !m_Open )
				return true;

			Map map = Map;

			if ( map == null )
				return false;

			//Point3D p = new Point3D( X - m_Offset.X, Y - m_Offset.Y, Z - m_Offset.Z );

            return true;// CheckFit(map, p, 16);
		}

		private bool CheckFit( Map map, Point3D p, int height )
		{
			if ( map == Map.Internal )
				return false;

			int x = p.X;
			int y = p.Y;
			int z = p.Z;

			Sector sector = map.GetSector( x, y );
			List<Item> items  = sector.Items;
			List<Mobile> mobs = sector.Mobiles;

			for ( int i = 0; i < items.Count; ++i )
			{
				Item item = items[i];

                if (!(item is BaseMulti) && item.ItemID <= TileData.MaxItemValue && item.AtWorldPoint(x, y) && !(item is BaseDoor))
                {
					ItemData id = item.ItemData;
					bool surface = id.Surface;
					bool impassable = id.Impassable;

					if ( (surface || impassable) && (item.Z + id.CalcHeight) > z && (z + height) > item.Z )
						return false;
				}
			}

			for ( int i = 0; i < mobs.Count; ++i )
			{
				Mobile m = mobs[i];

				if ( m.Location.X == x && m.Location.Y == y )
				{
					if ( m.Hidden && m.AccessLevel > AccessLevel.Player )
						continue;

					if ( !m.Alive )
						continue;

					if ( (m.Z + 16) > z && (z + height) > m.Z )
						return false;
				}
			}

			return true;
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int OpenedID
		{
			get => m_OpenedID;
            set => m_OpenedID = value;
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public int ClosedID
		{
			get => m_ClosedID;
            set => m_ClosedID = value;
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public int OpenedSound
		{
			get => m_OpenedSound;
            set => m_OpenedSound = value;
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public int ClosedSound
		{
			get => m_ClosedSound;
            set => m_ClosedSound = value;
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D Offset
		{
			get => m_Offset;
            set => m_Offset = value;
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public BaseDoor Link
		{
			get
			{
				if ( m_Link != null && m_Link.Deleted )
					m_Link = null;

				return m_Link;
			}
			set => m_Link = value;
        }

		public virtual bool UseChainedFunctionality => false;

        public List<BaseDoor> GetChain()
		{
			List<BaseDoor> list = new List<BaseDoor>();
			BaseDoor c = this;

			do
			{
				list.Add( c );
				c = c.Link;
			} while ( c != null && !list.Contains( c ) );

			return list;
		}

		public bool IsFreeToClose()
		{
			if ( !UseChainedFunctionality )
				return CanClose();

			List<BaseDoor> list = GetChain();

			bool freeToClose = true;

			for ( int i = 0; freeToClose && i < list.Count; ++i )
				freeToClose = list[i].CanClose();

			return freeToClose;
		}

		public void OnTelekinesis( Mobile from )
		{
			Effects.SendLocationParticles( EffectItem.Create( Location, Map, EffectItem.DefaultDuration ), 0x376A, 9, 32, 5022 );
			Effects.PlaySound( Location, Map, 0x1F5 );

			Use( from );
		}

		public virtual bool IsInside( Mobile from )
		{
			return false;
		}

		public virtual bool UseLocks()
		{
			return true;
		}

		public virtual void Use( Mobile from )
		{
            if (GuildID != -1) //Guild door
            {
                if (from.InRange(GetWorldLocation(), 2) && from.InLOS(this))
                {
                    if (GuildID != -1 && from.Guild != null && !(from.Guild.Disbanded))
                    {
                        if (from.Guild.Id == GuildID)
                        {
                            from.MoveToWorld(Location, Map);
                            Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
                            from.PlaySound(510);
                            return;
                        }
                    }
                    from.SendAsciiMessage("You cannot use that!");
                }
                else
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.

                return;
            }

		    if ( m_Locked && !m_Open && UseLocks() )
			{      
                if (from.AccessLevel >= AccessLevel.GameMaster)
                {
                    PrivateOverheadMessage(MessageType.Regular, 906, 502502, from.NetState);
                    //from.Send( new MessageLocalized( Serial, ItemID, MessageType.Regular, 0x3B2, 3, 502502, "", "" ) ); // That is locked, but you open it with your godly powers.
                }
                else if (KeyValue != 0 && Key.ContainsKey(from.Backpack, KeyValue))
                {
                    //from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 501282 ); // You quickly unlock, open, and relock the door
                }
                else
                {
                    if (Hue == 0x44E && Map == Map.Malas) // doom door into healer room in doom
                        SendLocalizedMessageTo(from, 1060014); // Only the dead may pass.
                    else
                    {
                        PrivateOverheadMessage(MessageType.Regular, 906, 502503, from.NetState); // That is locked.
                        return;
                    }
                }
			}

			if ( m_Open && !IsFreeToClose() )
				return;

			if ( m_Open )
				OnClosed( from );
			else
				OnOpened( from );

			if ( UseChainedFunctionality )
			{
				bool open = !m_Open;

				List<BaseDoor> list = GetChain();

				for ( int i = 0; i < list.Count; ++i )
					list[i].Open = open;
			}
			else
			{
				Open = !m_Open;

				BaseDoor link = Link;

				if ( m_Open && link != null && !link.Open )
					link.Open = true;
			}
		}

		public virtual void OnOpened( Mobile from )
		{
		}

		public virtual void OnClosed( Mobile from )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
            bool needsLOS = !from.InLOS(this) && !from.InRange(GetWorldLocation(), 2);
		    int zDiff = Location.Z - from.Location.Z;

			if ( from.AccessLevel == AccessLevel.Player && (needsLOS || !from.InRange( GetWorldLocation(), 4 ) || zDiff < -10 || zDiff > 10) )
				from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
			else
				Use( from );
		}

		public BaseDoor( int closedID, int openedID, int openedSound, int closedSound, Point3D offset ) : base( closedID )
		{
			m_OpenedID = openedID;
			m_ClosedID = closedID;
			m_OpenedSound = openedSound;
			m_ClosedSound = closedSound;
			m_Offset = offset;

			m_Timer = new InternalTimer( this );

			Movable = false;
		}

		public BaseDoor( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

            /* Erzse - changed to version 1 
			writer.Write( (int) 0 ); // version
*/
            /* Erzse - added to save the GuildID */
            writer.Write(1); // version

            writer.WriteEncodedInt(m_Guild);
            /* end Erzse */
    
			writer.Write( m_KeyValue );

			writer.Write( m_Open );
			writer.Write( m_Locked );
			writer.Write( m_OpenedID );
			writer.Write( m_ClosedID );
			writer.Write( m_OpenedSound );
			writer.Write( m_ClosedSound );
			writer.Write( m_Offset );
			writer.Write( m_Link );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
                /* Erzse - GuildID is now saved. */
                case 1:
                    {
                        m_Guild = reader.ReadEncodedInt();
                        goto case 0;
                    }
                /* end Erzse */

				case 0:
				{
					m_KeyValue = reader.ReadUInt();
					m_Open = reader.ReadBool();
					m_Locked = reader.ReadBool();
					m_OpenedID = reader.ReadInt();
					m_ClosedID = reader.ReadInt();
					m_OpenedSound = reader.ReadInt();
					m_ClosedSound = reader.ReadInt();
					m_Offset = reader.ReadPoint3D();
					m_Link = reader.ReadItem() as BaseDoor;

					m_Timer = new InternalTimer( this );

					if ( m_Open )
						m_Timer.Start();

					break;
				}
			}
		}
	}
}