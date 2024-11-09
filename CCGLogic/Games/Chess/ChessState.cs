namespace CCGLogic.Games.Chess
{
    public class ChessState
    {
        public ChessBoard Board { get; }
        public ChessPieceColor CurrentPlayer { get; private set; }

        private void ChangePlayer() => CurrentPlayer = CurrentPlayer.Opponent();
    }
}
