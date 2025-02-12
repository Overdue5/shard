using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBSEHats: SBInfo
	{
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

	    public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                Add(new GenericBuyInfo(typeof(Kasa), 31, Utility.RandomMinMax(15, 25), 0x2798, 0));
                Add(new GenericBuyInfo(typeof(LeatherJingasa), 11, Utility.RandomMinMax(15, 25), 0x2776, 0));
                Add(new GenericBuyInfo(typeof(ClothNinjaHood), 33, Utility.RandomMinMax(15, 25), 0x278F, 0));
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( Kasa ), 15 );
				Add( typeof( LeatherJingasa ), 5 );
				Add( typeof( ClothNinjaHood ), 16 );
			}
		}
	}
}