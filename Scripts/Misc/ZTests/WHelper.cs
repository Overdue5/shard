using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Scripts.ZTest
{
	public static class WHelper
	{
		#region Properties
		public static List<object> Weapons
		{
			get
			{
				if (m_Weapons == null)
				{
					m_Weapons = GetItems<BaseWeapon>();
					return m_Weapons;
				}
				else
				{
					return m_Weapons;
				}
			}
		}

		public static List<object> Armors
		{
			get
			{
				if (m_Armors == null)
				{
					m_Armors = GetItems<BaseArmor>();
					return m_Armors;
				}
				else
				{
					return m_Armors;
				}
			}
		}

		public static List<object> Mobiles
		{
			get
			{
				if (m_Mobiles == null)
				{
					m_Mobiles = GetItems<BaseCreature>();
					return m_Mobiles;
				}
				else
				{
					return m_Mobiles;
				}
			}
		}

		public static List<string> WeaponColumn
		{
			get
			{
				return ("LabelNumber,AsciiName,PrimaryAbility,SecondaryAbility,OldStrengthReq,OldMinDamage," +
						"OldMaxDamage,OldSpeed,InitMinHits,InitMaxHits,DefHitSound,DefMissSound," +
						"DefSkill,DefType,DefAnimation,StaffEffect,DefMaxRange,OldDexterityReq," +
						"OldIntelligenceReq,PhysicalResistance,FireResistance,ColdResistance,PoisonResistance," +
						"EnergyResistance,AccuracySkill,Hue,Weight,Layer,ItemID").ToLower().Split(',').ToList();
			}
		}

		public static List<string> ArmorColumn
		{
			get
			{
				return ("InitMinHits,InitMaxHits,LabelNumber,BaseColdResistance,BaseEnergyResistance,BasePhysicalResistance," +
						"BaseFireResistance,BasePoisonResistance,OldStrReq,ArmorBase,MaterialType,OldStrBonus,OldDexBonus," +
						"OldIntBonus,OldDexReq,OldIntReq,ArmorRating,StrBonus,DexBonus,IntBonus,StrRequirement," +
						"DexRequirement,IntRequirement,Resource,ArmorScalar,MaxHitPoints,HitPoints,PhysicalBonus," +
						"FireBonus,ColdBonus,PoisonBonus,EnergyBonus,PhysicalResistance,FireResistance,ColdResistance," +
						"PoisonResistance,EnergyResistance,BodyPosition," +
						"Hue,Weight,Layer,ItemID").ToLower().Split(',').ToList();
			}
		}

		public static List<string> MobileColumn
		{
			get
			{
				return ("CanTeach,ClickTitle,CanRegenHits,CanRegenStam,CanRegenMana,WeaponAbilityChance,BasePhysicalResistance," +
						"BaseFireResistance,BaseColdResistance,BasePoisonResistance,BaseEnergyResistance,PhysicalDamage," +
						"FireDamage,ColdDamage,PoisonDamage,EnergyDamage,FavoriteFood,AllowMaleTamer,AllowFemaleTamer," +
						"HitPoisonChance,DispelFocus,DamageMin,DamageMax,HitsMax,StamMax,ManaMax,CanOpenDoors,AI," +
						"FightMode,RangePerception,RangeFight,RangeHome,ActiveSpeed,PassiveSpeed,Tamable," +
						"MaxWeight,Skills,Fame,Karma,Blessed,Str,Dex,Int,Hits,Stam,Mana,Luck,Body,BodyValue," +
						"ShieldArmor,NeckArmor,HandArmor,HeadArmor,ArmsArmor,LegsArmor,ChestArmor,Talisman").ToLower().Split(',').ToList();
			}
		}

		public static List<string> IgnoreListEnums => "None,Invalid".ToLower().Split(',').ToList();

		#endregion

		#region Fields
		private static List<object> m_Weapons;
		private static List<object> m_Armors;
		private static List<object> m_Mobiles;
		#endregion

		#region Public Methods
		public static void ResultToCsv<T>(SortedDictionary<T, Dictionary<string, object>> data, string fileName)
		{
			string result = "";
			List<string> columnNames = new List<string> { };
			foreach (var rType in data.Keys)
			{
				columnNames = columnNames.Union(data[rType].Keys).ToList();
			}

			result += String.Join(",", columnNames.ToArray()) + "\n";
			foreach (var rType in data.Keys)
			{
				foreach (var column in columnNames)
				{
					if (data[rType].Keys.Contains(column))
					{
						result += $"{data[rType][column]},";
					}
					else
					{
						result += "-,";
					}
				}

				result = result.Remove(result.Length - 1);
				result += "\n";
			}

			File.WriteAllText(fileName, result);

		}

		public static List<object> GetItems<ClassType>()
		{
			var class_types = ScriptCompiler.FindTypeByBaseClass(typeof(ClassType));
			class_types.Add(typeof(ClassType));
			//var result = new Dictionary<ClassType, Dictionary<string, object>> { };
			var result = new List<object> { };
			foreach (var type in class_types)
			{
				ConstructorInfo[] ctors = type.GetConstructors();
				foreach (var ctor in ctors)
				{
					ParameterInfo[] paramList = ctor.GetParameters();

					if (0 == paramList.Length)
					{
						object item = ctor.Invoke(null);
						result.Add(item);
					}
					else if (1 == paramList.Length)
					{
						if (paramList[0].ParameterType.IsEnum)
						{
							foreach (var en in paramList[0].ParameterType.GetEnumValues())
							{
								if (!IgnoreListEnums.Contains(en.ToString().ToLower()))
									result.Add(ctor.Invoke(new Object[] { en }));
							}
						}
					}

				}
			}
			return result;
		}

		public static void DeleteItems(List<object> list)
		{
			if (list == null) return;
			foreach (var item in list)
			{
				(item as Item)?.Delete();
				(item as Mobile)?.Delete();
			}
		}

		public static string UpdatePropValue(object item, PropertyInfo prop)
		{
			string result = $"{prop.GetValue(item)}";
			if (prop.Name == "Skills")
			{
				var skills = prop.GetValue(item) as Skills;
				result = "";
				for (var i = 0; i < skills.Length; i++)
				{
					if (skills[i].Base > 0)
					{
						result += $"{skills[i].Name}:{skills[i].Base};";
					}
				}
			}
			else if (item is BaseArmor)
			{
				result = "";
				BaseArmor armor = item as BaseArmor;
				result += $"{armor.Name}:{armor.Hue};";
			}

			return result;
		}

		public static SortedDictionary<Type, Dictionary<string, object>> GetPropertiesValues(List<object> list, List<string> columnName)
		{
			var result = new SortedDictionary<Type, Dictionary<string, object>> { };
			foreach (var item in list)
			{
				Type type = item.GetType();
				PropertyInfo[] props = type.GetProperties();
				result[type] = new Dictionary<string, object> { };
				string str = "";
				foreach (var prop in props)
				{
					str += $"{prop.Name}:{prop.GetValue(item)},";
					if (columnName.Contains(prop.Name.ToLower()))
					{
						result[type][prop.Name] = UpdatePropValue(item, prop);
					}
				}
			}
			return result;
		}
		#endregion
	}
}
