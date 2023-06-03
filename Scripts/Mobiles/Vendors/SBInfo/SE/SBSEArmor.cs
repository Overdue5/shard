using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBSEArmor: SBInfo
	{
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

	    public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                Add(new GenericBuyInfo(typeof(PlateHatsuburi), 76, Utility.RandomMinMax(15, 25), 0x2775, 0));
                Add(new GenericBuyInfo(typeof(HeavyPlateJingasa), 76, Utility.RandomMinMax(15, 25), 0x2777, 0));
                Add(new GenericBuyInfo(typeof(DecorativePlateKabuto), 95, Utility.RandomMinMax(15, 25), 0x2778, 0));
                Add(new GenericBuyInfo(typeof(PlateDo), 310, Utility.RandomMinMax(15, 25), 0x277D, 0));
                Add(new GenericBuyInfo(typeof(PlateHiroSode), 222, Utility.RandomMinMax(15, 25), 0x2780, 0));
                Add(new GenericBuyInfo(typeof(PlateSuneate), 224, Utility.RandomMinMax(15, 25), 0x2788, 0));
                Add(new GenericBuyInfo(typeof(PlateHaidate), 235, Utility.RandomMinMax(15, 25), 0x278D, 0));
                Add(new GenericBuyInfo(typeof(ChainHatsuburi), 76, Utility.RandomMinMax(15, 25), 0x2774, 0));
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( PlateHatsuburi ), 38 );
				Add( typeof( HeavyPlateJingasa ), 38 );
				Add( typeof( DecorativePlateKabuto ), 47 );
				Add( typeof( PlateDo ), 155 );
				Add( typeof( PlateHiroSode ), 111 );
				Add( typeof( PlateSuneate ), 112 );
				Add( typeof( PlateHaidate), 117 );
				Add( typeof( ChainHatsuburi ), 38 );

			}
		}
	}
}