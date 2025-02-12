using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBSwordWeapon: SBInfo
	{
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

	    public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                Add(new GenericBuyInfo(typeof(Cutlass), 24, Utility.RandomMinMax(15, 25), 0x1441, 0));
                Add(new GenericBuyInfo(typeof(Katana), 33, Utility.RandomMinMax(15, 25), 0x13FF, 0));
                Add(new GenericBuyInfo(typeof(Kryss), 32, Utility.RandomMinMax(15, 25), 0x1401, 0));
                Add(new GenericBuyInfo(typeof(Broadsword), 35, Utility.RandomMinMax(15, 25), 0xF5E, 0));
                Add(new GenericBuyInfo(typeof(Longsword), 55, Utility.RandomMinMax(15, 25), 0xF61, 0));
                Add(new GenericBuyInfo(typeof(ThinLongsword), 27, Utility.RandomMinMax(15, 25), 0x13B8, 0));
                Add(new GenericBuyInfo(typeof(VikingSword), 55, Utility.RandomMinMax(15, 25), 0x13B9, 0));
                Add(new GenericBuyInfo(typeof(Scimitar), 36, Utility.RandomMinMax(15, 25), 0x13B6, 0));

				if ( Core.AOS )
				{
                    Add(new GenericBuyInfo(typeof(BoneHarvester), 35, Utility.RandomMinMax(15, 25), 0x26BB, 0));
                    Add(new GenericBuyInfo(typeof(CrescentBlade), 37, Utility.RandomMinMax(15, 25), 0x26C1, 0));
                    Add(new GenericBuyInfo(typeof(DoubleBladedStaff), 35, Utility.RandomMinMax(15, 25), 0x26BF, 0));
                    Add(new GenericBuyInfo(typeof(Lance), 34, Utility.RandomMinMax(15, 25), 0x26C0, 0));
                    Add(new GenericBuyInfo(typeof(Pike), 39, Utility.RandomMinMax(15, 25), 0x26BE, 0));
                    Add(new GenericBuyInfo(typeof(Scythe), 39, Utility.RandomMinMax(15, 25), 0x26BA, 0));
				}


			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( Broadsword ), 17 );
				Add( typeof( Cutlass ), 12 );
				Add( typeof( Katana ), 16 );
				Add( typeof( Kryss ), 16 );
				Add( typeof( Longsword ), 27 );
				Add( typeof( Scimitar ), 18 );
				Add( typeof( ThinLongsword ), 13 );
				Add( typeof( VikingSword ), 27 );

				if ( Core.AOS )
				{
					Add( typeof( Scythe ), 19 );
					Add( typeof( BoneHarvester ), 17 );
					Add( typeof( Scepter ), 18 );
					Add( typeof( BladedStaff ), 16 );
					Add( typeof( Pike ), 19 );
					Add( typeof( DoubleBladedStaff ), 17 );
					Add( typeof( Lance ), 17 );
					Add( typeof( CrescentBlade ), 18 );
				}
			}
		}
	}
}
