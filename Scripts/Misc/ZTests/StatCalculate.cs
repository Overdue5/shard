using System;
using System.Collections.Generic;
using Server;
using Server.Misc;
using Server.Mobiles;

namespace Scripts.ZTest
{
#if DEBUG
	public static partial class StatCalculate
	{
		public static void SkillTests()
		{
			SkillCheck.m_StatGainDelay = TimeSpan.Zero;
			var result = new SortedDictionary<string, Dictionary<string, object>>();
			var p = MHelper.GetMobile<PlayerMobile>();
			int startValue = 0;
            var rnd = new Random(DateTime.UtcNow.Millisecond);
			for (int cap = 0; cap <= 5; cap++)
			{
				p.Skills.Cap = 7000;
				for (var i = 0; i < p.Skills.Length; i++)
				{
					MHelper.SetAllSkills(p, startValue);
					p.Str = 10;
					p.Dex = 10;
					p.Int = 10;
					int ok_count = 0;
					int fail_count = 0;
					var skill = p.Skills[i];
                    while (p.SkillsTotal < 1000 * cap)
                    {
                        int r = rnd.Next(p.Skills.Length);
                        while (r == i)
                            r = rnd.Next(p.Skills.Length);
                        p.Skills[r].Base = 100;
                    }

                    string name = $"{skill.Name}_{cap*1000}";
					result[name] = new Dictionary<string, object>();
					result[name]["SkillName"] = name;
					while (skill.Base < 100)
					{
						if (SkillCheck.Mobile_SkillCheckLocation(p, skill.SkillName, 
							Utility.LimitMinMax(0,/*skill.Base-20*/0, 100), 30))
							ok_count++;
						else
							fail_count++;
						if (Math.Truncate((skill.Base + startValue) / 10) > result[name].Count - 1)
						{
							result[name][$"{Convert.ToInt32(Math.Truncate(skill.Base))}"] =
								$"T{ok_count}:F{fail_count}:S{p.Str - 10}:D{p.Dex - 10}:I{p.Int - 10}";
						}
					}
				}
			}
			WHelper.ResultToCsv<string>(result, "Skills.csv");
		}

		public static void FightEmulate()
		{
			//var attacker = MHelper.GetMobile<PlayerMobile>("attacker");
			//var defender = MHelper.GetMobile<PlayerMobile>("defender");
			//MHelper.SetStats(attacker);
			//MHelper.SetStats(defender);
			//MHelper.SetAllSkills(attacker, 100);
			//MHelper.SetAllSkills(defender, 100);
			//MHelper.EquipPlateArmor(defender, CraftResource.Valorite, new HeaterShield());
			//attacker.AddItem(new VikingSword());
			//defender.AddItem(new VikingSword());
			//attacker.SwingCounter = 1000;
			//attacker.SwingState = 1;
			//attacker.Weapon.OnSwing(attacker, defender);
		}

		public static void SaveItemsInfo()
		{
			//WHelper.ResultToCsv<Type>(WHelper.GetPropertiesValues(WHelper.Weapons, WHelper.WeaponColumn), "Weapon.csv");
			//WHelper.ResultToCsv<Type>(WHelper.GetPropertiesValues(WHelper.Armors, WHelper.ArmorColumn), "Armors.csv");
			//WHelper.ResultToCsv<Type>(WHelper.GetPropertiesValues(WHelper.Mobiles, WHelper.MobileColumn), "Mobile.csv");
		}

		public static void StartTest()
		{
			//SaveItemsInfo();
			//FightEmulate();
			SkillTests();
		}

		[CallPriority(Int32.MaxValue)]
		public static void Initialize()
		{
#if DEBUG
			//SkillTests();
			//EventSink.AfterWorldLoad += StartTest;
			//EventSink.AfterWorldLoad += args => { WHelper.DeleteItems(WHelper.Weapons); };
			//EventSink.AfterWorldLoad += args => { WHelper.DeleteItems(WHelper.Armors); };
			//EventSink.AfterWorldLoad += args => { WHelper.DeleteItems(WHelper.Mobiles); };
#endif
			//Mobile m1 = new Mobile();
			//Mobile m2 = new Mobile();
		}

	}
#endif
}
