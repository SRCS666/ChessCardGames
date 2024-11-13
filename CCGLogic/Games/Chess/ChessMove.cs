using CCGLogic.Utils;
using CCGLogic.Utils.ChessGame.GridBoardGame;
using System.Text.Json.Nodes;

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

        public static ChessMove CreateMoveFromJsonArray(ChessBoard board, JsonArray array)
        {
            ChessMoveType type = (ChessMoveType)array[0].GetValue<int>();
            GridPosition fromPos = GridPosition.Parse(Command.StringToJsonArray(array[1].ToString()));
            GridPosition toPos = GridPosition.Parse(Command.StringToJsonArray(array[2].ToString()));

            return type switch
            {
                ChessMoveType.NormalMove => new NormalMove(board, fromPos, toPos),
                ChessMoveType.DoublePawn => new DoublePawn(board, fromPos, toPos),
                ChessMoveType.EnPassant => new EnPassant(board, fromPos, toPos),
                ChessMoveType.PawnPromotion => new PawnPromotion(board, fromPos, toPos,
                    (ChessPieceType)array[3].GetValue<int>()),
                _ => null
            };
        }

        public JsonArray ToJsonArray()
        {
            JsonArray array = [];

            array.Add(Convert.ToInt32(Type));
            array.Add(FromPos.ToJsonArray());
            array.Add(ToPos.ToJsonArray());

            if (Type == ChessMoveType.PawnPromotion)
            {
                PawnPromotion prom = this as PawnPromotion;
                array.Add(Convert.ToInt32(prom.NewType));
            }

            return array;
        }

        protected virtual void InitializeInfo()
        {
            PieceToMove = board[FromPos];
            PieceToBeCaptured = board[ToPos];
            PieceToMoveHasMoved = PieceToMove.HasMoved;
        }

        public virtual bool IsLegal()
        {
            ChessPieceColor color = PieceToMove.Color;
            Execute();

            bool legal = !board.IsInCheck(color);
            Cancel();

            return legal;
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

    public class PawnPromotion(ChessBoard board, GridPosition fromPos, GridPosition toPos,
        ChessPieceType newType) : ChessMove(board, fromPos, toPos)
    {
        public override ChessMoveType Type => ChessMoveType.PawnPromotion;
        public ChessPieceType NewType = newType;

        public override void Execute()
        {
            ChessPiece promotionPiece = ChessPiece.CreatePieceFromType(NewType, board, PieceToMove.Color);

            board[ToPos] = promotionPiece;
            board[FromPos] = null;
            promotionPiece.HasMoved = true;
        }

        public override void Cancel()
        {
            board[ToPos] = PieceToBeCaptured;
            board[FromPos] = PieceToMove;
            PieceToMove.HasMoved = PieceToMoveHasMoved;
        }
    }
}
