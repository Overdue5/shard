using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Engines.Craft
{
	public enum CraftECA
	{
		ChanceMinusSixty,
		FiftyPercentChanceMinusTenPercent,
		ChanceMinusSixtyToFourtyFive,
        CustomChance // 85% chance minus 10%
	}

	public abstract class CraftSystem
	{
		private readonly int m_MinCraftEffect;
		private readonly int m_MaxCraftEffect;
		private readonly double m_Delay;

	    private readonly CraftItemCol m_CraftItems;
		private readonly CraftGroupCol m_CraftGroups;
		private readonly CraftSubResCol m_CraftSubRes;
		private readonly CraftSubResCol m_CraftSubRes2;

		public int MinCraftEffect => m_MinCraftEffect;
        public int MaxCraftEffect => m_MaxCraftEffect;
        public double Delay => m_Delay;

        public CraftItemCol CraftItems => m_CraftItems;
        public CraftGroupCol CraftGroups => m_CraftGroups;
        public CraftSubResCol CraftSubRes => m_CraftSubRes;
        public CraftSubResCol CraftSubRes2 => m_CraftSubRes2;

        public abstract SkillName MainSkill{ get; }

		public virtual int GumpTitleNumber => 0;
        public virtual string GumpTitleString => "";

        public virtual CraftECA ECA => CraftECA.ChanceMinusSixty;

        private readonly Dictionary<Mobile, CraftContext> m_ContextTable = new Dictionary<Mobile, CraftContext>();

		public abstract double GetChanceAtMin( CraftItem item );

		public virtual bool RetainsColorFrom( CraftItem item, Type type )
		{
			return false;
		}

		public CraftContext GetContext( Mobile m )
		{
			if ( m == null )
				return null;

			if ( m.Deleted )
			{
				m_ContextTable.Remove( m );
				return null;
			}

			CraftContext c = null;
			m_ContextTable.TryGetValue( m, out c );

			if ( c == null )
				m_ContextTable[m] = c = new CraftContext();

			return c;
		}

		public void OnMade( Mobile m, CraftItem item )
		{
			CraftContext c = GetContext( m );

			if ( c != null )
				c.OnMade( item );
		}

	    public bool Resmelt { get; set; }

	    public bool Repair { get; set; }

	    public bool MarkOption { get; set; }

	    public bool CanEnhance { get; set; }

	    public CraftSystem( int minCraftEffect, int maxCraftEffect, double delay )
		{
			m_MinCraftEffect = minCraftEffect;
			m_MaxCraftEffect = maxCraftEffect;
			m_Delay = delay;

			m_CraftItems = new CraftItemCol();
			m_CraftGroups = new CraftGroupCol();
			m_CraftSubRes = new CraftSubResCol();
			m_CraftSubRes2 = new CraftSubResCol();

			InitCraftList();
		}

		public virtual bool ConsumeOnFailure( Mobile from, Type resourceType, CraftItem craftItem )
		{
			return true;
		}

		public void CreateItem( Mobile from, Type type, Type typeRes, BaseTool tool, CraftItem realCraftItem)
		{
            CreateItem(from, type, typeRes, tool, realCraftItem, true);
		}

        public void CreateItem(Mobile from, Type type, Type typeRes, BaseTool tool, CraftItem realCraftItem, bool showGumps)
        {
            // Verify if the type is in the list of the craftable item
            CraftItem craftItem = m_CraftItems.SearchFor(type);

            if (craftItem != null)
            {
                realCraftItem.Craft(from, this, typeRes, tool, showGumps);
            }
        }


		public int AddCraft( Type typeItem, TextDefinition group, TextDefinition name, double minSkill, double maxSkill, Type typeRes, TextDefinition nameRes, int amount )
		{
			return AddCraft( typeItem, group, name, MainSkill, minSkill, maxSkill, typeRes, nameRes, amount, "" );
		}

		public int AddCraft( Type typeItem, TextDefinition group, TextDefinition name, double minSkill, double maxSkill, Type typeRes, TextDefinition nameRes, int amount, TextDefinition message )
		{
			return AddCraft( typeItem, group, name, MainSkill, minSkill, maxSkill, typeRes, nameRes, amount, message );
		}

		public int AddCraft( Type typeItem, TextDefinition group, TextDefinition name, SkillName skillToMake, double minSkill, double maxSkill, Type typeRes, TextDefinition nameRes, int amount )
		{
			return AddCraft( typeItem, group, name, skillToMake, minSkill, maxSkill, typeRes, nameRes, amount, "" );
		}

		public int AddCraft( Type typeItem, TextDefinition group, TextDefinition name, SkillName skillToMake, double minSkill, double maxSkill, Type typeRes, TextDefinition nameRes, int amount, TextDefinition message )
		{
			CraftItem craftItem = new CraftItem( typeItem, group, name );
			craftItem.AddRes( typeRes, nameRes, amount, message );
			craftItem.AddSkill( skillToMake, minSkill, maxSkill );

			DoGroup( group, craftItem );
			return m_CraftItems.Add( craftItem );
		}


		private void DoGroup( TextDefinition groupName, CraftItem craftItem )
		{
			int index = m_CraftGroups.SearchFor( groupName );

			if ( index == -1)
			{
				CraftGroup craftGroup = new CraftGroup( groupName );
				craftGroup.AddCraftItem( craftItem );
				m_CraftGroups.Add( craftGroup );
			}
			else
			{
				m_CraftGroups.GetAt( index ).AddCraftItem( craftItem );
			}
		}


		public void SetManaReq( int index, int mana )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.Mana = mana;
		}

		public void SetStamReq( int index, int stam )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.Stam = stam;
		}

		public void SetHitsReq( int index, int hits )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.Hits = hits;
		}
		
		public void SetUseAllRes( int index, bool useAll )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.UseAllRes = useAll;
		}

		public void SetNeedHeat( int index, bool needHeat )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.NeedHeat = needHeat;
		}

		public void SetNeedOven( int index, bool needOven )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.NeedOven = needOven;
		}

		public void SetNeedMill( int index, bool needMill )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.NeedMill = needMill;
		}

		public void SetNeededExpansion( int index, Expansion expansion )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.RequiredExpansion = expansion;
		}

		public void AddRes( int index, Type type, TextDefinition name, int amount )
		{
			AddRes( index, type, name, amount, "" );
		}

		public void AddRes( int index, Type type, TextDefinition name, int amount, TextDefinition message )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.AddRes( type, name, amount, message );
		}

		public void AddSkill( int index, SkillName skillToMake, double minSkill, double maxSkill )
		{
			CraftItem craftItem = m_CraftItems.GetAt(index);
			craftItem.AddSkill(skillToMake, minSkill, maxSkill);
		}

		public void SetUseSubRes2( int index, bool val )
		{
			CraftItem craftItem = m_CraftItems.GetAt(index);
			craftItem.UseSubRes2 = val;
		}

		public void AddRecipe( int index, int id )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.AddRecipe( id, this );
		}

		public void ForceNonExceptional( int index )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.ForceNonExceptional = true;
		}


		public void SetSubRes( Type type, string name )
		{
			m_CraftSubRes.ResType = type;
			m_CraftSubRes.NameString = name;
			m_CraftSubRes.Init = true;
		}

		public void SetSubRes( Type type, int name )
		{
			m_CraftSubRes.ResType = type;
			m_CraftSubRes.NameNumber = name;
			m_CraftSubRes.Init = true;
		}

		public void AddSubRes( Type type, int name, double reqSkill, object message )
		{
			CraftSubRes craftSubRes = new CraftSubRes( type, name, reqSkill, message );
			m_CraftSubRes.Add( craftSubRes );
		}

		public void AddSubRes( Type type, int name, double reqSkill, int genericName, object message )
		{
			CraftSubRes craftSubRes = new CraftSubRes( type, name, reqSkill, genericName, message );
			m_CraftSubRes.Add( craftSubRes );
		}

		public void AddSubRes( Type type, string name, double reqSkill, object message )
		{
			CraftSubRes craftSubRes = new CraftSubRes( type, name, reqSkill, message );
			m_CraftSubRes.Add( craftSubRes );
		}


		public void SetSubRes2( Type type, string name )
		{
			m_CraftSubRes2.ResType = type;
			m_CraftSubRes2.NameString = name;
			m_CraftSubRes2.Init = true;
		}

		public void SetSubRes2( Type type, int name )
		{
			m_CraftSubRes2.ResType = type;
			m_CraftSubRes2.NameNumber = name;
			m_CraftSubRes2.Init = true;
		}

		public void AddSubRes2( Type type, int name, double reqSkill, object message )
		{
			CraftSubRes craftSubRes = new CraftSubRes( type, name, reqSkill, message );
			m_CraftSubRes2.Add( craftSubRes );
		}

		public void AddSubRes2( Type type, int name, double reqSkill, int genericName, object message )
		{
			CraftSubRes craftSubRes = new CraftSubRes( type, name, reqSkill, genericName, message );
			m_CraftSubRes2.Add( craftSubRes );
		}

		public void AddSubRes2( Type type, string name, double reqSkill, object message )
		{
			CraftSubRes craftSubRes = new CraftSubRes( type, name, reqSkill, message );
			m_CraftSubRes2.Add( craftSubRes );
		}

		public abstract void InitCraftList();

		public abstract void PlayCraftEffect( Mobile from );
        //public abstract void PlayCraftEffect(Mobile from, Type resType);
		public abstract int PlayEndingEffect( Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item );

		public abstract int CanCraft( Mobile from, BaseTool tool, Type itemType );
	}
}