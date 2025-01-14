using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBAxeWeapon: SBInfo
	{
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

	    public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                Add(new GenericBuyInfo(typeof(ExecutionersAxe), 30, Utility.RandomMinMax(15, 25), 0xF45, 0));
                Add(new GenericBuyInfo(typeof(BattleAxe), 26, Utility.RandomMinMax(15, 25), 0xF47, 0));
                Add(new GenericBuyInfo(typeof(TwoHandedAxe), 32, Utility.RandomMinMax(15, 25), 0x1443, 0));
                Add(new GenericBuyInfo(typeof(Axe), 40, Utility.RandomMinMax(15, 25), 0xF49, 0));
                Add(new GenericBuyInfo(typeof(DoubleAxe), 52, Utility.RandomMinMax(15, 25), 0xF4B, 0));
                Add(new GenericBuyInfo(typeof(Pickaxe), 22, Utility.RandomMinMax(15, 25), 0xE86, 0));
                Add(new GenericBuyInfo(typeof(LargeBattleAxe), 33, Utility.RandomMinMax(15, 25), 0x13FB, 0));
                Add(new GenericBuyInfo(typeof(WarAxe), 29, Utility.RandomMinMax(15, 25), 0x13B0, 0));

			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( BattleAxe ), 13 );
				Add( typeof( DoubleAxe ), 26 );
				Add( typeof( ExecutionersAxe ), 15 );
				Add( typeof( LargeBattleAxe ),16 );
				Add( typeof( Pickaxe ), 11 );
				Add( typeof( TwoHandedAxe ), 16 );
				Add( typeof( WarAxe ), 14 );
				Add( typeof( Axe ), 20 );
			}
		}
	}
}
