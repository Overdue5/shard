using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles 
{ 
	public class SBFarmer : SBInfo 
	{
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

	    public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo> 
		{ 
			public InternalBuyInfo() 
			{
                Add(new GenericBuyInfo(typeof(Cabbage), 5, Utility.RandomMinMax(15, 25), 0xC7B, 0));
                Add(new GenericBuyInfo(typeof(Cantaloupe), 6, Utility.RandomMinMax(15, 25), 0xC79, 0));
                Add(new GenericBuyInfo(typeof(Carrot), 3, Utility.RandomMinMax(15, 25), 0xC78, 0));
                Add(new GenericBuyInfo(typeof(HoneydewMelon), 7, Utility.RandomMinMax(15, 25), 0xC74, 0));
                Add(new GenericBuyInfo(typeof(Squash), 3, Utility.RandomMinMax(15, 25), 0xC72, 0));
                Add(new GenericBuyInfo(typeof(Lettuce), 5, Utility.RandomMinMax(15, 25), 0xC70, 0));
                Add(new GenericBuyInfo(typeof(Onion), 3, Utility.RandomMinMax(15, 25), 0xC6D, 0));
                Add(new GenericBuyInfo(typeof(Pumpkin), 11, Utility.RandomMinMax(15, 25), 0xC6A, 0));
                Add(new GenericBuyInfo(typeof(GreenGourd), 3, Utility.RandomMinMax(15, 25), 0xC66, 0));
                Add(new GenericBuyInfo(typeof(YellowGourd), 3, Utility.RandomMinMax(15, 25), 0xC64, 0));
				//Add( new GenericBuyInfo( typeof( Turnip ), 6, 20, XXXXXX, 0 ) );
                Add(new GenericBuyInfo(typeof(Watermelon), 7, Utility.RandomMinMax(15, 25), 0xC5C, 0));
				//Add( new GenericBuyInfo( typeof( EarOfCorn ), 3, 20, XXXXXX, 0 ) );
                Add(new GenericBuyInfo(typeof(Eggs), 3, Utility.RandomMinMax(15, 25), 0x9B5, 0));
                Add(new BeverageBuyInfo(typeof(Pitcher), BeverageType.Milk, 7, Utility.RandomMinMax(15, 25), 0x9AD, 0));
                Add(new GenericBuyInfo(typeof(Peach), 3, Utility.RandomMinMax(15, 25), 0x9D2, 0));
                Add(new GenericBuyInfo(typeof(Pear), 3, Utility.RandomMinMax(15, 25), 0x994, 0));
                Add(new GenericBuyInfo(typeof(Lemon), 3, Utility.RandomMinMax(15, 25), 0x1728, 0));
                Add(new GenericBuyInfo(typeof(Lime), 3, Utility.RandomMinMax(15, 25), 0x172A, 0));
                Add(new GenericBuyInfo(typeof(Grapes), 3, Utility.RandomMinMax(15, 25), 0x9D1, 0));
                Add(new GenericBuyInfo(typeof(Apple), 3, Utility.RandomMinMax(15, 25), 0x9D0, 0));
                Add(new GenericBuyInfo(typeof(SheafOfHay), 2, Utility.RandomMinMax(15, 25), 0xF36, 0));

			} 
		} 

		public class InternalSellInfo : GenericSellInfo 
		{ 
			public InternalSellInfo() 
			{ 
				Add( typeof( Pitcher ), 5 );
				Add( typeof( Eggs ), 1 );
				Add( typeof( Apple ), 1 );
				Add( typeof( Grapes ), 1 );
				Add( typeof( Watermelon ), 3 );
				Add( typeof( YellowGourd ), 1 );
				Add( typeof( GreenGourd ), 1 );
				Add( typeof( Pumpkin ), 5 );
				Add( typeof( Onion ), 1 );
				Add( typeof( Lettuce ), 2 );
				Add( typeof( Squash ), 1 );
				Add( typeof( Carrot ), 1 );
				Add( typeof( HoneydewMelon ), 3 );
				Add( typeof( Cantaloupe ), 3 );
				Add( typeof( Cabbage ), 2 );
				Add( typeof( Lemon ), 1 );
				Add( typeof( Lime ), 1 );
				Add( typeof( Peach ), 1 );
				Add( typeof( Pear ), 1 );
				Add( typeof( SheafOfHay ), 1 );
			} 
		} 
	} 
}