using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles 
{ 
	public class SBHerbalist : SBInfo 
	{
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

	    public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo> 
		{ 
			public InternalBuyInfo() 
			{
                Add(new GenericBuyInfo(typeof(Ginseng), 4, Utility.RandomMinMax(15, 25), 0xF85, 0));
                Add(new GenericBuyInfo(typeof(Garlic), 4, Utility.RandomMinMax(15, 25), 0xF84, 0));
                Add(new GenericBuyInfo(typeof(MandrakeRoot), 4, Utility.RandomMinMax(15, 25), 0xF86, 0));
                Add(new GenericBuyInfo(typeof(Nightshade), 4, Utility.RandomMinMax(15, 25), 0xF88, 0));
                Add(new GenericBuyInfo(typeof(Bloodmoss), 4, Utility.RandomMinMax(15, 25), 0xF7B, 0));
                Add(new GenericBuyInfo(typeof(MortarPestle), 8, Utility.RandomMinMax(15, 25), 0xE9B, 0));
                Add(new GenericBuyInfo(typeof(Bottle), 5, Utility.RandomMinMax(15, 25), 0xF0E, 0)); 
 
			} 
		} 

		public class InternalSellInfo : GenericSellInfo 
		{ 
			public InternalSellInfo() 
			{ 
				Add( typeof( Bloodmoss ), 3 ); 
				Add( typeof( MandrakeRoot ), 2 ); 
				Add( typeof( Garlic ), 2 ); 
				Add( typeof( Ginseng ), 2 ); 
				Add( typeof( Nightshade ), 2 ); 
				Add( typeof( Bottle ), 3 ); 
				Add( typeof( MortarPestle ), 4 ); 
			} 
		} 
	} 
}