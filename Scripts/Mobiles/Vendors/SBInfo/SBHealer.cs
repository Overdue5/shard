using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBHealer : SBInfo
	{
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

	    public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                Add(new GenericBuyInfo(typeof(Bandage), 5, Utility.RandomMinMax(15, 25), 0xE21, 0));
                Add(new GenericBuyInfo(typeof(LesserHealPotion), 15, Utility.RandomMinMax(15, 25), 0xF0C, 0));
                Add(new GenericBuyInfo(typeof(Ginseng), 3, Utility.RandomMinMax(15, 25), 0xF85, 0));
                Add(new GenericBuyInfo(typeof(Garlic), 3, Utility.RandomMinMax(15, 25), 0xF84, 0));
                Add(new GenericBuyInfo(typeof(RefreshPotion), 15, Utility.RandomMinMax(15, 25), 0xF0B, 0));
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( Bandage ), 1 );
				Add( typeof( LesserHealPotion ), 7 );
				Add( typeof( RefreshPotion ), 7 );
				Add( typeof( Garlic ), 2 );
				Add( typeof( Ginseng ), 2 );
			}
		}
	}

    public class SBHealerRich : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add(new GenericBuyInfo(typeof(Bandage), 30, Utility.RandomMinMax(30, 40), 0xE21, 0));
                Add(new GenericBuyInfo(typeof(GreaterHealPotion), 200, Utility.RandomMinMax(5, 10), 0xF0C, 0));
                Add(new GenericBuyInfo(typeof(Ginseng), 30, Utility.RandomMinMax(15, 25), 0xF85, 0));
                Add(new GenericBuyInfo(typeof(Garlic), 30, Utility.RandomMinMax(15, 25), 0xF84, 0));
                Add(new GenericBuyInfo(typeof(RefreshPotion), 120, Utility.RandomMinMax(15, 25), 0xF0B, 0));
                
                Add(new GenericBuyInfo(typeof(RecallScroll), 300, Utility.RandomMinMax(2, 3), 0x1F4C, 0));
                Add(new GenericBuyInfo(typeof(BlackPearl), 30, Utility.RandomMinMax(15, 25), 0xF7A, 0));
                Add(new GenericBuyInfo(typeof(MandrakeRoot), 30, Utility.RandomMinMax(15, 25), 0xF86, 0));
                Add(new GenericBuyInfo(typeof(Bloodmoss), 30, Utility.RandomMinMax(15, 25), 0xF7B, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(Bandage), 1);
                Add(typeof(LesserHealPotion), 7);
                Add(typeof(RefreshPotion), 7);
                //Add(typeof(Garlic), 2);
                //Add(typeof(Ginseng), 2);

                Add(typeof(Bloodmoss), 2);
                Add(typeof(BaseReagent), 2);
            }
        }
    }
}