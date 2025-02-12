namespace Server.Items
{
	public class PieceWhiteChecker : BasePiece
	{
		public override string DefaultName => "white checker";

        public PieceWhiteChecker( BaseGameBoard board ) : base( 0x3584, board )
		{
		}

		public PieceWhiteChecker( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class PieceBlackChecker : BasePiece
	{
		public override string DefaultName => "black checker";

        public PieceBlackChecker(BaseGameBoard board)
            : base(0x358B, board)
		{
		}

		public PieceBlackChecker( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}