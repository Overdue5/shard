using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;


namespace Scripts.ZTest
{
	public static partial class MHelper
	{
		#region Public Methods

		public static Mobile GetMobile(string name = null)
		{
			var result = new ZDummy();
			if (name != null)
				result.Name = name;
			WHelper.Mobiles.Add(result);
			return result;
		}

		public static T GetMobile<T>(string name = null) where T : Mobile, new()
		{
			var result = new T();
			if (name != null)
				result.Name = name;
			WHelper.Mobiles.Add(result);
			return result;
		}

		public static void SetStats(Mobile mob, int str = 100, int dex = 100, int intel = 100, int hits = 100, int stam = 100, int mana = 100)
		{
			mob.Str = str; mob.Dex = dex; mob.Int = intel;
			mob.Hits = hits; mob.Stam = stam; mob.Mana = mana;
		}

		public static void SetAllSkills(Mobile mob, float value)
		{
			for (int i = 0; i < mob.Skills.Length; ++i)
				mob.Skills[i].Base = value;
		}

		public static void EquipArmor(Mobile mob, List<BaseArmor> armors)
		{
			foreach (var armor in armors)
			{
				var item = mob.FindItemOnLayer(armor.Layer);
				item?.Delete();
				mob.AddItem(armor);
			}
		}

		//public static void EquipPlateArmor(Mobile mob, CraftResource resource, BaseArmor shield = null)
		//{
		//    //var armors = new List<BaseArmor> {new PlateArms(), new PlateChest(), new PlateGloves(),
		//    //    new PlateGorget(), new PlateHelm(), new PlateLegs()};
		//    //if (shield != null)
		//    //    armors.Add(shield);
		//    //foreach (var armor in armors)
		//    //{
		//    //    armor.Resource = resource;
		//    //}

		//    //EquipArmor(mob, armors);
		//}

		#endregion
	}
}
