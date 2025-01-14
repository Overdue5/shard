//Rev 187

using System;
using Server.Items;

namespace Server.Engines.Craft
{
	public class DefBlacksmithy : CraftSystem
	{
		public override SkillName MainSkill => SkillName.Blacksmith;

        public override int GumpTitleNumber => 1044002; // <CENTER>BLACKSMITHY MENU</CENTER>

        private static CraftSystem m_CraftSystem;

		public static CraftSystem CraftSystem
		{
			get
			{
				if ( m_CraftSystem == null )
					m_CraftSystem = new DefBlacksmithy();

				return m_CraftSystem;
			}
		}

        public override CraftECA ECA => CraftECA.CustomChance;

        public override double GetChanceAtMin( CraftItem item )
		{
			return 0.0; // 0%
		}

		private DefBlacksmithy() : base( 2, 5, 1.0)// base( 1, 2, 1.7 )
		{
			/*
			
			base( MinCraftEffect, MaxCraftEffect, Delay )
			
			MinCraftEffect	: The minimum number of time the mobile will play the craft effect
			MaxCraftEffect	: The maximum number of time the mobile will play the craft effect
			Delay			: The delay between each craft effect
			
			Example: (3, 6, 1.7) would make the mobile do the PlayCraftEffect override
			function between 3 and 6 time, with a 1.7 second delay each time.
			
			*/ 
		}

		private static readonly Type typeofAnvil = typeof( AnvilAttribute );
		private static readonly Type typeofForge = typeof( ForgeAttribute );

		public static void CheckAnvilAndForge( Mobile from, int range, out bool anvil, out bool forge )
		{
			anvil = false;
			forge = false;

			Map map = from.Map;

			if ( map == null )
				return;

			IPooledEnumerable eable = map.GetItemsInRange( from.Location, range );

			foreach ( Item item in eable )
			{
				Type type = item.GetType();

				bool isAnvil = ( type.IsDefined( typeofAnvil, false ) || item.ItemID == 4015 || item.ItemID == 4016 || item.ItemID == 0x2DD5 || item.ItemID == 0x2DD6 );
				bool isForge = ( type.IsDefined( typeofForge, false ) || item.ItemID == 4017 || (item.ItemID >= 6522 && item.ItemID <= 6569) || item.ItemID == 0x2DD8 );

				if ( isAnvil || isForge )
				{
					if ( (from.Z + 16) < item.Z || (item.Z + 16) < from.Z || !from.InLOS( item ) )
						continue;

					anvil = anvil || isAnvil;
					forge = forge || isForge;

					if ( anvil && forge )
						break;
				}
			}

			eable.Free();

			for ( int x = -range; (!anvil || !forge) && x <= range; ++x )
			{
				for ( int y = -range; (!anvil || !forge) && y <= range; ++y )
				{
                    StaticTile[] tiles = map.Tiles.GetStaticTiles(from.X + x, from.Y + y, true);

					for ( int i = 0; (!anvil || !forge) && i < tiles.Length; ++i )
					{
                        int id = tiles[i].ID;

						bool isAnvil = ( id == 4015 || id == 4016 || id == 0x2DD5 || id == 0x2DD6 );
						bool isForge = ( id == 4017 || (id >= 6522 && id <= 6569) || id == 0x2DD8 );

						if ( isAnvil || isForge )
						{
							if ( (from.Z + 16) < tiles[i].Z || (tiles[i].Z + 16) < from.Z || !from.InLOS( new Point3D( from.X+x, from.Y+y, tiles[i].Z + (tiles[i].Height/2) + 1 ) ) )
								continue;

							anvil = anvil || isAnvil;
							forge = forge || isForge;
						}
					}
				}
			}
		}

		public override int CanCraft( Mobile from, BaseTool tool, Type itemType )
		{
			if ( tool == null || tool.Deleted || tool.UsesRemaining < 0 )
				return 1044038; // You have worn out your tool!
			else if ( !BaseTool.CheckTool( tool, from ) )
				return 1048146; // If you have a tool equipped, you must use that tool.
			else if ( !BaseTool.CheckAccessible( tool, from ) )
				return 1044263; // The tool must be on your person to use.

			bool anvil, forge;
			CheckAnvilAndForge( from, 3, out anvil, out forge );

			if ( anvil && forge )
				return 0;

			return 1044267; // You must be near an anvil and a forge to smith items.
		}

		public override void PlayCraftEffect( Mobile from )
		{
		    // no animation, instant sound


		    if (from.Body.Type == BodyType.Human)
		    {
		        if (!from.Mounted)
		            from.Animate(9, 5, 1, true, false, 0);
		        else
                    from.Animate(26, 5, 1, true, false, 0);
		    }


		    new InternalTimer( from ).Start();

			//from.PlaySound( 0x2A );
		}

		// Delay to synchronize the sound with the hit on the anvil
		private class InternalTimer : Timer
		{
			private readonly Mobile m_From;

			public InternalTimer( Mobile from ) : base( TimeSpan.FromSeconds( 1.0 ) )
			{
				m_From = from;
			}

			protected override void OnTick()
			{
				m_From.PlaySound( 0x2A );
			}
		}

		public override int PlayEndingEffect( Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item )
		{
			if ( toolBroken )
				from.SendLocalizedMessage( 1044038 ); // You have worn out your tool

			if ( failed )
			{
				if ( lostMaterial )
					return 1044043; // You failed to create the item, and some of your materials are lost.
				else
					return 1044157; // You failed to create the item, but no materials were lost.
			}
			else
			{
				if ( quality == 0 )
					return 502785; // You were barely able to make this item.  It's quality is below average.
                else if (makersMark && quality == 2 && from.Skills[SkillName.Blacksmith].Base >= 100.0)
					return 1044156; // You create an exceptional quality item and affix your maker's mark.
				else if ( quality == 2 )
					return 1044155; // You create an exceptional quality item.
				else				
					return 1044154; // You create the item.
			}
		}

		public override void InitCraftList()
		{
			/*
			Synthax for a SIMPLE craft item
			AddCraft( ObjectType, Group, MinSkill, MaxSkill, ResourceType, Amount, Message )
			
			ObjectType		: The type of the object you want to add to the build list.
			Group			: The group in wich the object will be showed in the craft menu.
			MinSkill		: The minimum of skill value
			MaxSkill		: The maximum of skill value
			ResourceType	: The type of the resource the mobile need to create the item
			Amount			: The amount of the ResourceType it need to create the item
			Message			: String or Int for Localized.  The message that will be sent to the mobile, if the specified ressource is missing.
			
			Synthax for a COMPLEXE craft item.  A complexe item is an item that need either more than
			only one skill, or more than only one resource.
			
			Coming soon....
			*/

			#region Ringmail
			AddCraft( typeof( RingmailGloves ), 1011076, 1025099, 12.0, 62.0, typeof( IronIngot ), 1044036, 10, 1044037 );
			AddCraft( typeof( RingmailLegs ), 1011076, 1025104, 19.4, 69.4, typeof( IronIngot ), 1044036, 16, 1044037 );
			AddCraft( typeof( RingmailArms ), 1011076, 1025103, 16.9, 66.9, typeof( IronIngot ), 1044036, 14, 1044037 );
			AddCraft( typeof( RingmailChest ), 1011076, 1025100, 21.9, 71.9, typeof( IronIngot ), 1044036, 18, 1044037 );
			#endregion

			#region Chainmail
			AddCraft( typeof( ChainCoif ), 1011077, 1025051, 14.5, 64.5, typeof( IronIngot ), 1044036, 10, 1044037 );
			AddCraft( typeof( ChainLegs ), 1011077, 1025054, 36.7, 86.7, typeof( IronIngot ), 1044036, 18, 1044037 );
			AddCraft( typeof( ChainChest ), 1011077, 1025055, 39.1, 89.1, typeof( IronIngot ), 1044036, 20, 1044037 );
			#endregion

			int index = -1;

			#region Platemail
			AddCraft( typeof( PlateArms ), 1011078, 1025136, 66.3, 103.7, typeof( IronIngot ), 1044036, 18, 1044037 );
			AddCraft( typeof( PlateGloves ), 1011078, 1025140, 58.9, 103.1, typeof( IronIngot ), 1044036, 12, 1044037 );
			AddCraft( typeof( PlateGorget ), 1011078, 1025139, 56.4, 101.8, typeof( IronIngot ), 1044036, 10, 1044037 );
			AddCraft( typeof( PlateLegs ), 1011078, 1025137, 68.8, 104.2, typeof( IronIngot ), 1044036, 20, 1044037 );
			AddCraft( typeof( PlateChest ), 1011078, 1046431, 75.0, 104.0, typeof( IronIngot ), 1044036, 25, 1044037 );
			AddCraft( typeof( FemalePlateChest ), 1011078, 1046430, 68.8, 105.0, typeof( IronIngot ), 1044036, 20, 1044037 );

			if ( Core.AOS ) // exact pre-aos functionality unknown
				AddCraft( typeof( DragonBardingDeed ), 1011078, 1053012, 72.5, 122.5, typeof( IronIngot ), 1044036, 750, 1044037 );

			if( Core.SE )
			{
				
				index = AddCraft( typeof( PlateMempo ), 1011078, 1030180, 80.0, 130.0, typeof( IronIngot ), 1044036, 18, 1044037 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( PlateDo ), 1011078, 1030184, 80.0, 130.0, typeof( IronIngot ), 1044036, 28, 1044037 ); //Double check skill
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( PlateHiroSode ), 1011078, 1030187, 80.0, 130.0, typeof( IronIngot ), 1044036, 16, 1044037 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( PlateSuneate ), 1011078, 1030195, 65.0, 115.0, typeof( IronIngot ), 1044036, 20, 1044037 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( PlateHaidate ), 1011078, 1030200, 65.0, 115.0, typeof( IronIngot ), 1044036, 20, 1044037 );
				SetNeededExpansion( index, Expansion.SE );
				
			}
			#endregion

			#region Helmets
			AddCraft( typeof( Bascinet ), 1011079, 1025132, 8.3, 58.3, typeof( IronIngot ), 1044036, 15, 1044037 );
			AddCraft( typeof( CloseHelm ), 1011079, 1025128, 37.9, 87.9, typeof( IronIngot ), 1044036, 15, 1044037 );
			AddCraft( typeof( Helmet ), 1011079, 1025130, 37.9, 87.9, typeof( IronIngot ), 1044036, 15, 1044037 );
			AddCraft( typeof( NorseHelm ), 1011079, 1025134, 37.9, 87.9, typeof( IronIngot ), 1044036, 15, 1044037 );
			AddCraft( typeof( PlateHelm ), 1011079, 1025138, 61.6, 104.2, typeof( IronIngot ), 1044036, 15, 1044037 );
			
			if( Core.SE )
			{
				index = AddCraft( typeof( ChainHatsuburi ), 1011079, 1030175, 30.0, 80.0, typeof( IronIngot ), 1044036, 20, 1044037 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( PlateHatsuburi ), 1011079, 1030176, 45.0, 95.0, typeof( IronIngot ), 1044036, 20, 1044037 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( HeavyPlateJingasa ), 1011079, 1030178, 45.0, 95.0, typeof( IronIngot ), 1044036, 20, 1044037 );
				SetNeededExpansion( index, Expansion.SE );
				
				index = AddCraft( typeof( LightPlateJingasa ), 1011079, 1030188, 45.0, 95.0, typeof( IronIngot ), 1044036, 20, 1044037 );
				SetNeededExpansion( index, Expansion.SE );
				
				index = AddCraft( typeof( SmallPlateJingasa ), 1011079, 1030191, 45.0, 95.0, typeof( IronIngot ), 1044036, 20, 1044037 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( DecorativePlateKabuto ), 1011079, 1030179, 90.0, 140.0, typeof( IronIngot ), 1044036, 25, 1044037 );
				SetNeededExpansion( index, Expansion.SE );
				
				index = AddCraft( typeof( PlateBattleKabuto ), 1011079, 1030192, 90.0, 140.0, typeof( IronIngot ), 1044036, 25, 1044037 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( StandardPlateKabuto ), 1011079, 1030196, 90.0, 140.0, typeof( IronIngot ), 1044036, 25, 1044037 );
				SetNeededExpansion( index, Expansion.SE );

                /*
				if( Core.ML )
				{
					index = AddCraft( typeof( Circlet ), 1011079, 1032645, 62.1, 112.1, typeof( IronIngot ), 1044036, 6, 1044037 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( RoyalCirclet ), 1011079, 1032646, 70.0, 120.0, typeof( IronIngot ), 1044036, 6, 1044037 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( GemmedCirclet ), 1011079, 1032647, 75.0, 125.0, typeof( IronIngot ), 1044036, 6, 1044037 );
					AddRes( index, typeof( Tourmaline ), 1044237, 1, 1044240 );
					AddRes( index, typeof( Amethyst ), 1044236, 1, 1044240 );
					AddRes( index, typeof( BlueDiamond ), 1032696, 1, 1044240 );
					SetNeededExpansion( index, Expansion.ML );

				}
                */
			}
			#endregion

			#region Shields
			AddCraft( typeof( Buckler ), 1011080, 1027027, -25.0, 25.0, typeof( IronIngot ), 1044036, 10, 1044037 );
			AddCraft( typeof( BronzeShield ), 1011080, 1027026, -15.2, 34.8, typeof( IronIngot ), 1044036, 12, 1044037 );
			AddCraft( typeof( HeaterShield ), 1011080, 1027030, 24.3, 74.3, typeof( IronIngot ), 1044036, 18, 1044037 );
            
            //index = AddCraft(typeof(CalvaryShield), 1011080, "calvary shield", 90.0, 102.0, typeof(IronIngot), 1044036, 20, 1044037);
            //AddRes(index, typeof(OakLog), "Oak Log", 5, "You lack the oak logs to make this");
            //AddRes(index, typeof(StarSapphire), "Star Sapphire", 20, "You lack the star sapphires to make this");
            //AddSkill(index, SkillName.Parry, 100.0, 100.0);

			AddCraft( typeof( MetalShield ), 1011080, 1027035, -10.2, 39.8, typeof( IronIngot ), 1044036, 14, 1044037 );
			AddCraft( typeof( MetalKiteShield ), 1011080, 1027028, 4.6, 54.6, typeof( IronIngot ), 1044036, 16, 1044037 );
			AddCraft( typeof( WoodenKiteShield ), 1011080, 1027032, -15.2, 34.8, typeof( IronIngot ), 1044036, 8, 1044037 );

			if ( Core.AOS )
			{
				AddCraft( typeof( ChaosShield ), 1011080, 1027107, 85.0, 135.0, typeof( IronIngot ), 1044036, 25, 1044037 );
				AddCraft( typeof( OrderShield ), 1011080, 1027108, 85.0, 135.0, typeof( IronIngot ), 1044036, 25, 1044037 );
			}
			#endregion

			#region Bladed

			if ( Core.AOS )
				AddCraft( typeof( BoneHarvester ), 1011081, 1029915, 33.0, 83.0, typeof( IronIngot ), 1044036, 10, 1044037 );

			AddCraft( typeof( Broadsword ), 1011081, 1023934, 35.4, 85.4, typeof( IronIngot ), 1044036, 10, 1044037 );

			if ( Core.AOS )
				AddCraft( typeof( CrescentBlade ), 1011081, 1029921, 45.0, 95.0, typeof( IronIngot ), 1044036, 14, 1044037 );

			AddCraft( typeof( Cutlass ), 1011081, 1025185, 24.3, 74.3, typeof( IronIngot ), 1044036, 8, 1044037 );
			AddCraft( typeof( Dagger ), 1011081, 1023921, -0.4, 49.6, typeof( IronIngot ), 1044036, 3, 1044037 );
			AddCraft( typeof( Katana ),1011081, 1025119, 44.1, 94.1, typeof( IronIngot ), 1044036, 8, 1044037 );
			AddCraft( typeof( Kryss ), 1011081, 1025121, 36.7, 86.7, typeof( IronIngot ), 1044036, 8, 1044037 );
			AddCraft( typeof( Longsword ), 1011081, 1023937, 28.0, 78.0, typeof( IronIngot ), 1044036, 12, 1044037 );
			AddCraft( typeof( Scimitar ), 1011081, 1025046, 31.7, 81.7, typeof( IronIngot ), 1044036, 10, 1044037 );
			AddCraft( typeof( VikingSword ), 1011081, 1025049, 24.3, 74.3, typeof( IronIngot ), 1044036, 14, 1044037 );

			if( Core.SE )
			{
				index = AddCraft( typeof( NoDachi ), 1011081, 1030221, 75.0, 125.0, typeof( IronIngot ), 1044036, 18, 1044037 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( Wakizashi ), 1011081, 1030223, 50.0, 100.0, typeof( IronIngot ), 1044036, 8, 1044037 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( Lajatang ), 1011081, 1030226, 80.0, 130.0, typeof( IronIngot ), 1044036, 25, 1044037 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( Daisho ), 1011081, 1030228, 60.0, 110.0, typeof( IronIngot ), 1044036, 15, 1044037 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( Tekagi ), 1011081, 1030230, 55.0, 105.0, typeof( IronIngot ), 1044036, 12, 1044037 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( Shuriken ), 1011081, 1030231, 45.0, 95.0, typeof( IronIngot ), 1044036, 5, 1044037 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( Kama ), 1011081, 1030232, 40.0, 90.0, typeof( IronIngot ), 1044036, 14, 1044037 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( Sai ), 1011081, 1030234, 50.0, 100.0, typeof( IronIngot ), 1044036, 12, 1044037 );
				SetNeededExpansion( index, Expansion.SE );

                /*
				if( Core.ML )
				{
					index = AddCraft( typeof( RadiantScimitar ), 1011081, 1031571, 75.0, 125.0, typeof( IronIngot ), 1044036, 15, 1044037 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( WarCleaver ), 1011081, 1031567, 70.0, 120.0, typeof( IronIngot ), 1044036, 18, 1044037 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( ElvenSpellblade ), 1011081, 1031564, 70.0, 120.0, typeof( IronIngot ), 1044036, 14, 1044037 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( AssassinSpike ), 1011081, 1031565, 70.0, 120.0, typeof( IronIngot ), 1044036, 9, 1044037 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( Leafblade ), 1011081, 1031566, 70.0, 120.0, typeof( IronIngot ), 1044036, 12, 1044037 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( RuneBlade ), 1011081, 1031570, 70.0, 120.0, typeof( IronIngot ), 1044036, 15, 1044037 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( ElvenMachete ), 1011081, 1031573, 70.0, 120.0, typeof( IronIngot ), 1044036, 14, 1044037 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( RuneCarvingKnife ), 1011081, 1072915, 70.0, 120.0, typeof( IronIngot ), 1044036, 9, 1044037 );
					AddRes( index, typeof( DreadHornMane ), 1032682, 1, 1053098 );
					AddRes( index, typeof( Putrefication ), 1032678, 10, 1053098 );
					AddRes( index, typeof( Muculent ), 1032680, 10, 1053098 );
					AddRecipe( index, 0 );
					ForceNonExceptional( index );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( ColdForgedBlade ), 1011081, 1072916, 70.0, 120.0, typeof( IronIngot ), 1044036, 18, 1044037 );
					AddRes( index, typeof( GrizzledBones ), 1032684, 1, 1053098 );
					AddRes( index, typeof( Taint ), 1032684, 10, 1053098 );
					AddRes( index, typeof( Blight ), 1032675, 10, 1053098 );
					AddRecipe( index, 1 );
					ForceNonExceptional( index );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( OverseerSunderedBlade ), 1011081, 1072920, 70.0, 120.0, typeof( IronIngot ), 1044036, 15, 1044037 );
					AddRes( index, typeof( GrizzledBones ), 1032684, 1, 1053098 );
					AddRes( index, typeof( Blight ), 1032675, 10, 1053098 );
					AddRes( index, typeof( Scourge ), 1032677, 10, 1053098 );
					AddRecipe( index, 2 );
					ForceNonExceptional( index );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( LuminousRuneBlade ), 1011081, 1072922, 70.0, 120.0, typeof( IronIngot ), 1044036, 15, 1044037 );
					AddRes( index, typeof( GrizzledBones ), 1032684, 1, 1053098 );
					AddRes( index, typeof( Corruption ), 1032676, 10, 1053098 );
					AddRes( index, typeof( Putrefication ), 1032678, 10, 1053098 );
					AddRecipe( index, 3 );
					ForceNonExceptional( index );
					SetNeededExpansion( index, Expansion.ML );


					index = AddCraft( typeof( TrueSpellblade ), 1011081, 1073513, 75.0, 125.0, typeof( IronIngot ), 1044036, 14, 1044037 );
					AddRes( index, typeof( BlueDiamond ), 1032696, 1, 1044240 );
					AddRecipe( index, 4 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( IcySpellblade ), 1011081, 1073514, 75.0, 125.0, typeof( IronIngot ), 1044036, 14, 1044037 );
					AddRes( index, typeof( Turquoise ), 1032691, 1, 1044240 );
					AddRecipe( index, 5 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( FierySpellblade ), 1011081, 1073515, 75.0, 125.0, typeof( IronIngot ), 1044036, 14, 1044037 );
					AddRes( index, typeof( FireRuby ), 1032695, 1, 1044240 );
					AddRecipe( index, 6 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( SpellbladeOfDefense ), 1011081, 1073516, 75.0, 125.0, typeof( IronIngot ), 1044036, 18, 1044037 );
					AddRes( index, typeof( WhitePearl ), 1032694, 1, 1044240 );
					AddRecipe( index, 7 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( TrueAssassinSpike ), 1011081, 1073517, 75.0, 125.0, typeof( IronIngot ), 1044036, 9, 1044037 );
					AddRes( index, typeof( DarkSapphire ), 1032690, 1, 1044240 );
					AddRecipe( index, 8 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( ChargedAssassinSpike ), 1011081, 1073518, 75.0, 125.0, typeof( IronIngot ), 1044036, 9, 1044037 );
					AddRes( index, typeof( EcruCitrine ), 1032693, 1, 1044240 );
					AddRecipe( index, 9 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( MagekillerAssassinSpike ), 1011081, 1073519, 75.0, 125.0, typeof( IronIngot ), 1044036, 9, 1044037 );
					AddRes( index, typeof( BrilliantAmber ), 1032697, 1, 1044240 );
					AddRecipe( index, 10 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( WoundingAssassinSpike ), 1011081, 1073520, 75.0, 125.0, typeof( IronIngot ), 1044036, 9, 1044037 );
					AddRes( index, typeof( PerfectEmerald ), 1032692, 1, 1044240 );
					AddRecipe( index, 11 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( TrueLeafblade ), 1011081, 1073521, 75.0, 125.0, typeof( IronIngot ), 1044036, 12, 1044037 );
					AddRes( index, typeof( BlueDiamond ), 1032696, 1, 1044240 );
					AddRecipe( index, 12 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( Luckblade ), 1011081, 1073522, 75.0, 125.0, typeof( IronIngot ), 1044036, 12, 1044037 );
					AddRes( index, typeof( WhitePearl ), 1032694, 1, 1044240 );
					AddRecipe( index, 13 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( MagekillerLeafblade ), 1011081, 1073523, 75.0, 125.0, typeof( IronIngot ), 1044036, 12, 1044037 );
					AddRes( index, typeof( FireRuby ), 1032695, 1, 1044240 );
					AddRecipe( index, 14 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( LeafbladeOfEase ), 1011081, 1073524, 75.0, 125.0, typeof( IronIngot ), 1044036, 12, 1044037 );
					AddRes( index, typeof( PerfectEmerald ), 1032692, 1, 1044240 );
					AddRecipe( index, 15 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( KnightsWarCleaver ), 1011081, 1073525, 75.0, 125.0, typeof( IronIngot ), 1044036, 18, 1044037 );
					AddRes( index, typeof( PerfectEmerald ), 1032692, 1, 1044240 );
					AddRecipe( index, 16 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( ButchersWarCleaver ), 1011081, 1073526, 75.0, 125.0, typeof( IronIngot ), 1044036, 18, 1044037 );
					AddRes( index, typeof( Turquoise ), 1032691, 1, 1044240 );
					AddRecipe( index, 17 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( SerratedWarCleaver ), 1011081, 1073527, 75.0, 125.0, typeof( IronIngot ), 1044036, 18, 1044037 );
					AddRes( index, typeof( EcruCitrine ), 1032693, 1, 1044240 );
					AddRecipe( index, 18 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( TrueWarCleaver ), 1011081, 1073528, 75.0, 125.0, typeof( IronIngot ), 1044036, 18, 1044037 );
					AddRes( index, typeof( BrilliantAmber ), 1032697, 1, 1044240 );
					AddRecipe( index, 19 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( AdventurersMachete ), 1011081, 1073533, 75.0, 125.0, typeof( IronIngot ), 1044036, 14, 1044037 );
					AddRes( index, typeof( WhitePearl ), 1032694, 1, 1044240 );
					AddRecipe( index, 20 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( OrcishMachete ), 1011081, 1073534, 75.0, 125.0, typeof( IronIngot ), 1044036, 14, 1044037 );
					AddRes( index, typeof( Scourge ), 1072136, 1, 1042081 );
					AddRecipe( index, 21 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( MacheteOfDefense ), 1011081, 1073535, 75.0, 125.0, typeof( IronIngot ), 1044036, 14, 1044037 );
					AddRes( index, typeof( BrilliantAmber ), 1032697, 1, 1044240 );
					AddRecipe( index, 22 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( DiseasedMachete ), 1011081, 1073536, 75.0, 125.0, typeof( IronIngot ), 1044036, 14, 1044037 );
					AddRes( index, typeof( Blight ), 1072134, 1, 1042081 );
					AddRecipe( index, 23 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( RuneSabre ), 1011081, 1073537, 75.0, 125.0, typeof( IronIngot ), 1044036, 15, 1044037 );
					AddRes( index, typeof( Turquoise ), 1032691, 1, 1044240 );
					AddRecipe( index, 24 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( MagesRuneBlade ), 1011081, 1073538, 75.0, 125.0, typeof( IronIngot ), 1044036, 15, 1044037 );
					AddRes( index, typeof( BlueDiamond ), 1032696, 1, 1044240 );
					AddRecipe( index, 25 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( RuneBladeOfKnowledge ), 1011081, 1073539, 75.0, 125.0, typeof( IronIngot ), 1044036, 15, 1044037 );
					AddRes( index, typeof( EcruCitrine ), 1032693, 1, 1044240 );
					AddRecipe( index, 26 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( CorruptedRuneBlade ), 1011081, 1073540, 75.0, 125.0, typeof( IronIngot ), 1044036, 15, 1044037 );
					AddRes( index, typeof( Corruption ), 1072135, 1, 1042081 );
					AddRecipe( index, 27 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( TrueRadiantScimitar ), 1011081, 1073541, 75.0, 125.0, typeof( IronIngot ), 1044036, 15, 1044037 );
					AddRes( index, typeof( BrilliantAmber ), 1032697, 1, 1044240 );
					AddRecipe( index, 28 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( DarkglowScimitar ), 1011081, 1073542, 75.0, 125.0, typeof( IronIngot ), 1044036, 15, 1044037 );
					AddRes( index, typeof( DarkSapphire ), 1032690, 1, 1044240 );
					AddRecipe( index, 29 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( IcyScimitar ), 1011081, 1073543, 75.0, 125.0, typeof( IronIngot ), 1044036, 15, 1044037 );
					AddRes( index, typeof( DarkSapphire ), 1032690, 1, 1044240 );
					AddRecipe( index, 30 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( TwinklingScimitar ), 1011081, 1073544, 75.0, 125.0, typeof( IronIngot ), 1044036, 15, 1044037 );
					AddRes( index, typeof( DarkSapphire ), 1032690, 1, 1044240 );
					AddRecipe( index, 31 );
					SetNeededExpansion( index, Expansion.ML );

					index = AddCraft( typeof( BoneMachete ), 1011081, 1020526, 75.0, 125.0, typeof( IronIngot ), 1044036, 20, 1044037 );
					AddRes( index, typeof( Bone ), 1049064, 6, 1049063 );
					AddRecipe( index, 32 );
					SetNeededExpansion( index, Expansion.ML );
				}
                 * */
            }
			#endregion

			#region Axes
			AddCraft( typeof( Axe ), 1011082, 1023913, 34.2, 84.2, typeof( IronIngot ), 1044036, 14, 1044037 );
			AddCraft( typeof( BattleAxe ), 1011082, 1023911, 30.5, 80.5, typeof( IronIngot ), 1044036, 14, 1044037 );
			AddCraft( typeof( DoubleAxe ), 1011082, 1023915, 29.3, 79.3, typeof( IronIngot ), 1044036, 12, 1044037 );
			AddCraft( typeof( ExecutionersAxe ), 1011082, 1023909, 34.2, 84.2, typeof( IronIngot ), 1044036, 14, 1044037 );
			AddCraft( typeof( LargeBattleAxe ), 1011082, 1025115, 28.0, 78.0, typeof( IronIngot ), 1044036, 12, 1044037 );
			AddCraft( typeof( TwoHandedAxe ), 1011082, 1025187, 33.0, 83.0, typeof( IronIngot ), 1044036, 16, 1044037 );
			AddCraft( typeof( WarAxe ), 1011082, 1025040, 39.1, 89.1, typeof( IronIngot ), 1044036, 16, 1044037 );
            /*
            if( Core.ML )
            {
                index = AddCraft( typeof( OrnateAxe ), 1011082, 1031572, 70.0, 120.0, typeof( IronIngot ), 1044036, 18, 1044037 );
                SetNeededExpansion( index, Expansion.ML );

                index = AddCraft( typeof( GuardianAxe ), 1011082, 1073545, 75.0, 125.0, typeof( IronIngot ), 1044036, 15, 1044037 );
                AddRes( index, typeof( BlueDiamond ), 1032696, 1, 1044240 );
                AddRecipe( index, 33 );
                SetNeededExpansion( index, Expansion.ML );

                index = AddCraft( typeof( SingingAxe ), 1011082, 1073546, 75.0, 125.0, typeof( IronIngot ), 1044036, 15, 1044037 );
                AddRes( index, typeof( BrilliantAmber ), 1032697, 1, 1044240 );
                AddRecipe( index, 34 );
                SetNeededExpansion( index, Expansion.ML );

                index = AddCraft( typeof( ThunderingAxe ), 1011082, 1073547, 75.0, 125.0, typeof( IronIngot ), 1044036, 15, 1044037 );
                AddRes( index, typeof( EcruCitrine ), 1032693, 1, 1044240 );
                AddRecipe( index, 35 );
                SetNeededExpansion( index, Expansion.ML );

                index = AddCraft( typeof( HeavyOrnateAxe ), 1011082, 1073548, 75.0, 125.0, typeof( IronIngot ), 1044036, 15, 1044037 );
                AddRes( index, typeof( Turquoise ), 1032691, 1, 1044240 );
                AddRecipe( index, 36 );
                SetNeededExpansion( index, Expansion.ML );
            			 
            }*/

            #endregion

			#region Pole Arms

			AddCraft( typeof( Bardiche ), 1011083, 1023917, 31.7, 81.7, typeof( IronIngot ), 1044036, 18, 1044037 );

			if ( Core.AOS )
				AddCraft( typeof( BladedStaff ), 1011083, 1029917, 40.0, 90.0, typeof( IronIngot ), 1044036, 12, 1044037 );

			if ( Core.AOS )
				AddCraft( typeof( DoubleBladedStaff ), 1011083, 1029919, 45.0, 95.0, typeof( IronIngot ), 1044036, 16, 1044037 );

			AddCraft( typeof( Halberd ), 1011083, 1025183, 39.1, 89.1, typeof( IronIngot ), 1044036, 20, 1044037 );

			if ( Core.AOS )
				AddCraft( typeof( Lance ), 1011083, 1029920, 48.0, 98.0, typeof( IronIngot ), 1044036, 20, 1044037 );

			if ( Core.AOS )
				AddCraft( typeof( Pike ), 1011083, 1029918, 47.0, 97.0, typeof( IronIngot ), 1044036, 12, 1044037 );

			AddCraft( typeof( ShortSpear ), 1011083, 1025123, 45.3, 95.3, typeof( IronIngot ), 1044036, 6, 1044037 );

			if ( Core.AOS )
				AddCraft( typeof( Scythe ), 1011083, 1029914, 39.0, 89.0, typeof( IronIngot ), 1044036, 14, 1044037 );

			AddCraft( typeof( Spear ), 1011083, 1023938, 49.0, 99.0, typeof( IronIngot ), 1044036, 12, 1044037 );
			AddCraft( typeof( WarFork ), 1011083, 1025125, 42.9, 92.9, typeof( IronIngot ), 1044036, 12, 1044037 );

			// Not craftable (is this an AOS change ??)
			//AddCraft( typeof( Pitchfork ), 1011083, 1023720, 36.1, 86.1, typeof( IronIngot ), 1044036, 12, 1044037 );
			#endregion

			#region Bashing
			AddCraft( typeof( HammerPick ), 1011084, 1025181, 34.2, 84.2, typeof( IronIngot ), 1044036, 16, 1044037 );
			AddCraft( typeof( Mace ), 1011084, 1023932, 14.5, 64.5, typeof( IronIngot ), 1044036, 6, 1044037 );
			AddCraft( typeof( Maul ), 1011084, 1025179, 19.4, 69.4, typeof( IronIngot ), 1044036, 10, 1044037 );

			if ( Core.AOS )
				AddCraft( typeof( Scepter ), 1011084, 1029916, 21.4, 71.4, typeof( IronIngot ), 1044036, 10, 1044037 );

			AddCraft( typeof( WarMace ), 1011084, 1025127, 28.0, 78.0, typeof( IronIngot ), 1044036, 14, 1044037 );
			AddCraft( typeof( WarHammer ), 1011084, 1025177, 34.2, 84.2, typeof( IronIngot ), 1044036, 16, 1044037 );

			if( Core.SE )
			{
				index = AddCraft( typeof( Tessen ), 1011084, 1030222, 85.0, 135.0, typeof( IronIngot ), 1044036, 16, 1044037 );
				AddSkill( index, SkillName.Tailoring, 50.0, 55.0 );
				AddRes( index, typeof( Cloth ), 1044286, 10, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
			}

            /*
			if( Core.ML )
			{
				index = AddCraft( typeof( DiamondMace ), 1011084, 1073568, 70.0, 120.0, typeof( IronIngot ), 1044036, 20, 1044037 );
				SetNeededExpansion( index, Expansion.ML );

				index = AddCraft( typeof( ShardThrasher ), 1011084, 1072918, 70.0, 120.0, typeof( IronIngot ), 1044036, 20, 1044037 );
				AddRes( index, typeof( EyeOfTheTravesty ), 1073126, 1, 1042081 );
				AddRes( index, typeof( Muculent ), 1072139, 10, 1042081 );
				AddRes( index, typeof( Corruption ), 1072135, 10, 1042081 );
				AddRecipe( index, 37 );
				ForceNonExceptional( index );
				SetNeededExpansion( index, Expansion.ML );

				index = AddCraft( typeof( RubyMace ), 1011084, 1073529, 75.0, 125.0, typeof( IronIngot ), 1044036, 20, 1044037 );
				AddRes( index, typeof( FireRuby ), 1032695, 1, 1044240 );
				AddRecipe( index, 38 );
				SetNeededExpansion( index, Expansion.ML );

				index = AddCraft( typeof( EmeraldMace ), 1011084, 1073530, 75.0, 125.0, typeof( IronIngot ), 1044036, 20, 1044037 );
				AddRes( index, typeof( PerfectEmerald ), 1032692, 1, 1044240 );
				AddRecipe( index, 39 );
				SetNeededExpansion( index, Expansion.ML );

				index = AddCraft( typeof( SapphireMace ), 1011084, 1073531, 75.0, 125.0, typeof( IronIngot ), 1044036, 20, 1044037 );
				AddRes( index, typeof( DarkSapphire ), 1032690, 1, 1044240 );
				AddRecipe( index, 40 );
				SetNeededExpansion( index, Expansion.ML );

				index = AddCraft( typeof( SilverEtchedMace ), 1011084, 1073532, 75.0, 125.0, typeof( IronIngot ), 1044036, 20, 1044037 );
				AddRes( index, typeof( BlueDiamond ), 1032696, 1, 1044240 );
				AddRecipe( index, 41 );
				SetNeededExpansion( index, Expansion.ML );
			}*/
			#endregion

            #region Colored Weapons

            index = AddCraft(typeof(SkyfallLongsword), "Colored Weapons", "Skyfall Longsword", 95.0, 102.4, typeof(MytherilIngot), "Mytheril Ingots", 12, 1044037);
            AddRes(index, typeof(IronIngot), "Iron Ingots", 100, 1044037);
            AddRes(index, typeof(Ruby), "Rubies", 3, "You lack rubies to create this");
            AddRes(index, typeof(Log), 1044041, 10, 1044351);
            AddSkill(index, SkillName.Tactics, 100.0, 100.0);

            index = AddCraft(typeof(HellsHalberd), "Colored Weapons", "Hell's Halberd", 95.0, 102.4, typeof(DaemonSteelIngot), "Daemon Steel Ingots", 16, 1044037);
            AddRes(index, typeof(IronIngot), "Iron Ingots", 120, 1044037);
            AddRes(index, typeof(Ruby), "Rubies", 13, "You lack rubies to create this");
            AddRes(index, typeof(FleshOfTheUndead), "Flesh of the Undead", 3, "You lack flesh of the undead to create this");
            AddRes(index, typeof(Log), 1044041, 10, 1044351);
		    AddSkill(index, SkillName.Tinkering, 100.0, 100.0);
            AddSkill(index, SkillName.Tactics, 100.0, 100.0);
            AddSkill(index, SkillName.Alchemy, 100.0, 100.0);

            index = AddCraft(typeof(JudgementHammer), "Colored Weapons", "Judgement Hammer", 95.0, 102.3, typeof(SilverIngot), "Silver Ingots", 100, 1044037);
            AddRes(index, typeof(IronIngot), "Iron Ingots", 50, 1044037);
            AddRes(index, typeof(FrostwoodLog), "Frostwood Logs", 25, "You lack frostwood logs to create this");
            AddRes(index, typeof(WhiteScales), "White Scales", 5, "You lack white scales to create this");
            AddSkill(index, SkillName.Tinkering, 100.0, 100.0);
            AddSkill(index, SkillName.Tactics, 100.0, 100.0);
            AddSkill(index, SkillName.Alchemy, 90.0, 100.0);

            index = AddCraft(typeof(SoulBardiche), "Colored Weapons", "Soul Infused Bardiche", 96.0, 101.9, typeof(SilverIngot), "Silver Ingots", 100, 1044037);
            AddRes(index, typeof(IronIngot), "Iron Ingots", 120, 1044037);            
			AddRes(index, typeof(DragonsBlood), "Dragon's blood", 14, "You lack dragon's blood to create this");
            AddSkill(index, SkillName.Tactics, 100.0, 100.0);
			AddSkill(index, SkillName.Tinkering, 100.0, 100.0);
			AddSkill(index, SkillName.Alchemy, 90.0, 100.0);
			
            index = AddCraft(typeof(DwarvenBattleAxe), "Colored Weapons", "Dwarven Battle Axe", 91.0, 101.2, typeof(MytherilIngot), "Mytheril Ingots", 40, 1044037);
            AddRes(index, typeof(ValoriteIngot), "Valorite Ingots", 20, 1044037);
            AddRes(index, typeof(StarSapphire), "Star Sapphires", 10, "You lack star sapphires to create this");
            AddRes(index, typeof(Log), 1044041, 12, 1044351);
            AddSkill(index, SkillName.Tinkering, 100.0, 100.0);
            AddSkill(index, SkillName.Tactics, 90.0, 100.0);

            index = AddCraft(typeof(AzazelsBlade), "Colored Weapons", "Azazel's Blade", 92.0, 101.1, typeof(VeriteIngot), "Verite Ingots", 50, 1044037);
            AddRes(index, typeof(DragonsBlood), "Dragons Blood", 75, 1044037);
            AddRes(index, typeof(Emerald), "Emeralds", 10, "You lack emeralds to create this");
            AddRes(index, typeof(GreenScales), "Green Scales", 15, "You lack green scales to create this");
            AddSkill(index, SkillName.Tinkering, 90.0, 100.0);
            AddSkill(index, SkillName.Tactics, 80.0, 100.0);

            index = AddCraft(typeof(SuperiorAzazelsBlade), "Colored Weapons", "Superior Azazel's Blade", 96.0, 101.9, typeof(ReactiveIngot), "Reactive Ingots", 40, 1044037);
            AddRes(index, typeof(DragonsBlood), "Dragons Blood", 150, 1044037);
            AddRes(index, typeof(GreenScales), "Green Scales", 10, "You lack green scales to create this");
            AddRes(index, typeof(AzazelsBlade), "Azazel's Blade", 1, "You lack a azazel's blade to create this.");
            AddSkill(index, SkillName.Tinkering, 100.0, 100.0);
            AddSkill(index, SkillName.Tactics, 100.0, 100.0);

            index = AddCraft(typeof(SerpentsTongue), "Colored Weapons", "Serpent's Tongue", 92.0, 101.2, typeof(OldCopperIngot), "Old Copper Ingots", 50, 1044037);
            AddRes(index, typeof(VeriteIngot), "Verite Ingots", 35, 1044037);
            AddRes(index, typeof(Emerald), "Emeralds", 10, "You lack emeralds to create this");
            AddRes(index, typeof(GreenScales), "Green Scales", 15, "You lack green scales to create this");
            AddSkill(index, SkillName.Tinkering, 90.0, 100.0);
            AddSkill(index, SkillName.Tactics, 80.0, 100.0);

            index = AddCraft(typeof(DwarfWarHammer), "Colored Weapons", "Dwarf War Hammer", 88.0, 100.8, typeof(DwarvenIngot), "Dwarven Ingots", 40, 1044037);
            AddRes(index, typeof(ValoriteIngot), "Valorite Ingots", 10, 1044037);
            AddRes(index, typeof(Sapphire), "Sapphires", 10, "You lack sapphire to create this");
            AddRes(index, typeof(Log), 1044041, 10, 1044351);
            AddSkill(index, SkillName.Tinkering, 90.0, 100.0);
            AddSkill(index, SkillName.Tactics, 70.0, 100.0);

            index = AddCraft(typeof(DwarvenBattleAxe), "Colored Weapons", "Dwarven Battle Axe", 88.0, 100.8, typeof(DwarvenIngot), "Dwarven Ingots", 64, 1044037);
            AddRes(index, typeof(ValoriteIngot), "Valorite Ingots", 34, 1044037);
            AddRes(index, typeof(Sapphire), "Sapphires", 18, "You lack sapphire to create this");
            AddRes(index, typeof(Log), 1044041, 10, 1044351);
            AddRes(index, typeof(IronIngot), "Iron Ingots", 80, 1044037);
            AddSkill(index, SkillName.Tinkering, 90.0, 100.0);
            AddSkill(index, SkillName.Tactics, 70.0, 100.0);

            index = AddCraft(typeof(BloodTentacle), "Colored Weapons", "Blood Tentacle", 85.0, 100.5, typeof(BloodRockIngot), "Blood Rock Ingots", 65, 1044037);
            AddRes(index, typeof(IronIngot), "Iron Ingots", 100, 1044037);
            AddRes(index, typeof(Ruby), "Rubies", 25, "You lack rubies to create this");
            AddRes(index, typeof(RedScales), "Red Scales", 8, "You lack red scales to create this");
            AddSkill(index, SkillName.Tinkering, 80.0, 100.0);

            index = AddCraft(typeof(KissOfDeath), "Colored Weapons", "Kiss Of Death", 97.0, 100.9, typeof(ShadowIronIngot), "Shadow Iron Ingots", 150, 1044037);
			AddRes(index, typeof(IronIngot), "Iron Ingots", 200, 1044037);
            AddRes(index, typeof(Tourmaline), "Tourmalines", 20, "You lack tourmalines to create this");
            AddRes(index, typeof(Log), 1044041, 50, 1044351);
            AddSkill(index, SkillName.Fletching, 95.0, 100.0);
            AddSkill(index, SkillName.Tactics, 80.0, 100.0);

            index = AddCraft(typeof(ApalalasClaw), "Colored Weapons", "Apalalas Claw", 75.0, 100.0, typeof(FireIngot), "Fire Ingots", 56, 1044037);
            AddRes(index, typeof(Citrine), "Citrines", 20, "You lack citrines to create this");
            AddRes(index, typeof(IronIngot), "Iron Ingots", 50, 1044037);
            AddRes(index, typeof(Log), 1044041, 20, 1044351);

            index = AddCraft(typeof(GoblinClooba), "Colored Weapons", "Goblin Clooba", 96.0, 101.9, typeof(SilverIngot), "SilverIngot Ingots", 20, 1044037);
            AddRes(index, typeof(Log), 1044041, 15, 1044351);
            AddRes(index, typeof(IronIngot), "Iron Ingots", 45, 1044037);
            AddSkill(index, SkillName.Tactics, 100.0, 100.0);
            #endregion

            //#region Dragon Scale Armor
            //index = AddCraft( typeof( DragonGloves ), 1053114, 1029795, 68.9, 118.9, typeof( RedScales ), 1060883, 16, 1060884 );
            //SetUseSubRes2( index, true );

            //index = AddCraft( typeof( DragonHelm ), 1053114, 1029797, 72.6, 122.6, typeof( RedScales ), 1060883, 20, 1060884 );
            //SetUseSubRes2( index, true );

            //index = AddCraft( typeof( DragonLegs ), 1053114, 1029799, 78.8, 128.8, typeof( RedScales ), 1060883, 28, 1060884 );
            //SetUseSubRes2( index, true );

            //index = AddCraft( typeof( DragonArms ), 1053114, 1029815, 76.3, 126.3, typeof( RedScales ), 1060883, 24, 1060884 );
            //SetUseSubRes2( index, true );

            //index = AddCraft( typeof( DragonChest ), 1053114, 1029793, 85.0, 135.0, typeof( RedScales ), 1060883, 36, 1060884 );
            //SetUseSubRes2( index, true );
            //#endregion

            // Set the overridable material
            SetSubRes( typeof( IronIngot ), "Iron" );

            //1044036 = ingots

            AddSubRes(typeof(IronIngot), "Iron", 00.0, 1044267);
            AddSubRes(typeof(OldCopperIngot), "Old Copper", 35.0, 1044268);
            AddSubRes(typeof(ShadowIronIngot), "Shadow", 50.0, 1044268);
            AddSubRes(typeof(SilverIngot), "Silver", 55.0, 1044268);
            AddSubRes(typeof(VeriteIngot), "Verite", 60.0, 1044268);
            AddSubRes(typeof(RoseIngot), "Rose", 65.0, 1044268);
            AddSubRes(typeof(GoldIngot), "Gold", 70.0, 1044268);
            AddSubRes(typeof(IceIngot), "Ice", 72.5, 1044268);
            AddSubRes(typeof(AmethystIngot), "Amethyst", 72.5, 1044268);
            AddSubRes(typeof(ValoriteIngot), "Valorite", 75.0, 1044268);
            AddSubRes(typeof(BloodRockIngot), "Blood Rock", 75.0, 1044268);
            AddSubRes(typeof(AquaIngot), "Aqua", 80.0, 1044268);
            AddSubRes(typeof(FireIngot), "Fire", 82.0, 1044268);
            AddSubRes(typeof(MytherilIngot), "Mytheril", 87.5, 1044268);
            AddSubRes(typeof(SandRockIngot), "Sand Rock", 90.0, 1044268);
            AddSubRes(typeof(DwarvenIngot), "Dwarven", 90.0, 1044268);
            AddSubRes(typeof(BlackDiamondIngot), "Black Diamond", 92.5, 1044268);
            AddSubRes(typeof(BlackRockIngot), "Black Rock", 95.0, 1044268);
            AddSubRes(typeof(OceanicIngot), "Oceanic", 97.0, 1044268);
            AddSubRes(typeof(DaemonSteelIngot), "Daemon Steel", 99.0, 1044268);
            AddSubRes(typeof(ReactiveIngot), "Reactive", 99.5, 1044268);
            AddSubRes(typeof(AdamantiumIngot), "Adamantium", 100.0, 1044268);
            AddSubRes(typeof(SapphireIngot), "Sapphire", 100.0, 1044268);

            //Unused
            //new CraftResourceInfo( 0x973, 1053108, "Dull Copper",	CraftAttributeInfo.DullCopper,	CraftResource.DullCopper,		typeof( DullCopperIngot ),	typeof( DullCopperOre ),	typeof( DullCopperGranite ) ),
            //new CraftResourceInfo( 0x96D, 1053106, "Copper",		CraftAttributeInfo.Copper,		CraftResource.Copper,			typeof( CopperIngot ),		typeof( CopperOre ),		typeof( CopperGranite ) ),
            //new CraftResourceInfo( 0x972, 1053105, "Bronze",		CraftAttributeInfo.Bronze,		CraftResource.Bronze,			typeof( BronzeIngot ),		typeof( BronzeOre ),		typeof( BronzeGranite ) ),
            //new CraftResourceInfo( 0x979, 1053103, "Agapite",		CraftAttributeInfo.Agapite,		CraftResource.Agapite,			typeof( AgapiteIngot ),		typeof( AgapiteOre ),		typeof( AgapiteGranite ) ),


            //SetSubRes2( typeof( RedScales ), 1060875 );

            //AddSubRes2( typeof( RedScales ),		1060875, 0.0, 1053137, 1044268 );
            //AddSubRes2( typeof( YellowScales ),		1060876, 0.0, 1053137, 1044268 );
            //AddSubRes2( typeof( BlackScales ),		1060877, 0.0, 1053137, 1044268 );
            //AddSubRes2( typeof( GreenScales ),		1060878, 0.0, 1053137, 1044268 );
            //AddSubRes2( typeof( WhiteScales ),		1060879, 0.0, 1053137, 1044268 );
            //AddSubRes2( typeof( BlueScales ),		1060880, 0.0, 1053137, 1044268 );

			Resmelt = true;
			Repair = true;
			MarkOption = true;
			CanEnhance = Core.AOS;
		}
	}

	public class ForgeAttribute : Attribute
	{
	}

	public class AnvilAttribute : Attribute
	{
	}
}