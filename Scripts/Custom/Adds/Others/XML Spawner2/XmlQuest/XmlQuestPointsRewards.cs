using System;
using System.Collections;
using Server.Items;
using Server.Mobiles;

/*
** XmlQuestPointsRewards
** ArteGordon
** updated 9/18/05
**
** this class lets you specify rewards that can be purchased for XmlQuestPoints quest Credits.
** The items will be displayed in the QuestPointsRewardGump that is opened by the QuestPointsRewardStone
*/

namespace Server.Engines.XmlSpawner2
{
    public class XmlQuestPointsRewards
    {
        public int Cost;       // cost of the reward in credits
        public Type  RewardType;   // this will be used to create an instance of the reward
        public string Name;         // used to describe the reward in the gump
        public int ItemID;     // used for display purposes
        public object [] RewardArgs; // arguments passed to the reward constructor
        public int MinPoints;   // the minimum points requirement for the reward

        private static readonly ArrayList    PointsRewardList = new ArrayList();
        
        public static ArrayList RewardsList => PointsRewardList;

        public XmlQuestPointsRewards(int minpoints, Type reward, string name, int cost, int id, object[] args)
        {
            RewardType = reward;
            Cost = cost;
            ItemID = id;
            Name = name;
            RewardArgs = args;
            MinPoints = minpoints;
        }
        
        public static void Initialize()
        {
            // these are items as rewards. Note that the args list must match a constructor for the reward type specified.
            PointsRewardList.Add( new XmlQuestPointsRewards( 1000, typeof(PowerScroll), "105 Smithing powerscroll", 1000, 0x14F0, new object[] { SkillName.Blacksmith, 105 }));
            PointsRewardList.Add( new XmlQuestPointsRewards( 2000, typeof(PowerScroll), "110 Smithing powerscroll", 2000, 0x14F0, new object[] { SkillName.Blacksmith, 110 }));
            PointsRewardList.Add( new XmlQuestPointsRewards( 4000, typeof(PowerScroll), "115 Smithing powerscroll", 4000, 0x14F0, new object[] { SkillName.Blacksmith, 115 }));
            PointsRewardList.Add( new XmlQuestPointsRewards( 500, typeof(AncientSmithyHammer), "+20 Ancient Smithy Hammer, 50 uses", 500, 0x13E4, new object[] { 20, 50 }));
            PointsRewardList.Add( new XmlQuestPointsRewards( 200, typeof(ColoredAnvil), "Colored Anvil", 400, 0xFAF, null ));
            PointsRewardList.Add( new XmlQuestPointsRewards( 100, typeof(PowderOfTemperament), "Powder Of Temperament, 10 uses", 300, 4102, new object[] { 10 }));
            PointsRewardList.Add( new XmlQuestPointsRewards( 100, typeof(LeatherGlovesOfMining), "+20 Leather Gloves Of Mining", 200, 0x13c6, new object[] { 20 }));

            // this is an example of adding a mobile as a reward
            PointsRewardList.Add( new XmlQuestPointsRewards( 0, typeof(Llama),"Llama", 1, 0x20f6, null));

            // this is an example of adding an attachment as a reward
            //PointsRewardList.Add( new XmlQuestPointsRewards( 0, typeof(XmlEnemyMastery), "+200% Balron Mastery for 1 day", 2, 0, new object[] { "Balron", 50, 200, 1440.0 }));
            //PointsRewardList.Add( new XmlQuestPointsRewards( 0, typeof(XmlStr), "+20 Strength for 1 day", 10, 0, new object[] { 20, 86400.0 }));
        }

    }
}
