using CCGLogic.Utils.ChessGame.GridBoardGame;

namespace CCGLogic.Games.Chess
{
    public enum ChessPieceType
    {
        King,
        Queen,
        Bishop,
        Knight,
        Rook,
        Pawn
    }

    public enum ChessPieceColor
    {
        White,
        Black,
        None
    }

    public static class ChessPieceColorExtensions
    {
        public static ChessPieceColor Opponent(this ChessPieceColor color) => color switch
        {
            ChessPieceColor.White => ChessPieceColor.Black,
            ChessPieceColor.Black => ChessPieceColor.White,
            _ => ChessPieceColor.None
        };
    }

    public abstract class ChessPiece(ChessBoard board, ChessPieceColor color)
    {
        protected ChessBoard board = board;

        public abstract ChessPieceType Type { get; }
        public ChessPieceColor Color { get; } = color;

        public bool HasMoved { get; set; }
        public GridPosition Position => board.GetPiecePos(this);

        public abstract IEnumerable<ChessMove> GetMoves();

        protected bool CanMoveTo(GridPosition pos) =>
            board.IsInside(pos) && board.IsEmpty(pos);
        protected bool CanCaptureAt(GridPosition pos) =>
            board.IsInside(pos) && !board.IsEmpty(pos) && board[pos].Color != Color;

        private IEnumerable<GridPosition> MultiStepMovePositionsInDir(GridDirection dir)
        {
            for (GridPosition pos = Position + dir; board.IsInside(pos); pos += dir)
            {
                if (CanMoveTo(pos))
                {
                    yield return pos;
                    continue;
                }

                if (CanCaptureAt(pos))
                {
                    yield return pos;
                }
                break;
            }
        }
    }

    public class King(ChessBoard board, ChessPieceColor color) : ChessPiece(board, color)
    {
        public override ChessPieceType Type => ChessPieceType.King;

        public override IEnumerable<ChessMove> GetMoves()
        {
            return [];
        }
    }

    public class Queen(ChessBoard board, ChessPieceColor color) : ChessPiece(board, color)
    {
        public override ChessPieceType Type => ChessPieceType.Queen;

        public override IEnumerable<ChessMove> GetMoves()
        {
            return [];
        }
    }

    public class Bishop(ChessBoard board, ChessPieceColor color) : ChessPiece(board, color)
    {
        public override ChessPieceType Type => ChessPieceType.Bishop;

        public override IEnumerable<ChessMove> GetMoves()
        {
            return [];
        }
    }

    public class Knight(ChessBoard board, ChessPieceColor color) : ChessPiece(board, color)
    {
        public override ChessPieceType Type => ChessPieceType.Knight;

        public override IEnumerable<ChessMove> GetMoves()
        {
            return [];
        }
    }

    public class Rook(ChessBoard board, ChessPieceColor color) : ChessPiece(board, color)
    {
        public override ChessPieceType Type => ChessPieceType.Rook;

        public override IEnumerable<ChessMove> GetMoves()
        {
            return [];
        }
    }

    public class Pawn(ChessBoard board, ChessPieceColor color) : ChessPiece(board, color)
    {
        public override ChessPieceType Type => ChessPieceType.Pawn;

        public override IEnumerable<ChessMove> GetMoves()
        {
            return [];
        }
    }
}
