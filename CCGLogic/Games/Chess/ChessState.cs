namespace CCGLogic.Games.Chess
{
    public class ChessState
    {
        public ChessBoard Board { get; }
        public ChessPieceColor CurrentPlayer { get; private set; }
    }
}
