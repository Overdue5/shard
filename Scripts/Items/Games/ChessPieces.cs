namespace Server.Items
{
	public class PieceWhiteKing : BasePiece
	{
		public override string DefaultName => "white king";

        public PieceWhiteKing(BaseGameBoard board)
            : base(0x3587, board)
		{
		}

		public PieceWhiteKing( Serial serial ) : base( serial )
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

	public class PieceBlackKing : BasePiece
	{
		public override string DefaultName => "black king";

        public PieceBlackKing(BaseGameBoard board)
            : base(0x358E, board)
		{
		}

		public PieceBlackKing( Serial serial ) : base( serial )
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

	public class PieceWhiteQueen : BasePiece
	{
		public override string DefaultName => "white queen";

        public PieceWhiteQueen(BaseGameBoard board)
            : base(0x358A, board)
		{
		}

		public PieceWhiteQueen( Serial serial ) : base( serial )
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

	public class PieceBlackQueen : BasePiece
	{
		public override string DefaultName => "black queen";

        public PieceBlackQueen(BaseGameBoard board)
            : base(0x3591, board)
		{
		}

		public PieceBlackQueen( Serial serial ) : base( serial )
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

	public class PieceWhiteRook : BasePiece
	{
		public override string DefaultName => "white rook";

        public PieceWhiteRook(BaseGameBoard board)
            : base(0x3586, board)
		{
		}

		public PieceWhiteRook( Serial serial ) : base( serial )
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

	public class PieceBlackRook : BasePiece
	{
		public override string DefaultName => "black rook";

        public PieceBlackRook(BaseGameBoard board)
            : base(0x358D, board)
		{
		}

		public PieceBlackRook( Serial serial ) : base( serial )
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

	public class PieceWhiteBishop : BasePiece
	{
		public override string DefaultName => "white bishop";

        public PieceWhiteBishop(BaseGameBoard board)
            : base(0x3585, board)
		{
		}

		public PieceWhiteBishop( Serial serial ) : base( serial )
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

	public class PieceBlackBishop : BasePiece
	{
		public override string DefaultName => "black bishop";

        public PieceBlackBishop(BaseGameBoard board)
            : base(0x358C, board)
		{
		}

		public PieceBlackBishop( Serial serial ) : base( serial )
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

	public class PieceWhiteKnight : BasePiece
	{
		public override string DefaultName => "white knight";

        public PieceWhiteKnight(BaseGameBoard board)
            : base(0x3588, board)
		{
		}

		public PieceWhiteKnight( Serial serial ) : base( serial )
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

	public class PieceBlackKnight : BasePiece
	{
		public override string DefaultName => "black knight";

        public PieceBlackKnight(BaseGameBoard board)
            : base(0x358F, board)
		{
		}

		public PieceBlackKnight( Serial serial ) : base( serial )
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

	public class PieceWhitePawn : BasePiece
	{
		public override string DefaultName => "white pawn";

        public PieceWhitePawn(BaseGameBoard board)
            : base(0x3589, board)
		{
		}

		public PieceWhitePawn( Serial serial ) : base( serial )
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

	public class PieceBlackPawn : BasePiece
	{
		public override string DefaultName => "black pawn";

        public PieceBlackPawn(BaseGameBoard board)
            : base(0x3590, board)
		{
		}

		public PieceBlackPawn( Serial serial ) : base( serial )
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
