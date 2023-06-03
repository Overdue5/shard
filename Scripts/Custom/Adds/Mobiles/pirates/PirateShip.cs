namespace Server.Multis
{
    public class PirateShip_Boat : BaseBoat
    {
        public override int NorthID => 0x4014;
        public override int EastID => 0x4015;
        public override int SouthID => 0x4016;
        public override int WestID => 0x4017;

        public override int HoldDistance => 5;
        public override int TillerManDistance => -5;

        public override Point2D StarboardOffset => new Point2D(2, -1);
        public override Point2D PortOffset => new Point2D(-2, -1);

        public override Point3D MarkOffset => new Point3D(0, 0, 3);

        public override BaseDockedBoat DockedBoat => new LargeDockedDragonBoat(this);

        [Constructable]
        public PirateShip_Boat()
        {
            Name = "A Pirate Ship";
            
        }

        public PirateShip_Boat(Serial serial): base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }
    }

    public class PirateShip_Boat_Deed : BaseBoatDeed
    {
        public override int LabelNumber => 1041210;
        public override BaseBoat Boat => new PirateShip_Boat();

        [Constructable]
        public PirateShip_Boat_Deed(): base(0x4014, new Point3D(0, -1, 0))
        {
            Name = "A Pirate Ship";
            Hue = 1157;
        }

        public PirateShip_Boat_Deed(Serial serial): base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }
    }

    public class PirateShip_Boat_Docked : BaseDockedBoat
    {
        public override BaseBoat Boat => new LargeDragonBoat();

        public PirateShip_Boat_Docked(BaseBoat boat): base(0x4014, new Point3D(0, -1, 0), boat)
        {
        }

        public PirateShip_Boat_Docked(Serial serial): base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }
    }
}