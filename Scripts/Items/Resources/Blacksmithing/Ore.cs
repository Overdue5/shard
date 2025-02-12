using System;
using Server.Engines.Craft;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
	public abstract class BaseOre : Item, ICommodity
	{
		private CraftResource m_Resource;

		[CommandProperty( AccessLevel.GameMaster )]
		public CraftResource Resource
		{
			get => m_Resource;
            set{ m_Resource = value; InvalidateProperties(); }
		}

        int ICommodity.DescriptionNumber => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public abstract BaseIngot GetIngot();

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, string.Format("{0} {1} ore{2}", Amount, CraftResources.GetName(m_Resource).ToLower(), Amount != 1 ? "s" : ""));
        }

        protected override void OnAmountChange(int oldValue)
        {
            base.OnAmountChange(oldValue);

            switch (Amount)
            {
                case 1:
                    ItemID = 6583;
                    break;
                case 2:
                    ItemID = 6586;
                    break;
                case 3:
                    ItemID = 6584;
                    break;
                default:
                    ItemID = 6585;
                    break;
            }
        }

        public override bool StackWith(Mobile from, Item dropped, bool playSound)
        {
            if (dropped is BaseOre && ((BaseOre)dropped).Resource == Resource)
                dropped.ItemID = ItemID; //Taran: Change ItemID so ores with different amounts can stack (since they have different ID's)

            return base.StackWith(from, dropped, playSound);
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 1 ); // version

			writer.Write( (int) m_Resource );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_Resource = (CraftResource)reader.ReadInt();
					break;
				}
				case 0:
				{
					OreInfo info;

					switch ( reader.ReadInt() )
					{
						case 0: info = OreInfo.Iron; break;
						case 1: info = OreInfo.DullCopper; break;
						case 2: info = OreInfo.ShadowIron; break;
						case 3: info = OreInfo.Copper; break;
						case 4: info = OreInfo.Bronze; break;
						case 5: info = OreInfo.Gold; break;
						case 6: info = OreInfo.Agapite; break;
						case 7: info = OreInfo.Verite; break;
						case 8: info = OreInfo.Valorite; break;
						default: info = null; break;
					}

					m_Resource = CraftResources.GetFromOreInfo( info );
					break;
				}
			}
		}

		public BaseOre( CraftResource resource ) : this( resource, 1 )
		{
		}

        //public BaseOre( CraftResource resource, int amount ) : base( Utility.Random( 4 ) )
		public BaseOre( CraftResource resource, int amount ) : base( 0x19B9 )
		{
			Stackable = true;
			Weight = 2.5;
			Amount = amount;

			Hue = CraftResources.GetHue( resource );

			m_Resource = resource;

            switch (Amount)
            {
                case 1:
                    ItemID = 6583;
                    break;
                case 2:
                    ItemID = 6586;
                    break;
                case 3:
                    ItemID = 6584;
                    break;
                default:
                    ItemID = 6585;
                    break;
            }
		}

		public BaseOre( Serial serial ) : base( serial )
		{
		}

		public override void AddNameProperty( ObjectPropertyList list )
		{
			if ( Amount > 1 )
				list.Add( 1050039, "{0}\t#{1}", Amount, 1026583 ); // ~1_NUMBER~ ~2_ITEMNAME~
			else
				list.Add( 1026583 ); // ore
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( !CraftResources.IsStandard( m_Resource ) )
			{
				int num = CraftResources.GetLocalizationNumber( m_Resource );

				if ( num > 0 )
					list.Add( num );
				else
					list.Add( CraftResources.GetName( m_Resource ) );
			}
		}

		public override int LabelNumber
		{
			get
			{
				if ( m_Resource >= CraftResource.DullCopper && m_Resource <= CraftResource.Valorite )
					return 1042845 + (m_Resource - CraftResource.DullCopper);

				return 1042853; // iron ore;
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !Movable )
				return;

            if (RootParent is BaseCreature)
            {
                from.SendLocalizedMessage(500447); // That is not accessible
                return;
            }

		    if (from.InRange(GetWorldLocation(), 3) && from.InLOS(this))
		        SmeltOre(from);
		    else
		        from.SendLocalizedMessage( 501976 ); // The ore is too far away.
		}

        public object IsForgeClose(Mobile from)
        {
            IPooledEnumerable eable = from.Map.GetItemsInRange(from.Location, 3);

            foreach (Item item in eable)
            {
                Type type = item.GetType();

                bool isForge = (type.IsDefined(typeof(ForgeAttribute), false) || item.ItemID == 4017 || (item.ItemID >= 6522 && item.ItemID <= 6569));

                if (isForge)
                    return item;
            }

            return null;
        }

        protected void SmeltOre(Mobile from)
        {
            if (Deleted)
                return;

            if (!from.InRange(GetWorldLocation(), 4) || !from.InLOS(this))
            {
                from.SendAsciiMessage("The ore is too far away.");
                return;
            }

            if (IsForgeClose(from) == null)
            {
                from.SendAsciiMessage("You neeed to stand close to a forge to do this.");
                return;
            }

            double difficulty;

            switch (Resource)
            {
                default:
                    difficulty = 0.0;
                    break;
                case CraftResource.OldCopper:
                    difficulty = 35.0;
                    break;
                case CraftResource.ShadowIron:
                    difficulty = 50.0;
                    break;
                case CraftResource.Silver:
                    difficulty = 55.0;
                    break;
                case CraftResource.Verite:
                    difficulty = 60.0;
                    break;
                case CraftResource.Rose:
                    difficulty = 65.0;
                    break;
                case CraftResource.Gold:
                    difficulty = 70.0;
                    break;
                case CraftResource.Ice:
                    difficulty = 72.5;
                    break;
                case CraftResource.Havoc:
                    difficulty = 72.5;
                    break;
                case CraftResource.Valorite:
                    difficulty = 75.0;
                    break;
                case CraftResource.BloodRock:
                    difficulty = 75.0;
                    break;
                case CraftResource.Aqua:
                    difficulty = 80.0;
                    break;
                case CraftResource.Fire:
                    difficulty = 82.0;
                    break;
                case CraftResource.Mytheril:
                    difficulty = 87.5;
                    break;
                case CraftResource.SandRock:
                    difficulty = 90.0;
                    break;
                case CraftResource.BlackDiamond:
                    difficulty = 92.5;
                    break;
                case CraftResource.BlackRock:
                    difficulty = 95.0;
                    break;
                case CraftResource.Oceanic:
                    difficulty = 97.0;
                    break;
                case CraftResource.DaemonSteel:
                    difficulty = 99.0;
                    break;
                case CraftResource.Reactive:
                    difficulty = 99.5;
                    break;
                case CraftResource.Adamantium:
                    difficulty = 100.0;
                    break;
                case CraftResource.Sapphire:
                    difficulty = 100.0;
                    break;

                    //Unused
                case CraftResource.DullCopper:
                    difficulty = 65.0;
                    break;
                case CraftResource.Copper:
                    difficulty = 100.0;
                    break;
                case CraftResource.Bronze:
                    difficulty = 65.0;
                    break;
                case CraftResource.Agapite:
                    difficulty = 100.0;
                    break;
                case CraftResource.Opiate:
                    difficulty = 82.5;
                    break;
            }

            double minSkill = difficulty - 25.0;
            double maxSkill = difficulty + 25.0;

            if (difficulty > 50.0 && difficulty > from.Skills[SkillName.Mining].Value)
            {
                from.SendAsciiMessage("You do not have the required skill to smelt this ore!");
                return;
            }

            if (Amount <= 50 && from.Skills[SkillName.Mining].Value >= minSkill)
            {
                int toConsume = Amount;

                if (toConsume <= 0)
                {
                    from.SendLocalizedMessage(501987); // There is not enough metal-bearing ore in this pile to make an ingot.
                }
                else
                {
                    if (toConsume > 30000)
                        toConsume = 30000;

                    BaseIngot ingot = GetIngot();
                    ingot.Amount = toConsume;

                    Consume(toConsume);
                    from.AddToBackpack(ingot);
                    from.SendLocalizedMessage(501988); // You smelt the ore removing the impurities and put the metal in your backpack.
                }
            }
            else if (from.CheckTargetSkill(SkillName.Mining, IsForgeClose(from), minSkill, maxSkill))
            {
                int toConsume = Amount;

                if (toConsume <= 0)
                {
                    from.SendLocalizedMessage(501987); // There is not enough metal-bearing ore in this pile to make an ingot.
                }
                else
                {
                    if (toConsume > 30000)
                        toConsume = 30000;

                    BaseIngot ingot = GetIngot();
                    ingot.Amount = toConsume;

                    Consume(toConsume);
                    from.AddToBackpack(ingot);

                    from.SendLocalizedMessage(501988); // You smelt the ore removing the impurities and put the metal in your backpack.
                }
            }
            else if (Amount < 2)
            {
                from.SendLocalizedMessage(501989); // You burn away the impurities but are left with no useable metal.
                Delete();
            }
            else
            {
                from.SendLocalizedMessage(501990); // You burn away the impurities but are left with less useable metal.
                Amount /= 2;
            }
        }

		private class InternalTarget : Target
		{
			private readonly BaseOre m_Ore;

			public InternalTarget( BaseOre ore ) :  base ( 2, false, TargetFlags.None )
			{
				m_Ore = ore;
			}

			private bool IsForge( object obj )
			{
                if (Core.ML && obj is Mobile && ((Mobile)obj).IsDeadBondedPet)
                    return false;

				if ( obj.GetType().IsDefined( typeof( ForgeAttribute ), false ) )
					return true;

				int itemID = 0;

				if ( obj is Item )
					itemID = ((Item)obj).ItemID;
				else if ( obj is StaticTarget )
                    itemID = ((StaticTarget)obj).ItemID;

				return ( itemID == 4017 || (itemID >= 6522 && itemID <= 6569) );
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Ore.Deleted )
					return;

				if ( !from.InRange( m_Ore.GetWorldLocation(), 3 ) || !from.InLOS(m_Ore) )
				{
					from.SendLocalizedMessage( 501976 ); // The ore is too far away.
					return;
				}

                #region Combine Ore
                /*
                if (targeted is BaseOre)
                {
                    BaseOre ore = (BaseOre)targeted;
                    if (!ore.Movable)
                        return;
                    else if (m_Ore == ore)
                    {
                        from.SendLocalizedMessage(501972); // Select another pile or ore with which to combine this.
                        from.Target = new InternalTarget(ore);
                        return;
                    }
                    else if (ore.Resource != m_Ore.Resource)
                    {
                        from.SendLocalizedMessage(501979); // You cannot combine ores of different metals.
                        return;
                    }

                    int worth = ore.Amount;
                    if (ore.ItemID == 0x19B9)
                        worth *= 8;
                    else if (ore.ItemID == 0x19B7)
                        worth *= 2;
                    else
                        worth *= 4;
                    int sourceWorth = m_Ore.Amount;
                    if (m_Ore.ItemID == 0x19B9)
                        sourceWorth *= 8;
                    else if (m_Ore.ItemID == 0x19B7)
                        sourceWorth *= 2;
                    else
                        sourceWorth *= 4;
                    worth += sourceWorth;

                    int plusWeight = 0;
                    int newID = ore.ItemID;
                    if (ore.DefaultWeight != m_Ore.DefaultWeight)
                    {
                        if (ore.ItemID == 0x19B7 || m_Ore.ItemID == 0x19B7)
                        {
                            newID = 0x19B7;
                        }
                        else if (ore.ItemID == 0x19B9)
                        {
                            newID = m_Ore.ItemID;
                            plusWeight = ore.Amount * 2;
                        }
                        else
                        {
                            plusWeight = m_Ore.Amount * 2;
                        }
                    }

                    if ((ore.ItemID == 0x19B9 && worth > 120000) || ((ore.ItemID == 0x19B8 || ore.ItemID == 0x19BA) && worth > 60000) || (ore.ItemID == 0x19B7 && worth > 30000))
                    {
                        from.SendLocalizedMessage(1062844); // There is too much ore to combine.
                        return;
                    }
                    else if (ore.RootParent is Mobile && (plusWeight + ((Mobile)ore.RootParent).Backpack.TotalWeight) > ((Mobile)ore.RootParent).Backpack.MaxWeight)
                    {
                        from.SendLocalizedMessage(501978); // The weight is too great to combine in a container.
                        return;
                    }

                    ore.ItemID = newID;
                    if (ore.ItemID == 0x19B9)
                    {
                        ore.Amount = worth / 8;
                        m_Ore.Delete();
                    }
                    else if (ore.ItemID == 0x19B7)
                    {
                        ore.Amount = worth / 2;
                        m_Ore.Delete();
                    }
                    else
                    {
                        ore.Amount = worth / 4;
                        m_Ore.Delete();
                    }
                    return;
                }
                */
                #endregion

				if ( IsForge( targeted ) )
				{
					double difficulty;

					switch ( m_Ore.Resource )
					{
						default: difficulty = 50.0; break;
						case CraftResource.DullCopper: difficulty = 65.0; break;
						case CraftResource.ShadowIron: difficulty = 70.0; break;
						case CraftResource.Copper: difficulty = 75.0; break;
						case CraftResource.Bronze: difficulty = 80.0; break;
						case CraftResource.Gold: difficulty = 85.0; break;
						case CraftResource.Agapite: difficulty = 90.0; break;
						case CraftResource.Verite: difficulty = 95.0; break;
						case CraftResource.Valorite: difficulty = 99.0; break;
					}

					double minSkill = difficulty - 25.0;
					double maxSkill = difficulty + 25.0;
					
					if ( difficulty > 50.0 && difficulty > from.Skills[SkillName.Mining].Value )
					{
						from.SendLocalizedMessage( 501986 ); // You have no idea how to smelt this strange ore!
						return;
					}

                    /*
                    if (m_Ore.Amount <= 1 && m_Ore.ItemID == 0x19B7)
                    {
                        from.SendLocalizedMessage(501987); // There is not enough metal-bearing ore in this pile to make an ingot.
                        return;
                    }
                    */

					if ( from.CheckTargetSkill( SkillName.Mining, targeted, minSkill, maxSkill ) )
					{
						int toConsume = m_Ore.Amount;

						if ( toConsume <= 0 )
						{
							from.SendLocalizedMessage( 501987 ); // There is not enough metal-bearing ore in this pile to make an ingot.
						}
						else
						{
                            //int amount = m_Ore.Amount;
                            //if (m_Ore.Amount > 30000)
                            //    amount = 30000;

							if ( toConsume > 30000 )
								toConsume = 30000;

							BaseIngot ingot = m_Ore.GetIngot();

                            /*
                            if (m_Ore.ItemID == 0x19B7)
                            {
                                if (m_Ore.Amount % 2 == 0)
                                {
                                    amount /= 2;
                                    m_Ore.Delete();
                                }
                                else
                                {
                                    amount /= 2;
                                    m_Ore.Amount = 1;
                                }
                            }

                            else if (m_Ore.ItemID == 0x19B9)
                            {
                                amount *= 2;
                                m_Ore.Delete();
                            }

                            else
                            {
                                amount /= 1;
                                m_Ore.Delete();
                            }
                            
                            ingot.Amount = amount;
                            */

                            ingot.Amount = toConsume * 2;

							m_Ore.Consume( toConsume );
							from.AddToBackpack( ingot );

							from.SendLocalizedMessage( 501988 ); // You smelt the ore removing the impurities and put the metal in your backpack.
						}
					}
                    //else if ( m_Ore.Amount < 2 && m_Ore.ItemID == 0x19B9 )
					else if ( m_Ore.Amount < 2 )
					{
                        //from.SendLocalizedMessage( 501990 ); // You burn away the impurities but are left with less useable metal.
                        //m_Ore.ItemID = 0x19B8;
						from.SendLocalizedMessage( 501989 ); // You burn away the impurities but are left with no useable metal.
						m_Ore.Delete();
					}
                    /*
                    else if (m_Ore.Amount < 2 && m_Ore.ItemID == 0x19B8 || m_Ore.ItemID == 0x19BA)
                    {
                        from.SendLocalizedMessage(501990); // You burn away the impurities but are left with less useable metal.
                        m_Ore.ItemID = 0x19B7;
                    }
                    */
					else
					{
						from.SendLocalizedMessage( 501990 ); // You burn away the impurities but are left with less useable metal.
						m_Ore.Amount /= 2;
					}
				}
			}
		}
	}

    public class IronOre : BaseOre
	{
		[Constructable]
		public IronOre() : this( 1 )
		{
		}

		[Constructable]
		public IronOre( int amount ) : base( CraftResource.Iron, amount )
		{
		}

		public IronOre( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

        public override BaseIngot GetIngot()
		{
			return new IronIngot();
		}
	}

	public class DullCopperOre : BaseOre
	{
		[Constructable]
		public DullCopperOre() : this( 1 )
		{
		}

		[Constructable]
		public DullCopperOre( int amount ) : base( CraftResource.DullCopper, amount )
		{
		}

		public DullCopperOre( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		

		public override BaseIngot GetIngot()
		{
			return new DullCopperIngot();
		}
	}

	public class ShadowIronOre : BaseOre
	{
		[Constructable]
		public ShadowIronOre() : this( 1 )
		{
		}

		[Constructable]
		public ShadowIronOre( int amount ) : base( CraftResource.ShadowIron, amount )
		{
		}

		public ShadowIronOre( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		

		public override BaseIngot GetIngot()
		{
			return new ShadowIronIngot();
		}
	}

	public class CopperOre : BaseOre
	{
		[Constructable]
		public CopperOre() : this( 1 )
		{
		}

		[Constructable]
		public CopperOre( int amount ) : base( CraftResource.Copper, amount )
		{
		}

		public CopperOre( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		

		public override BaseIngot GetIngot()
		{
			return new CopperIngot();
		}
	}

	public class BronzeOre : BaseOre
	{
		[Constructable]
		public BronzeOre() : this( 1 )
		{
		}

		[Constructable]
		public BronzeOre( int amount ) : base( CraftResource.Bronze, amount )
		{
		}

		public BronzeOre( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		

		public override BaseIngot GetIngot()
		{
			return new BronzeIngot();
		}
	}

	public class GoldOre : BaseOre
	{
		[Constructable]
		public GoldOre() : this( 1 )
		{
		}

		[Constructable]
		public GoldOre( int amount ) : base( CraftResource.Gold, amount )
		{
		}

		public GoldOre( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override BaseIngot GetIngot()
		{
			return new GoldIngot();
		}
	}

	public class AgapiteOre : BaseOre
	{
		[Constructable]
		public AgapiteOre() : this( 1 )
		{
		}

		[Constructable]
		public AgapiteOre( int amount ) : base( CraftResource.Agapite, amount )
		{
		}

		public AgapiteOre( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override BaseIngot GetIngot()
		{
			return new AgapiteIngot();
		}
	}

	public class VeriteOre : BaseOre
	{
		[Constructable]
		public VeriteOre() : this( 1 )
		{
		}

		[Constructable]
		public VeriteOre( int amount ) : base( CraftResource.Verite, amount )
		{
		}

		public VeriteOre( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		

		public override BaseIngot GetIngot()
		{
			return new VeriteIngot();
		}
	}

	public class ValoriteOre : BaseOre
	{
		[Constructable]
		public ValoriteOre() : this( 1 )
		{
		}

		[Constructable]
		public ValoriteOre( int amount ) : base( CraftResource.Valorite, amount )
		{
		}

		public ValoriteOre( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override BaseIngot GetIngot()
		{
			return new ValoriteIngot();
		}
	}
}