using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBPlayerBarkeeper : SBInfo
	{
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

	    public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                Add(new BeverageBuyInfo(typeof(BeverageBottle), BeverageType.Ale, 7, Utility.RandomMinMax(15, 25), 0x99F, 0));
                Add(new BeverageBuyInfo(typeof(BeverageBottle), BeverageType.Wine, 7, Utility.RandomMinMax(15, 25), 0x9C7, 0));
                Add(new BeverageBuyInfo(typeof(BeverageBottle), BeverageType.Liquor, 7, Utility.RandomMinMax(15, 25), 0x99B, 0));
                Add(new BeverageBuyInfo(typeof(Jug), BeverageType.Cider, 13, Utility.RandomMinMax(15, 25), 0x9C8, 0));
                Add(new BeverageBuyInfo(typeof(Pitcher), BeverageType.Milk, 7, Utility.RandomMinMax(15, 25), 0x9F0, 0));
                Add(new BeverageBuyInfo(typeof(Pitcher), BeverageType.Ale, 11, Utility.RandomMinMax(15, 25), 0x1F95, 0));
                Add(new BeverageBuyInfo(typeof(Pitcher), BeverageType.Cider, 11, Utility.RandomMinMax(15, 25), 0x1F97, 0));
                Add(new BeverageBuyInfo(typeof(Pitcher), BeverageType.Liquor, 11, Utility.RandomMinMax(15, 25), 0x1F99, 0));
                Add(new BeverageBuyInfo(typeof(Pitcher), BeverageType.Wine, 11, Utility.RandomMinMax(15, 25), 0x1F9B, 0));
                Add(new BeverageBuyInfo(typeof(Pitcher), BeverageType.Water, 11, Utility.RandomMinMax(15, 25), 0x1F9D, 0));
				// TODO: pizza
				// TODO: bowl of *, tomato soup
                Add(new GenericBuyInfo("1016450", typeof(Chessboard), 2, Utility.RandomMinMax(15, 25), 0xFA6, 0));
                Add(new GenericBuyInfo("1016449", typeof(CheckerBoard), 2, Utility.RandomMinMax(15, 25), 0xFA6, 0));
                Add(new GenericBuyInfo(typeof(Backgammon), 2, Utility.RandomMinMax(15, 25), 0xE1C, 0));
                Add(new GenericBuyInfo(typeof(Dices), 2, Utility.RandomMinMax(15, 25), 0xFA7, 0));
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
		}
	}
}