using System;
using Server.Logging;
using Server.Mobiles;
using Server.Multis;
using Server.Regions;

namespace Server.Items
{
	public class ContractOfEmployment : Item
	{
		public override int LabelNumber => 1041243; // a contract of employment

		[Constructable]
		public ContractOfEmployment() : base( 0x14F0 )
		{
                        Hue = 2546;
			Weight = 1.0;
			//LootType = LootType.Blessed;
		}

		public ContractOfEmployment( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); //version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else if ( from.AccessLevel >= AccessLevel.GameMaster )
			{
				from.SendLocalizedMessage( 503248 ); // Your godly powers allow you to place this vendor whereever you wish.

				Mobile v = new PlayerVendor( from, BaseHouse.FindHouseAt( from ) );

				v.Direction = from.Direction & Direction.Mask;
				v.MoveToWorld( from.Location, from.Map );

				v.SayTo( from, 503246 ); // Ah! it feels good to be working again.

				Delete();
			}
			else
			{
                bool canplace = false;
				BaseHouse house = BaseHouse.FindHouseAt( from );
                CustomRegion cR;

                if ((cR = from.Region as CustomRegion) != null && !cR.Controller.CanPlaceVendors)
                {
                    if (house == null)
                        from.SendAsciiMessage("Vendors can only be placed in houses or specified areas.");
                    else
                        from.SendAsciiMessage("You cannot place a vendor in this region");
                }
                else if ((cR = from.Region as CustomRegion) != null && cR.Controller.CanPlaceVendors)
                {
                    canplace = true;

                    try
                    {
                    foreach (Mobile mob in cR.GetMobiles())
                    {
                        if (mob is PlayerVendor && (mob as PlayerVendor).Owner.Account == from.Account)
                        {
                            from.SendAsciiMessage("You alread have a vendor placed in this region.");
                            canplace = false;
                            return;
                        }
                    }
                    }
                    catch (Exception e)
                    {
                    	ConsoleLog.Write.Warning(e);
                    }
                }
				else if ( house == null )
				{
                    from.SendAsciiMessage("Vendors can only be placed in houses or specified areas.");
				}
				else if ( !BaseHouse.NewVendorSystem && !house.IsFriend( from ) )
				{
					from.SendLocalizedMessage( 503242 ); // You must ask the owner of this building to name you a friend of the household in order to place a vendor here.
				}
				else if ( BaseHouse.NewVendorSystem && !house.IsOwner( from ) )
				{
					from.SendLocalizedMessage( 1062423 ); // Only the house owner can directly place vendors.  Please ask the house owner to offer you a vendor contract so that you may place a vendor in this house.
				}
				else if ( !house.Public || !house.CanPlaceNewVendor() ) 
				{
					from.SendLocalizedMessage( 503241 ); // You cannot place this vendor or barkeep.  Make sure the house is public and has sufficient storage available.
				}
                else
                {
                    canplace = true;
                }

                if ( canplace )
				{
					bool vendor, contract;
					BaseHouse.IsThereVendor( from.Location, from.Map, out vendor, out contract );

					if ( vendor )
					{
						from.SendLocalizedMessage( 1062677 ); // You cannot place a vendor or barkeep at this location.
					}
					else if ( contract )
					{
						from.SendLocalizedMessage( 1062678 ); // You cannot place a vendor or barkeep on top of a rental contract!
					}
					else
					{
						Mobile v = new PlayerVendor( from, house );

						v.Direction = from.Direction & Direction.Mask;
						v.MoveToWorld( from.Location, from.Map );

						v.SayTo( from, 503246 ); // Ah! it feels good to be working again.

						Delete();
					}
				}
			}
		}
	}
}