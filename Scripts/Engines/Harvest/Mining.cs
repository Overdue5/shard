using System;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Engines.Harvest
{
	public class Mining : HarvestSystem
	{
		private static Mining m_System;

		public static Mining System
		{
			get
			{
				if ( m_System == null )
					m_System = new Mining();

				return m_System;
			}
		}

		private readonly HarvestDefinition m_OreAndStone;
	    private readonly HarvestDefinition m_Sand;

	    public HarvestDefinition OreAndStone => m_OreAndStone;

        public HarvestDefinition Sand => m_Sand;

        private Mining()
		{
			HarvestResource[] res;
			HarvestVein[] veins;

			#region Mining for ore and stone
			HarvestDefinition oreAndStone = m_OreAndStone = new HarvestDefinition();

			// Resource banks are every 1x1 tiles
			oreAndStone.BankWidth = 1;
			oreAndStone.BankHeight = 1;

			// Every bank holds 15 ores
			oreAndStone.MinTotal = 15;
			oreAndStone.MaxTotal = 15;

			// A resource bank will respawn its content every 35 to 55 minutes
			oreAndStone.MinRespawn = TimeSpan.FromMinutes( 35.0 );
			oreAndStone.MaxRespawn = TimeSpan.FromMinutes( 55.0 );

			// Skill checking is done on the Mining skill
			oreAndStone.Skill = SkillName.Mining;

			// Set the list of harvestable tiles
			oreAndStone.Tiles = m_MountainAndCaveTiles;

			// Players must be within 2 tiles to harvest
			oreAndStone.MaxRange = 2;

			// One ore per harvest action
			oreAndStone.ConsumedPerHarvest = 1;
			oreAndStone.ConsumedPerFeluccaHarvest = 2;

			// The digging effect
			oreAndStone.EffectActions = new int[]{ 11 };
            oreAndStone.EffectActionsRiding = new int[] { 26 };
			oreAndStone.EffectSounds = new int[]{ 0x125, 0x126 };
			oreAndStone.EffectCounts = new int[]{ 1 };
			oreAndStone.EffectDelay = TimeSpan.FromSeconds( 1.0 );
			oreAndStone.EffectSoundDelay = TimeSpan.FromSeconds( 1.0 );

			oreAndStone.NoResourcesMessage = 503040; // There is no metal here to mine.
			oreAndStone.DoubleHarvestMessage = 503042; // Someone has gotten to the metal before you.
			oreAndStone.TimedOutOfRangeMessage = 503041; // You have moved too far away to continue mining.
			oreAndStone.OutOfRangeMessage = 500446; // That is too far away.
			oreAndStone.FailMessage = 503043; // You loosen some rocks but fail to find any useable ore.
			oreAndStone.PackFullMessage = 1010481; // Your backpack is full, so the ore you mined is lost.
			oreAndStone.ToolBrokeMessage = 1044038; // You have worn out your tool!

            res = new HarvestResource[]
				{
                    new HarvestResource( 00.0, 00.0, 100.0, "You dig some Iron ore and put it in your backpack", typeof( IronOre ),		typeof( Granite ) ),
                    new HarvestResource( 35.0, 35.0, 100.0, "You dig some Old Copper ore and put it in your backpack" , typeof( OldCopperOre ),	typeof( OldCopperGranite ), typeof( OldCopperElemental ) ),
					new HarvestResource( 50.0, 50.0, 100.0, "You dig some Shadow Iron ore and put it in your backpack", typeof( ShadowIronOre ),	typeof( ShadowIronGranite ),		typeof( ShadowIronElemental ) ),
                    new HarvestResource( 55, 55.0, 100.0, "You dig some Silver ore and put it in your backpack", typeof( SilverOre ),	typeof( SilverGranite ), typeof( SilverElemental ) ),
                    new HarvestResource( 60, 60.0, 100.0, "You dig some Verite ore and put it in your backpack", typeof( VeriteOre ),	typeof( VeriteGranite ), typeof( VeriteElemental ) ), 
                    new HarvestResource( 65, 65.0, 100.0, "You dig some Rose ore and put it in your backpack", typeof( RoseOre ),	typeof( RoseGranite ), typeof( RoseElemental ) ),
                    new HarvestResource( 70.0, 70.0, 100.0, "You dig some Gold ore and put it in your backpack", typeof( GoldOre ),		typeof( GoldGranite ),			typeof( GoldenElemental ) ),
                    new HarvestResource( 72.5, 72.5, 100.0, "You dig some Ice ore and put it in your backpack", typeof( IceOre ),	typeof( IceGranite ) ),
                    new HarvestResource( 72.5, 72.5, 100.0, "You dig some Amethyst ore and put it in your backpack", typeof( AmethystOre ),	typeof( AmethystGranite ) ),
					new HarvestResource( 75.0, 75.0, 100.0, "You dig some Valorite ore and put it in your backpack", typeof( ValoriteOre ),		typeof( ValoriteGranite ),		typeof( ValoriteElemental ) ),
                    new HarvestResource( 75.0, 75.0, 100.0, "You dig some Blood Rock ore and put it in your backpack" , typeof( BloodRockOre ), typeof( BloodRockGranite ) ),
                    new HarvestResource( 80.0, 80.0, 100.0, "You dig some Aqua ore and put it in your backpack" , typeof( AquaOre ), typeof( AquaGranite ) ),
                    new HarvestResource( 82.0, 82.0, 100.0, "You dig some Fire ore and put it in your backpack", typeof( FireOre ), typeof(Granite) ),  //Fire
                    new HarvestResource( 87.5, 87.5, 100.0, "You dig some Mytheril ore and put it in your backpack" , typeof( MytherilOre ), typeof( MytherilGranite ) ),
                    new HarvestResource( 90.0, 90.0, 100.0, "You dig some Dwarven ore and put it in your backpack" , typeof( DwarvenOre ), typeof( DwarvenGranite ) ),
                    new HarvestResource( 92.5, 92.5, 100.0, "You dig some Black Diamond ore and put it in your backpack" , typeof( BlackDiamondOre ), typeof( Granite ) ),
                    new HarvestResource( 95, 95.0, 100.0, "You dig some Black Rock ore and put it in your backpack", typeof( BlackRockOre ),	typeof( Granite ) ),
                    new HarvestResource( 97.0, 97.0, 100.0, "You dig some Oceanic ore and put it in your backpack" , typeof( OceanicOre ), typeof( Granite ) ),
                    new HarvestResource( 99.0, 99.0, 100.0, "You dig some Daemon Steel ore and put it in your backpack" , typeof( DaemonSteelOre ), typeof( Granite ) ),
                    new HarvestResource( 99.5, 99.5, 100.0, "You dig some Reactive ore and put it in your backpack", typeof( ReactiveOre ),		typeof( Granite ) ),
                    new HarvestResource( 100.0, 100.0, 100.0, "You dig some Adamantium ore and put it in your backpack", typeof( AdamantiumOre ),		typeof( Granite ) ),
                    new HarvestResource( 100.0, 100.0, 100.0, "You dig some Sapphire ore and put it in your backpack", typeof( SapphireOre ),		typeof( Granite ) )
                    
				};

			veins = new HarvestVein[]
				{
					new HarvestVein( 48.1, 0.0, res[0], null   ), // Iron 
                    new HarvestVein( 06.5, 0.0, res[1], null   ), // OldCopper
                    new HarvestVein( 05.7, 0.0, res[2], null   ), // Shadow
                    new HarvestVein( 05.4, 0.0, res[3], null   ), // Silver
                    new HarvestVein( 04.8, 0.0, res[4], null   ), // Verite
                    new HarvestVein( 04.5, 0.0, res[5], null   ), // Rose
                    new HarvestVein( 04.2, 0.0, res[6], null   ), // Gold
                    //63.8

                    new HarvestVein( 03.1, 0.0, res[7], null   ), // Ice
                    new HarvestVein( 02.5, 0.0, res[8], null  ), // Amethyst
                    new HarvestVein( 02.3, 0.0, res[9], null   ), // Valorite
                    new HarvestVein( 02.1, 0.0, res[10], null   ), // Blood rock
                    new HarvestVein( 01.9, 0.0, res[11], null   ), // Aqua
                    new HarvestVein( 01.6, 0.0, res[12], null   ), // Fire
                    new HarvestVein( 01.6, 0.0, res[13], null   ), // Mytheril
                    //26.6

                    new HarvestVein( 01.5, 0.0, res[14], null   ), // Dwarven
                    new HarvestVein( 01.1, 0.0, res[15], null   ), // Black diamond
                    new HarvestVein( 00.9, 0.0, res[16], null   ), // Black Rock
                    new HarvestVein( 00.8, 0.0, res[17], null   ), // Oceanic
                    new HarvestVein( 00.5, 0.0, res[18], null   ), // Daemon steel
                    new HarvestVein( 00.4, 0.0, res[19], null   ), // Reactive 
                    new HarvestVein( 00.3, 0.0, res[20], null   ), // Adamantium
                    new HarvestVein( 00.2, 0.0, res[20], null   ), // Sapphire 
                    //9.6
				};
			
			oreAndStone.Resources = res;
			oreAndStone.Veins = veins;
            if (Core.ML)
            {
                oreAndStone.BonusResources = new BonusHarvestResource[]
				{
					new BonusHarvestResource( 0, 99.4, null, null ),	//Nothing
					new BonusHarvestResource( 100, .1, 1072562, typeof( BlueDiamond ) ),
					new BonusHarvestResource( 100, .1, 1072567, typeof( DarkSapphire ) ),
					new BonusHarvestResource( 100, .1, 1072570, typeof( EcruCitrine ) ),
					new BonusHarvestResource( 100, .1, 1072564, typeof( FireRuby ) ),
					new BonusHarvestResource( 100, .1, 1072566, typeof( PerfectEmerald ) ),
					new BonusHarvestResource( 100, .1, 1072568, typeof( Turquoise ) )
				};
            }

			oreAndStone.RaceBonus = Core.ML;
            oreAndStone.RandomizeVeins = Core.ML;

			Definitions.Add( oreAndStone );
			#endregion

			#region Mining for sand
			HarvestDefinition sand = m_Sand = new HarvestDefinition();

			// Resource banks are every 3x3 tiles
			sand.BankWidth = 3;
			sand.BankHeight = 3;

			// Every bank holds from 6 to 15 sand
			sand.MinTotal = 6;
			sand.MaxTotal = 12;

			// A resource bank will respawn its content every 10 to 20 minutes
			sand.MinRespawn = TimeSpan.FromMinutes( 10.0 );
			sand.MaxRespawn = TimeSpan.FromMinutes( 20.0 );

			// Skill checking is done on the Mining skill
			sand.Skill = SkillName.Mining;

			// Set the list of harvestable tiles
			sand.Tiles = m_SandTiles;

			// Players must be within 2 tiles to harvest
			sand.MaxRange = 2;

			// One sand per harvest action
			sand.ConsumedPerHarvest = 1;
			sand.ConsumedPerFeluccaHarvest = 1;

			// The digging effect
			sand.EffectActions = new int[]{ 11 };
			sand.EffectSounds = new int[]{ 0x125, 0x126 };
			sand.EffectCounts = new int[]{ 3 };
			sand.EffectDelay = TimeSpan.FromSeconds( 1.6 );
			sand.EffectSoundDelay = TimeSpan.FromSeconds( 0.9 );

			sand.NoResourcesMessage = 1044629; // There is no sand here to mine.
			sand.DoubleHarvestMessage = 1044629; // There is no sand here to mine.
			sand.TimedOutOfRangeMessage = 503041; // You have moved too far away to continue mining.
			sand.OutOfRangeMessage = 500446; // That is too far away.
			sand.FailMessage = 1044630; // You dig for a while but fail to find any of sufficient quality for glassblowing.
			sand.PackFullMessage = 1044632; // Your backpack can't hold the sand, and it is lost!
			sand.ToolBrokeMessage = 1044038; // You have worn out your tool!

			res = new HarvestResource[]
				{
					new HarvestResource( 100.0, 70.0, 100.0, 1044631, typeof( Sand ) ),
                    new HarvestResource( 100.0, 90.0, 100.0, "You dig some Sand Rock ore and put it in your backpack" , typeof( SandRockOre ))//, typeof( Granite ) )
				};

			veins = new HarvestVein[]
				{
					new HarvestVein( 95.0, 0.0, res[0], null ),
                    new HarvestVein( 5.0, 0.0, res[1], null   ) // Sand rock
				};

			sand.Resources = res;
			sand.Veins = veins;

			Definitions.Add( sand );
			#endregion
		}

		public override Type GetResourceType( Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, HarvestResource resource )
		{
			if ( def == m_OreAndStone )
			{
				PlayerMobile pm = from as PlayerMobile;
				if ( pm != null && pm.StoneMining && pm.ToggleMiningStone && from.Skills[SkillName.Mining].Base >= 100.0 && 0.1 > Utility.RandomDouble() )
					return resource.Types[1];

				return resource.Types[0];
			}

			return base.GetResourceType( from, tool, def, map, loc, resource );
		}

		public override bool CheckHarvest( Mobile from, Item tool )
		{
			if ( !base.CheckHarvest( from, tool ) )
				return false;

            /*if (from.IsBodyMod && !from.Body.IsHuman)
			{
				from.SendLocalizedMessage( 501865 ); // You can't mine while polymorphed.
				return false;
			}*/

			return true;
		}

		public override void SendSuccessTo( Mobile from, Item item, HarvestResource resource )
		{
			if ( item is BaseGranite )
				from.SendLocalizedMessage( 1044606 ); // You carefully extract some workable stone from the ore vein!
			else
				base.SendSuccessTo( from, item, resource );
		}

		public override bool CheckHarvest( Mobile from, Item tool, HarvestDefinition def, object toHarvest )
		{
			if ( !base.CheckHarvest( from, tool, def, toHarvest ) )
				return false;

			if ( def == m_Sand && !(from is PlayerMobile && from.Skills[SkillName.Mining].Base >= 100.0 && ((PlayerMobile)from).SandMining) )
			{
				OnBadHarvestTarget( from, tool, toHarvest );
				return false;
			}
			/*else if ( from.IsBodyMod && !from.Body.IsHuman )
			{
				from.SendLocalizedMessage( 501865 ); // You can't mine while polymorphed.
				return false;
			}*/

			return true;
		}

		public override HarvestVein MutateVein( Mobile from, Item tool, HarvestDefinition def, HarvestBank bank, object toHarvest, HarvestVein vein )
		{
			if ( tool is GargoylesPickaxe && def == m_OreAndStone )
			{
				int veinIndex = Array.IndexOf( def.Veins, vein );

				if ( veinIndex >= 0 && veinIndex < (def.Veins.Length - 1) )
					return def.Veins[veinIndex + 1];
			}

			return base.MutateVein( from, tool, def, bank, toHarvest, vein );
		}

		private static readonly int[] m_Offsets = new int[]
			{
				-1, -1,
				-1,  0,
				-1,  1,
				 0, -1,
				 0,  1,
				 1, -1,
				 1,  0,
				 1,  1
			};

		public override void OnHarvestFinished( Mobile from, Item tool, HarvestDefinition def, HarvestVein vein, HarvestBank bank, HarvestResource resource, object harvested )
		{
			if ( tool is GargoylesPickaxe && def == m_OreAndStone && 0.1 > Utility.RandomDouble() )
			{
				HarvestResource res = vein.PrimaryResource;

				if ( res == resource && res.Types.Length >= 3 )
				{
					try
					{
						Map map = from.Map;

						if ( map == null )
							return;

						BaseCreature spawned = Activator.CreateInstance( res.Types[2], new object[]{ 25 } ) as BaseCreature;

						if ( spawned != null )
						{
							int offset = Utility.Random( 8 ) * 2;

							for ( int i = 0; i < m_Offsets.Length; i += 2 )
							{
								int x = from.X + m_Offsets[(offset + i) % m_Offsets.Length];
								int y = from.Y + m_Offsets[(offset + i + 1) % m_Offsets.Length];

								if ( map.CanSpawnMobile( x, y, from.Z ) )
								{
                                    spawned.OnBeforeSpawn(new Point3D(x, y, from.Z), map);
									spawned.MoveToWorld( new Point3D( x, y, from.Z ), map );
									spawned.Combatant = from;
									return;
								}
								else
								{
									int z = map.GetAverageZ( x, y );

									if ( map.CanSpawnMobile( x, y, z ) )
									{
                                        spawned.OnBeforeSpawn(new Point3D(x, y, z), map);
										spawned.MoveToWorld( new Point3D( x, y, z ), map );
										spawned.Combatant = from;
										return;
									}
								}
							}
                            spawned.OnBeforeSpawn(from.Location, from.Map);
							spawned.MoveToWorld( from.Location, from.Map );
							spawned.Combatant = from;
						}
					}
					catch
					{
					}
				}
			}

            if (bank != null && bank.Vein != null && bank.Vein.PrimaryResource != null)
            {
                int bankAmount = bank.Current;
                double skillReq = bank.Vein.PrimaryResource.ReqSkill;

                if (skillReq >= 99.0 && bankAmount <= 13)
                    bank.Consume(def, bankAmount, from);
                else if (skillReq >= 98.0 && bankAmount <= 12)
                    bank.Consume(def, bankAmount, from);
                else if (skillReq >= 97.0 && bankAmount <= 10)
                    bank.Consume(def, bankAmount, from);
                else if (skillReq >= 90.0 && bankAmount <= 8)
                    bank.Consume(def, bankAmount, from);
                else if (skillReq >= 75.0 && bankAmount <= 6)
                    bank.Consume(def, bankAmount, from);
                else if (skillReq >= 65.0 && bankAmount <= 4)
                    bank.Consume(def, bankAmount, from);
            }
		}

		public override bool BeginHarvesting( Mobile from, Item tool )
		{
			if ( !base.BeginHarvesting( from, tool ) )
				return false;

			return true;
		}

        public override void OnHarvestStarted(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
        {
            base.OnHarvestStarted(from, tool, def, toHarvest);

            if (Core.ML)
                from.RevealingAction();
        }

		public override void OnBadHarvestTarget( Mobile from, Item tool, object toHarvest )
		{
			if ( toHarvest is LandTarget )
				from.SendLocalizedMessage( 501862 ); // You can't mine there.
			else
				from.SendLocalizedMessage( 501863 ); // You can't mine that.
		}

		#region Tile lists
		private static readonly int[] m_MountainAndCaveTiles = new int[]
			{
				220, 221, 222, 223, 224, 225, 226, 227, 228, 229,
				230, 231, 236, 237, 238, 239, 240, 241, 242, 243,
				244, 245, 246, 247, 252, 253, 254, 255, 256, 257,
				258, 259, 260, 261, 262, 263, 268, 269, 270, 271,
				272, 273, 274, 275, 276, 277, 278, 279, 286, 287,
				288, 289, 290, 291, 292, 293, 294, 296, 296, 297,
				321, 322, 323, 324, 467, 468, 469, 470, 471, 472,
				473, 474, 476, 477, 478, 479, 480, 481, 482, 483,
				484, 485, 486, 487, 492, 493, 494, 495, 543, 544,
				545, 546, 547, 548, 549, 550, 551, 552, 553, 554,
				555, 556, 557, 558, 559, 560, 561, 562, 563, 564,
				565, 566, 567, 568, 569, 570, 571, 572, 573, 574,
				575, 576, 577, 578, 579, 581, 582, 583, 584, 585,
				586, 587, 588, 589, 590, 591, 592, 593, 594, 595,
				596, 597, 598, 599, 600, 601, 610, 611, 612, 613,

				1010, 1741, 1742, 1743, 1744, 1745, 1746, 1747, 1748, 1749,
				1750, 1751, 1752, 1753, 1754, 1755, 1756, 1757, 1771, 1772,
				1773, 1774, 1775, 1776, 1777, 1778, 1779, 1780, 1781, 1782,
				1783, 1784, 1785, 1786, 1787, 1788, 1789, 1790, 1801, 1802,
				1803, 1804, 1805, 1806, 1807, 1808, 1809, 1811, 1812, 1813,
				1814, 1815, 1816, 1817, 1818, 1819, 1820, 1821, 1822, 1823,
				1824, 1831, 1832, 1833, 1834, 1835, 1836, 1837, 1838, 1839,
				1840, 1841, 1842, 1843, 1844, 1845, 1846, 1847, 1848, 1849,
				1850, 1851, 1852, 1853, 1854, 1861, 1862, 1863, 1864, 1865,
				1866, 1867, 1868, 1869, 1870, 1871, 1872, 1873, 1874, 1875,
				1876, 1877, 1878, 1879, 1880, 1881, 1882, 1883, 1884, 1981,
				1982, 1983, 1984, 1985, 1986, 1987, 1988, 1989, 1990, 1991,
				1992, 1993, 1994, 1995, 1996, 1997, 1998, 1999, 2000, 2001,
				2002, 2003, 2004, 2028, 2029, 2030, 2031, 2032, 2033, 2100,
				2101, 2102, 2103, 2104, 2105,

				0x453B, 0x453C, 0x453D, 0x453E, 0x453F, 0x4540, 0x4541,
				0x4542, 0x4543, 0x4544,	0x4545, 0x4546, 0x4547, 0x4548,
				0x4549, 0x454A, 0x454B, 0x454C, 0x454D, 0x454E,	0x454F
			};

		private static readonly int[] m_SandTiles = new int[]
			{
				22, 23, 24, 25, 26, 27, 28, 29, 30, 31,
				32, 33, 34, 35, 36, 37, 38, 39, 40, 41,
				42, 43, 44, 45, 46, 47, 48, 49, 50, 51,
				52, 53, 54, 55, 56, 57, 58, 59, 60, 61,
				62, 68, 69, 70, 71, 72, 73, 74, 75,

				286, 287, 288, 289, 290, 291, 292, 293, 294, 295,
				296, 297, 298, 299, 300, 301, 402, 424, 425, 426,
				427, 441, 442, 443, 444, 445, 446, 447, 448, 449,
				450, 451, 452, 453, 454, 455, 456, 457, 458, 459,
				460, 461, 462, 463, 464, 465, 642, 643, 644, 645,
				650, 651, 652, 653, 654, 655, 656, 657, 821, 822,
				823, 824, 825, 826, 827, 828, 833, 834, 835, 836,
				845, 846, 847, 848, 849, 850, 851, 852, 857, 858,
				859, 860, 951, 952, 953, 954, 955, 956, 957, 958,
				967, 968, 969, 970,

				1447, 1448, 1449, 1450, 1451, 1452, 1453, 1454, 1455,
				1456, 1457, 1458, 1611, 1612, 1613, 1614, 1615, 1616,
				1617, 1618, 1623, 1624, 1625, 1626, 1635, 1636, 1637,
				1638, 1639, 1640, 1641, 1642, 1647, 1648, 1649, 1650
			};
		#endregion
	}
}