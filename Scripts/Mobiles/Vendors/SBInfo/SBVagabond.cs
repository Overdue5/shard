using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles 
{ 
	public class SBVagabond : SBInfo
	{
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

	    public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                Add(new GenericBuyInfo(typeof(GoldRing), 27, Utility.RandomMinMax(15, 25), 0x108A, 0));
                Add(new GenericBuyInfo(typeof(Necklace), 26, Utility.RandomMinMax(15, 25), 0x1085, 0));
                Add(new GenericBuyInfo(typeof(GoldNecklace), 27, Utility.RandomMinMax(15, 25), 0x1088, 0));
                Add(new GenericBuyInfo(typeof(GoldBeadNecklace), 27, Utility.RandomMinMax(15, 25), 0x1089, 0));
                Add(new GenericBuyInfo(typeof(Beads), 27, Utility.RandomMinMax(15, 25), 0x108B, 0));
                Add(new GenericBuyInfo(typeof(GoldBracelet), 27, Utility.RandomMinMax(15, 25), 0x1086, 0));
                Add(new GenericBuyInfo(typeof(GoldEarrings), 27, Utility.RandomMinMax(15, 25), 0x1087, 0));

                Add(new GenericBuyInfo(typeof(Board), 3, Utility.RandomMinMax(15, 25), 0x1BD7, 0));
                Add(new GenericBuyInfo(typeof(IronIngot), 6, Utility.RandomMinMax(15, 25), 0x1BF2, 0));

                Add(new GenericBuyInfo(typeof(StarSapphire), 125, Utility.RandomMinMax(15, 25), 0xF21, 0));
                Add(new GenericBuyInfo(typeof(Emerald), 100, Utility.RandomMinMax(15, 25), 0xF10, 0));
                Add(new GenericBuyInfo(typeof(Sapphire), 100, Utility.RandomMinMax(15, 25), 0xF19, 0));
                Add(new GenericBuyInfo(typeof(Ruby), 75, Utility.RandomMinMax(15, 25), 0xF13, 0));
                Add(new GenericBuyInfo(typeof(Citrine), 50, Utility.RandomMinMax(15, 25), 0xF15, 0));
                Add(new GenericBuyInfo(typeof(Amethyst), 100, Utility.RandomMinMax(15, 25), 0xF16, 0));
                Add(new GenericBuyInfo(typeof(Tourmaline), 75, Utility.RandomMinMax(15, 25), 0xF2D, 0));
                Add(new GenericBuyInfo(typeof(Amber), 50, Utility.RandomMinMax(15, 25), 0xF25, 0));
                Add(new GenericBuyInfo(typeof(Diamond), 200, Utility.RandomMinMax(15, 25), 0xF26, 0));

                Add(new GenericBuyInfo(typeof(Board), 3, Utility.RandomMinMax(15, 25), 0x1BD7, 0));
                Add(new GenericBuyInfo(typeof(IronIngot), 6, Utility.RandomMinMax(15, 25), 0x1BF2, 0));

			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( Board ), 1 );
				Add( typeof( IronIngot ), 3 );

				Add( typeof( Amber ), 25 );
				Add( typeof( Amethyst ), 50 );
				Add( typeof( Citrine ), 25 );
				Add( typeof( Diamond ), 100 );
				Add( typeof( Emerald ), 50 );
				Add( typeof( Ruby ), 37 );
				Add( typeof( Sapphire ), 50 );
				Add( typeof( StarSapphire ), 62 );
				Add( typeof( Tourmaline ), 47 );
				Add( typeof( GoldRing ), 13 );
				Add( typeof( SilverRing ), 10 );
				Add( typeof( Necklace ), 13 );
				Add( typeof( GoldNecklace ), 13 );
				Add( typeof( GoldBeadNecklace ), 13 );
				Add( typeof( SilverNecklace ), 10 );
				Add( typeof( SilverBeadNecklace ), 10 );
				Add( typeof( Beads ), 13 );
				Add( typeof( GoldBracelet ), 13 );
				Add( typeof( SilverBracelet ), 10 );
				Add( typeof( GoldEarrings ), 13 );
				Add( typeof( SilverEarrings ), 10 );
			}
		}
	}
}