using System.Collections.Generic;
using Server.Items;
using Server.Multis;

namespace Server.Mobiles
{
	public class SBTavernKeeper : SBInfo
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

                Add(new GenericBuyInfo(typeof(BreadLoaf), 6, Utility.RandomMinMax(5, 15), 0x103B, 0));
                Add(new GenericBuyInfo(typeof(CheeseWheel), 21, Utility.RandomMinMax(5, 15), 0x97E, 0));
                Add(new GenericBuyInfo(typeof(CookedBird), 17, Utility.RandomMinMax(15, 25), 0x9B7, 0));
                Add(new GenericBuyInfo(typeof(LambLeg), 8, Utility.RandomMinMax(15, 25), 0x160A, 0));
                Add(new GenericBuyInfo(typeof(ChickenLeg), 5, Utility.RandomMinMax(15, 25), 0x1608, 0));
                Add(new GenericBuyInfo(typeof(Ribs), 7, Utility.RandomMinMax(15, 25), 0x9F2, 0));

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

                Add(new GenericBuyInfo(typeof(ApplePie), 7, Utility.RandomMinMax(15, 25), 0x1041, 0)); //OSI just has Pie, not Apple/Fruit/Meat

                Add(new GenericBuyInfo("1016450", typeof(Chessboard), 2, Utility.RandomMinMax(15, 25), 0xFA6, 0));
                Add(new GenericBuyInfo("1016449", typeof(CheckerBoard), 2, Utility.RandomMinMax(15, 25), 0xFA6, 0));
                Add(new GenericBuyInfo(typeof(Backgammon), 2, Utility.RandomMinMax(15, 25), 0xE1C, 0));
                Add(new GenericBuyInfo(typeof(Dices), 2, Utility.RandomMinMax(15, 25), 0xFA7, 0));
                Add(new GenericBuyInfo("1041243", typeof(ContractOfEmployment), 1252, Utility.RandomMinMax(15, 25), 0x14F0, 0));
                Add(new GenericBuyInfo("a barkeep contract", typeof(BarkeepContract), 1252, Utility.RandomMinMax(15, 25), 0x14F0, 0));

				if ( BaseHouse.NewVendorSystem )
                    Add(new GenericBuyInfo("1062332", typeof(VendorRentalContract), 1252, Utility.RandomMinMax(15, 25), 0x14F0, 0x672));
					
				/*if ( Map == Tokuno )
					{
						Add( new GenericBuyInfo( typeof( Wasabi ), 2, 20, 0x24E8, 0 ) );
						Add( new GenericBuyInfo( typeof( Wasabi ), 2, 20, 0x24E9, 0 ) );
						Add( new GenericBuyInfo( typeof( BentoBox ), 6, 20, 0x2836, 0 ) );
						Add( new GenericBuyInfo( typeof( BentoBox ), 6, 20, 0x2837, 0 ) );
						Add( new GenericBuyInfo( typeof( GreenTeaBasket ), 2, 20, 0x284B, 0 ) );
					}*/
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
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
				Add( typeof( BeverageBottle ), 3 );
				Add( typeof( Jug ), 6 );
				Add( typeof( Pitcher ), 5 );
				Add( typeof( GlassMug ), 1 );
				Add( typeof( BreadLoaf ), 3 );
				Add( typeof( CheeseWheel ), 12 );
				Add( typeof( Ribs ), 6 );
				Add( typeof( Peach ), 1 );
				Add( typeof( Pear ), 1 );
				Add( typeof( Grapes ), 1 );
				Add( typeof( Apple ), 1 );
				Add( typeof( Banana ), 1 );
				Add( typeof( Candle ), 3 );
				Add( typeof( Chessboard ), 1 );
				Add( typeof( CheckerBoard ), 1 );
				Add( typeof( Backgammon ), 1 );
				Add( typeof( Dices ), 1 );
				Add( typeof( ContractOfEmployment ), 626 );
			}
		}
	}
}