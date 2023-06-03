using System;
using System.Collections.Generic;
using System.Linq;
using Server.Gumps;
using Server.Logging;
using Server.Network;

namespace Server.Commands
{
	public class GMItems
	{
		public static void Initialize()
		{
			CommandSystem.Register("gmitems", AccessLevel.Owner, new CommandEventHandler(GMItems_OnCommand));
		}

		[Usage("gmitems")]
		[Description("Remove/find items")]
		private static void GMItems_OnCommand(CommandEventArgs e)
		{
			e.Mobile.SendGump(new GMItemsGump(e.Mobile));
		}
	}

	public class GMItemsGump : Gump
	{
		public const int TabSize = 25;
		public GumpInfo Info;
		public Mobile mobile;

		public GMItemsGump(Mobile mob) : base(100, 100)
		{
			mobile = mob;
			Info = new GumpInfo();
			Info.IsMobiles = false;
			Info.IsItems = false;
			Info.MapsCheckBox = new Dictionary<string, bool>();
			Info.ListItems = new List<Item>();
			Info.ListMobiles = new List<Mobile>();
			Info.PrintId = 0;
			Info.InitMaps();
			Activate();
		}

		public GMItemsGump(Mobile mob, GumpInfo info) : base(100, 100)
		{
			mobile = mob;
			Info = info;
			Activate();
		}

		public void PrintMobiles()
		{
			AddLabel(190, 35, 0, @"Serial");
			AddLabel(290, 35, 0, @"Name");
			AddLabel(393, 35, 0, @"Type");
			AddLabel(485, 35, 0, @"Location");
			var mob = Info.ListMobiles.ToArray();
			AddLabel(200, 10, 0, $"Total found:{mob.Length}");
			var count = Info.PrintId + TabSize;
			if (count > mob.Length)
				count = mob.Length;
			for (int i = Info.PrintId; i < count; i++)
			{
				var offset = 60 + 20 * (i - Info.PrintId);
				AddImageTiled(170, offset, 80, 20, 3004);
				AddImageTiled(270, offset, 80, 20, 3004);
				AddImageTiled(370, offset, 80, 20, 3004);
				AddImageTiled(470, offset, 80, 20, 3004);
				AddImageTiled(570, offset, 250, 20, 3004);
				AddLabel(175, offset + 5, 0, Utility.LineCut(mob[i].Serial.Value.ToString("X"), 10));
				AddLabel(275, offset + 5, 0, Utility.LineCut(mob[i].Name, 10));
				AddLabel(375, offset + 5, 0, Utility.LineCut(mob[i].GetType().Name, 10));
				AddLabel(475, offset + 5, 0, PrintMapName(mob[i]));
				AddButton(770, offset, 4005, 4005, 1000 + i, GumpButtonType.Reply, 0);
				AddButton(740, offset + 5, 4002, 4003, 100000 + i, GumpButtonType.Reply, 0);
			}
		}

		public void PrintItems()
		{
			AddLabel(190, 35, 0, @"Serial");
			AddLabel(290, 35, 0, @"Name");
			AddLabel(393, 35, 0, @"Type");
			AddLabel(485, 35, 0, @"Location");
			var items = Info.ListItems.ToArray();
			var count = Info.PrintId + TabSize;
			AddLabel(200, 10, 0, $"Total found:{items.Length}");
			if (count > items.Length)
				count = items.Length;
			for (int i = Info.PrintId; i < count; i++)
			{
				var offset = 60 + 20 * (i - Info.PrintId);
				AddImageTiled(170, offset, 80, 20, 3004);
				AddImageTiled(270, offset, 80, 20, 3004);
				AddImageTiled(370, offset, 80, 20, 3004);
				AddImageTiled(470, offset, 80, 20, 3004);
				AddImageTiled(570, offset, 250, 20, 3004);
				//AddImage(120, offset - 20, items[i].ItemID);
				//AddAlphaRegion(170, offset, 80, 20);
				//AddAlphaRegion(270, offset, 80, 20);
				//AddAlphaRegion(370, offset, 80, 20);
				//AddAlphaRegion(470, offset, 80, 20);
				AddLabel(175, offset + 5, 0, items[i].Serial.Value.ToString("X"));
				AddLabel(275, offset + 5, 0, Utility.LineCut(items[i].CalculateName(), 10));
				AddLabel(375, offset + 5, 0, Utility.LineCut(items[i].GetType().Name, 10));
				AddLabel(475, offset + 5, 0, PrintMapName(items[i]));
				AddButton(770, offset + 5, 4005, 4005, 1000 + i, GumpButtonType.Reply, 0);
				AddButton(740, offset + 5, 4002, 4003, 100000 + i, GumpButtonType.Reply, 0);
			}
		}

		public string PrintMapName(Item item, int len = 10)
		{
			if (item.Map != null)
				return Utility.LineCut(item.Map.Name, len);
			return "None";
		}

		public string PrintMapName(Mobile item, int len = 10)
		{
			if (item.Map != null)
				return Utility.LineCut(item.Map.Name, len);
			return "None";
		}

		public void Activate()
		{
			Dragable = true;
			Closable = true;
			Resizable = true;
			Disposable = false;
			AddPage(0);
			AddBackground(7, 6, 923, 600, 0x13BE);
			AddCheck(36, 36, 210, 211, Info.IsMobiles, 0);
			AddCheck(36, 66, 210, 211, Info.IsItems, 1);
			var offset = 0;
			foreach (var map in Info.MapsCheckBox)
			{
				AddLabel(820, 20 + offset * 20, 0, $"{map.Key}");
				AddCheck(890, 20 + offset * 20, 210, 211, map.Value, 10 + offset);
				offset++;
			}

			AddLabel(65, 36, 0, @"Mobiles");
			AddLabel(63, 66, 0, @"Items");
			//AddAlphaRegion(170, 60, 650, 20);

			if (Info.IsMobiles)
			{
				PrintMobiles();
				if (Info.PrintId < Info.ListMobiles.Count)
					AddButton(301, 570, 5224, 5224, 102, GumpButtonType.Reply, 0); //Next
			}
			else
			{
				PrintItems();
				if (Info.PrintId < Info.ListItems.Count)
					AddButton(301, 570, 5224, 5224, 102, GumpButtonType.Reply, 0); //Next
			}

			AddButton(856, 570, 247, 248, 100, GumpButtonType.Reply, 0); //Generate Info
			if (Info.PrintId >= TabSize)
				AddButton(176, 570, 5223, 5223, 101, GumpButtonType.Reply, 0); //Previous

			AddLabel(350, 570, 0, @"Request");
			AddTextEntry(420, 570, 200, 20, 0, 0, Info.LastRequest);
			AddButton(630, 570, 247, 248, 105, GumpButtonType.Reply, 0);

			//AddAlphaRegion(18, 150, 860, 20);
		}

		public void UpdateFlags(int[] flags)
		{
			foreach (var flag in flags)
			{
				switch (flag)
				{
					case 0:
						{
							Info.IsMobiles = true;
							Info.IsItems = false;
							continue;
						}

					case 1:
						{
							Info.IsItems = true;
							Info.IsMobiles = false;
							continue;
						}
					case int n when (n >= 10 && n <= 16):
						{
							Info.UpdateMapCheck(flag);
							continue;
						}
				}
			}
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			Mobile from = state.Mobile;

			switch (info.ButtonID)
			{
				case 0: // Closed or Cancel
					{
						return;
					}
				default:
					{
						if (info.ButtonID == 100)
						{
							Info.ResetFlags();
							UpdateFlags(info.Switches);
							Info.Update();
							Info.PrintId = 0;
							mobile.SendGump(new GMItemsGump(mobile, Info));
						}

						if (info.ButtonID == 101)
						{
							Info.PrintId -= TabSize;
							if (Info.PrintId < 0)
								Info.PrintId = 0;
							mobile.SendGump(new GMItemsGump(mobile, Info));
						}

						if (info.ButtonID == 102)
						{
							Info.PrintId += TabSize;
							mobile.SendGump(new GMItemsGump(mobile, Info));
						}

						if (info.ButtonID == 105)
						{
							Info.LastRequest += info.TextEntries[0].Text;
							mobile.SendGump(new GMItemsGump(mobile, Info));
						}

						if (info.ButtonID >= 1000 && info.ButtonID <= 99999)
						{
							if (Info.IsMobiles)
							{
								var mob = Info.ListMobiles.ToArray()[info.ButtonID - 1000];
								mobile.MoveToWorld(mob.Location, mob.Map);
							}
							else
							{
								var item = Info.ListItems.ToArray()[info.ButtonID - 1000];
								mobile.MoveToWorld(Utility.GetWorldObjLocation(item), item.Map);
							}

							mobile.SendGump(new GMItemsGump(mobile, Info));
						}

						if (info.ButtonID >= 100000)
						{
							IEntity ent;
							ent = World.FindEntity(Info.IsMobiles
								? Info.ListMobiles.ToArray()[info.ButtonID - 100000].Serial.Value
								: Info.ListItems.ToArray()[info.ButtonID - 100000].Serial.Value);
							mobile.SendGump(new PropertiesGump(mobile, ent));
							mobile.SendGump(new GMItemsGump(mobile, Info));
						}

						break;
					}
			}
		}
	}

	public class GumpInfo
	{
		public const string AllMaps = "All";

		public Dictionary<string, bool> MapsCheckBox;
		public bool IsMobiles, IsItems;
		public List<Item> ListItems;
		public List<Mobile> ListMobiles;
		public int PrintId;
		public string LastRequest = "";

		public bool IsAnyMap => MapsCheckBox.ContainsKey(AllMaps) && MapsCheckBox[AllMaps];

        public void ResetFlags()
		{
			IsMobiles = false;
			IsItems = false;
			//LastRequest = "";
			foreach (var key in MapsCheckBox.Keys.ToArray())
			{
				MapsCheckBox[key] = false;
			}
		}

		public void InitMaps()
		{
			foreach (var map in Map.Maps)
			{
				if (map != null)
					MapsCheckBox[map.Name] = false;
				else
					break;
			}

			MapsCheckBox["All"] = false;
		}

		public void UpdateMapCheck(int id)
		{
			int count = 0;
			foreach (var map in MapsCheckBox)
			{
				if (count + 10 == id)
				{
					MapsCheckBox[map.Key] = true;
					return;
				}

				count++;
			}
		}

		public void Update()
		{
			if (IsMobiles)
				UpdateMobilesList();
			else
				UpdateItemList();
		}

		public void UpdateMobilesList()
		{
			ListMobiles = new List<Mobile>();
			foreach (var mobile in World.Mobiles.Values)
			{
				if (mobile.Deleted || mobile.Map == null)
					continue;

				if (IsAnyMap || (MapsCheckBox.Keys.Contains(mobile.Map.Name) && MapsCheckBox[mobile.Map.Name]))
				{
					if (!String.IsNullOrEmpty(LastRequest.Trim()))
					{
						var type = ScriptCompiler.FindTypeByName(LastRequest.Trim(), true);
						if (type != null && (Utility.IsBaseType(mobile.GetType(), type)|| mobile.GetType() == type))
						{
							ListMobiles.Add(mobile);
						}

					}
					else
						ListMobiles.Add(mobile);
				}
			}
		}

		public void UpdateItemList()
		{
			ListItems = new List<Item>();
			foreach (var item in World.Items.Values)
			{
				try
				{
					if (item.Deleted || item.Map == null)
						continue;

					if (IsAnyMap || (MapsCheckBox.Keys.Contains(item.Map.Name) && MapsCheckBox[item.Map.Name]))
					{
						if (!String.IsNullOrEmpty(LastRequest.Trim()))
						{
							var type = ScriptCompiler.FindTypeByName(LastRequest.Trim(), true);
							if (type != null && Utility.IsBaseType(item.GetType(), type))
							{
								ListItems.Add(item);
							}

						}
						else
							ListItems.Add(item);
					}
				}
				catch (Exception e)
				{
					ConsoleLog.Write.Error(e.Message);
				}
			}
		}
	}

}