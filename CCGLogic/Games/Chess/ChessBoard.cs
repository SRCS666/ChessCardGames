using CCGLogic.Utils.ChessGame.GridBoardGame;

namespace CCGLogic.Games.Chess
{
    public class ChessBoard : GridBoard
    {
        public override int RowCount => 8;
        public override int ColumnCount => 8;

        private readonly ChessPiece[,] pieces = new ChessPiece[8, 8];
        private readonly Stack<ChessMove> moveHistory = [];

        public ChessPiece this[int row, int column]
        {
            get { return pieces[row, column]; }
            set { pieces[row, column] = value; }
        }

        public ChessPiece this[GridPosition pos]
        {
            get { return pieces[pos.Row, pos.Column]; }
            set { pieces[pos.Row, pos.Column] = value; }
        }

        public Stack<ChessMove> GetMoveHistory() => moveHistory;

        public GridPosition GetPiecePos(ChessPiece piece)
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    if (this[r, c] == piece)
                    {
                        return new(r, c);
                    }
                }
            }
            return null;
        }

        public bool IsEmpty(GridPosition pos) => this[pos] == null;

        private IEnumerable<GridPosition> PiecePositions()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    GridPosition pos = new(r, c);
                    if (!IsEmpty(pos))
                    {
                        yield return pos;
                    }
                }
            }
        }

        private IEnumerable<GridPosition> PiecePositionsFor(ChessPieceColor color) =>
            PiecePositions().Where(pos => this[pos].Color == color);

        public bool IsInCheck(ChessPieceColor color) => PiecePositionsFor(color.Opponent())
            .Any(pos => this[pos].CanCaptureOpponentKing());
    }
}
