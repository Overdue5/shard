using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Factions
{
	public class FactionItemDefinition
	{
		private readonly int m_SilverCost;
		private readonly Type m_VendorType;

		public int SilverCost => m_SilverCost;
        public Type VendorType => m_VendorType;

        public FactionItemDefinition( int silverCost, Type vendorType )
		{
			m_SilverCost = silverCost;
			m_VendorType = vendorType;
		}

		private static readonly FactionItemDefinition m_MetalArmor	= new FactionItemDefinition( 1000, typeof( Blacksmith ) );
		private static readonly FactionItemDefinition m_Weapon		= new FactionItemDefinition( 1000, typeof( Blacksmith ) );
		private static readonly FactionItemDefinition m_RangedWeapon	= new FactionItemDefinition( 1000, typeof( Bowyer ) );
		private static readonly FactionItemDefinition m_LeatherArmor	= new FactionItemDefinition(  750, typeof( Tailor ) );
		private static readonly FactionItemDefinition m_Clothing		= new FactionItemDefinition(  200, typeof( Tailor ) );
		private static readonly FactionItemDefinition m_Scroll		= new FactionItemDefinition(  500, typeof( Mage ) );

		public static FactionItemDefinition Identify( Item item )
		{
			if ( item is BaseArmor )
			{
				if ( CraftResources.GetType( ((BaseArmor)item).Resource ) == CraftResourceType.Leather )
					return m_LeatherArmor;

				return m_MetalArmor;
			}

			if ( item is BaseRanged )
				return m_RangedWeapon;
			else if ( item is BaseWeapon )
				return m_Weapon;
			else if ( item is BaseClothing )
				return m_Clothing;
			else if ( item is SpellScroll )
				return m_Scroll;

			return null;
		}
	}
}