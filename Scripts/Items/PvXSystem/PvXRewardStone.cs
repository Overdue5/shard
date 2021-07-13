using Scripts.SpecialSystems;
using Server.Gumps; 


namespace Server.Items
{
    public class PvMRewardStone: PvXRewardStone
    {
        [Constructable]
        public PvMRewardStone() : base(3804)
        {
            Hue = 1154;
            Name = "Token Reward Terminal";
            boardType = PvXType.PVM;
        }

        public PvMRewardStone(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            boardType = PvXType.PVM;
            int version = reader.ReadInt();
        }
    }

    public class PvPRewardStone : PvXRewardStone
    {
        [Constructable]
        public PvPRewardStone() : base(3804)
        {
            Hue = 1154;
            Name = "Token Reward Terminal";
            boardType = PvXType.PVP;
        }

        public PvPRewardStone(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            boardType = PvXType.PVP;
            int version = reader.ReadInt();
        }
    }

    public abstract class PvXRewardStone : Item 
	{
        public PvXType boardType;

        public PvXRewardStone() : base( 3804 ) 
		{ 
			Movable = false; 
		} 

		public override void OnDoubleClick( Mobile from ) 
		{ 
			//if ( from.InRange( this.GetWorldLocation(), 2 ) ) 
			//{ 
			//	from.CloseGump( typeof( PvXRewardGump ) );
			//	from.SendGump( new PvXRewardGump() );
			//} 
		} 

		public PvXRewardStone( Serial serial ) : base( serial ) 
		{ 
		} 

		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 

			writer.Write( (int) 0 ); // version 
		} 

		public override void Deserialize( GenericReader reader ) 
		{ 
			base.Deserialize( reader ); 

			int version = reader.ReadInt(); 
		} 
	} 
} 