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

        protected bool CanMoveTo(GridPosition pos) => board.IsInside(pos) && board.IsEmpty(pos);
        protected bool CanCaptureAt(GridPosition pos) => board.IsInside(pos) && !board.IsEmpty(pos) && board[pos].Color != Color;
        protected bool CanMoveToOrCaptureAt(GridPosition pos) => CanMoveTo(pos) || CanCaptureAt(pos);

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

        protected IEnumerable<GridPosition> MultiStepMovePositionsInDirs(IEnumerable<GridDirection> dirs) =>
            dirs.SelectMany(MultiStepMovePositionsInDir);

        public static ChessPiece CreatePieceFromType(ChessPieceType type,
            ChessBoard board, ChessPieceColor color) => type switch
        {
            ChessPieceType.King => new King(board, color),
            ChessPieceType.Queen => new Queen(board, color),
            ChessPieceType.Bishop => new Bishop(board, color),
            ChessPieceType.Knight => new Knight(board, color),
            ChessPieceType.Rook => new Rook(board, color),
            ChessPieceType.Pawn => new Pawn(board, color),
            _ => null
        };
    }

    public abstract class MultiStepPiece(ChessBoard board, ChessPieceColor color) : ChessPiece(board, color)
    {
        public abstract IEnumerable<GridDirection> Directions { get; }

        public override IEnumerable<ChessMove> GetMoves() => MultiStepMovePositionsInDirs(Directions)
            .Select(to => new NormalMove(board, Position, to));
    }

    public class King(ChessBoard board, ChessPieceColor color) : ChessPiece(board, color)
    {
        public override ChessPieceType Type => ChessPieceType.King;

        public override IEnumerable<ChessMove> GetMoves()
        {
            return [];
        }
    }

    public class Queen(ChessBoard board, ChessPieceColor color) : MultiStepPiece(board, color)
    {
        public override ChessPieceType Type => ChessPieceType.Queen;

        public override IEnumerable<GridDirection> Directions => GridDirection.EightDirections;
    }

    public class Bishop(ChessBoard board, ChessPieceColor color) : MultiStepPiece(board, color)
    {
        public override ChessPieceType Type => ChessPieceType.Bishop;

        public override IEnumerable<GridDirection> Directions => GridDirection.DiagonalDirections;
    }

    public class Knight(ChessBoard board, ChessPieceColor color) : ChessPiece(board, color)
    {
        public override ChessPieceType Type => ChessPieceType.Knight;

        private IEnumerable<GridPosition> PotentialMovePositionsForKnight()
        {
            foreach (GridDirection vDir in GridDirection.NorthAndSouth)
            {
                foreach (GridDirection hDir in GridDirection.EastAndWest)
                {
                    yield return Position + 2 * vDir + hDir;
                    yield return Position + 2 * hDir + vDir;
                }
            }
        }

        public override IEnumerable<ChessMove> GetMoves() => PotentialMovePositionsForKnight()
            .Where(CanMoveToOrCaptureAt).Select(to => new NormalMove(board, Position, to));
    }

    public class Rook(ChessBoard board, ChessPieceColor color) : MultiStepPiece(board, color)
    {
        public override ChessPieceType Type => ChessPieceType.Rook;

        public override IEnumerable<GridDirection> Directions => GridDirection.StraightDirections;
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
