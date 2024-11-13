namespace CCGLogic.Utils.ChessGame.GridBoardGame
{
    public abstract class GridBoard
    {
        public abstract int RowCount { get; }
        public abstract int ColumnCount { get; }

        public bool IsInside(GridPosition pos) =>
            pos.Row >= 0 && pos.Row < RowCount &&
            pos.Column >= 0 && pos.Column < ColumnCount;

        public GridPosition Rotate(GridPosition pos) =>
            new(RowCount - pos.Row - 1, ColumnCount - pos.Column - 1);
    }
}
