using System;
using System.Collections.Generic;
using System.Reflection;
using Scripts.SpecialSystems;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	public class PvMRewardStone : PvXRewardStone
	{
		#region Public Constructors

		[Constructable]
		public PvMRewardStone() : base()
		{
			Hue = 1154;
			rewardType = PvXType.PVM;
		}

		public PvMRewardStone(Serial serial) : base(serial)
		{
		}

		#endregion Public Constructors

		#region Public Methods

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			rewardType = PvXType.PVM;
			int version = reader.ReadInt();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0); // version
		}

		#endregion Public Methods
	}

	public class PvPRewardStone : PvXRewardStone
	{
		#region Public Constructors

		[Constructable]
		public PvPRewardStone() : base()
		{
			Hue = 1154;
			rewardType = PvXType.PVP;
		}

		public PvPRewardStone(Serial serial) : base(serial)
		{
		}

		#endregion Public Constructors

		#region Public Methods

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			rewardType = PvXType.PVP;
			int version = reader.ReadInt();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version
		}

		#endregion Public Methods
	}

	public class PvXRewardGump : Gump
	{
		#region Private Fields

		private PlayerMobile m_mob;
		private PvXType m_xtype;

		#endregion Private Fields

		#region Public Constructors

		public PvXRewardGump(PlayerMobile mob, PvXType xtype) : base(100, 100)
		{
			m_mob = mob;
			m_xtype = xtype;
			Activate();
		}

		#endregion Public Constructors

		#region Public Methods

		public void Activate()
		{
			Dragable = true;
			Closable = true;
			Resizable = false;
			Disposable = false;
			AddPage(0);
			int count = PvXRewardStone.RewardsDict[m_xtype].Count;
			AddBackground(150, 300, 400, 100 + 20 * count, 9200);
			AddLabel(250, 310, 0, $"You can spend {PvXData.GetUnSpendPoints(m_xtype, m_mob)} {m_xtype.ToString()} points");
			count = 0;
			foreach (var rI in PvXRewardStone.RewardsDict[m_xtype])
			{
				string name = $"{rI.RewardName}, Price:{rI.RewardCost}";
				if (rI.RewardCount > 0)
					name = $"{rI.RewardCount} {name}";
				AddLabel(200, 340 + 20 * count, 0, name);
				AddButton(180, 340 + 20 * count, 216, 216, count + 1, GumpButtonType.Reply, 0);
				count++;
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			switch (info.ButtonID)
			{
				case 0: // Closed or Cancel
					{
						return;
					}
				default:
					{
						var infoR = PvXRewardStone.RewardsDict[m_xtype][info.ButtonID - 1];
						var total = PvXData.GetUnSpendPoints(m_xtype, m_mob);
						if (infoR.RewardCost <= total && PvXData.GetPvXData(m_xtype).ContainsKey(m_mob.Serial.Value))
						{
							ConstructorInfo[] ctors = infoR.RewardType.GetConstructors();
							object obj = null;
							foreach (var ctor in ctors)
							{
								ParameterInfo[] paramList = ctor.GetParameters();
								if (0 == paramList.Length && infoR.RewardCount == 0)
								{
									obj = ctor.Invoke(null);
								}
								else if (1 == paramList.Length && infoR.RewardCount > 0)
								{
									obj = ctor.Invoke(new Object[] { infoR.RewardCount });
								}

								if (obj != null)
								{
									break;
								}
							}

							if (obj == null)
							{
								m_mob.SendMessage($"The gods did not answer your request.");
								Logs.PvXLog.WriteLine(m_mob, $"Error get:{infoR}");
								Utility.ConsoleWriteLine(Utility.ConsoleMsgType.Error,
									$"Error create item:{infoR} for player:{m_mob}");
								return;
							}

							Item i = (Item)obj;
							if (infoR.RewardHue > 0)
							{
								i.Hue = infoR.RewardHue;
							}

							var origValue = PvXData.GetPvXData(m_xtype)[m_mob.Serial.Value];

							try
							{
								PvXData.GetPvXData(m_xtype)[m_mob.Serial.Value].TotalPointsSpent += infoR.RewardCost;
								if (!i.DropToItem(m_mob, m_mob.Backpack, new Point3D(-1, -1, 0)))
								{
									i.Delete();
									PvXData.GetPvXData(m_xtype)[m_mob.Serial.Value] = origValue;
								}
							}
							catch (Exception e)
							{
								Logs.PvXLog.WriteLine(m_mob, $"Error get rewards {infoR}. Ex:{e.Message}");
								i.Delete();
								PvXData.GetPvXData(m_xtype)[m_mob.Serial.Value] = origValue;
							}
						}
						else
						{
							m_mob.SendMessage($"You have only {total} {m_xtype.ToString()} points");
							Logs.PvXLog.WriteLine(m_mob, $"Try to spend {total} {m_xtype.ToString()} points.");
						}
					}
					return;
			}
		}

		#endregion Public Methods
	}

	public abstract class PvXRewardStone : Item
	{
		#region Public Fields

		public static Dictionary<PvXType, List<RewardsInfo>> RewardsDict = new Dictionary<PvXType, List<RewardsInfo>>();
		public PvXType rewardType;

		#endregion Public Fields

		#region Public Constructors

		public PvXRewardStone() : base(3804)
		{
			Name = $"{rewardType.ToString()} Reward Terminal";
			Movable = false;
		}

		public PvXRewardStone(Serial serial) : base(serial)
		{
		}

		#endregion Public Constructors

		#region Public Methods

		public static void Initialize()
		{
			ReadConfig();
		}

		public static void ReadConfig()
		{
			foreach (var pvx in (PvXType[])Enum.GetValues(typeof(PvXType)))
			{
				RewardsDict[pvx] = new List<RewardsInfo>();
				string value = null;
				int count = 1;
				while (value != "")
				{
					value = Config.Get($"PvXsystem.{pvx}Rewards{count++}", "");
					try
					{
                        if (value != "")
                        {
                            var r = new RewardsInfo(value);
                            if (r.RewardType == null)
                                throw new Exception($"Error parse {value}, wrong type name");
                            RewardsDict[pvx].Add(r);
                        }
                    }
					catch
					{
						Utility.ConsoleWriteLine(Utility.ConsoleMsgType.Error,
							$"Error parse {value} for {pvx.ToString()} in PvXRewardStone, ignoring");
					}
				}
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (from is PlayerMobile && from.Alive && from.InRange(this.GetWorldLocation(), 2))
			{
				from.RevealingAction();
				from.CloseGump(typeof(PvXRewardGump));
				from.SendGump(new PvXRewardGump((PlayerMobile)from, rewardType));
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version
		}

		#endregion Public Methods
	}

	public class RewardsInfo
	{
		#region Public Fields

		public int RewardCost;
		public int RewardCount;
		public int RewardHue;
		public string RewardName;
		public Type RewardType;

		#endregion Public Fields

		#region Public Constructors

		public RewardsInfo(string value)
		{
			var values = value.Split(':');
			RewardType = ScriptCompiler.FindTypeByName(values[0], ignoreCase: true);
			RewardName = values[1];
			RewardCount = Math.Abs(Convert.ToInt32(values[2]));
			RewardHue = Math.Abs(Convert.ToInt32(values[3]));
			RewardCost = Math.Abs(Convert.ToInt32(values[4]));
		}

		#endregion Public Constructors

		#region Public Methods

		public override string ToString()
		{
			return $"Type:{RewardType};Name:{RewardName};Hue:{RewardHue};Count:{RewardCount};Price:{RewardCost}";
		}

		#endregion Public Methods
	}
}