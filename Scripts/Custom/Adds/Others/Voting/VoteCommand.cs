using Server.Gumps;
using Server.Commands;

namespace Server.Voting
{
	public static class VoteCommand
	{
		public static void Initialize()
		{
			CommandSystem.Register("Vote", AccessLevel.Player, Vote_OnCommand);
			CommandSystem.Register("VoteFix", AccessLevel.GameMaster, VoteUpdate_OnCommand);
			CommandSystem.Register("VoteConfig", AccessLevel.GameMaster, VoteConfig_OnCommand);
			CommandSystem.Register("VoteInstance", AccessLevel.GameMaster, VoteInstance_OnCommand);
		}


        [Usage("VoteFix")]
        [Description("Cast a vote for your shard.")]
        private static void VoteUpdate_OnCommand(CommandEventArgs e)
        {
            foreach (var item in World.Items.Values)
            {
				if (item is VoteItem vi)
                {
                    if (vi.VoteSite.URL.Contains("google"))
                        vi.VoteSite.URL = VoteConfig.__DefaultURL;
                }
			}
        }

        [Usage("Vote")]
		[Description("Cast a vote for your shard.")]
		private static void Vote_OnCommand(CommandEventArgs e)
		{
			if (e.Mobile == null || e.Mobile.Deleted)
				return;

			VoteHelper.CastVote(e.Mobile, VoteItem.Instance.VoteSite);
		}

		[Usage("VoteInstance")]
		[Description("Gets the property list for the internal Vote Item instance.")]
		private static void VoteInstance_OnCommand(CommandEventArgs e)
		{
			if (e.Mobile == null || e.Mobile.Deleted)
				return;

			e.Mobile.SendGump(new PropertiesGump(e.Mobile, VoteItem.Instance));
		}

		[Usage("VoteConfig")]
		[Description("Gets the property list for the Vote System config.")]
		private static void VoteConfig_OnCommand(CommandEventArgs e)
		{
			if (e.Mobile == null || e.Mobile.Deleted)
				return;

			e.Mobile.SendGump(new PropertiesGump(e.Mobile, VoteConfig.Instance));
		}
	}
}
