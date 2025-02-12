using Server.Mobiles;

namespace Server.Engines.Quests.Ninja
{
	public class GreenNinjaQuestTeleporter : DynamicTeleporter
	{
		public override int LabelNumber => 1026157; // teleporter

		[Constructable]
		public GreenNinjaQuestTeleporter() : base( 0x51C, 0x17E )
		{
		}

		public override int NotWorkingMessage => 1063198; // You stand on the strange floor tile but nothing happens.

		public override bool GetDestination( PlayerMobile player, ref Point3D loc, ref Map map )
		{
			QuestSystem qs = player.Quest;

			if ( qs is EminosUndertakingQuest && qs.FindObjective( typeof( UseTeleporterObjective ) ) != null )
			{
				loc = new Point3D( 410, 1125, 0 );
				map = Map.Malas;

				return true;
			}

			return false;
		}

		public GreenNinjaQuestTeleporter( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}