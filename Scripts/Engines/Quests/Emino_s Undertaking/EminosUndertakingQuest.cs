using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Ninja
{
	public class EminosUndertakingQuest : QuestSystem
	{
		private static readonly Type[] m_TypeReferenceTable = new Type[]
			{
				typeof( AcceptConversation ),
				typeof( FindZoelConversation ),
				typeof( RadarConversation ),
				typeof( EnterCaveConversation ),
				typeof( SneakPastGuardiansConversation ),
				typeof( NeedToHideConversation ),
				typeof( UseTeleporterConversation ),
				typeof( GiveZoelNoteConversation ),
				typeof( LostNoteConversation ),
				typeof( GainInnInformationConversation ),
				typeof( ReturnFromInnConversation ),
				typeof( SearchForSwordConversation ),
				typeof( HallwayWalkConversation ),
				typeof( ReturnSwordConversation ),
				typeof( SlayHenchmenConversation ),
				typeof( ContinueSlayHenchmenConversation ),
				typeof( GiveEminoSwordConversation ),
				typeof( LostSwordConversation ),
				typeof( EarnGiftsConversation ),
				typeof( EarnLessGiftsConversation ),
				typeof( FindEminoBeginObjective ),
				typeof( FindZoelObjective ),
				typeof( EnterCaveObjective ),
				typeof( SneakPastGuardiansObjective ),
				typeof( UseTeleporterObjective ),
				typeof( GiveZoelNoteObjective ),
				typeof( GainInnInformationObjective ),
				typeof( ReturnFromInnObjective ),
				typeof( SearchForSwordObjective ),
				typeof( HallwayWalkObjective ),
				typeof( ReturnSwordObjective ),
				typeof( SlayHenchmenObjective ),
				typeof( GiveEminoSwordObjective )
			};

		public override Type[] TypeReferenceTable => m_TypeReferenceTable;

        public override object Name =>
            // Emino's Undertaking
            1063173;

        public override object OfferMessage =>
            // Your value as a Ninja must be proven. Find Daimyo Emino and accept the test he offers.
            1063174;

        public override TimeSpan RestartDelay => TimeSpan.MaxValue;
        public override bool IsTutorial => true;

        public override int Picture => 0x15D5;

        public EminosUndertakingQuest( PlayerMobile from ) : base( from )
		{
		}

		// Serialization
		public EminosUndertakingQuest()
		{
		}

		public override void Accept()
		{
			base.Accept();

			AddConversation( new AcceptConversation() );
		}

		private bool m_SentRadarConversion;

		public override void Slice()
		{
			if ( !m_SentRadarConversion && ( From.Map != Map.Malas || From.X < 407 || From.X > 431 || From.Y < 801 || From.Y > 830 ) )
			{
				m_SentRadarConversion = true;
				AddConversation( new RadarConversation() );
			}

			base.Slice();
		}

		public override void ChildDeserialize( GenericReader reader )
		{
			int version = reader.ReadEncodedInt();

			m_SentRadarConversion = reader.ReadBool();
		}

		public override void ChildSerialize( GenericWriter writer )
		{
			writer.WriteEncodedInt( 0 ); // version

			writer.Write( m_SentRadarConversion );
		}

		public static bool HasLostNoteForZoel( Mobile from )
		{
			PlayerMobile pm = from as PlayerMobile;

			if ( pm == null )
				return false;

			QuestSystem qs = pm.Quest;

			if ( qs is EminosUndertakingQuest )
			{
				if ( qs.IsObjectiveInProgress( typeof( GiveZoelNoteObjective ) ) )
				{
					Container pack = from.Backpack;

					return ( pack == null || pack.FindItemByType( typeof( NoteForZoel ) ) == null );
				}
			}

			return false;
		}

		public static bool HasLostEminosKatana( Mobile from )
		{
			PlayerMobile pm = from as PlayerMobile;

			if ( pm == null )
				return false;

			QuestSystem qs = pm.Quest;

			if ( qs is EminosUndertakingQuest )
			{
				if ( qs.IsObjectiveInProgress( typeof( GiveEminoSwordObjective ) ) )
				{
					Container pack = from.Backpack;

					return ( pack == null || pack.FindItemByType( typeof( EminosKatana ) ) == null );
				}
			}

			return false;
		}
	}
}