using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBAnimalTrainer : SBInfo
	{
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

	    public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                Add(new AnimalBuyInfo(1, typeof(Cat), 132, Utility.RandomMinMax(5, 15), 201, 0));
                Add(new AnimalBuyInfo(1, typeof(Dog), 170, Utility.RandomMinMax(5, 15), 217, 0));
                Add(new AnimalBuyInfo(1, typeof(Horse), 450, Utility.RandomMinMax(5, 15), 204, 0));
                Add(new AnimalBuyInfo(1, typeof(PackHorse), 141, Utility.RandomMinMax(5, 15), 291, 0));
                Add(new AnimalBuyInfo(1, typeof(PackLlama), 215, Utility.RandomMinMax(5, 15), 292, 0));
                Add(new AnimalBuyInfo(1, typeof(Rabbit), 106, Utility.RandomMinMax(5, 15), 205, 0));
                Add(new GenericBuyInfo(typeof(Apple), 3, Utility.RandomMinMax(15, 25), 0x9D0, 0));
                Add(new GenericBuyInfo(typeof(SheafOfHay), 3, Utility.RandomMinMax(15, 25), 0xF36, 0));
                Add(new GenericBuyInfo(typeof(Carrot), 3, Utility.RandomMinMax(15, 25), 0xC78, 0));

				if( !Core.AOS )
				{
                    Add(new AnimalBuyInfo(1, typeof(Eagle), 102, Utility.RandomMinMax(5, 15), 5, 0));
                    Add(new AnimalBuyInfo(1, typeof(BrownBear), 255, Utility.RandomMinMax(5, 15), 167, 0));
                    Add(new AnimalBuyInfo(1, typeof(GrizzlyBear), 767, Utility.RandomMinMax(5, 15), 212, 0));
                    Add(new AnimalBuyInfo(1, typeof(Panther), 871, Utility.RandomMinMax(5, 15), 214, 0));
                    Add(new AnimalBuyInfo(1, typeof(TimberWolf), 468, Utility.RandomMinMax(5, 15), 225, 0));
                    Add(new AnimalBuyInfo(1, typeof(Rat), 107, Utility.RandomMinMax(5, 15), 238, 0));
				}
					
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
            public InternalSellInfo()
            {
                Add(typeof(SheafOfHay), 2);
                Add(typeof(WheatSheaf), 2);
                Add(typeof(Carrot), 1);
                Add(typeof(Apple), 1);
            }
		}
	}
}
