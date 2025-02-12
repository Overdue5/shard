using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBRingmailArmor: SBInfo
	{
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

	    public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                Add(new GenericBuyInfo(typeof(RingmailChest), 121, Utility.RandomMinMax(15, 25), 0x13ec, 0));
                Add(new GenericBuyInfo(typeof(RingmailLegs), 90, Utility.RandomMinMax(15, 25), 0x13F0, 0));
                Add(new GenericBuyInfo(typeof(RingmailArms), 85, Utility.RandomMinMax(15, 25), 0x13EE, 0));
                Add(new GenericBuyInfo(typeof(RingmailGloves), 93, Utility.RandomMinMax(15, 25), 0x13eb, 0));

			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( RingmailArms ), 6 );
				Add( typeof( RingmailChest ), 3 );
				Add( typeof( RingmailGloves ), 6 );
				Add( typeof( RingmailLegs ), 3 );
			}
		}
	}
}
