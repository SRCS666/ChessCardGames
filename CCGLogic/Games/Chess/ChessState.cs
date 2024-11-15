﻿namespace CCGLogic.Games.Chess
{
    public class ChessState
    {
        public ChessBoard Board { get; }
        public ChessPieceColor CurrentPlayer { get; set; }

        public ChessState() => Board = new();

        public void AddStartPieces()
        {
            Board[0, 0] = new Rook(Board, ChessPieceColor.Black);
            Board[0, 1] = new Knight(Board, ChessPieceColor.Black);
            Board[0, 2] = new Bishop(Board, ChessPieceColor.Black);
            Board[0, 3] = new Queen(Board, ChessPieceColor.Black);
            Board[0, 4] = new King(Board, ChessPieceColor.Black);
            Board[0, 5] = new Bishop(Board, ChessPieceColor.Black);
            Board[0, 6] = new Knight(Board, ChessPieceColor.Black);
            Board[0, 7] = new Rook(Board, ChessPieceColor.Black);

            Board[7, 0] = new Rook(Board, ChessPieceColor.White);
            Board[7, 1] = new Knight(Board, ChessPieceColor.White);
            Board[7, 2] = new Bishop(Board, ChessPieceColor.White);
            Board[7, 3] = new Queen(Board, ChessPieceColor.White);
            Board[7, 4] = new King(Board, ChessPieceColor.White);
            Board[7, 5] = new Bishop(Board, ChessPieceColor.White);
            Board[7, 6] = new Knight(Board, ChessPieceColor.White);
            Board[7, 7] = new Rook(Board, ChessPieceColor.White);

            for (int c = 0; c < 8; c++)
            {
                Board[1, c] = new Pawn(Board, ChessPieceColor.Black);
                Board[6, c] = new Pawn(Board, ChessPieceColor.White);
            }
        }

        private void ChangePlayer()
        {
            CurrentPlayer = CurrentPlayer.Opponent();
        }

        public static IEnumerable<ChessMove> LegalMovesForPiece(ChessPiece piece) =>
            piece.GetMoves().Where(move => move.IsLegal());

        public void MakeMove(ChessMove move)
        {
            move.Execute();
            Board.GetMoveHistory().Push(move);
            ChangePlayer();
        }
    }
}
