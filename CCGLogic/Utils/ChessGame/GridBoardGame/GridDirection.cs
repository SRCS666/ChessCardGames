namespace CCGLogic.Utils.ChessGame.GridBoardGame
{
    public class GridDirection(int rowDelta, int columnDelta)
    {
        public static readonly GridDirection North = new(-1, 0);
        public static readonly GridDirection South = new(1, 0);
        public static readonly GridDirection East = new(0, 1);
        public static readonly GridDirection West = new(0, -1);

        public static readonly GridDirection NorthEast = North + East;
        public static readonly GridDirection NorthWest = North + West;
        public static readonly GridDirection SouthEast = South + East;
        public static readonly GridDirection SouthWest = South + West;

        public int RowDelta { get; } = rowDelta;
        public int ColumnDelta { get; } = columnDelta;

        public static IEnumerable<GridDirection> NorthAndSouth => [North, South];
        public static IEnumerable<GridDirection> EastAndWest => [East, West];
        public static IEnumerable<GridDirection> StraightDirections => NorthAndSouth.Concat(EastAndWest);
        public static IEnumerable<GridDirection> DiagonalDirections => [NorthEast, NorthWest, SouthEast, SouthWest];
        public static IEnumerable<GridDirection> EightDirections => StraightDirections.Concat(DiagonalDirections);

        public static GridDirection operator +(GridDirection dir1, GridDirection dir2) =>
            new(dir1.RowDelta + dir2.RowDelta, dir1.ColumnDelta + dir2.ColumnDelta);
        public static GridDirection operator *(int scalar, GridDirection dir) =>
            new(scalar * dir.RowDelta, scalar * dir.ColumnDelta);
    }
}
