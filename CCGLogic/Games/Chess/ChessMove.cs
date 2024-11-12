using CCGLogic.Utils.ChessGame.GridBoardGame;

namespace CCGLogic.Games.Chess
{
    public enum ChessMoveType
    {
        NormalMove,
        DoublePawn,
        EnPassant,
        PawnPromotion,
        CastleKS,
        CastleQS
    }

    public abstract class ChessMove
    {
        protected ChessBoard board;

        public abstract ChessMoveType Type { get; }

        public GridPosition FromPos { get; }
        public GridPosition ToPos { get; }

        public ChessPiece PieceToMove { get; protected set; }
        public ChessPiece PieceToBeCaptured { get; protected set; }
        public bool PieceToMoveHasMoved { get; protected set; }

        public bool IsPawnOrCapture => PieceToMove.Type == ChessPieceType.Pawn || PieceToBeCaptured != null;

        public ChessMove(ChessBoard board, GridPosition fromPos, GridPosition toPos)
        {
            this.board = board;
            FromPos = fromPos;
            ToPos = toPos;

            InitializeInfo();
        }

        protected virtual void InitializeInfo()
        {
            PieceToMove = board[FromPos];
            PieceToBeCaptured = board[ToPos];
            PieceToMoveHasMoved = PieceToMove.HasMoved;
        }

        public abstract void Execute();
        public abstract void Cancel();
    }

    public class NormalMove(ChessBoard board, GridPosition fromPos, GridPosition toPos) : ChessMove(board, fromPos, toPos)
    {
        public override ChessMoveType Type => ChessMoveType.NormalMove;

        public override void Execute()
        {
            board[ToPos] = PieceToMove;
            board[FromPos] = null;
            PieceToMove.HasMoved = true;
        }

        public override void Cancel()
        {
            board[ToPos] = PieceToBeCaptured;
            board[FromPos] = PieceToMove;
            PieceToMove.HasMoved = PieceToMoveHasMoved;
        }
    }

    public class DoublePawn(ChessBoard board, GridPosition fromPos, GridPosition toPos) : NormalMove(board, fromPos, toPos)
    {
        public override ChessMoveType Type => ChessMoveType.DoublePawn;
    }

    public class EnPassant(ChessBoard board, GridPosition fromPos, GridPosition toPos) : NormalMove(board, fromPos, toPos)
    {
        public override ChessMoveType Type => ChessMoveType.EnPassant;

        private readonly GridPosition capturePos = new(fromPos.Row, toPos.Column);

        protected override void InitializeInfo()
        {
            base.InitializeInfo();
            PieceToBeCaptured = board[capturePos];
        }

        public override void Execute()
        {
            base.Execute();
            board[capturePos] = null;
        }

        public override void Cancel()
        {
            base.Cancel();
            board[capturePos] = PieceToBeCaptured;
        }
    }
}
