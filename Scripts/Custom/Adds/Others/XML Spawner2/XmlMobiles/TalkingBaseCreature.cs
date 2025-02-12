using System;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Engines.XmlSpawner2;
using Server.Items;
using Server.Network;
using CPA = Server.CommandPropertyAttribute;

/*
** TalkingBaseCreature
** A mobile that can be programmed with branching conversational sequences that are advanced by keywords at each sequence point.
**
** 2/10/05
** modified to use the XmlDialog attachment
*/
namespace Server.Mobiles
{
	public class TalkingBaseCreature : BaseCreature
	{ 

		private XmlDialog m_DialogAttachment;

        
		public XmlDialog DialogAttachment {get => m_DialogAttachment;
            set => m_DialogAttachment = value;
        }

		private DateTime lasteffect;		
		private int m_EItemID = 0;  // 0 = disable, 14202 = sparkle, 6251 = round stone, 7885 = light pyramid
		private int m_Duration = 70;
		private Point3D m_Offset = new Point3D(0,0,20); // overhead
		private int m_EHue = 68; // green

		[CommandProperty( AccessLevel.GameMaster )]
		public int EItemID { 
			get => m_EItemID;
            set => m_EItemID = value;
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D EOffset 
		{ 
			get => m_Offset;
            set => m_Offset = value;
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public int EDuration 
		{ 
			get => m_Duration;
            set => m_Duration = value;
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public int EHue 
		{ 
			get => m_EHue;
            set => m_EHue = value;
        }
		public void DisplayHighlight()
		{
			if(EItemID > 0)
			{
				 //SendOffsetTargetEffect(this, new Point3D(Location.X + EOffset.X, Location.Y + EOffset.Y, Location.Z + EOffset.Z), EItemID, 10, EDuration, EHue, 0);
				Effects.SendLocationEffect(new Point3D(Location.X + EOffset.X, Location.Y + EOffset.Y, Location.Z + EOffset.Z), Map, EItemID, EDuration, EHue, 0);

				lasteffect = DateTime.UtcNow;

			}
		}

		public static void SendOffsetTargetEffect( IEntity target, Point3D loc, int itemID, int speed, int duration, int hue, int renderMode )
		{
			if ( target is Mobile )
				((Mobile)target).ProcessDelta();

			Effects.SendPacket( loc, target.Map, new OffsetTargetEffect( target, loc, itemID, speed, duration, hue, renderMode ) );
		}

		public sealed class OffsetTargetEffect : HuedEffect
		{
			public OffsetTargetEffect(IEntity e, Point3D loc, int itemID, int speed, int duration, int hue, int renderMode)
				: base(EffectType.FixedFrom, e.Serial, Serial.Zero, itemID, loc, loc, speed, duration, true, false, hue, renderMode)
			{
			}
		}

		public override void OnThink()
		{
			base.OnThink();

			if(lasteffect + TimeSpan.FromSeconds(1) < DateTime.UtcNow)
			{
				DisplayHighlight();
			}
		}
        
		public override bool Move( Direction d )
		{
			bool didmove = base.Move( d );

			DisplayHighlight();

			return didmove;
		}

		private string m_TalkText;

		[CommandProperty( AccessLevel.GameMaster )]
		public string TalkText {get => m_TalkText;
            set => m_TalkText = value;
        }

		// properties below are modified to access the equivalent XmlDialog properties
		// this is largely for backward compatibility, but it does also add some convenience

		public Mobile ActivePlayer
		{
			get 
			{
				if(DialogAttachment != null)
					return DialogAttachment.ActivePlayer;
				else
					return null;
			}
			set 
			{
				if(DialogAttachment != null)
					DialogAttachment.ActivePlayer = value;
			}
		}

		public ArrayList SpeechEntries 
		{
			get 
			{
				if(DialogAttachment != null)
					return DialogAttachment.SpeechEntries;
				else
					return null;
			}
			set 
			{
				if(DialogAttachment != null)
					DialogAttachment.SpeechEntries = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan GameTOD
		{
			get
			{
				int hours;
				int minutes;

				Server.Items.Clock.GetTime(Map, Location.X, Location.Y, out  hours, out  minutes);
				return (new DateTime(DateTime.UtcNow.Year,DateTime.UtcNow.Month,DateTime.UtcNow.Day,hours, minutes,0).TimeOfDay);
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan RealTOD => DateTime.UtcNow.TimeOfDay;

        [CommandProperty( AccessLevel.GameMaster )]
		public int RealDay => DateTime.UtcNow.Day;

        [CommandProperty( AccessLevel.GameMaster )]
		public int RealMonth => DateTime.UtcNow.Month;

        [CommandProperty( AccessLevel.GameMaster )]
		public DayOfWeek RealDayOfWeek => DateTime.UtcNow.DayOfWeek;


        [CommandProperty( AccessLevel.GameMaster )]
		public MoonPhase MoonPhase => Clock.GetMoonPhase( Map, Location.X, Location.Y );

        [CommandProperty( AccessLevel.GameMaster )]
		public AccessLevel TriggerAccessLevel 
		{
			get 
			{
				if(DialogAttachment != null)
					return DialogAttachment.TriggerAccessLevel;
				else
					return AccessLevel.Player;
			}
			set 
			{
				if(DialogAttachment != null)
					DialogAttachment.TriggerAccessLevel = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime LastInteraction 
		{
			get 
			{
				if(DialogAttachment != null)
					return DialogAttachment.LastInteraction;
				else
					return DateTime.MinValue;
			}
			set 
			{
				if(DialogAttachment != null)
					DialogAttachment.LastInteraction = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool DoReset 
		{
			get => false;
            set 
			{
				if(DialogAttachment != null)
					DialogAttachment.DoReset = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsActive 
		{
			get 
			{
				if(DialogAttachment != null)
					return DialogAttachment.IsActive;
				else
					return false;
			}
			set 
			{
				if(DialogAttachment != null)
					DialogAttachment.IsActive = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool AllowGhostTrig 
		{
			get 
			{
				if(DialogAttachment != null)
					return DialogAttachment.AllowGhostTrig;
				else
					return false;
			}
			set 
			{
				if(DialogAttachment != null)
					DialogAttachment.AllowGhostTrig = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Running
		{
			get
			{ 
				if(DialogAttachment != null)
					return DialogAttachment.Running;
				else
					return false;

			}
			set
			{
				if(DialogAttachment != null)
					DialogAttachment.Running = value;
			}
		}
        
		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan ResetTime
		{
			get
			{
				if(DialogAttachment != null)
					return DialogAttachment.ResetTime;
				else
					return TimeSpan.Zero;
			}
			set
			{ 
				if(DialogAttachment != null)
					DialogAttachment.ResetTime = value;
			}
		}
        
		[CommandProperty( AccessLevel.GameMaster )]
		public int SpeechPace
		{
			get
			{
				if(DialogAttachment != null)
					return DialogAttachment.SpeechPace;
				else
					return 0;
			}
			set
			{ 
				if(DialogAttachment != null)
					DialogAttachment.SpeechPace = value;
			}

		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string Keywords
		{
			get
			{
				if(DialogAttachment != null && DialogAttachment.CurrentEntry != null)
				{
					return DialogAttachment.CurrentEntry.Keywords;
				}
				else
					return null;
			}
			set
			{ 
				if(DialogAttachment != null && DialogAttachment.CurrentEntry != null)
					DialogAttachment.CurrentEntry.Keywords = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string Action
		{
			get
			{
				if(DialogAttachment != null && DialogAttachment.CurrentEntry != null)
				{
					return DialogAttachment.CurrentEntry.Action;
				}
				else
					return null;
			}
			set
			{ 
				if(DialogAttachment != null && DialogAttachment.CurrentEntry != null)
					DialogAttachment.CurrentEntry.Action = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string Condition
		{
			get
			{
				if(DialogAttachment != null && DialogAttachment.CurrentEntry != null)
				{
					return DialogAttachment.CurrentEntry.Condition;
				}
				else
					return null;
			}
			set
			{ 
				if(DialogAttachment != null && DialogAttachment.CurrentEntry != null)
					DialogAttachment.CurrentEntry.Condition = value;
			}

		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string Text
		{
			get
			{
				if(DialogAttachment != null && DialogAttachment.CurrentEntry != null)
				{
					return DialogAttachment.CurrentEntry.Text;
				}
				else
					return null;
			}
			set
			{ 
				if(DialogAttachment != null && DialogAttachment.CurrentEntry != null)
					DialogAttachment.CurrentEntry.Text = value;
			}
		}



		[CommandProperty( AccessLevel.GameMaster )]
		public string DependsOn
		{
			get
			{
				if(DialogAttachment != null && DialogAttachment.CurrentEntry != null)
				{
					return DialogAttachment.CurrentEntry.DependsOn;
				}
				else
					return "-1";
			}
			set
			{ 
				if(DialogAttachment != null && DialogAttachment.CurrentEntry != null)
					DialogAttachment.CurrentEntry.DependsOn = value;
			}

		}
        
		[CommandProperty( AccessLevel.GameMaster )]
		public bool LockConversation
		{
			get
			{
				if(DialogAttachment != null && DialogAttachment.CurrentEntry != null)
				{
					return DialogAttachment.CurrentEntry.LockConversation;
				}
				else
					return false;
			}
			set
			{ 
				if(DialogAttachment != null && DialogAttachment.CurrentEntry != null)
					DialogAttachment.CurrentEntry.LockConversation = value;
			}

		}
        
		[CommandProperty( AccessLevel.GameMaster )]
		public MessageType SpeechStyle
		{

			get
			{
				if(DialogAttachment != null && DialogAttachment.CurrentEntry != null)
				{
					return DialogAttachment.CurrentEntry.SpeechStyle;
				}
				else
					return MessageType.Regular;
			}
			set
			{ 
				if(DialogAttachment != null && DialogAttachment.CurrentEntry != null)
					DialogAttachment.CurrentEntry.SpeechStyle = value;
			}

		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool AllowNPCTrigger
		{
			get
			{
				if(DialogAttachment != null && DialogAttachment.CurrentEntry != null)
				{
					return DialogAttachment.CurrentEntry.AllowNPCTrigger;
				}
				else
					return false;
			}
			set
			{ 
				if(DialogAttachment != null && DialogAttachment.CurrentEntry != null)
					DialogAttachment.CurrentEntry.AllowNPCTrigger = value;
			}

		}


		[CommandProperty( AccessLevel.GameMaster )]
		public int Pause
		{
        
			get
			{
				if(DialogAttachment != null && DialogAttachment.CurrentEntry != null)
				{
					return DialogAttachment.CurrentEntry.Pause;
				}
				else
					return -1;
			}
			set
			{ 
				if(DialogAttachment != null && DialogAttachment.CurrentEntry != null)
					DialogAttachment.CurrentEntry.Pause = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int PrePause
		{
			get
			{
				if(DialogAttachment != null && DialogAttachment.CurrentEntry != null)
				{
					return DialogAttachment.CurrentEntry.PrePause;
				}
				else
					return -1;
			}
			set
			{ 
				if(DialogAttachment != null && DialogAttachment.CurrentEntry != null)
					DialogAttachment.CurrentEntry.PrePause = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int ID
		{
			get
			{
				if(DialogAttachment != null && DialogAttachment.CurrentEntry != null)
				{
					return DialogAttachment.CurrentEntry.ID;
				}
				else
					return -1;
			}
			set
			{ 
				if(DialogAttachment != null && DialogAttachment.CurrentEntry != null)
					DialogAttachment.CurrentEntry.ID = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int EntryNumber
		{
			get 
			{
				if(DialogAttachment != null)
					return DialogAttachment.EntryNumber;
				else
					return -1;
			}
			set 
			{
				if(DialogAttachment != null)
				{
					DialogAttachment.EntryNumber = value;
				}
			}
		}
        
		[CommandProperty( AccessLevel.GameMaster )]
		public int ProximityRange
		{
			get 
			{
				if(DialogAttachment != null)
					return DialogAttachment.ProximityRange;
				else
					return -1;
			}
			set 
			{
				if(DialogAttachment != null)
				{
					DialogAttachment.ProximityRange = value;
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public  string ConfigFile
		{
			get 
			{
				if(DialogAttachment != null)
					return DialogAttachment.ConfigFile;
				else
					return null;
			}
			set 
			{
				if(DialogAttachment != null)
				{
					DialogAttachment.ConfigFile = value;
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public  bool LoadConfig
		{
			get => false;
            set{ if(value && DialogAttachment != null) DialogAttachment.DoLoadNPC(null,ConfigFile);}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public  bool SaveConfig
		{
			get => false;
            set
			{ 
				if(value && DialogAttachment != null) 
					DialogAttachment.DoSaveNPC(null,ConfigFile, false);
			}
		}
        
		[CommandProperty( AccessLevel.GameMaster )]
		public string TriggerOnCarried
		{
			get 
			{
				if(DialogAttachment != null)
					return DialogAttachment.TriggerOnCarried;
				else
					return null;
			}
			set 
			{
				if(DialogAttachment != null)
				{
					DialogAttachment.TriggerOnCarried = value;
				}
			}

		}
		[CommandProperty( AccessLevel.GameMaster )]
		public string NoTriggerOnCarried
		{
			get 
			{
				if(DialogAttachment != null)
					return DialogAttachment.NoTriggerOnCarried;
				else
					return null;
			}
			set 
			{
				if(DialogAttachment != null)
				{
					DialogAttachment.NoTriggerOnCarried = value;
				}
			}

		}

		public XmlDialog.SpeechEntry CurrentEntry
		{
			get 
			{
				if(DialogAttachment != null)
					return DialogAttachment.CurrentEntry;
				else
					return null;
			}
			set 
			{
				if(DialogAttachment != null)
				{
					DialogAttachment.CurrentEntry = value;
				}
			}

		}
        
		public override bool OnDragDrop( Mobile from, Item item)
		{

			return XmlQuest.RegisterGive(from, this, item);

			//return base.OnDragDrop(from, item);
		}

		private class TalkEntry : ContextMenuEntry
		{
			private readonly TalkingBaseCreature m_NPC;

			public TalkEntry( TalkingBaseCreature npc ) : base( 6146 )
			{
				m_NPC = npc;
			}

			public override void OnClick()
			{
				Mobile from = Owner.From;

				if ( m_NPC == null || m_NPC.Deleted || !from.CheckAlive() || m_NPC.DialogAttachment == null )
					return;

				// process the talk text
				//m_NPC.DialogAttachment.ProcessSpeech(from, m_NPC.TalkText);
				from.DoSpeech(m_NPC.TalkText,new int[] {},MessageType.Regular,from.SpeechHue);
			}
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			if ( from.Alive )
			{
				if ( TalkText != null && TalkText.Length > 0 && DialogAttachment != null)
				{
					list.Add( new TalkEntry( this ) );
				}
			}

			base.GetContextMenuEntries( from, list );
		}



		public TalkingBaseCreature(AIType ai,
			FightMode mode,
			int iRangePerception,
			int iRangeFight,
			double dActiveSpeed, 
			double dPassiveSpeed): base( ai, mode, iRangePerception, iRangeFight, dActiveSpeed, dPassiveSpeed )
		{
			// add the XmlDialog attachment
			m_DialogAttachment = new XmlDialog((string)null);
			XmlAttach.AttachTo(this, m_DialogAttachment);

		}

		public TalkingBaseCreature( Serial serial ) : base( serial )
		{
		}
        
		public static void Initialize()
		{
			// reestablish the DialogAttachment assignment
			foreach(Mobile m in World.Mobiles.Values)
			{
				if(m is TalkingBaseCreature)
				{
					XmlDialog xa = XmlAttach.FindAttachment(m, typeof(XmlDialog)) as XmlDialog;
					((TalkingBaseCreature)m).DialogAttachment = xa;
				}
			}
		}



		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 7 ); // version

			// version 7
			writer.Write( m_EItemID);
			writer.Write( m_Duration);
			writer.Write( m_Offset);
			writer.Write( m_EHue);

			// version 6
			writer.Write( m_TalkText);

			// Version 5
			// all serialized data now handled by the XmlDialog attachment

		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if(version < 5)
			{
				// have to add the XmlDialog attachment
				m_DialogAttachment = new XmlDialog((string)null);
				XmlAttach.AttachTo(this, m_DialogAttachment);
			}

			switch ( version )
			{
				case 7:
					m_EItemID = reader.ReadInt();
					m_Duration = reader.ReadInt();
					m_Offset = reader.ReadPoint3D();
					m_EHue = reader.ReadInt();
					goto case 6;
				case 6:
					TalkText = reader.ReadString();
					break;
				case 5:
				{
					break;
				}
				case 4:
				{
					int count = reader.ReadInt();

					SpeechEntries = new ArrayList();
					for(int i = 0; i<count;i++)
					{
						XmlDialog.SpeechEntry newentry = new XmlDialog.SpeechEntry();

						newentry.Condition = reader.ReadString();

						SpeechEntries.Add(newentry);
					}

					goto case 3;
				}
				case 3:
				{
					TriggerOnCarried = reader.ReadString();
					NoTriggerOnCarried = reader.ReadString();
					goto case 2;
				}
				case 2:
				{
					SpeechPace = reader.ReadInt();

					int count = reader.ReadInt();
					if(version < 4)
					{
						SpeechEntries = new ArrayList();
					}
					for(int i = 0; i<count;i++)
					{
						if(version < 4)
						{
							XmlDialog.SpeechEntry newentry = new XmlDialog.SpeechEntry();
    
							newentry.PrePause = reader.ReadInt();
							newentry.LockConversation = reader.ReadBool();
							newentry.AllowNPCTrigger = reader.ReadBool();
							newentry.SpeechStyle = (MessageType)reader.ReadInt();
    
							SpeechEntries.Add(newentry);
						} 
						else 
						{
							XmlDialog.SpeechEntry newentry = (XmlDialog.SpeechEntry)SpeechEntries[i];

							newentry.PrePause = reader.ReadInt();
							newentry.LockConversation = reader.ReadBool();
							newentry.AllowNPCTrigger = reader.ReadBool();
							newentry.SpeechStyle = (MessageType)reader.ReadInt();
						}
					}
					goto case 1;
				}
				case 1:
				{
					ActivePlayer = reader.ReadMobile();
					goto case 0;
				}
				case 0:
				{
					IsActive = reader.ReadBool();
					ResetTime = reader.ReadTimeSpan();
					LastInteraction = reader.ReadDateTime();
					AllowGhostTrig = reader.ReadBool();
					ProximityRange = reader.ReadInt();
					Running = reader.ReadBool();
					ConfigFile = reader.ReadString();
					int count = reader.ReadInt();
					if(version < 2)
					{
						SpeechEntries = new ArrayList();
					}
					for(int i = 0; i<count;i++)
					{

						if(version < 2)
						{
							XmlDialog.SpeechEntry newentry = new XmlDialog.SpeechEntry();

							newentry.EntryNumber = reader.ReadInt();
							newentry.ID = reader.ReadInt();
							newentry.Text = reader.ReadString();
							newentry.Keywords = reader.ReadString();
							newentry.Action = reader.ReadString();
							newentry.DependsOn = reader.ReadInt().ToString();
							newentry.Pause = reader.ReadInt();

							SpeechEntries.Add(newentry);
						} 
						else
						{
							XmlDialog.SpeechEntry newentry = (XmlDialog.SpeechEntry)SpeechEntries[i];

							newentry.EntryNumber = reader.ReadInt();
							newentry.ID = reader.ReadInt();
							newentry.Text = reader.ReadString();
							newentry.Keywords = reader.ReadString();
							newentry.Action = reader.ReadString();
							newentry.DependsOn = reader.ReadInt().ToString();
							newentry.Pause = reader.ReadInt();
						}
					}
					// read in the current entry number. Note this will also set the current entry
					EntryNumber = reader.ReadInt();
					// restart the timer if it was active
					bool isrunning = reader.ReadBool();
					if(isrunning)
					{
						Mobile trigmob = reader.ReadMobile();
						TimeSpan delay = reader.ReadTimeSpan();
						if(DialogAttachment != null)
							DialogAttachment.DoTimer(delay,trigmob);
					}
					break;
				}
			}
		}
	}
}
