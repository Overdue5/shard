using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBLeatherArmor: SBInfo
	{
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

	    public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                Add(new GenericBuyInfo(typeof(LeatherArms), 80, Utility.RandomMinMax(15, 25), 0x13CD, 0));
                Add(new GenericBuyInfo(typeof(LeatherChest), 101, Utility.RandomMinMax(15, 25), 0x13CC, 0));
                Add(new GenericBuyInfo(typeof(LeatherGloves), 60, Utility.RandomMinMax(15, 25), 0x13C6, 0));
                Add(new GenericBuyInfo(typeof(LeatherGorget), 74, Utility.RandomMinMax(15, 25), 0x13C7, 0));
                Add(new GenericBuyInfo(typeof(LeatherLegs), 80, Utility.RandomMinMax(15, 25), 0x13cb, 0));
                Add(new GenericBuyInfo(typeof(LeatherCap), 10, Utility.RandomMinMax(15, 25), 0x1DB9, 0));
                Add(new GenericBuyInfo(typeof(FemaleLeatherChest), 116, Utility.RandomMinMax(15, 25), 0x1C06, 0));
                Add(new GenericBuyInfo(typeof(LeatherBustierArms), 97, Utility.RandomMinMax(15, 25), 0x1C0A, 0));
                Add(new GenericBuyInfo(typeof(LeatherShorts), 86, Utility.RandomMinMax(15, 25), 0x1C00, 0));
                Add(new GenericBuyInfo(typeof(LeatherSkirt), 87, Utility.RandomMinMax(15, 25), 0x1C08, 0));
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( LeatherArms ), 12 );
				Add( typeof( LeatherChest ), 4 );
				Add( typeof( LeatherGloves ), 5 );
				Add( typeof( LeatherGorget ), 6 );
				Add( typeof( LeatherLegs ), 7 );
				Add( typeof( LeatherCap ), 8 );


				Add( typeof( FemaleLeatherChest ), 12 );
				Add( typeof( FemaleStuddedChest ), 13 );
				Add( typeof( LeatherShorts ), 14 );
				Add( typeof( LeatherSkirt ), 11 );
				Add( typeof( LeatherBustierArms ), 11 );
				Add( typeof( StuddedBustierArms ), 27 );
			}
		}
	}
}
