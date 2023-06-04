using Server.Mobiles;

namespace Server.Custom.Zed
{
    public static class CheckNotorietyForEvents
    {
        public static bool PlayersInEvents(Mobile attaker, Mobile defender)
        {
            return BtceKeyStone.PlayerInEvent(GetOwner(attaker)) && BtceKeyStone.PlayerInEvent(GetOwner(defender));
        }

        private static Mobile GetOwner(Mobile mob)
        {
            if (mob is PlayerMobile)
                return mob;
            if (mob is BaseCreature bc)
                if (bc.LastOwner != null)
                    return bc.LastOwner;
                else if (bc.Summoned && bc.SummonMaster != null)
                    return bc.SummonMaster;
                else if (bc.BardMaster != null)
                    return bc.BardMaster;
            return mob;
        }
    }
    
}
