using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Haven
{
	public class UzeraanTurmoilQuest : QuestSystem
	{
		private static readonly Type[] m_TypeReferenceTable = new Type[]
			{
				typeof( AcceptConversation ),
				typeof( UzeraanTitheConversation ),
				typeof( UzeraanFirstTaskConversation ),
				typeof( UzeraanReportConversation ),
				typeof( SchmendrickConversation ),
				typeof( UzeraanScrollOfPowerConversation ),
				typeof( DryadConversation ),
				typeof( UzeraanFertileDirtConversation ),
				typeof( UzeraanDaemonBloodConversation ),
				typeof( UzeraanDaemonBoneConversation ),
				typeof( BankerConversation ),
				typeof( RadarConversation ),
				typeof( LostScrollOfPowerConversation ),
				typeof( LostFertileDirtConversation ),
				typeof( DryadAppleConversation ),
				typeof( LostDaemonBloodConversation ),
				typeof( LostDaemonBoneConversation ),
				typeof( FindUzeraanBeginObjective ),
				typeof( TitheGoldObjective ),
				typeof( FindUzeraanFirstTaskObjective ),
				typeof( KillHordeMinionsObjective ),
				typeof( FindUzeraanAboutReportObjective ),
				typeof( FindSchmendrickObjective ),
				typeof( FindApprenticeObjective ),
				typeof( ReturnScrollOfPowerObjective ),
				typeof( FindDryadObjective ),
				typeof( ReturnFertileDirtObjective ),
				typeof( GetDaemonBloodObjective ),
				typeof( ReturnDaemonBloodObjective ),
				typeof( GetDaemonBoneObjective ),
				typeof( ReturnDaemonBoneObjective ),
				typeof( CashBankCheckObjective ),
				typeof( FewReagentsConversation )
			};

		public override Type[] TypeReferenceTable => m_TypeReferenceTable;

        public override object Name =>
            // "Uzeraan's Turmoil"
            1049007;

        public override object OfferMessage =>
            /* <I>The guard speaks to you as you come closer... </I><BR><BR>
				 * 
				 * Greetings traveler! <BR><BR>
				 * 
				 * Uzeraan, the lord of this house and overseer of this city -
				 * <a href="?ForceTopic72">Haven</a>, has requested an audience with you. <BR><BR>
				 * 
				 * Hordes of gruesome hell spawn are beginning to overrun the
				 * city and terrorize the inhabitants.  No one seems to be able
				 * to stop them.<BR><BR>
				 * 
				 * Our fine city militia is falling to the evil creatures
				 * one battalion after the other.<BR><BR>
				 * 
				 * Uzeraan, whom you can find through these doors, is looking to
				 * hire mercenaries to aid in the battle. <BR><BR>
				 * 
				 * Will you assist us?
				 */
            1049008;

        public override TimeSpan RestartDelay => TimeSpan.MaxValue;
        public override bool IsTutorial => true;

        public override int Picture
		{
			get
			{
				switch ( From.Profession )
				{
					case 1: return 0x15C9; // warrior
					case 2: return 0x15C1; // magician
					default: return 0x15D3; // paladin
				}
			}
		}

		private bool m_HasLeftTheMansion;

		public override void Slice()
		{
			if ( !m_HasLeftTheMansion && ( From.Map != Map.Trammel || From.X < 3573 || From.X > 3611 || From.Y < 2568 || From.Y > 2606 ) )
			{
				m_HasLeftTheMansion = true;
				AddConversation( new RadarConversation() );
			}

			base.Slice();
		}

		public UzeraanTurmoilQuest( PlayerMobile from ) : base( from )
		{
		}

		// Serialization
		public UzeraanTurmoilQuest()
		{
		}

		public override void Accept()
		{
			base.Accept();

			AddConversation( new AcceptConversation() );
		}

		public override void ChildDeserialize( GenericReader reader )
		{
			int version = reader.ReadEncodedInt();

			m_HasLeftTheMansion = reader.ReadBool();
		}

		public override void ChildSerialize( GenericWriter writer )
		{
			writer.WriteEncodedInt( 0 ); // version

			writer.Write( m_HasLeftTheMansion );
		}

		public static bool HasLostScrollOfPower( Mobile from )
		{
			PlayerMobile pm = from as PlayerMobile;

			if ( pm == null )
				return false;

			QuestSystem qs = pm.Quest;

			if ( qs is UzeraanTurmoilQuest )
			{
				if ( qs.IsObjectiveInProgress( typeof( ReturnScrollOfPowerObjective ) ) )
				{
					Container pack = from.Backpack;

					return ( pack == null || pack.FindItemByType( typeof( SchmendrickScrollOfPower ) ) == null );
				}
			}

			return false;
		}

		public static bool HasLostFertileDirt( Mobile from )
		{
			PlayerMobile pm = from as PlayerMobile;

			if ( pm == null )
				return false;

			QuestSystem qs = pm.Quest;

			if ( qs is UzeraanTurmoilQuest )
			{
				if ( qs.IsObjectiveInProgress( typeof( ReturnFertileDirtObjective ) ) )
				{
					Container pack = from.Backpack;

					return ( pack == null || pack.FindItemByType( typeof( QuestFertileDirt ) ) == null );
				}
			}

			return false;
		}

		public static bool HasLostDaemonBlood( Mobile from )
		{
			PlayerMobile pm = from as PlayerMobile;

			if ( pm == null )
				return false;

			QuestSystem qs = pm.Quest;

			if ( qs is UzeraanTurmoilQuest )
			{
				if ( qs.IsObjectiveInProgress( typeof( ReturnDaemonBloodObjective ) ) )
				{
					Container pack = from.Backpack;

					return ( pack == null || pack.FindItemByType( typeof( QuestDaemonBlood ) ) == null );
				}
			}

			return false;
		}

		public static bool HasLostDaemonBone( Mobile from )
		{
			PlayerMobile pm = from as PlayerMobile;

			if ( pm == null )
				return false;

			QuestSystem qs = pm.Quest;

			if ( qs is UzeraanTurmoilQuest )
			{
				if ( qs.IsObjectiveInProgress( typeof( ReturnDaemonBoneObjective ) ) )
				{
					Container pack = from.Backpack;

					return ( pack == null || pack.FindItemByType( typeof( QuestDaemonBone ) ) == null );
				}
			}

			return false;
		}
	}
}