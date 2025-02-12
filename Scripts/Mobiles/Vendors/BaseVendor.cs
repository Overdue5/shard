//Rev 149

using System;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Custom;
using Server.Engines.BulkOrders;
using Server.Factions;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Network;
using Server.Regions;

namespace Server.Mobiles
{
	public enum VendorShoeType
	{
		None,
		Shoes,
		Boots,
		Sandals,
		ThighBoots
	}

	public abstract class BaseVendor : BaseCreature, IVendor
    {
        public static Dictionary<Serial, BaseVendor> Vendors = new Dictionary<Serial, BaseVendor>();
        public static Dictionary<SkillName, HashSet<BaseVendor>> SkilledVendors = new Dictionary<SkillName, HashSet<BaseVendor>>();
        private const int MaxSell = 999;

        protected abstract List<SBInfo> SBInfos { get; }

		private readonly ArrayList m_ArmorBuyInfo = new ArrayList();
		private readonly ArrayList m_ArmorSellInfo = new ArrayList();

		private DateTime m_LastRestock;

		public override bool CanTeach => true;

        public override bool BardImmune => true;

        public override bool PlayerRangeSensitive => true;

        public virtual bool IsActiveVendor => true;
        public virtual bool IsActiveBuyer => IsActiveVendor; // response to vendor SELL
		public virtual bool IsActiveSeller => IsActiveVendor; // repsonse to vendor BUY

		public virtual NpcGuild NpcGuild => NpcGuild.None;

        public virtual bool IsInvulnerable => false;

        public override bool ShowFameTitle => false;

        public virtual bool IsValidBulkOrder( Item item )
		{
			return false;
		}

		public virtual Item CreateBulkOrder( Mobile from, bool fromContextMenu )
		{
			return null;
		}

		public virtual bool SupportsBulkOrders( Mobile from )
		{
			return false;
		}

		public virtual TimeSpan GetNextBulkOrder( Mobile from )
		{
			return TimeSpan.Zero;
		}

        public virtual void OnSuccessfulBulkOrderReceive(Mobile from)
		{
		}

		#region Faction
		public virtual int GetPriceScalar()
		{
			var town = Town.FromRegion( Region );

			if ( town != null )
				return (100 + town.Tax);

			return 100;
		}

		public void UpdateBuyInfo()
		{
			var priceScalar = GetPriceScalar();
			
			var buyinfo = (IBuyItemInfo[])m_ArmorBuyInfo.ToArray( typeof( IBuyItemInfo ) );

			if ( buyinfo != null )
			{
				foreach ( var info in buyinfo )
					info.PriceScalar = priceScalar;
			}
		}
		#endregion

		private class BulkOrderInfoEntry : ContextMenuEntry
		{
			private readonly Mobile m_From;
			private readonly BaseVendor m_Vendor;

			public BulkOrderInfoEntry( Mobile from, BaseVendor vendor ) : base( 6152 )
			{
				m_From = from;
				m_Vendor = vendor;
			}

			public override void OnClick()
			{
				if ( m_Vendor.SupportsBulkOrders( m_From ) )
				{
					var ts = m_Vendor.GetNextBulkOrder( m_From );

					var totalSeconds = (int)ts.TotalSeconds;
					var totalHours = (totalSeconds + 3599) / 3600;
					var totalMinutes = (totalSeconds + 59) / 60;

					if ( ((Core.SE ) ? totalMinutes == 0 : totalHours == 0) )
					{
						m_From.SendLocalizedMessage( 1049038 ); // You can get an order now.

						if ( Core.AOS )
						{
							var bulkOrder = m_Vendor.CreateBulkOrder( m_From, true );

							if ( bulkOrder is LargeBOD )
								m_From.SendGump( new LargeBODAcceptGump( m_From, (LargeBOD)bulkOrder ) );
							else if ( bulkOrder is SmallBOD )
								m_From.SendGump( new SmallBODAcceptGump( m_From, (SmallBOD)bulkOrder ) );
						}
					}
					else
					{
						var oldSpeechHue = m_Vendor.SpeechHue;
						m_Vendor.SpeechHue = 0;
						
						if( Core.SE )
							m_Vendor.SayTo( m_From, 1072058, totalMinutes.ToString() ); // An offer may be available in about ~1_minutes~ minutes.
						else
							m_Vendor.SayTo( m_From, 1049039, totalHours.ToString() ); // An offer may be available in about ~1_hours~ hours.

						m_Vendor.SpeechHue = oldSpeechHue;
					}
				}
			}
		}

		public BaseVendor( string title ) : base( AIType.AI_Vendor, FightMode.None, 2, 1, 0.5, 2 )
		{
			LoadSBInfo();

			Title = title;
			InitBody();
			InitOutfit();

		    //these packs MUST exist, or the client will crash when the packets are sent
			Container pack = new Backpack {Layer = Layer.ShopBuy, Movable = false, Visible = false};
		    AddItem( pack );

			pack = new Backpack {Layer = Layer.ShopResale, Movable = false, Visible = false};
		    AddItem( pack );

			m_LastRestock = DateTime.UtcNow;
            m_RestockDelay = GetRestockDelay();
            Timer.DelayCall(TimeSpan.FromSeconds(5), () => { UpdateVendorTables(this); });
        }
		
		public BaseVendor( Serial serial ) : base( serial )
		{
		}

        public override void OnDelete()
        {
            base.OnDelete();
            DeleteVendorTables(this);
        }

        public DateTime LastRestock
		{
			get => m_LastRestock;
            set => m_LastRestock = value;
        }

		public virtual TimeSpan RestockDelay => m_RestockDelay;

        private TimeSpan m_RestockDelay;

        private static TimeSpan GetRestockDelay()
        {
            return TimeSpan.FromMinutes(Utility.RandomMinMax(20, 45));
        }

		public Container BuyPack
		{
			get
			{
				var pack = FindItemOnLayer( Layer.ShopBuy ) as Container;

				if ( pack == null )
				{
					pack = new Backpack {Layer = Layer.ShopBuy, Visible = false};
				    AddItem( pack );
				}

				return pack;
			}
		}

		public abstract void InitSBInfo();

		public virtual bool IsTokunoVendor => ( Map == Map.Tokuno );

        protected void LoadSBInfo()
		{
			m_LastRestock = DateTime.UtcNow;

			for ( var i = 0; i < m_ArmorBuyInfo.Count; ++i )
			{
				var buy = m_ArmorBuyInfo[i] as GenericBuyInfo;

				if ( buy != null )
					buy.DeleteDisplayEntity();
			}

			SBInfos.Clear();

			InitSBInfo();

			m_ArmorBuyInfo.Clear();
			m_ArmorSellInfo.Clear();

			for ( var i = 0; i < SBInfos.Count; i++ )
			{
				var sbInfo = (SBInfo)SBInfos[i];
				m_ArmorBuyInfo.AddRange( sbInfo.BuyInfo );
				m_ArmorSellInfo.Add( sbInfo.SellInfo );
			}
		}

		public virtual bool GetGender()
		{
			return Utility.RandomBool();
		}

		public virtual void InitBody()
		{
			InitStats( 100, 100, 25 );

			SpeechHue = 0;
			Hue = Utility.RandomSkinHue();

			if ( IsInvulnerable && !Core.AOS )
				NameHue = -1;

			if ( Female == GetGender() )
			{
				Body = 0x191;
				Name = NameList.RandomName( "female" );
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName( "male" );
			}
		}

		public virtual int GetRandomHue()
		{
			switch ( Utility.Random( 5 ) )
			{
				default:return Utility.RandomBlueHue();
				case 1: return Utility.RandomGreenHue();
				case 2: return Utility.RandomRedHue();
				case 3: return Utility.RandomYellowHue();
				case 4: return Utility.RandomNeutralHue();
			}
		}

		public virtual int GetShoeHue()
		{
			if ( 0.1 > Utility.RandomDouble() )
				return 0;

			return Utility.RandomNeutralHue();
		}

		public virtual VendorShoeType ShoeType => VendorShoeType.Shoes;

        public virtual int RandomBrightHue()
		{
			if ( 0.1 > Utility.RandomDouble() )
				return Utility.RandomList( 0x62, 0x71 );

			return Utility.RandomList( 0x03, 0x0D, 0x13, 0x1C, 0x21, 0x30, 0x37, 0x3A, 0x44, 0x59 );
		}

		public virtual void CheckMorph()
		{
			if ( CheckGargoyle() )
				return;

			if ( CheckNecromancer() )
				return;

			CheckTokuno();
		}

		public virtual bool CheckTokuno()
		{
			if ( Map != Map.Tokuno )
				return false;

			NameList n;

			if ( Female )
				n = NameList.GetNameList( "tokuno female" );
			else
				n = NameList.GetNameList( "tokuno male" );

			if ( !n.ContainsName( Name ) )
				TurnToTokuno();

			return true;
		}

		public virtual void TurnToTokuno()
		{
			if ( Female )
				Name = NameList.RandomName( "tokuno female" );
			else
				Name = NameList.RandomName( "tokuno male" );
		}

		public virtual bool CheckGargoyle()
		{
			var map = Map;

			if ( map != Map.Ilshenar )
				return false;

			if ( !Region.IsPartOf( "Gargoyle City" ) )
				return false;

			if ( Body != 0x2F6 || (Hue & 0x8000) == 0 )
				TurnToGargoyle();

			return true;
		}

		public virtual bool CheckNecromancer()
		{
			var map = Map;

			if ( map != Map.Malas )
				return false;

			if ( !Region.IsPartOf( "Umbra" ) )
				return false;

			if ( Hue != 0x83E8 )
				TurnToNecromancer();

			return true;
		}

		public override void OnAfterSpawn()
		{
			CheckMorph();
		}

		protected override void OnMapChange( Map oldMap )
		{
			base.OnMapChange( oldMap );

			CheckMorph();

			LoadSBInfo();
		}

		public virtual int GetRandomNecromancerHue()
		{
			switch ( Utility.Random( 20 ) )
			{
				case 0: return 0;
				case 1: return 0x4E9;
				default: return Utility.RandomList( 0x485, 0x497 );
			}
		}

		public virtual void TurnToNecromancer()
		{
			for ( var i = 0; i < Items.Count; ++i )
			{
				var item = Items[i];

				if ( item is Hair || item is Beard )
					item.Hue = 0;
				else if ( item is BaseClothing || item is BaseWeapon || item is BaseArmor || item is BaseTool )
					item.Hue = GetRandomNecromancerHue();
			}

			HairHue = 0;
			FacialHairHue = 0;

			Hue = 0x83E8;
		}

        public override void OnSingleClick(Mobile from)
        {
            int nameHue;

            if (NameHue != -1)
                nameHue = NameHue;
            else if (AccessLevel > AccessLevel.Player)
                nameHue = 11;
            else
                nameHue = Notoriety.GetHue(Notoriety.Compute(from, this));

            PrivateOverheadMessage(MessageType.Label, nameHue, AsciiClickMessage, Name, from.NetState);    

        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);
			VendorBuy(from);
        }

        public virtual void TurnToGargoyle()
		{
			for ( var i = 0; i < Items.Count; ++i )
			{
				var item = Items[i];

				if ( item is BaseClothing || item is Hair || item is Beard )
					item.Delete();
			}

			HairItemID = 0;
			FacialHairItemID = 0;

			Body = 0x2F6;
			Hue = RandomBrightHue() | 0x8000;
			Name = NameList.RandomName( "gargoyle vendor" );

			CapitalizeTitle();
		}

		public virtual void CapitalizeTitle()
		{
			var title = Title;

			if ( title == null )
				return;

			var split = title.Split( ' ' );

			for ( var i = 0; i < split.Length; ++i )
			{
				if ( Insensitive.Equals( split[i], "the" ) )
					continue;

				if ( split[i].Length > 1 )
					split[i] = Char.ToUpper( split[i][0] ) + split[i].Substring( 1 );
				else if ( split[i].Length > 0 )
					split[i] = Char.ToUpper( split[i][0] ).ToString();
			}

			Title = String.Join( " ", split );
		}

		public virtual int GetHairHue()
		{
			return Utility.RandomHairHue();
		}

		public virtual void InitOutfit()
		{
			switch ( Utility.Random( 3 ) )
			{
				case 0: AddItem( new FancyShirt( GetRandomHue() ) ); break;
				case 1: AddItem( new Doublet( GetRandomHue() ) ); break;
				case 2: AddItem( new Shirt( GetRandomHue() ) ); break;
			}

			switch ( ShoeType )
			{
				case VendorShoeType.Shoes: AddItem( new Shoes( GetShoeHue() ) ); break;
				case VendorShoeType.Boots: AddItem( new Boots( GetShoeHue() ) ); break;
				case VendorShoeType.Sandals: AddItem( new Sandals( GetShoeHue() ) ); break;
				case VendorShoeType.ThighBoots: AddItem( new ThighBoots( GetShoeHue() ) ); break;
			}

			var hairHue = GetHairHue();

			Utility.AssignRandomHair( this, hairHue );
			Utility.AssignRandomFacialHair( this, hairHue );

			if ( Female )
			{
				switch ( Utility.Random( 6 ) )
				{
					case 0: AddItem( new ShortPants( GetRandomHue() ) ); break;
					case 1:
					case 2: AddItem( new Kilt( GetRandomHue() ) ); break;
					case 3:
					case 4:
					case 5: AddItem( new Skirt( GetRandomHue() ) ); break;
				}
			}
			else
			{
				switch ( Utility.Random( 2 ) )
				{
					case 0: AddItem( new LongPants( GetRandomHue() ) ); break;
					case 1: AddItem( new ShortPants( GetRandomHue() ) ); break;
				}
			}

			PackGold( 20, 30 );
		}

		public virtual void Restock()
		{
			m_LastRestock = DateTime.UtcNow;

			var buyInfo = GetBuyInfo();

			foreach ( var bii in buyInfo )
				bii.OnRestock();

            //Vendors get a new restock delay every time they restock
            m_RestockDelay = GetRestockDelay();
		}

		private static readonly TimeSpan InventoryDecayTime = TimeSpan.FromHours( 1.0 );

		public virtual void VendorBuy( Mobile from )
		{
			if ( !IsActiveSeller )
				return;

			if ( !from.CheckAlive() )
				return;

            if (!from.InRange(this, 5) || !from.InLOS(this))
                return;

			if ( !CheckVendorAccess( from ) )
			{
				Say( 501522 ); // I shall not treat with scum like thee!
				return;
			}

			if ( DateTime.UtcNow - m_LastRestock > RestockDelay )
				Restock();

			UpdateBuyInfo();

		    int count = 0;
            List<BuyItemState> list;
            IBuyItemInfo[] buyInfo = this.GetBuyInfo();
            IShopSellInfo[] sellInfo = this.GetSellInfo();

            list = new List<BuyItemState>(buyInfo.Length);
            Container cont = BuyPack;

			//ArrayList opls = new ArrayList();
            var opls = new List<ObjectPropertyList>();

			for (var idx=0;idx<buyInfo.Length;idx++)
			{
				var buyItem = buyInfo[idx];

				if ( buyItem.Amount <= 0 || list.Count >= 250 )
					continue;

				// NOTE: Only GBI supported; if you use another implementation of IBuyItemInfo, this will crash
				var gbi = (GenericBuyInfo) buyItem;
				var disp = gbi.GetDisplayEntity();

				list.Add( new BuyItemState( buyItem.Name, cont.Serial, disp == null ? (Serial) 0x7FC0FFEE : disp.Serial, buyItem.Price, buyItem.Amount, buyItem.ItemID, buyItem.Hue ) );
				count++;

				if ( disp is Item )
					opls.Add( (disp as Item).PropertyList );
				else if ( disp is Mobile )
					opls.Add( (disp as Mobile).PropertyList );
			}

			var playerItems = cont.Items;

			for ( var i = playerItems.Count - 1; i >= 0; --i )
			{
				if ( i >= playerItems.Count )
					continue;

				var item = playerItems[i];

				if ( (item.LastMoved + InventoryDecayTime) <= DateTime.UtcNow )
					item.Delete();
			}

			for ( var i = 0; i < playerItems.Count; ++i )
			{
				var item = playerItems[i];

				var price = 0;
				string name = null;

				foreach( var ssi in sellInfo )
				{
					if ( ssi.IsSellable( item ) )
					{
						price = ssi.GetBuyPriceFor( item );
						name = ssi.GetNameFor( item );
						break;
					}
				}

				if ( name != null && list.Count < 250 )
				{
					list.Add( new BuyItemState( name, cont.Serial, item.Serial, price, item.Amount, item.ItemID, item.Hue ) );
					count++;

					opls.Add( item.PropertyList );
				}
			}

			//one (not all) of the packets uses a byte to describe number of items in the list.  Osi = dumb.
			//if ( list.Count > 255 )
			//	ConsoleLog.Write.Information( "Vendor Warning: Vendor {0} has more than 255 buy items, may cause client errors!", this );

			if ( list.Count > 0 )
			{
				list.Sort( new BuyItemStateComparer() );

				SendPacksTo( from );

                NetState ns = from.NetState;

                if (ns == null) 
                    return;

                if (ns.ContainerGridLines)
                    from.Send(new VendorBuyContent6017(list));
                else
                    from.Send(new VendorBuyContent(list));

                from.Send(new VendorBuyList(this, list));

                if (ns.HighSeas)
                    from.Send(new DisplayBuyListHS(this));
                else
                    from.Send(new DisplayBuyList(this));

				from.Send( new MobileStatusExtended( from ) );//make sure their gold amount is sent

				for ( var i = 0; i < opls.Count; ++i )
					from.Send( opls[i] );

				SayTo( from, 500186 ); // Greetings.  Have a look around.
			}
		}

		public virtual void SendPacksTo( Mobile from )
		{
			var pack = FindItemOnLayer( Layer.ShopBuy );

			if ( pack == null )
			{
				pack = new Backpack {Layer = Layer.ShopBuy, Movable = false, Visible = false};
			    AddItem( pack );
			}

			from.Send( new EquipUpdate( pack ) );

			pack = FindItemOnLayer( Layer.ShopSell );

			if ( pack != null )
				from.Send( new EquipUpdate( pack ) );

			pack = FindItemOnLayer( Layer.ShopResale );

			if ( pack == null )
			{
				pack = new Backpack {Layer = Layer.ShopResale, Movable = false, Visible = false};
			    AddItem( pack );
			}

			from.Send( new EquipUpdate( pack ) );
		}

		public virtual void VendorSell( Mobile from )
		{
			if ( !IsActiveBuyer )
				return;

			if ( !from.CheckAlive() )
				return;

            if (!from.InRange(this, 5) || !from.InLOS(this))
                return;

			if ( !CheckVendorAccess( from ) )
			{
				Say( 501522 ); // I shall not treat with scum like thee!
				return;
			}

			var pack = from.Backpack;

			if ( pack != null )
			{
				var info = GetSellInfo();

				var table = new Hashtable();

				foreach ( var ssi in info )
				{
					var items = pack.FindItemsByType( ssi.Types );

					foreach ( var item in items )
					{
						if ( item is Container && item.Items.Count != 0 )
							continue;

						if ( item.IsStandardLoot() && item.Movable && ssi.IsSellable( item ) )
							table[item] = new SellItemState( item, ssi.GetSellPriceFor( item ), ssi.GetNameFor( item ) );
					}
				}

				if ( table.Count > 0 )
				{
					SendPacksTo( from );

					from.Send( new VendorSellList( this, table ) );
				}
				else
				{
					Say( true, "You have nothing I would be interested in." );
				}
			}
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
            /* TODO: Thou art giving me? and fame/karma for gold gifts */

			if ( dropped is SmallBOD || dropped is LargeBOD )
			{
                if (Core.ML)
                {
                    if (((PlayerMobile)from).NextBODTurnInTime > DateTime.UtcNow)
                    {
                        SayTo(from, 1079976);	//
                        return false;
                    }
                }

				if ( !IsValidBulkOrder( dropped ) || !SupportsBulkOrders( from ) )
				{
					SayTo( from, 1045130 ); // That order is for some other shopkeeper.
					return false;
				}
			    if ( (dropped is SmallBOD && !((SmallBOD)dropped).Complete) || (dropped is LargeBOD && !((LargeBOD)dropped).Complete) )
			    {
			        SayTo( from, 1045131 ); // You have not completed the order yet.
			        return false;
			    }

			    Item reward;
				int gold, fame;

				if ( dropped is SmallBOD )
					((SmallBOD)dropped).GetRewards( out reward, out gold, out fame );
				else
					((LargeBOD)dropped).GetRewards( out reward, out gold, out fame );

				from.SendSound( 0x3D );

				SayTo( from, 1045132 ); // Thank you so much!  Here is a reward for your effort.

				if ( reward != null )
					from.AddToBackpack( reward );

				if ( gold > 1000 )
					from.AddToBackpack( new BankCheck( gold ) );
				else if ( gold > 0 )
					from.AddToBackpack( new Gold( gold ) );

				Titles.AwardFame( from, fame, true );

                OnSuccessfulBulkOrderReceive(from);

                if (Core.ML)
                {
                    ((PlayerMobile)from).NextBODTurnInTime = DateTime.UtcNow + TimeSpan.FromSeconds(10.0);
                }

				dropped.Delete();
				return true;
			}

			return base.OnDragDrop( from, dropped );
		}

		private GenericBuyInfo LookupDisplayObject( object obj )
		{
			var buyInfo = GetBuyInfo();

			for ( var i = 0; i < buyInfo.Length; ++i ) {
				var gbi = (GenericBuyInfo)buyInfo[i];

				if ( gbi.GetDisplayEntity() == obj )
					return gbi;
			}

			return null;
		}

        private void ProcessSinglePurchase(BuyItemResponse buy, IBuyItemInfo bii, List<BuyItemResponse> validBuy, ref int controlSlots, ref bool fullPurchase, ref int totalCost)
        {
			var amount = buy.Amount;

			if ( amount > bii.Amount )
				amount = bii.Amount;

			if ( amount <= 0 )
				return;

			//int slots = bii.ControlSlots * amount;

            //if ( controlSlots >= slots )
            //{
            //    controlSlots -= slots;
            //}
            //else
            //{
            //    fullPurchase = false;
            //    return;
            //}

			totalCost += bii.Price * amount;
			validBuy.Add( buy );
		}

		private static void ProcessValidPurchase( int amount, IBuyItemInfo bii, Mobile buyer, Container cont )
		{
			if ( amount > bii.Amount )
				amount = bii.Amount;

			if ( amount < 1 )
				return;

			bii.Amount -= amount;

			var o = bii.GetEntity();

			if ( o is Item )
			{
				var item = (Item)o;

				if ( item.Stackable )
				{
					item.Amount = amount;

					if ( cont == null || !cont.TryDropItem( buyer, item, false ) )
						item.MoveToWorld( buyer.Location, buyer.Map );
				}
				else
				{
					item.Amount = 1;

					if ( cont == null || !cont.TryDropItem( buyer, item, false ) )
						item.MoveToWorld( buyer.Location, buyer.Map );

					for (var i=1;i<amount;i++)
					{
						item = bii.GetEntity() as Item;

						if ( item != null )
						{
							item.Amount = 1;

							if ( cont == null || !cont.TryDropItem( buyer, item, false ) )
								item.MoveToWorld( buyer.Location, buyer.Map );
						}
					}
				}
			}
			else if ( o is Mobile )
			{
				var m = (Mobile)o;

				m.Direction = (Direction)Utility.Random( 8 );
				m.MoveToWorld( buyer.Location, buyer.Map );
				m.PlaySound( m.GetIdleSound() );

				if ( m is BaseCreature )
					((BaseCreature)m).SetControlMaster( buyer );

				for ( var i = 1; i < amount; ++i )
				{
					m = bii.GetEntity() as Mobile;

					if ( m != null )
					{
						m.Direction = (Direction)Utility.Random( 8 );
						m.MoveToWorld( buyer.Location, buyer.Map );

						if ( m is BaseCreature )
							((BaseCreature)m).SetControlMaster( buyer );
					}
				}
			}
		}

        public virtual bool OnBuyItems(Mobile buyer, List<BuyItemResponse> list)
        {
			if ( !IsActiveSeller )
				return false;

			if ( !buyer.CheckAlive() )
				return false;

            if (!buyer.InRange(this, 5) || !buyer.InLOS(this))
            {
                buyer.SendLocalizedMessage(500295); // You are too far away to do that.
                return false;
            }

            if ( !CheckVendorAccess( buyer ) )
			{
				Say( 501522 ); // I shall not treat with scum like thee!
				return false;
			}

			UpdateBuyInfo();

            //IBuyItemInfo[] buyInfo = GetBuyInfo();
            IShopSellInfo[] info = GetSellInfo();
			int totalCost = 0;
            List<BuyItemResponse> validBuy = new List<BuyItemResponse>(list.Count);
            bool fromBank = false;
            bool fullPurchase = true;
			int controlSlots = buyer.FollowersMax - buyer.Followers;

			foreach ( BuyItemResponse buy in list )
			{
				var ser = buy.Serial;
				var amount = buy.Amount;

				if ( ser.IsItem )
				{
					var item = World.FindItem( ser );

					if ( item == null )
						continue;

					var gbi = LookupDisplayObject( item );

					if ( gbi != null )
					{
						ProcessSinglePurchase( buy, gbi, validBuy, ref controlSlots, ref fullPurchase, ref totalCost );
					}
					else if ( item.RootParent == this )
					{
						if ( amount > item.Amount )
							amount = item.Amount;

						if ( amount <= 0 )
							continue;

						foreach ( var ssi in info )
						{
							if ( ssi.IsSellable( item ) )
							{
								if ( ssi.IsResellable( item ) )
								{
									totalCost += ssi.GetBuyPriceFor( item ) * amount;
									validBuy.Add( buy );
									break;
								}
							}
						}
					}
				}
				else if ( ser.IsMobile )
				{
					var mob = World.FindMobile( ser );

					if ( mob == null )
						continue;

					var gbi = LookupDisplayObject( mob );

					if ( gbi != null )
						ProcessSinglePurchase( buy, gbi, validBuy, ref controlSlots, ref fullPurchase, ref totalCost );
				}
			}//foreach

			if ( fullPurchase && validBuy.Count == 0 )
				SayTo( buyer, 500190 ); // Thou hast bought nothing!
			else if ( validBuy.Count == 0 )
				SayTo( buyer, 500187 ); // Your order cannot be fulfilled, please try again.

			if ( validBuy.Count == 0 )
				return false;

            //Does RunUO already have a similar method?
            var bought = ConsumeBackpackAndBankGold(buyer, totalCost);        
            
            /*else
            {//Keeping this just in case there might be some mobiles buying.

                int toWithdraw = 0; //backpack + bank buying

                cont = buyer.Backpack;
                if (!bought && cont != null)
                {
                    if (cont.ConsumeTotal(typeof(Gold), totalCost))
                        bought = true;
                    else
                        toWithdraw = cont.TotalGold; //Current backpack gold
                }

                if (!bought)
                {
                    cont = buyer.FindBankNoCreate();

                    //Prolly cant be, but anyways...
                    if (toWithdraw < 0)
                        toWithdraw = 0;

                    if (cont != null && cont.ConsumeTotal(typeof(Gold), totalCost - toWithdraw))
                    {
                        //If part of the cost is paied through backpack
                        if (toWithdraw > 0)
                            buyer.Backpack.ConsumeTotal(typeof(Gold), toWithdraw);

                        bought = true;
                        fromBank = true;
                    }
                    else
                    {
                        SayTo(buyer, 500191); //Begging thy pardon, but thy bank account lacks these funds.
                    }
                }
            }*/

			if ( !bought )
            {
                SayTo(buyer, true, "Alas, thou dost not possess sufficient gold for this purchase!");
				return false;
            }
		    buyer.PlaySound( 0x32 );

		    var cont = buyer.Backpack;
			if ( cont == null )
				cont = buyer.BankBox;

			foreach ( BuyItemResponse buy in validBuy )
			{
				var ser = buy.Serial;
				var amount = buy.Amount;

				if ( amount < 1 )
					continue;

				if ( ser.IsItem )
				{
					var item = World.FindItem( ser );

					if ( item == null )
						continue;

					var gbi = LookupDisplayObject( item );

					if ( gbi != null )
					{
						ProcessValidPurchase( amount, gbi, buyer, cont );
					}
					else
					{
						if ( amount > item.Amount )
							amount = item.Amount;

						foreach ( var ssi in info )
						{
							if ( ssi.IsSellable( item ) )
							{
								if ( ssi.IsResellable( item ) )
								{
									Item buyItem;
									if ( amount >= item.Amount )
									{
										buyItem = item;
									}
									else
									{
										buyItem = LiftItemDupe( item, item.Amount - amount );

										if ( buyItem == null )
											buyItem = item;
									}

									if ( cont == null || !cont.TryDropItem( buyer, buyItem, false ) )
										buyItem.MoveToWorld( buyer.Location, buyer.Map );

									break;
								}
							}
						}
					}
				}
				else if ( ser.IsMobile )
				{
					var mob = World.FindMobile( ser );

					if ( mob == null )
						continue;

					var gbi = LookupDisplayObject( mob );

					if ( gbi != null )
						ProcessValidPurchase( amount, gbi, buyer, cont );
				}
			}//foreach

			if ( fullPurchase )
			{
				//if ( buyer.AccessLevel >= AccessLevel.GameMaster )
				//	SayTo( buyer, true, "I would not presume to charge thee anything.  Here are the goods you requested." );
				if ( fromBank )
					SayTo( buyer, true, "The total of thy purchase is {0} gold, which has been withdrawn from your bank account.  My thanks for the patronage.", totalCost );
				else
					SayTo( buyer, true, "The total of thy purchase is {0} gold.  My thanks for the patronage.", totalCost );
			}
			else
			{
				//if ( buyer.AccessLevel >= AccessLevel.GameMaster )
				//	SayTo( buyer, true, "I would not presume to charge thee anything.  Unfortunately, I could not sell you all the goods you requested." );
				if ( fromBank )
					SayTo( buyer, true, "The total of thy purchase is {0} gold, which has been withdrawn from your bank account.  My thanks for the patronage.  Unfortunately, I could not sell you all the goods you requested.", totalCost );
				else
					SayTo( buyer, true, "The total of thy purchase is {0} gold.  My thanks for the patronage.  Unfortunately, I could not sell you all the goods you requested.", totalCost );
			}

			return true;
		}

		public virtual bool CheckVendorAccess( Mobile from )
		{
			var reg = (GuardedRegion) Region.GetRegion( typeof( GuardedRegion ) );

			if ( reg != null && !reg.CheckVendorAccess( this, from ) )
				return false;

			if ( Region != from.Region )
			{
				reg = (GuardedRegion) from.Region.GetRegion( typeof( GuardedRegion ) );

				if ( reg != null && !reg.CheckVendorAccess( this, from ) )
					return false;
			}

			return true;
		}

        public virtual bool OnSellItems(Mobile seller, List<SellItemResponse> list)
        {
			if ( !IsActiveBuyer )
				return false;

			if ( !seller.CheckAlive() )
				return false;

            if (!seller.InRange(this, 5) || !seller.InLOS(this))
            {
                seller.SendLocalizedMessage(500295); // You are too far away to do that.
                return false;
            }

			if ( !CheckVendorAccess( seller ) )
			{
				Say( 501522 ); // I shall not treat with scum like thee!
				return false;
			}

			seller.PlaySound( 0x32 );

			var info = GetSellInfo();
			var buyInfo = GetBuyInfo();
			var GiveGold = 0;
			var Sold = 0;
			Container cont;

			foreach ( SellItemResponse resp in list )
			{
				if ( resp.Item.RootParent != seller || resp.Amount <= 0 || !resp.Item.IsStandardLoot() || !resp.Item.Movable || (resp.Item is Container && resp.Item.Items.Count != 0) )
					continue;

				foreach( var ssi in info )
				{
					if ( ssi.IsSellable( resp.Item ) )
					{
						Sold++;
						break;
					}
				}
			}

			if ( Sold > MaxSell )
			{
				SayTo( seller, true, "You may only sell {0} items at a time!", MaxSell );
				return false;
			}
		    if ( Sold == 0 )
		    {
		        return true;
		    }

		    foreach ( SellItemResponse resp in list )
			{
				if ( resp.Item.RootParent != seller || resp.Amount <= 0 || !resp.Item.IsStandardLoot() || !resp.Item.Movable || (resp.Item is Container && resp.Item.Items.Count != 0) )
					continue;

				foreach( var ssi in info )
				{
					if ( ssi.IsSellable( resp.Item ) )
					{
						var amount = resp.Amount;

						if ( amount > resp.Item.Amount )
							amount = resp.Item.Amount;

						if ( ssi.IsResellable( resp.Item ) )
						{
							var found = false;

							foreach ( var bii in buyInfo )
							{
								if ( bii.Restock( resp.Item, amount ) )
								{
									resp.Item.Consume( amount );
									found = true;

									break;
								}
							}

							if ( !found )
							{
								cont = BuyPack;

								if ( amount < resp.Item.Amount )
								{
									var item = LiftItemDupe( resp.Item, resp.Item.Amount - amount );

									if ( item != null )
									{
										item.SetLastMoved();
										cont.DropItem( item );
									}
									else
									{
										resp.Item.SetLastMoved();
										cont.DropItem( resp.Item );
									}
								}
								else
								{
									resp.Item.SetLastMoved();
									cont.DropItem( resp.Item );
								}
							}
						}
						else
						{
							if ( amount < resp.Item.Amount )
								resp.Item.Amount -= amount;
							else
								resp.Item.Delete();
						}

						GiveGold += ssi.GetSellPriceFor( resp.Item )*amount;
						break;
					}
				}
			}

			if ( GiveGold > 0 )
			{
				while ( GiveGold > 60000 )
				{
					seller.AddToBackpack( new Gold( 60000 ) );
					GiveGold -= 60000;
				}

				seller.AddToBackpack( new Gold( GiveGold ) );

				seller.PlaySound( 0x0037 );//Gold dropping sound

				if ( SupportsBulkOrders( seller ) )
				{
					var bulkOrder = CreateBulkOrder( seller, false );

					if ( bulkOrder is LargeBOD )
						seller.SendGump( new LargeBODAcceptGump( seller, (LargeBOD)bulkOrder ) );
					else if ( bulkOrder is SmallBOD )
						seller.SendGump( new SmallBODAcceptGump( seller, (SmallBOD)bulkOrder ) );
				}
			}
			//no cliloc for this?
			//SayTo( seller, true, "Thank you! I bought {0} item{1}. Here is your {2}gp.", Sold, (Sold > 1 ? "s" : ""), GiveGold );

			return true;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 1 ); // version

            List<SBInfo> sbInfos = this.SBInfos;

			for ( var i = 0; sbInfos != null && i < sbInfos.Count; ++i )
			{
                SBInfo sbInfo = sbInfos[i];
                List<GenericBuyInfo> buyInfo = sbInfo.BuyInfo;

				for ( var j = 0; buyInfo != null && j < buyInfo.Count; ++j )
				{
					var gbi = (GenericBuyInfo)buyInfo[j];

					var maxAmount = gbi.MaxAmount;
					var doubled = 0;

					switch ( maxAmount )
					{
						case  40: doubled = 1; break;
						case  80: doubled = 2; break;
						case 160: doubled = 3; break;
						case 320: doubled = 4; break;
						case 640: doubled = 5; break;
						case 999: doubled = 6; break;
					}

					if ( doubled > 0 )
					{
						writer.WriteEncodedInt( 1 + ((j * sbInfos.Count) + i) );
						writer.WriteEncodedInt( doubled );
					}
				}
			}

			writer.WriteEncodedInt( 0 );
		}

        private static void UpdateVendorTables(BaseVendor vendor)
        {
            if (!vendor.Deleted)
            {
                Vendors[vendor.Serial] = vendor;
                for (int i = 0; i < vendor.Skills.Length; i++)
                {
                    if (vendor.Skills[i].Base >= 30)
                    {
                        if (!SkilledVendors.ContainsKey(vendor.Skills[i].SkillName))
                            SkilledVendors[vendor.Skills[i].SkillName] = new HashSet<BaseVendor>();
                        SkilledVendors[vendor.Skills[i].SkillName].Add(vendor);
                    }
                }
            }
        }

        private static void DeleteVendorTables(BaseVendor vendor)
        {
            if (Vendors.ContainsKey(vendor.Serial))
                Vendors.Remove(vendor.Serial);
            for (int i = 0; i < vendor.Skills.Length; i++)
            {
                if (vendor.Skills[i].Base > 80)
                {
                    if (SkilledVendors.ContainsKey(vendor.Skills[i].SkillName) && SkilledVendors[vendor.Skills[i].SkillName].Contains(vendor))
                        SkilledVendors[vendor.Skills[i].SkillName].Remove(vendor);
                }
            }
        }

        public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

            UpdateVendorTables(this);

            int version = reader.ReadInt();

			LoadSBInfo();

            List<SBInfo> sbInfos = this.SBInfos;

			switch ( version )
			{
				case 1:
				{
					int index;

					while ( (index = reader.ReadEncodedInt()) > 0 )
					{
						var doubled = reader.ReadEncodedInt();

						if ( sbInfos != null )
						{
							index -= 1;
							var sbInfoIndex = index % sbInfos.Count;
							var buyInfoIndex = index / sbInfos.Count;
 
							if ( sbInfoIndex >= 0 && sbInfoIndex < sbInfos.Count )
							{
                                SBInfo sbInfo = sbInfos[sbInfoIndex];
                                List<GenericBuyInfo> buyInfo = sbInfo.BuyInfo;

								if ( buyInfo != null && buyInfoIndex >= 0 && buyInfoIndex < buyInfo.Count )
								{
									var gbi = (GenericBuyInfo)buyInfo[buyInfoIndex];

									var amount = 20;

									switch ( doubled )
									{
										case 1: amount =  40; break;
										case 2: amount =  80; break;
										case 3: amount = 160; break;
										case 4: amount = 320; break;
										case 5: amount = 640; break;
										case 6: amount = 999; break;
									}

									gbi.Amount = gbi.MaxAmount = amount;
								}
							}
						}
					}

					break;
				}
			}

			if ( IsParagon )
				IsParagon = false;

			if ( Core.AOS && NameHue == 0x35 )
				NameHue = -1;

			Timer.DelayCall( TimeSpan.Zero, new TimerCallback( CheckMorph ) );
		}

		public override void AddCustomContextEntries( Mobile from, List<ContextMenuEntry> list )
		{
			if ( from.Alive && IsActiveVendor )
			{
                if (SupportsBulkOrders(from))
                    list.Add(new BulkOrderInfoEntry(from, this));

				if ( IsActiveSeller )
					list.Add( new VendorBuyEntry( from, this ) );

				if ( IsActiveBuyer )
					list.Add( new VendorSellEntry( from, this ) );
				list.Add(new VendorParcelMenu(from, this));
			}

			base.AddCustomContextEntries( from, list );
		}

		public virtual IShopSellInfo[] GetSellInfo()
		{
			return (IShopSellInfo[])m_ArmorSellInfo.ToArray( typeof( IShopSellInfo ) );
		}

		public virtual IBuyItemInfo[] GetBuyInfo()
		{
			return (IBuyItemInfo[])m_ArmorBuyInfo.ToArray( typeof( IBuyItemInfo ) );
		}

		public override bool CanBeDamaged()
		{
			return !IsInvulnerable;
		}

        public static bool ConsumeBackpackAndBankGold(Mobile buyer, int toConsume)
        {
            var backpackGold = 0;

            if (buyer.Backpack != null)
                if (buyer.Backpack.TotalGold >= toConsume)
                {
                    buyer.Backpack.ConsumeTotal(typeof(Gold), toConsume);
                    return true;
                }
                else
                    backpackGold = buyer.Backpack.TotalGold;

            if (buyer.FindBankNoCreate() != null)
                if (buyer.BankBox.TotalGold >= (toConsume - backpackGold))
                {
                    //It's pretty much a "null" check
                    if (backpackGold > 0)
                        buyer.Backpack.ConsumeTotal(typeof(Gold), backpackGold);

                    buyer.BankBox.ConsumeTotal(typeof(Gold), (toConsume - backpackGold));
                    return true;
                }

            return false;
        }
	}
}

namespace Server.ContextMenus
{
	public class VendorBuyEntry : ContextMenuEntry
	{
		private readonly BaseVendor m_Vendor;

		public VendorBuyEntry( Mobile from, BaseVendor vendor ) : base( 6103, 8 )
		{
			m_Vendor = vendor;
			Enabled = vendor.CheckVendorAccess( from );
		}

		public override void OnClick()
		{
			m_Vendor.VendorBuy( Owner.From );
		}
	}

	public class VendorSellEntry : ContextMenuEntry
	{
		private readonly BaseVendor m_Vendor;

		public VendorSellEntry( Mobile from, BaseVendor vendor ) : base( 6104, 8 )
		{
			m_Vendor = vendor;
			Enabled = vendor.CheckVendorAccess( from );
		}

		public override void OnClick()
		{
			m_Vendor.VendorSell( Owner.From );
		}
	}
}

namespace Server
{
	public interface IShopSellInfo
	{
		//get display name for an item
		string GetNameFor( Item item );

		//get price for an item which the player is selling
		int GetSellPriceFor( Item item );

		//get price for an item which the player is buying
		int GetBuyPriceFor( Item item );

		//can we sell this item to this vendor?
		bool IsSellable( Item item );

		//What do we sell?
		Type[] Types{ get; }

		//does the vendor resell this item?
		bool IsResellable( Item item );
	}

	public interface IBuyItemInfo
	{
		//get a new instance of an object (we just bought it)
		IEntity GetEntity();

		int ControlSlots{ get; }

		int PriceScalar{ get; set; }

		//display price of the item
		int Price{ get; }

		//display name of the item
		string Name{ get; }

		//display hue
		int Hue{ get; }

		//display id
		int ItemID{ get; }

		//amount in stock
		int Amount{ get; set; }

		//max amount in stock
		int MaxAmount{ get; }

		//Attempt to restock with item, (return true if restock sucessful)
		bool Restock( Item item, int amount );

		//called when its time for the whole shop to restock
		void OnRestock();
	}
}