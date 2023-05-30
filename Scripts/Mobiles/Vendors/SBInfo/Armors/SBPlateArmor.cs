using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBPlateArmor: SBInfo
	{
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

	    public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                Add(new GenericBuyInfo(typeof(PlateGorget), 104, Utility.RandomMinMax(15, 25), 0x1413, 0));
                Add(new GenericBuyInfo(typeof(PlateChest), 243, Utility.RandomMinMax(15, 25), 0x1415, 0));
                Add(new GenericBuyInfo(typeof(PlateLegs), 218, Utility.RandomMinMax(15, 25), 0x1411, 0));
                Add(new GenericBuyInfo(typeof(PlateArms), 188, Utility.RandomMinMax(15, 25), 0x1410, 0));
                Add(new GenericBuyInfo(typeof(PlateGloves), 155, Utility.RandomMinMax(15, 25), 0x1414, 0));

			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( PlateArms ), 5 );
				Add( typeof( PlateChest ), 6 );
				Add( typeof( PlateGloves ), 12 );
				Add( typeof( PlateGorget ), 5 );
				Add( typeof( PlateLegs ), 15 );

				Add( typeof( FemalePlateChest ), 13 );

			}
		}
	}
}
