namespace CCGLogic.Utils.ChessGame.GridBoardGame
{
    public class GridPosition(int row, int column)
    {
        public int Row { get; } = row;
        public int Column { get; } = column;

        public override bool Equals(object obj) => obj is GridPosition position &&
            Row == position.Row && Column == position.Column;
        public override int GetHashCode() => HashCode.Combine(Row, Column);

        public static bool operator ==(GridPosition left, GridPosition right) => EqualityComparer<GridPosition>.Default.Equals(left, right);
        public static bool operator !=(GridPosition left, GridPosition right) => !(left == right);

        public static GridPosition operator +(GridPosition pos, GridDirection dir) =>
            new(pos.Row + dir.RowDelta, pos.Column + dir.ColumnDelta);
    }
}
