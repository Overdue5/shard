using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles 
{ 
	public class SBCook : SBInfo 
	{
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

	    public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo> 
		{ 
			public InternalBuyInfo() 
			{
                Add(new GenericBuyInfo(typeof(BreadLoaf), 5, Utility.RandomMinMax(15, 25), 0x103B, 0));
                Add(new GenericBuyInfo(typeof(ApplePie), 7, Utility.RandomMinMax(15, 25), 0x1041, 0)); //OSI just has Pie, not Apple/Fruit/Meat
                Add(new GenericBuyInfo(typeof(Cake), 13, Utility.RandomMinMax(15, 25), 0x9E9, 0));
                Add(new GenericBuyInfo(typeof(Muffins), 3, Utility.RandomMinMax(15, 25), 0x9EA, 0));

                Add(new GenericBuyInfo(typeof(CheeseWheel), 21, Utility.RandomMinMax(5, 15), 0x97E, 0));
                Add(new GenericBuyInfo(typeof(CookedBird), 17, Utility.RandomMinMax(15, 25), 0x9B7, 0));
                Add(new GenericBuyInfo(typeof(LambLeg), 8, Utility.RandomMinMax(15, 25), 0x160A, 0));
                Add(new GenericBuyInfo(typeof(ChickenLeg), 5, Utility.RandomMinMax(15, 25), 0x1608, 0));

                Add(new GenericBuyInfo(typeof(WoodenBowlOfCarrots), 3, Utility.RandomMinMax(15, 25), 0x15F9, 0));
                Add(new GenericBuyInfo(typeof(WoodenBowlOfCorn), 3, Utility.RandomMinMax(15, 25), 0x15FA, 0));
                Add(new GenericBuyInfo(typeof(WoodenBowlOfLettuce), 3, Utility.RandomMinMax(15, 25), 0x15FB, 0));
                Add(new GenericBuyInfo(typeof(WoodenBowlOfPeas), 3, Utility.RandomMinMax(15, 25), 0x15FC, 0));
                Add(new GenericBuyInfo(typeof(EmptyPewterBowl), 2, Utility.RandomMinMax(15, 25), 0x15FD, 0));
                Add(new GenericBuyInfo(typeof(PewterBowlOfCorn), 3, Utility.RandomMinMax(15, 25), 0x15FE, 0));
                Add(new GenericBuyInfo(typeof(PewterBowlOfLettuce), 3, Utility.RandomMinMax(15, 25), 0x15FF, 0));
                Add(new GenericBuyInfo(typeof(PewterBowlOfPeas), 3, Utility.RandomMinMax(15, 25), 0x1600, 0));
                Add(new GenericBuyInfo(typeof(PewterBowlOfPotatos), 3, Utility.RandomMinMax(15, 25), 0x1601, 0));
                Add(new GenericBuyInfo(typeof(WoodenBowlOfStew), 3, Utility.RandomMinMax(15, 25), 0x1604, 0));
                Add(new GenericBuyInfo(typeof(WoodenBowlOfTomatoSoup), 3, Utility.RandomMinMax(15, 25), 0x1606, 0));

                Add(new GenericBuyInfo(typeof(RoastPig), 106, Utility.RandomMinMax(15, 25), 0x9BB, 0));
                Add(new GenericBuyInfo(typeof(SackFlour), 3, Utility.RandomMinMax(15, 25), 0x1039, 0));
                Add(new GenericBuyInfo(typeof(JarHoney), 3, Utility.RandomMinMax(15, 25), 0x9EC, 0));
                Add(new GenericBuyInfo(typeof(RollingPin), 2, Utility.RandomMinMax(15, 25), 0x1043, 0));
                Add(new GenericBuyInfo(typeof(FlourSifter), 2, Utility.RandomMinMax(15, 25), 0x103E, 0));
                Add(new GenericBuyInfo("1044567", typeof(Skillet), 3, Utility.RandomMinMax(15, 25), 0x97F, 0));

			} 
		} 

		public class InternalSellInfo : GenericSellInfo 
		{ 
			public InternalSellInfo() 
			{ 
				Add( typeof( CheeseWheel ), 12 );
				Add( typeof( CookedBird ), 8 );
				Add( typeof( RoastPig ), 53 );
				Add( typeof( Cake ), 5 );
				Add( typeof( JarHoney ), 1 );
				Add( typeof( SackFlour ), 1 );
				Add( typeof( BreadLoaf ), 2 );
				Add( typeof( ChickenLeg ), 3 );
				Add( typeof( LambLeg ), 4 );
				Add( typeof( Skillet ), 1 );
				Add( typeof( FlourSifter ), 1 );
				Add( typeof( RollingPin ), 1 );
				Add( typeof( Muffins ), 1 );
				Add( typeof( ApplePie ), 3 );

				Add( typeof( WoodenBowlOfCarrots ), 1 );
				Add( typeof( WoodenBowlOfCorn ), 1 );
				Add( typeof( WoodenBowlOfLettuce ), 1 );
				Add( typeof( WoodenBowlOfPeas ), 1 );
				Add( typeof( EmptyPewterBowl ), 1 );
				Add( typeof( PewterBowlOfCorn ), 1 );
				Add( typeof( PewterBowlOfLettuce ), 1 );
				Add( typeof( PewterBowlOfPeas ), 1 );
				Add( typeof( PewterBowlOfPotatos ), 1 );
				Add( typeof( WoodenBowlOfStew ), 1 );
				Add( typeof( WoodenBowlOfTomatoSoup ), 1 );
			} 
		} 
	} 
}