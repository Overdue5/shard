using Server.ContextMenus;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Server.Commands.GMUtils;
using Server.Engines;
using Server.Gumps;
using Server.Items;
using Server.Logging;
using Server.Network;

namespace Server.Custom
{
    public static class VendorLogs
    {
        public static void Write(string text)
        {
            Logs.QuestLog.WriteLine($"QVendorParcel: {text}");
        }
    }

    public class VendorParcel : Item
    {
        private Mobile m_Carrier;
        private Mobile m_Source;
        private Mobile m_Recipient;
        private DateTime m_DateTime;
        private int m_Dist;
        private int m_Debt;
        private int m_TextId;
        private bool m_IsBroken;
        private bool m_IsIland;
        private static HashSet<string> m_Ilands = new HashSet<string> { "Magincia", "Ocllo", "Jhelom", "Jhelom Island", "Skara Brae", "Buccaneer's Den", "Nujel'm", "Moonglow" };

        [CommandProperty(AccessLevel.Counselor)]
        public Mobile Carrier
        {
            get => m_Carrier;
            set => m_Carrier = value;
        }

        [CommandProperty(AccessLevel.Counselor)]
        public Mobile Source
        {
            get => m_Source;
            set => m_Source = value;
        }

        [CommandProperty(AccessLevel.Counselor)]
        public Mobile Recipient
        {
            get => m_Recipient;
            set => m_Recipient = value;
        }

        [CommandProperty(AccessLevel.Counselor)]
        public int Dist => m_Dist;

        [CommandProperty(AccessLevel.Counselor)]
        public bool IsBroken
        {
            get => m_IsBroken;
            set => m_IsBroken = value;
        }

        [CommandProperty(AccessLevel.Counselor)]
        public int Debt
        {
            get => m_Debt;
            set => m_Debt = value;
        }

        public VendorParcel(Mobile carrier, Mobile source, Mobile recipient, int debt, int textId):base(0x2DF3)
        {
            m_Carrier = carrier;
            m_IsBroken = false;
            m_Source = source;
            m_Recipient = recipient;
            m_IsIland = (m_Ilands.Contains(m_Source.Region.Name) || m_Ilands.Contains(m_Recipient.Region.Name));
            Name = $"A package bound for {recipient.Name} from the far reaches of {recipient.Region.Name} arrives, carried by {m_Carrier.Name}.";
            m_Debt = debt;
            Weight = 100;
            m_DateTime = DateTime.UtcNow;
            m_TextId = textId;
            m_Dist = Math.Abs(source.X - recipient.X) + Math.Abs(source.Y - recipient.Y);
            VendorLogs.Write($"{m_Carrier}:0x{Serial.Value:X8},L:{m_Carrier.Location}: From {source.Name} - {source.Region.Name} - Loc:{source.Location} to " +
                             $"{recipient.Name} - {recipient.Region.Name} - Loc:{recipient.Location}, dist:{m_Dist}, debt:{m_Debt}, IsIland:{m_IsIland}");
        }

        public static void CheckTeleportedItems(Mobile mob)
        {
            bool broke = false;
            foreach (var item in mob.Backpack.Items)
            {
                if (item is VendorParcel vp)
                {
                    vp.m_IsBroken = true;
                    broke = true;
                    mob.Virtues.Honesty -= 5;
                    VendorLogs.Write($"0x{vp.Serial.Value:X8} was broken in portal by {mob}, {mob.Location}");
                }
            }

            if (mob.Holding is VendorParcel vp1) 
            {
                vp1.m_IsBroken = true;
                mob.Virtues.Honesty -= 5;
                broke = true;
                VendorLogs.Write($"0x{vp1.Serial.Value:X8} was broken in portal by {mob}, {mob.Location}");
            }
            if (broke)
            {
                mob.SendAsciiMessage("You hear an unsettling clatter emanating from the parcel, as if something within has shattered.");
            }
        }
        public VendorParcel(Serial serial):base(serial)
        {
        }
        
        public override bool OnDroppedToMobile(Mobile from, Mobile target)
        {
            if (target.Serial != m_Recipient.Serial) return false;
            int summ = CalculateCost();
            if (m_IsBroken)
            {
                target.Say($"Inside is a mess, brave {from.Name}. You have been entrusted with handling them with care.");
                if (m_Debt>0)
                    target.Say("Unfortunately, I will not be able to return the full amount of the deposit.");
                from.AddGoldToBackPack(summ);
                VendorLogs.Write($"0x{this.Serial.Value:X8} finished with broken item, {from} earn {summ} ");
                Delete();
            }
            else
            {
                from.Virtues.Honesty += 10;
                from.AddToBackpack(new Gold(summ));
                if (!VendorParcelText.VendorTexts.ContainsKey(m_TextId) || string.IsNullOrEmpty(VendorParcelText.VendorTexts[m_TextId].After))
                    target.Say("Delivery complete. You've earned a moment of rest. Take it easy, and keep on delivering");
                else
                {
                    target.Say(VendorParcelText.VendorTexts[m_TextId].PrepareStringA(from, target));
                }

                VendorLogs.Write($"0x{this.Serial.Value:X8} finished. {from} earn {summ}, distance {m_Dist},  delivery time:{(DateTime.UtcNow - m_DateTime).TotalSeconds}");
                Delete();
            }
            return true;
        }

        private int CalculateCost()
        {
            var multi = 1;
            if (m_IsIland)
                multi = 2;
            if (m_IsBroken)
                return multi * 200 + m_Debt/2;
            var duration = (int)(1+ (DateTime.UtcNow - m_DateTime).TotalSeconds);
            return m_Debt + multi * Utility.LimitMinMax(200, m_Dist * 3 + m_Dist - duration * 3, m_Dist * 2);
        }


        public override bool OnDroppedInto(Mobile from, Container target, Point3D p)
        {
            if (target == from.Backpack) return base.OnDroppedInto(from, target, p);
            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
            writer.WriteMobile(m_Carrier);
            writer.WriteMobile(m_Source);
            writer.WriteMobile(m_Recipient);
            writer.Write(m_DateTime);
            writer.Write(m_Dist);
            writer.Write(m_Debt);
            writer.Write(m_TextId);
            writer.Write(m_IsBroken);
            writer.Write(m_IsIland);
    }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            _ = reader.ReadInt();
            m_Carrier = reader.ReadMobile();
            m_Source = reader.ReadMobile();
            m_Recipient = reader.ReadMobile();
            m_DateTime = reader.ReadDateTime();
            m_Dist = reader.ReadInt();
            m_Debt = reader.ReadInt();
            m_TextId = reader.ReadInt();
            m_IsBroken = reader.ReadBool();
            m_IsIland = reader.ReadBool();
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
                                   "I beseech thee to provide a deposit of $Gold$ gp, safeguarding the precious cargo's path. " +
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
                ConsoleLog.Write.Error($"Error load file:{filePath}");
                throw new Exception("File not found");
            }
        }

        public VendorParcelText(string q, string a)
        {
            Quest = q;
            After = a;
        }

        public string PrepareStringQ(Mobile player, Mobile vendor, int gold)
        {
            var result = PrepareString(player, vendor, Quest);
            if (gold > 0)
                result = $"{result}\n{Utility.RandomList(LowHonestyMessages).Replace(m_Gold, gold.ToString())}";
            return $"{result}\n{Utility.RandomList(NoRecallMessages).Replace(m_playerName, player.Name)}";
        }
        public string PrepareStringA(Mobile player, Mobile vendor)
        {
            return PrepareString(player, vendor, After);
        }

        private static string PrepareString(Mobile player, Mobile vendor, string text)
        {
            return text.Replace(m_playerName, player.Name).Replace(m_RecipName, vendor.Name).Replace(m_RecipCity, vendor.Region.Name);
        }
    }

    public class VendorParcelMenu : ContextMenuEntry
    {
        private readonly BaseVendor m_Vendor;
        private readonly Mobile m_Player;
        private static Dictionary<Serial, DateTime> ParcelHistory = new Dictionary<Serial, DateTime>();

        public static void Initialize()
        {
        
        }

        public VendorParcelMenu(Mobile from, BaseVendor vendor) : base(1024, 8)
        {
            m_Vendor = vendor;
            m_Player = from;
            if (ParcelHistory.ContainsKey(vendor.Serial) && ParcelHistory[vendor.Serial] > DateTime.UtcNow)
                Enabled = false;
            else
                Enabled = vendor.CheckVendorAccess(from) && m_Player.Backpack != null;
        }

        public override void OnClick()
        {
            if (m_Player.Virtues.Honesty < -1000)
            {
                m_Vendor.Say("Thy reputation precedes thee, and whispers of dishonesty shroud thy name. I shan't partake in thy ventures.");
                return;
            }

            VendorParcel parcel = m_Player.Backpack.Items.FirstOrDefault(m => m is VendorParcel vp && vp.Recipient == m_Vendor) as VendorParcel;
            if (parcel != null)
            {
                parcel.OnDroppedToMobile(m_Player, m_Vendor);
            }
            else
            {
                var recipient = GetRecipient();
                if (recipient != null)
                {
                    ParcelHistory[m_Vendor.Serial] = DateTime.UtcNow + UtilityWorldTime.DayTime;
                    m_Player.SendGump(new VendorParcelGump(m_Player, m_Vendor, recipient));
                }
            }
        }

        private BaseVendor GetRecipient()
        {

            return BaseVendor.Vendors.Values.Where(m => m.Map == m_Vendor.Map && m.Alive && !m.Deleted && !m.Criminal && !m.Warmode && m.Fame >= 0 && m.Karma >= 0
                                                        && !String.IsNullOrEmpty(m.Region.Name) && !m.Region.Name.Contains("Custom") && !m.Region.Name.Contains("Territory") &&
                                                        (Math.Abs(m.X - m_Vendor.X) + Math.Abs(m.Y - m_Vendor.Y) > 1000)).OrderBy( _ => Utility.RandomCnt.Next()).FirstOrDefault();
        }
    }

    public class VendorParcelGump : Gump
    {
        private readonly Mobile m_Player;
        private readonly Mobile m_Vendor;
        private readonly Mobile m_Recipient;
        private readonly int m_Deposit;
        private readonly int m_TextId;
        public VendorParcelGump(Mobile m, BaseVendor vendor, BaseVendor recipient) : base(0, 0)
        {
            m_Vendor = vendor;
            m_Player = m;
            m_Recipient = recipient;
            m_Deposit = GenerateDebt();
            m_TextId = Utility.Random(VendorParcelText.VendorTexts.Count);

            Dragable = true;
            Closable = true;
            Resizable = false;
            Disposable = false;
            AddPage(0);
            AddBackground(285, 50, 462, 374, 3500);
            AddImage(300, 90, 5536);
            AddButton(520, 380, 238, 239, (int)Buttons.ButtonOk, GumpButtonType.Reply, 0);
            AddButton(610, 380, 241, 242, (int)Buttons.ButtonCancel, GumpButtonType.Reply, 0);

            AddHtml(400, 90, 328, 273, $"{VendorParcelText.VendorTexts[m_TextId].PrepareStringQ(m_Player, m_Recipient, m_Deposit)}", true, true);
            AddLabel(310, 60, 0, $"Delivery {m_Vendor.Name} to {m_Recipient.Title} {m_Recipient.Name} from {m_Recipient.Region.Name}");
            AddImage(320, 330, 9005);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 1:
                    if (!m_Player.Alive || m_Player.Backpack == null ) return;

                    if (m_Vendor.Map == m_Player.Map && m_Recipient.Map == m_Player.Map && m_Player.Map != Map.Internal &&
                        m_Player.GetDistanceToSqrt(m_Vendor) > 100)
                    {
                        m_Player.Virtues.Honesty -= 500;
                        VendorLogs.Write($"!!! {m_Player} location:{m_Player.Location} assert quest between {m_Vendor} {m_Vendor.Location} and {m_Recipient} {m_Recipient} location");
                    }

                    if (!BaseVendor.ConsumeBackpackAndBankGold(m_Player, m_Deposit))
                    {
                        m_Player.SendAsciiMessage($"You don't have {m_Deposit} gp for deposit");
                        return;
                    }
                    var item = new VendorParcel(m_Player, m_Vendor, m_Recipient, m_Deposit, m_TextId);
                    if (!item.DropToItem(m_Player, m_Player.Backpack, Point3D.Zero))
                    {
                        item.DropToWorld(m_Player, m_Player.Location);
                    }
                    break;
            }        
        }

        private int GenerateDebt()
        {
            int minHonestyVal = 100;
            int positiveHonestyFee = 50;
            int negativeHonestyFee = 200;
            if (m_Player.Virtues.Honesty < 0)
                return Utility.LimitMinMax(100, negativeHonestyFee * Math.Abs(m_Player.Virtues.Honesty) + positiveHonestyFee * minHonestyVal, 30000);
            else if (m_Player.Virtues.Honesty >=0 && m_Player.Virtues.Honesty < minHonestyVal)
                return Utility.Max(100, positiveHonestyFee * (minHonestyVal - m_Player.Virtues.Honesty));
            return 0;
        }

        public enum Buttons
        {
            ButtonOk = 1,
            ButtonCancel = 2,
        }
    }

}
