using System;
using Server.Items;

namespace Server.Engines.Craft
{
	public class DefTailoring : CraftSystem
	{
		public override SkillName MainSkill => SkillName.Tailoring;

        public override int GumpTitleNumber => 1044005; // <CENTER>TAILORING MENU</CENTER>

        private static CraftSystem m_CraftSystem;

		public static CraftSystem CraftSystem => m_CraftSystem ?? (m_CraftSystem = new DefTailoring());

        public override CraftECA ECA => CraftECA.CustomChance;

        public override double GetChanceAtMin( CraftItem item )
		{
			return 0.5; // 50%
		}

		private DefTailoring() : base( 2, 5, 1.0 )// base( 1, 1, 4.5 )
		{
		}

		public override int CanCraft( Mobile from, BaseTool tool, Type itemType )
		{
			if( tool == null || tool.Deleted || tool.UsesRemaining < 0 )
				return 1044038; // You have worn out your tool!
		    if ( !BaseTool.CheckAccessible( tool, from ) )
		        return 1044263; // The tool must be on your person to use.

		    return 0;
		}

        public static bool IsNonColorable(Type type)
        {
            for (int i = 0; i < m_TailorNonColorables.Length; ++i)
            {
                if (m_TailorNonColorables[i] == type)
                {
                    return true;
                }
            }

            return false;
        }

        private static readonly Type[] m_TailorNonColorables = new[]
			{
				typeof( OrcHelm ), typeof( ElvenLeatherArms ), typeof( ElvenLeatherCap), typeof(ElvenLeatherChest), typeof(ElvenLeatherGloves),
                typeof(ElvenLeatherGorget), typeof(ElvenLeatherLegs), typeof(ElvenLeatherSkirt), typeof(LunarChest), typeof(LunarLegs), 
                typeof(LunarArms), typeof(LunarHelm), typeof(LunarGloves), typeof(UndineChest), typeof(UndineLegs), typeof(UndineArms),
                typeof(UndineHelm), typeof(UndineGloves), typeof(DragonScalemailChest), typeof(DragonScalemailLegs), typeof(DragonScalemailArms),
                typeof(DragonScalemailCap), typeof(DragonScalemailGloves), typeof(DragonScalemailGorget), typeof(DragonScalemailFemaleChest), 
                typeof(DragonScalemailSkirt), typeof(BoneHelm), typeof(BoneGloves), typeof(BoneArms), typeof(BoneLegs), typeof(BoneChest)
			};

        private static readonly Type[] m_TailorColorables = new[]
			{
				typeof( GozaMatEastDeed ), typeof( GozaMatSouthDeed ),
				typeof( SquareGozaMatEastDeed ), typeof( SquareGozaMatSouthDeed ),
				typeof( BrocadeGozaMatEastDeed ), typeof( BrocadeGozaMatSouthDeed ),
				typeof( BrocadeSquareGozaMatEastDeed ), typeof( BrocadeSquareGozaMatSouthDeed )
			};

        public override bool RetainsColorFrom(CraftItem item, Type type)
        {
            if (type != typeof(Cloth) && type != typeof(UncutCloth))
                return false;

            type = item.ItemType;

            bool contains = false;

            for (int i = 0; !contains && i < m_TailorColorables.Length; ++i)
                contains = (m_TailorColorables[i] == type);

            return contains;
        }

		public override void PlayCraftEffect( Mobile from )
		{
			from.PlaySound( 0x248 );
		}

		public override int PlayEndingEffect( Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item )
		{
			if ( toolBroken )
				from.SendLocalizedMessage( 1044038 ); // You have worn out your tool

			if ( failed )
			{
			    if ( lostMaterial )
					return 1044043; // You failed to create the item, and some of your materials are lost.
			    return 1044157; // You failed to create the item, but no materials were lost.
			}

		    if ( quality == 0 )
		        return 502785; // You were barely able to make this item.  It's quality is below average.
		    if (makersMark && quality == 2 && from.Skills[SkillName.Tailoring].Base >= 100.0)
		        return 1044156; // You create an exceptional quality item and affix your maker's mark.
		    if ( quality == 2 )
		        return 1044155; // You create an exceptional quality item.
		    return 1044154; // You create the item.
		}

		public override void InitCraftList()
		{
			int index;

			#region Hats
			AddCraft( typeof( SkullCap ), 1011375, 1025444, 0.0, 25.0, typeof( Cloth ), 1044286, 2, 1044287 );
			AddCraft( typeof( Bandana ), 1011375, 1025440, 0.0, 25.0, typeof( Cloth ), 1044286, 2, 1044287 );
			AddCraft( typeof( FloppyHat ), 1011375, 1025907, 6.2, 31.2, typeof( Cloth ), 1044286, 11, 1044287 );
            AddCraft(typeof(Cap), 1011375, 1025909, 6.2, 31.2, typeof(Cloth), 1044286, 11, 1044287);
            AddCraft(typeof(WideBrimHat), 1011375, 1025908, 6.2, 31.2, typeof(Cloth), 1044286, 12, 1044287);
			AddCraft( typeof( StrawHat ), 1011375, 1025911, 6.2, 31.2, typeof( Cloth ), 1044286, 10, 1044287 );
			AddCraft( typeof( TallStrawHat ), 1011375, 1025910, 6.7, 31.7, typeof( Cloth ), 1044286, 13, 1044287 );
			AddCraft( typeof( WizardsHat ), 1011375, 1025912, 7.2, 32.2, typeof( Cloth ), 1044286, 15, 1044287 );
			AddCraft( typeof( Bonnet ), 1011375, 1025913, 6.2, 31.2, typeof( Cloth ), 1044286, 11, 1044287 );
			AddCraft( typeof( FeatheredHat ), 1011375, 1025914, 6.2, 31.2, typeof( Cloth ), 1044286, 12, 1044287 );
			AddCraft( typeof( TricorneHat ), 1011375, 1025915, 6.2, 31.2, typeof( Cloth ), 1044286, 12, 1044287 );
			AddCraft( typeof( JesterHat ), 1011375, 1025916, 7.2, 32.2, typeof( Cloth ), 1044286, 15, 1044287 );

			if ( Core.AOS )
				AddCraft( typeof( FlowerGarland ), 1011375, 1028965, 10.0, 35.0, typeof( Cloth ), 1044286, 5, 1044287 );

			if( Core.SE )
			{
				index = AddCraft( typeof( ClothNinjaHood ), 1011375, 1030202, 80.0, 105.0, typeof( Cloth ), 1044286, 13, 1044287 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( Kasa ), 1011375, 1030211, 60.0, 85.0, typeof( Cloth ), 1044286, 12, 1044287 );	
				SetNeededExpansion( index, Expansion.SE );
			}
			#endregion

			#region Shirts
			AddCraft( typeof( Doublet ), 1015269, 1028059, 0, 25.0, typeof( Cloth ), 1044286, 8, 1044287 );
			AddCraft( typeof( Shirt ), 1015269, 1025399, 20.7, 45.7, typeof( Cloth ), 1044286, 8, 1044287 );
			AddCraft( typeof( FancyShirt ), 1015269, 1027933, 24.8, 49.8, typeof( Cloth ), 1044286, 8, 1044287 );
			AddCraft( typeof( Tunic ), 1015269, 1028097, 00.0, 25.0, typeof( Cloth ), 1044286, 12, 1044287 );
			AddCraft( typeof( Surcoat ), 1015269, 1028189, 8.2, 33.2, typeof( Cloth ), 1044286, 14, 1044287 );
			AddCraft( typeof( PlainDress ), 1015269, 1027937, 12.4, 37.4, typeof( Cloth ), 1044286, 10, 1044287 );
			AddCraft( typeof( FancyDress ), 1015269, 1027935, 33.1, 58.1, typeof( Cloth ), 1044286, 12, 1044287 );
			AddCraft( typeof( Cloak ), 1015269, 1025397, 41.4, 66.4, typeof( Cloth ), 1044286, 14, 1044287 );
			AddCraft( typeof( Robe ), 1015269, 1027939, 53.9, 78.9, typeof( Cloth ), 1044286, 16, 1044287 );
			AddCraft( typeof( JesterSuit ), 1015269, 1028095, 8.2, 33.2, typeof( Cloth ), 1044286, 24, 1044287 );

			if ( Core.AOS )
			{
				AddCraft( typeof( FurCape ), 1015269, 1028969, 35.0, 60.0, typeof( Cloth ), 1044286, 13, 1044287 );
				AddCraft( typeof( GildedDress ), 1015269, 1028973, 37.5, 62.5, typeof( Cloth ), 1044286, 16, 1044287 );
				AddCraft( typeof( FormalShirt ), 1015269, 1028975, 26.0, 51.0, typeof( Cloth ), 1044286, 16, 1044287 );
			}

			if( Core.SE )
			{
				index = AddCraft( typeof( ClothNinjaJacket ), 1015269, 1030207, 75.0, 100.0, typeof( Cloth ), 1044286, 12, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( Kamishimo ), 1015269, 1030212, 75.0, 100.0, typeof( Cloth ), 1044286, 15, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( HakamaShita ), 1015269, 1030215, 40.0, 65.0, typeof( Cloth ), 1044286, 14, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( MaleKimono ), 1015269, 1030189, 50.0, 75.0, typeof( Cloth ), 1044286, 16, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( FemaleKimono ), 1015269, 1030190, 50.0, 75.0, typeof( Cloth ), 1044286, 16, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( JinBaori ), 1015269, 1030220, 30.0, 55.0, typeof( Cloth ), 1044286, 12, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
			}

			// Pants
			#endregion

			#region Pants
			AddCraft( typeof( ShortPants ), 1015279, 1025422, 24.8, 49.8, typeof( Cloth ), 1044286, 6, 1044287 );
			AddCraft( typeof( LongPants ), 1015279, 1025433, 24.8, 49.8, typeof( Cloth ), 1044286, 8, 1044287 );
			AddCraft( typeof( Kilt ), 1015279, 1025431, 20.7, 45.7, typeof( Cloth ), 1044286, 8, 1044287 );
			AddCraft( typeof( Skirt ), 1015279, 1025398, 29.0, 54.0, typeof( Cloth ), 1044286, 10, 1044287 );

			if ( Core.AOS )
				AddCraft( typeof( FurSarong ), 1015279, 1028971, 35.0, 60.0, typeof( Cloth ), 1044286, 12, 1044287 );

			if( Core.SE )
			{
				index = AddCraft( typeof( Hakama ), 1015279, 1030213, 50.0, 75.0, typeof( Cloth ), 1044286, 16, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( TattsukeHakama ), 1015279, 1030214, 50.0, 75.0, typeof( Cloth ), 1044286, 16, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
			}

            // Pants
			#endregion

			#region Misc
			AddCraft( typeof( BodySash ), 1015283, 1025441, 4.1, 29.1, typeof( Cloth ), 1044286, 4, 1044287 );
			AddCraft( typeof( HalfApron ), 1015283, 1025435, 20.7, 45.7, typeof( Cloth ), 1044286, 6, 1044287 );
			AddCraft( typeof( FullApron ), 1015283, 1025437, 29.0, 54.0, typeof( Cloth ), 1044286, 10, 1044287 );
            AddCraft(typeof(Backpack), 1015283, "backpack", 34.9, 40.0, typeof(Leather), 1044462, 4, 1044463);
            AddCraft(typeof(Bag), 1015283, "bag", 31.9, 35.0, typeof(Leather), 1044462, 2, 1044463);
            AddCraft(typeof(Pouch), 1015283, "pouch", 31.9, 35.0, typeof(Leather), 1044462, 2, 1044463);

			if( Core.SE )
			{
				index = AddCraft( typeof( Obi ), 1015283, 1030219, 20.0, 45.0, typeof( Cloth ), 1044286, 6, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
			}

            if (Core.ML)
            {
                index = AddCraft(typeof(ElvenQuiver), 1015283, 1032657, 65.0, 115.0, typeof(Leather), 1044462, 28, 1044463);
                AddRecipe(index, 501);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(QuiverOfFire), 1015283, 1073109, 65.0, 115.0, typeof(Leather), 1044462, 28, 1044463);
                AddRes(index, typeof(FireRuby), 1032695, 15, 1042081);
                AddRecipe(index, 502);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(QuiverOfIce), 1015283, 1073110, 65.0, 115.0, typeof(Leather), 1044462, 28, 1044463);
                AddRes(index, typeof(WhitePearl), 1032694, 15, 1042081);
                AddRecipe(index, 503);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(QuiverOfBlight), 1015283, 1073111, 65.0, 115.0, typeof(Leather), 1044462, 28, 1044463);
                AddRes(index, typeof(Blight), 1032675, 10, 1042081);
                AddRecipe(index, 504);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(QuiverOfLightning), 1015283, 1073112, 65.0, 115.0, typeof(Leather), 1044462, 28, 1044463);
                AddRes(index, typeof(Corruption), 1032676, 10, 1042081);
                AddRecipe(index, 505);
                SetNeededExpansion(index, Expansion.ML);
            }

			//AddCraft( typeof( OilCloth ), 1015283, 1041498, 74.6, 99.6, typeof( Cloth ), 1044286, 1, 1044287 );

			if( Core.SE )
			{
				index = AddCraft( typeof( GozaMatEastDeed ), 1015283, 1030404, 55.0, 80.0, typeof( Cloth ), 1044286, 25, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( GozaMatSouthDeed ), 1015283, 1030405, 55.0, 80.0, typeof( Cloth ), 1044286, 25, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( SquareGozaMatEastDeed ), 1015283, 1030407, 55.0, 80.0, typeof( Cloth ), 1044286, 25, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( SquareGozaMatSouthDeed ), 1015283, 1030406, 55.0, 80.0, typeof( Cloth ), 1044286, 25, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( BrocadeGozaMatEastDeed ), 1015283, 1030408, 55.0, 80.0, typeof( Cloth ), 1044286, 25, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( BrocadeGozaMatSouthDeed ), 1015283, 1030409, 55.0, 80.0, typeof( Cloth ), 1044286, 25, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( BrocadeSquareGozaMatEastDeed ), 1015283, 1030411, 55.0, 80.0, typeof( Cloth ), 1044286, 25, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( BrocadeSquareGozaMatSouthDeed ), 1015283, 1030410, 55.0, 80.0, typeof( Cloth ), 1044286, 25, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
			}

			#endregion

			#region Footwear
			if ( Core.AOS )
				AddCraft( typeof( FurBoots ), 1015288, 1028967, 50.0, 75.0, typeof( Cloth ), 1044286, 12, 1044287 );

			if( Core.SE )
			{
				index = AddCraft( typeof( NinjaTabi ), 1015288, 1030210, 70.0, 95.0, typeof( Cloth ), 1044286, 10, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( SamuraiTabi ), 1015288, 1030209, 20.0, 45.0, typeof( Cloth ), 1044286, 6, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
			}

			AddCraft( typeof( Sandals ), 1015288, 1025901, 12.4, 37.4, typeof( Leather ), 1044462, 4, 1044463 );
			AddCraft( typeof( Shoes ), 1015288, 1025904, 16.5, 41.5, typeof( Leather ), 1044462, 6, 1044463 );
			AddCraft( typeof( Boots ), 1015288, 1025899, 33.1, 58.1, typeof( Leather ), 1044462, 8, 1044463 );
			AddCraft( typeof( ThighBoots ), 1015288, 1025906, 41.4, 66.4, typeof( Leather ), 1044462, 10, 1044463 );
			#endregion

			#region Leather Armor

			AddCraft( typeof( LeatherGorget ), 1015293, 1025063, 53.9, 78.9, typeof( Leather ), 1044462, 4, 1044463 );
			AddCraft( typeof( LeatherCap ), 1015293, 1027609, 6.2, 31.2, typeof( Leather ), 1044462, 2, 1044463 );
			AddCraft( typeof( LeatherGloves ), 1015293, 1025062, 51.8, 76.8, typeof( Leather ), 1044462, 3, 1044463 );
			AddCraft( typeof( LeatherArms ), 1015293, 1025061, 53.9, 78.9, typeof( Leather ), 1044462, 4, 1044463 );
			AddCraft( typeof( LeatherLegs ), 1015293, 1025067, 66.3, 91.3, typeof( Leather ), 1044462, 10, 1044463 );
			AddCraft( typeof( LeatherChest ), 1015293, 1025068, 70.5, 95.5, typeof( Leather ), 1044462, 12, 1044463 );

			if( Core.SE )
			{
				index = AddCraft( typeof( LeatherJingasa ), 1015293, 1030177, 45.0, 70.0, typeof( Leather ), 1044462, 4, 1044463 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( LeatherMempo ), 1015293, 1030181, 80.0, 105.0, typeof( Leather ), 1044462, 8, 1044463 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( LeatherDo ), 1015293, 1030182, 75.0, 100.0, typeof( Leather ), 1044462, 12, 1044463 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( LeatherHiroSode ), 1015293, 1030185, 55.0, 80.0, typeof( Leather ), 1044462, 5, 1044463 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( LeatherSuneate ), 1015293, 1030193, 68.0, 93.0, typeof( Leather ), 1044462, 12, 1044463 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( LeatherHaidate ), 1015293, 1030197, 68.0, 93.0, typeof( Leather ), 1044462, 12, 1044463 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( LeatherNinjaPants ), 1015293, 1030204, 80.0, 105.0, typeof( Leather ), 1044462, 13, 1044463 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( LeatherNinjaJacket ), 1015293, 1030206, 85.0, 110.0, typeof( Leather ), 1044462, 13, 1044463 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( LeatherNinjaBelt ), 1015293, 1030203, 50.0, 75.0, typeof( Leather ), 1044462, 15, 1044463 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( LeatherNinjaMitts ), 1015293, 1030205, 65.0, 90.0, typeof( Leather ), 1044462, 12, 1044463 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( LeatherNinjaHood ), 1015293, 1030201, 90.0, 115.0, typeof( Leather ), 1044462, 14, 1044463 );
				SetNeededExpansion( index, Expansion.SE );
			}

			#endregion

			#region Studded Armor
			AddCraft( typeof( StuddedGorget ), 1015300, 1025078, 78.8, 103.8, typeof( Leather ), 1044462, 6, 1044463 );
			AddCraft( typeof( StuddedGloves ), 1015300, 1025077, 82.9, 107.9, typeof( Leather ), 1044462, 8, 1044463 );
			AddCraft( typeof( StuddedArms ), 1015300, 1025076, 87.1, 112.1, typeof( Leather ), 1044462, 10, 1044463 );
			AddCraft( typeof( StuddedLegs ), 1015300, 1025082, 91.2, 116.2, typeof( Leather ), 1044462, 12, 1044463 );
			AddCraft( typeof( StuddedChest ), 1015300, 1025083, 94.0, 119.0, typeof( Leather ), 1044462, 14, 1044463 );

			if( Core.SE )
			{
				index = AddCraft( typeof( StuddedMempo ), 1015300, 1030216, 80.0, 105.0, typeof( Leather ), 1044462, 8, 1044463 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( StuddedDo ), 1015300, 1030183, 95.0, 120.0, typeof( Leather ), 1044462, 14, 1044463 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( StuddedHiroSode ), 1015300, 1030186, 85.0, 110.0, typeof( Leather ), 1044462, 8, 1044463 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( StuddedSuneate ), 1015300, 1030194, 92.0, 117.0, typeof( Leather ), 1044462, 14, 1044463 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( StuddedHaidate ), 1015300, 1030198, 92.0, 117.0, typeof( Leather ), 1044462, 14, 1044463 );
				SetNeededExpansion( index, Expansion.SE );
			}

			#endregion

			#region Female Armor
			AddCraft( typeof( LeatherShorts ), 1015306, 1027168, 62.2, 87.2, typeof( Leather ), 1044462, 8, 1044463 );
			AddCraft( typeof( LeatherSkirt ), 1015306, 1027176, 58.0, 83.0, typeof( Leather ), 1044462, 6, 1044463 );
			AddCraft( typeof( LeatherBustierArms ), 1015306, 1027178, 58.0, 83.0, typeof( Leather ), 1044462, 6, 1044463 );
			AddCraft( typeof( StuddedBustierArms ), 1015306, 1027180, 82.9, 107.9, typeof( Leather ), 1044462, 8, 1044463 );
			AddCraft( typeof( FemaleLeatherChest ), 1015306, 1027174, 62.2, 87.2, typeof( Leather ), 1044462, 8, 1044463 );
			AddCraft( typeof( FemaleStuddedChest ), 1015306, 1027170, 87.1, 112.1, typeof( Leather ), 1044462, 10, 1044463 );
			#endregion

			#region Bone Armor
			index = AddCraft( typeof( BoneHelm ), 1049149, 1025206, 85.0, 110.0, typeof( Leather ), 1044462, 4, 1044463 );
			AddRes( index, typeof( Bone ), 1049064, 2, 1049063 );
			
			index = AddCraft( typeof( BoneGloves ), 1049149, 1025205, 89.0, 114.0, typeof( Leather ), 1044462, 6, 1044463 );
			AddRes( index, typeof( Bone ), 1049064, 2, 1049063 );

			index = AddCraft( typeof( BoneArms ), 1049149, 1025203, 92.0, 117.0, typeof( Leather ), 1044462, 8, 1044463 );
			AddRes( index, typeof( Bone ), 1049064, 4, 1049063 );

			index = AddCraft( typeof( BoneLegs ), 1049149, 1025202, 95.0, 120.0, typeof( Leather ), 1044462, 10, 1044463 );
			AddRes( index, typeof( Bone ), 1049064, 6, 1049063 );
		
			index = AddCraft( typeof( BoneChest ), 1049149, 1025199, 96.0, 121.0, typeof( Leather ), 1044462, 12, 1044463 );
			AddRes( index, typeof( Bone ), 1049064, 10, 1049063 );

            index = AddCraft(typeof(OrcHelm), 1049149, 1027947, 90.0, 115.0, typeof(Leather), 1044462, 6, 1044463);
            AddRes(index, typeof(Bone), 1049064, 4, 1049063);
			#endregion

            #region Magical Armor
            //elven
            index = AddCraft(typeof(ElvenLeatherChest), "Magical Armor", "Elven Leather Tunic", 87.0, 104.0, typeof(Leather), 1044462, 25, 1044463);
            AddRes(index, typeof(DragonsBlood), "Dragon's Blood", 16, "You do not have enough resources to make that.");
            AddRes(index, typeof(ProtectionScroll), "Protection Scroll", 10, "You do not have enough resources to make that.");
            AddSkill(index, SkillName.Magery, 100.0, 100.0);
            AddSkill(index, SkillName.Blacksmith, 70.0, 80.0);

            index = AddCraft(typeof(ElvenLeatherLegs), "Magical Armor", "Elven Leather Leggings", 80.8, 104.2, typeof(Leather), 1044462, 20, 1044463);
            AddRes(index, typeof(DragonsBlood), "Dragon's Blood", 12, "You do not have enough resources to make that.");
            AddRes(index, typeof(ProtectionScroll), "Protection Scroll", 10, "You do not have enough resources to make that.");
            AddSkill(index, SkillName.Magery, 100.0, 100.0);
            AddSkill(index, SkillName.Blacksmith, 70.0, 80.0);

            index = AddCraft(typeof(ElvenLeatherArms), "Magical Armor", "Elven Leather Sleeves", 78.3, 103.7, typeof(Leather), 1044462, 18, 1044463);
            AddRes(index, typeof(DragonsBlood), "Dragon's Blood", 10, "You do not have enough resources to make that.");
            AddRes(index, typeof(ProtectionScroll), "Protection Scroll", 10, "You do not have enough resources to make that.");
            AddSkill(index, SkillName.Magery, 100.0, 100.0);
            AddSkill(index, SkillName.Blacksmith, 70.0, 80.0);

            index = AddCraft(typeof(ElvenLeatherCap), "Magical Armor", "Elven Leather Cap", 73.6, 104.2, typeof(Leather), 1044462, 15, 1044463);
            AddRes(index, typeof(DragonsBlood), "Dragon's Blood", 9, "You do not have enough resources to make that.");
            AddRes(index, typeof(ProtectionScroll), "Protection Scroll", 10, "You do not have enough resources to make that.");
            AddSkill(index, SkillName.Magery, 100.0, 100.0);
            AddSkill(index, SkillName.Blacksmith, 70.0, 80.0);

            index = AddCraft(typeof(ElvenLeatherGloves), "Magical Armor", "Elven Leather Gloves", 70.9, 103.1, typeof(Leather), 1044462, 12, 1044463);
            AddRes(index, typeof(DragonsBlood), "Dragon's Blood", 7, "You do not have enough resources to make that.");
            AddRes(index, typeof(ProtectionScroll), "Protection Scroll", 10, "You do not have enough resources to make that.");
            AddSkill(index, SkillName.Magery, 100.0, 100.0);
            AddSkill(index, SkillName.Blacksmith, 70.0, 80.0);

            index = AddCraft(typeof(ElvenLeatherGorget), "Magical Armor", "Elven Leather Gorget", 68.4, 101.8, typeof(Leather), 1044462, 10, 1044463);
            AddRes(index, typeof(DragonsBlood), "Dragon's Blood", 6, "You do not have enough resources to make that.");
            AddRes(index, typeof(ProtectionScroll), "Protection Scroll", 10, "You do not have enough resources to make that.");
            AddSkill(index, SkillName.Magery, 100.0, 100.0);
            AddSkill(index, SkillName.Blacksmith, 70.0, 80.0);

            index = AddCraft(typeof(ElvenLeatherSkirt), "Magical Armor", "Elven Leather Skirt", 80.8, 104.2, typeof(Leather), 1044462, 20, 1044463);
            AddRes(index, typeof(DragonsBlood), "Dragon's Blood", 12, "You do not have enough resources to make that.");
            AddRes(index, typeof(ProtectionScroll), "Protection Scroll", 10, "You do not have enough resources to make that.");
            AddSkill(index, SkillName.Magery, 100.0, 100.0);
            AddSkill(index, SkillName.Blacksmith, 70.0, 80.0);

            index = AddCraft(typeof(ElvenFemaleLeatherChest), "Magical Armor", "Elven Female Leather Chest", 87.0, 104.0, typeof(Leather), 1044462, 25, 1044463);
            AddRes(index, typeof(DragonsBlood), "Dragon's Blood", 16, "You do not have enough resources to make that.");
            AddRes(index, typeof(ProtectionScroll), "Protection Scroll", 10, "You do not have enough resources to make that.");
            AddSkill(index, SkillName.Magery, 100.0, 100.0);
            AddSkill(index, SkillName.Blacksmith, 70.0, 80.0);

            //lunar
            
            index = AddCraft(typeof(LunarChest), "Magical Armor", "Lunar Chest", 87.0, 104.0, typeof(Leather), 1044462, 16, 1044463);
            AddRes(index, typeof(Bone), 1049064, 32, 1049063);
            AddRes(index, typeof(LunarBone), "Lunar Bones", 12, "You do not have enough resources to make that.");
            AddRes(index, typeof(ArchProtectionScroll), "Arch Protection Scroll", 5, "You do not have enough resources to make that.");
            AddSkill(index, SkillName.Magery, 100.0, 100.0);
            AddSkill(index, SkillName.Blacksmith, 82.5, 82.5);

            index = AddCraft(typeof(LunarLegs), "Magical Armor", "Lunar Leggings", 80.8, 104.2, typeof(Leather), 1044462, 14, 1044463);
            AddRes(index, typeof(Bone), 1049064, 28, 1049063);
            AddRes(index, typeof(LunarBone), "Lunar Bones", 10, "You do not have enough resources to make that.");
            AddRes(index, typeof(ArchProtectionScroll), "Arch Protection Scroll", 5, "You do not have enough resources to make that.");
            AddSkill(index, SkillName.Magery, 100.0, 100.0);
            AddSkill(index, SkillName.Blacksmith, 82.5, 82.5);

            index = AddCraft(typeof(LunarArms), "Magical Armor", "Lunar Arms", 78.3, 103.7, typeof(Leather), 1044462, 12, 1044463);
            AddRes(index, typeof(Bone), 1049064, 22, 1049063);
            AddRes(index, typeof(LunarBone), "Lunar Bones", 8, "You do not have enough resources to make that.");
            AddRes(index, typeof(ArchProtectionScroll), "Arch Protection Scroll", 5, "You do not have enough resources to make that.");
            AddSkill(index, SkillName.Magery, 100.0, 100.0);
            AddSkill(index, SkillName.Blacksmith, 82.5, 82.5);

            index = AddCraft(typeof(LunarHelm), "Magical Armor", "Lunar Helm", 73.6, 104.2, typeof(Leather), 1044462, 10, 1044463);
            AddRes(index, typeof(Bone), 1049064, 20, 1049063);
            AddRes(index, typeof(LunarBone), "Lunar Bones", 6, "You do not have enough resources to make that.");
            AddRes(index, typeof(ArchProtectionScroll), "Arch Protection Scroll", 5, "You do not have enough resources to make that.");
            AddSkill(index, SkillName.Magery, 100.0, 100.0);
            AddSkill(index, SkillName.Blacksmith, 82.5, 82.5);

            index = AddCraft(typeof(LunarGloves), "Magical Armor", "Lunar Gloves", 70.9, 103.1, typeof(Leather), 1044462, 8, 1044463);
            AddRes(index, typeof(Bone), 1049064, 18, 1049063);
            AddRes(index, typeof(LunarBone), "Lunar Bones", 4, "You do not have enough resources to make that.");
            AddRes(index, typeof(ArchProtectionScroll), "Arch Protection Scroll", 5, "You do not have enough resources to make that.");
            AddSkill(index, SkillName.Magery, 100.0, 100.0);
            AddSkill(index, SkillName.Blacksmith, 82.5, 82.5);

            //undine

            index = AddCraft(typeof(UndineChest), "Magical Armor", "Undine Chest", 87.0, 104.0, typeof(Leather), 1044462, 16, 1044463);
            AddRes(index, typeof(Bone), 1049064, 32, 1049063);
            AddRes(index, typeof(DaemonHeart), "Daemon Hearts", 5, "You do not have enough resources to make that.");
            AddRes(index, typeof(ArchProtectionScroll), "Arch Protection Scroll", 5, "You do not have enough resources to make that.");
            AddSkill(index, SkillName.Magery, 100.0, 100.0);
            AddSkill(index, SkillName.Blacksmith, 82.5, 82.5);

            index = AddCraft(typeof(UndineLegs), "Magical Armor", "Undine Leggings", 80.8, 104.2, typeof(Leather), 1044462, 14, 1044463);
            AddRes(index, typeof(Bone), 1049064, 28, 1049063);
            AddRes(index, typeof(DaemonHeart), "Daemon Hearts", 4, "You do not have enough resources to make that.");
            AddRes(index, typeof(ArchProtectionScroll), "Arch Protection Scroll", 5, "You do not have enough resources to make that.");
            AddSkill(index, SkillName.Magery, 100.0, 100.0);
            AddSkill(index, SkillName.Blacksmith, 82.5, 82.5);

            index = AddCraft(typeof(UndineArms), "Magical Armor", "Undine Arms", 78.3, 103.7, typeof(Leather), 1044462, 12, 1044463);
            AddRes(index, typeof(Bone), 1049064, 22, 1049063);
            AddRes(index, typeof(DaemonHeart), "Daemon Hearts", 3, "You do not have enough resources to make that.");
            AddRes(index, typeof(ArchProtectionScroll), "Arch Protection Scroll", 5, "You do not have enough resources to make that.");
            AddSkill(index, SkillName.Magery, 100.0, 100.0);
            AddSkill(index, SkillName.Blacksmith, 82.5, 82.5);

            index = AddCraft(typeof(UndineHelm), "Magical Armor", "Undine Helm", 73.6, 104.2, typeof(Leather), 1044462, 10, 1044463);
            AddRes(index, typeof(Bone), 1049064, 20, 1049063);
            AddRes(index, typeof(DaemonHeart), "Daemon Hearts", 2, "You do not have enough resources to make that.");
            AddRes(index, typeof(ArchProtectionScroll), "Arch Protection Scroll", 5, "You do not have enough resources to make that.");
            AddSkill(index, SkillName.Magery, 100.0, 100.0);
            AddSkill(index, SkillName.Blacksmith, 82.5, 82.5);

            index = AddCraft(typeof(UndineGloves), "Magical Armor", "Undine Gloves", 70.9, 103.1, typeof(Leather), 1044462, 8, 1044463);
            AddRes(index, typeof(Bone), 1049064, 18, 1049063);
            AddRes(index, typeof(DaemonHeart), "Daemon Hearts", 2, "You do not have enough resources to make that.");
            AddRes(index, typeof(ArchProtectionScroll), "Arch Protection Scroll", 5, "You do not have enough resources to make that.");
            AddSkill(index, SkillName.Magery, 100.0, 100.0);
            AddSkill(index, SkillName.Blacksmith, 82.5, 82.5);

            //dragonscalemail

            index = AddCraft(typeof(DragonScalemailChest), "Magical Armor", "Dragon Scalemail Tunic", 87.0, 104.0, typeof(Leather), 1044462, 25, 1044463);
            AddRes(index, typeof(DragonsBlood), "Dragon's Blood", 10, "You do not have enough resources to make that.");
            AddRes(index, typeof(DaemonSteelIngot), "DaemonSteel Ingot", 3, "You do not have enough resources to make that.");
            AddRes(index, typeof(RedScales), "Red Scales", 28, "You do not have enough resources to make that.");
            AddSkill(index, SkillName.Magery, 100.0, 100.0);
            AddSkill(index, SkillName.Blacksmith, 90.0, 90.0);

            index = AddCraft(typeof(DragonScalemailLegs), "Magical Armor", "Dragon Scalemail Leggings", 80.8, 104.2, typeof(Leather), 1044462, 20, 1044463);
            AddRes(index, typeof(DragonsBlood), "Dragon's Blood", 10, "You do not have enough resources to make that.");
            AddRes(index, typeof(DaemonSteelIngot), "DaemonSteel Ingot", 3, "You do not have enough resources to make that.");
            AddRes(index, typeof(RedScales), "Red Scales", 24, "You do not have enough resources to make that.");
            AddSkill(index, SkillName.Magery, 100.0, 100.0);
            AddSkill(index, SkillName.Blacksmith, 90.0, 90.0);

            index = AddCraft(typeof(DragonScalemailArms), "Magical Armor", "Dragon Scalemail Sleeves", 78.3, 103.7, typeof(Leather), 1044462, 18, 1044463);
            AddRes(index, typeof(DragonsBlood), "Dragon's Blood", 10, "You do not have enough resources to make that.");
            AddRes(index, typeof(DaemonSteelIngot), "DaemonSteel Ingot", 3, "You do not have enough resources to make that.");
            AddRes(index, typeof(RedScales), "Red Scales", 22, "You do not have enough resources to make that.");
            AddSkill(index, SkillName.Magery, 100.0, 100.0);
            AddSkill(index, SkillName.Blacksmith, 90.0, 90.0);

            index = AddCraft(typeof(DragonScalemailCap), "Magical Armor", "Dragon Scalemail Cap", 73.6, 104.2, typeof(Leather), 1044462, 15, 1044463);
            AddRes(index, typeof(DragonsBlood), "Dragon's Blood", 10, "You do not have enough resources to make that.");
            AddRes(index, typeof(DaemonSteelIngot), "DaemonSteel Ingot", 3, "You do not have enough resources to make that.");
            AddRes(index, typeof(RedScales), "Red Scales", 18, "You do not have enough resources to make that.");
            AddSkill(index, SkillName.Magery, 100.0, 100.0);
            AddSkill(index, SkillName.Blacksmith, 90.0, 90.0);

            index = AddCraft(typeof(DragonScalemailGloves), "Magical Armor", "Dragon Scalemail Gloves", 70.9, 103.1, typeof(Leather), 1044462, 12, 1044463);
            AddRes(index, typeof(DragonsBlood), "Dragon's Blood", 10, "You do not have enough resources to make that.");
            AddRes(index, typeof(DaemonSteelIngot), "DaemonSteel Ingot", 3, "You do not have enough resources to make that.");
            AddRes(index, typeof(RedScales), "Red Scales", 15, "You do not have enough resources to make that.");
            AddSkill(index, SkillName.Magery, 100.0, 100.0);
            AddSkill(index, SkillName.Blacksmith, 90.0, 90.0);

            index = AddCraft(typeof(DragonScalemailGorget), "Magical Armor", "Dragon Scalemail Gorget", 68.4, 101.8, typeof(Leather), 1044462, 10, 1044463);
            AddRes(index, typeof(DragonsBlood), "Dragon's Blood", 10, "You do not have enough resources to make that.");
            AddRes(index, typeof(DaemonSteelIngot), "DaemonSteel Ingot", 3, "You do not have enough resources to make that.");
            AddRes(index, typeof(RedScales), "Red Scales", 13, "You do not have enough resources to make that.");
            AddSkill(index, SkillName.Magery, 100.0, 100.0);
            AddSkill(index, SkillName.Blacksmith, 90.0, 90.0);

            index = AddCraft(typeof(DragonScalemailSkirt), "Magical Armor", "Dragon Scalemail Skirt", 80.8, 104.2, typeof(Leather), 1044462, 20, 1044463);
            AddRes(index, typeof(DragonsBlood), "Dragon's Blood", 10, "You do not have enough resources to make that.");
            AddRes(index, typeof(DaemonSteelIngot), "DaemonSteel Ingot", 3, "You do not have enough resources to make that.");
            AddRes(index, typeof(RedScales), "Red Scales", 24, "You do not have enough resources to make that.");
            AddSkill(index, SkillName.Magery, 100.0, 100.0);
            AddSkill(index, SkillName.Blacksmith, 90.0, 90.0);

            index = AddCraft(typeof(DragonScalemailFemaleChest), "Magical Armor", "Dragon Scalemail Female Chest", 87.0, 104.0, typeof(Leather), 1044462, 25, 1044463);
            AddRes(index, typeof(DragonsBlood), "Dragon's Blood", 10, "You do not have enough resources to make that.");
            AddRes(index, typeof(DaemonSteelIngot), "DaemonSteel Ingot", 3, "You do not have enough resources to make that.");
            AddRes(index, typeof(RedScales), "Red Scales", 28, "You do not have enough resources to make that.");
            AddSkill(index, SkillName.Magery, 100.0, 100.0);
            AddSkill(index, SkillName.Blacksmith, 90.0, 90.0);
            #endregion

            // Set the overridable material
			SetSubRes( typeof( Leather ), 1049150 );

			// Add every material you want the player to be able to choose from
			// This will override the overridable material
            AddSubRes( typeof( Leather ),		1049150, 00.0, 1044462, 1049311 );
            AddSubRes( typeof( SpinedLeather ),	1049151, 65.0, 1044462, 1049311 );
            AddSubRes( typeof( HornedLeather ),	1049152, 80.0, 1044462, 1049311 );
            AddSubRes( typeof( BarbedLeather ),	1049153, 99.0, 1044462, 1049311 );

			MarkOption = true;
			Repair = true;
			CanEnhance = Core.AOS;
		}
	}
}