using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBMetalShields : SBInfo
	{
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

	    public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                Add(new GenericBuyInfo(typeof(BronzeShield), 66, Utility.RandomMinMax(15, 25), 0x1B72, 0));
                Add(new GenericBuyInfo(typeof(Buckler), 50, Utility.RandomMinMax(15, 25), 0x1B73, 0));
                Add(new GenericBuyInfo(typeof(MetalKiteShield), 123, Utility.RandomMinMax(15, 25), 0x1B74, 0));
                Add(new GenericBuyInfo(typeof(HeaterShield), 231, Utility.RandomMinMax(15, 25), 0x1B76, 0));
                Add(new GenericBuyInfo(typeof(WoodenKiteShield), 70, Utility.RandomMinMax(15, 25), 0x1B78, 0));
                Add(new GenericBuyInfo(typeof(MetalShield), 121, Utility.RandomMinMax(15, 25), 0x1B7B, 0));

			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( Buckler ), 4 );
				Add( typeof( BronzeShield ), 6 );
				Add( typeof( MetalShield ), 5 );
				Add( typeof( MetalKiteShield ), 4 );
				Add( typeof( HeaterShield ), 3 );
				Add( typeof( WoodenKiteShield ), 7 );
			}
		}
	}
}