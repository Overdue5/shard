using Server.ContextMenus;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Server.Engines;
using Server.Gumps;
using Server.Network;

namespace Server.Custom
{
    public class VendorParcel : Item
    {
        private BaseVendor m_Source;
        private BaseVendor m_Recipient;
        private WorldDateTime m_BeginTime;
        private int m_Debt;

        public VendorParcel(BaseVendor source, BaseVendor recipient, int debt = 0):base(0x2DF3)
        {
            Name = $"Parcel to {recipient.Name} from {recipient.Region.Name}";
            m_Source = source;
            m_Recipient = recipient;
            m_Debt = debt;
            Weight = 100;
            m_BeginTime = WorldDateTime.Now;
        }

        public VendorParcel(Serial serial):base(serial)
        {
        }

        public override bool OnDroppedToMobile(Mobile from, Mobile target)
        {
            if (target.Serial != m_Recipient.Serial) return false;
            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }
    }

    public class VendorParcelText
    {
        private static string m_playerName = "$Player$";
        private static string m_RecipName = "$Trader2$";
        private static string m_RecipCity = "$CityT2$";
        private static string m_Gold = "$Gold$";

        public static List<string> LowHonestyMessages;
        public static List<string> NoRecallMessages;
        public static Dictionary<int, VendorParcelText> VendorTexts;
        public string Quest;
        public string After;

        public static void Initialize()
        {
            LowHonestyMessages = new List<string>();
            NoRecallMessages = new List<string>();
            LowHonestyMessages.Add("While your presence in our realm is renowned, your virtues in our world remain unknown. " +
                                   "To ensure the security of the entrusted package, I humbly request a deposit of $Gold$ gp. " +
                                   "It shall serve as a testament to your commitment and dedication. Fret not, for upon the safe delivery of the package," +
                                   " the deposit shall be returned in its entirety. May this be the first step in forging " +
                                   "a bond of trust between us, where your honor shines brighter than the sun's rays.");
            LowHonestyMessages.Add("As your avatar graces our realm, we embark on a journey together, " +
                                   "yet your virtues are yet to echo in our lands. In light of this uncertainty, " +
                                   "I beseech thee to provide a deposit of $gold$ gp, safeguarding the precious cargo's path. " +
                                   "Worry not, for upon the triumphant delivery, the deposit shall return to thee like a phoenix reborn. " +
                                   "May this act cement the foundations of trust, where thy virtuous aura illuminates our shared path, " +
                                   "and prosperity dances in the wake of thy valorous steps");
            LowHonestyMessages.Add("Though your presence graces our realm, thy virtues remain but whispers on the wind. " +
                                   "To ensure the safety of my cherished wares, I request a modest deposit of $Gold$ gp, a token of trust. " +
                                   "Fear not, for upon the successful delivery, the deposit shall return to thy possession like a treasure reclaimed. " +
                                   "Let this transaction be the crucible where thy virtues shine, forging a bond of trust that withstands the tests of time. " +
                                   "May the virtues guide thee on this quest, noble Avatar!");
            LowHonestyMessages.Add("As your name echoes through our world, your virtues have yet to be inscribed upon our annals. " +
                                   "To ensure the sanctity of my precious cargo, I beseech thee to provide a deposit of $Gold$ gp, a shield against uncertainty. " +
                                   "Fret not, for upon the triumphant delivery, the deposit shall return to thee like a phoenix reborn. " +
                                   "May this pact forge the bedrock of trust, where thy virtuous deeds resound and riches flow like a river of destiny. " +
                                   "Thy honor shall shine brighter than the sun itself!");
            LowHonestyMessages.Add("Ыщ афк in this realm, thy virtues are but tales untold. " +
                                   "To safeguard the parcel's voyage, I implore thee to grant a humble deposit of $Gold$ gp, a beacon of assurance. " +
                                   "Fear not, for when the package reaches its rightful destination, the deposit shall be returned, a testament to thy honor. " +
                                   "May this endeavor ignite a flame of trust, where thy virtuous path illuminates our world, and fortune embraces thee as a trusted ally. " +
                                   "Onward, valiant Avatar, for thy destiny awaits");

            NoRecallMessages.Add("$Player$, I implore thee, for the precious cargo's fragility and value, abstain from employing magical travel, lest its essence be marred");
            NoRecallMessages.Add("Noble $Player$, the cargo's fragility and worth transcend measure; I beseech thee, forsake the allure of magical transport, for its delicate nature shall not withstand such arcane forces.");
            NoRecallMessages.Add("Honorable $Player$! The cargo's fragility and value defy words. I entreat thee, refrain from wielding the magic of travel, for its gentle essence would succumb to its unpredictable currents.");
            NoRecallMessages.Add("Wise $Player$, the cargo's fragility and value are beyond measure. I implore thee, eschew the temptation of magical travel, for its turbulent energies would bring naught but harm to its delicate form");
            NoRecallMessages.Add("O $Player$ of great repute, the cargo's fragility and worth render it a cherished gem. I beseech thee, refrain from invoking travel magic, as its turbulent forces may shatter its delicate essence like glass");
            LoadQuestTExts();
        }

        public static void LoadQuestTExts()
        {
            VendorParcelText.VendorTexts = new Dictionary<int, VendorParcelText>();
            string filePath = Path.Combine(Core.BaseDirectory, "Data/TextExamples/VendorParcelTexts.txt");
            var qstart = "Q";
            var astart = "A";

            if (File.Exists(filePath))
            {
                using (StreamReader ip = new StreamReader(filePath))
                {
                    string line;
                    string q = null;
                    string a = null;
                    int count = 0;
                    while ((line = ip.ReadLine()) != null)
                    {
                        if (line == qstart)
                        {
                            if (q != null && a != null)
                            {
                                VendorParcelText.VendorTexts[count++] = new VendorParcelText(q, a);
                                a = null;
                            }
                            q = "";
                            continue;
                        }
                        if (line == astart)
                        {
                            a = "";
                            continue;
                        }

                        if (a != null)
                        {
                            a += line;
                            continue;
                        }

                        if (q != null)
                        {
                            q += line;
                        }

                    }

                    VendorParcelText.VendorTexts[count] = new VendorParcelText(q, a);
                }
            }
            else
            {
                Utility.ConsoleWriteLine(Utility.ConsoleMsgType.Error, $"Error load file:{filePath}");
                throw new Exception("File not found");
            }
        }

        public VendorParcelText(string q, string a)
        {
            Quest = q;
            After = a;
        }

        public string PrepareStringQ(Mobile player, BaseVendor vendor, int gold)
        {
            var result = PrepareString(player, vendor, Quest);
            if (gold > 0)
                result = $"{result}\n{Utility.RandomList(LowHonestyMessages).Replace(m_Gold, gold.ToString())}";
            return $"{result}\n{Utility.RandomList(NoRecallMessages).Replace(m_playerName, player.Name)}";
        }
        public string PrepareStringA(Mobile player, BaseVendor vendor)
        {
            return PrepareString(player, vendor, After);
        }

        private static string PrepareString(Mobile player, BaseVendor vendor, string text)
        {
            return text.Replace(m_playerName, player.Name).Replace(m_RecipName, vendor.Name).Replace(m_RecipCity, vendor.Region.Name);
        }
    }

    public class VendorParcelMenu : ContextMenuEntry
    {
        private readonly BaseVendor m_Vendor;
        private readonly Mobile m_Player;
        private static Random m_Random = new Random(DateTime.UtcNow.Millisecond);

        public static void Initialize()
        {
        
        }

        public VendorParcelMenu(Mobile from, BaseVendor vendor) : base(6209, 8)
        {
            m_Vendor = vendor;
            m_Player = from;
            Enabled = vendor.CheckVendorAccess(from);
        }

        public override void OnClick()
        {
            var recipient = GetRecipient();
            m_Player.SendGump(new VendorParcelGump(m_Player, m_Vendor, recipient));
        }

        private BaseVendor GetRecipient()
        {

            return BaseVendor.Vendors.Values.Where(m => m.Map == m_Vendor.Map && m.Alive && !m.Deleted && !m.Criminal && !m.Warmode && !String.IsNullOrEmpty(m.Region.Name) && 
                                                        (Math.Abs(m.X - m_Vendor.X) + Math.Abs(m.Y - m_Vendor.Y) > 1000)).OrderBy( _ => m_Random.Next()).FirstOrDefault();
        }
    }

    public class VendorParcelGump : Gump
    {
        private Mobile m_Player;
        private BaseVendor m_Vendor;
        private BaseVendor m_Recipient;
        private int m_deposit;
        private int textId;
        public VendorParcelGump(Mobile m, BaseVendor vendor, BaseVendor recipient) : base(0, 0)
        {
            m_Vendor = vendor;
            m_Player = m;
            m_Recipient = recipient;
            m_deposit = GenerateDebt();

            Dragable = true;
            Closable = true;
            Resizable = false;
            Disposable = false;
            textId = Utility.Random(VendorParcelText.VendorTexts.Count);
            AddPage(0);
            AddBackground(285, 50, 462, 374, 3500);
            AddImage(300, 90, 5536);
            AddButton(520, 380, 238, 239, (int)Buttons.ButtonOk, GumpButtonType.Reply, 0);
            AddButton(610, 380, 241, 242, (int)Buttons.ButtonCancel, GumpButtonType.Reply, 0);

            AddHtml(400, 90, 328, 273, $"{VendorParcelText.VendorTexts[textId].PrepareStringQ(m_Player, m_Recipient, m_deposit)}", true, true);
            AddLabel(310, 60, 0, $"Delivery {m_Vendor.Name} to {m_Recipient.Name} from {m_Recipient.Region.Name}");
            AddImage(320, 330, 9005);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 1:
                    if (!BaseVendor.ConsumeBackpackAndBankGold(m_Player, m_deposit))
                    {
                        m_Player.Virtues.Honesty--;
                        m_Player.SendAsciiMessage($"You don't have {m_deposit} gp for deposit");
                        return;
                    }
                    var item = new VendorParcel(m_Vendor, m_Recipient, m_deposit);
                    if (!item.DropToItem(m_Player, m_Player.Backpack, Point3D.Zero))
                    {
                        item.DropToWorld(m_Player, m_Player.Location);
                    }
                    break;
                default:
                    break;
            }        
        }

        private int GenerateDebt()
        {
            int minHonestyVal = 100;
            int positiveHonestyFee = 50;
            int negativeHonestyFee = 200;
            if (m_Player.Virtues.Honesty < 0)
                return Utility.LimitMinMax(100, negativeHonestyFee * Math.Abs(m_Player.Virtues.Honesty) + positiveHonestyFee * minHonestyVal, 20000);
            else if (m_Player.Virtues.Honesty >=0 && m_Player.Virtues.Honesty < minHonestyVal)
                return Utility.Max(100, positiveHonestyFee * (minHonestyVal - m_Player.Virtues.Honesty));
            return 0;
        }

        public enum Buttons
        {
            ButtonOk = 1,
            ButtonCancel = 2,
        }

        public enum Inputs
        {
            TextEntry1 = 1,
        }
    }

}
