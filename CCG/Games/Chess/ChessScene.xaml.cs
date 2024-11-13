using CCG.Utils;
using CCGLogic.Games.Chess;
using CCGLogic.Utils.ChessGame.GridBoardGame;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CCG.Games.Chess
{
    public partial class ChessScene : UserControl
    {
        private readonly MainWindow mainWindow;
        private readonly ChessClient client;

        private readonly Rectangle[,] highlights = new Rectangle[8, 8];
        private readonly UserControl[,] pieces = new UserControl[8, 8];

        private readonly Dictionary<GridPosition, List<ChessMove>> moveCache = [];
        private GridPosition selectedPos;

        private Color highlightSelected = Color.FromArgb(127, 0, 127, 255);
        private Color highlightCanMoveTo = Color.FromArgb(127, 0, 191, 255);
        private Color highlightSpecialMove = Color.FromArgb(127, 127, 0, 191);
        private Color highlightCanCaptureAt = Color.FromArgb(127, 255, 127, 191);
        private Color highlightChecked = Color.FromArgb(150, 127, 0, 0);

        public ChessScene(MainWindow mainWindow, ChessClient client)
        {
            InitializeComponent();
            InitializeBoard();

            this.mainWindow = mainWindow;
            this.client = client;

            client.StateInitialized += InitState;
            client.MoveMade += UpdateMove;
        }

        private void InitializeBoard()
        {
            BoardGrid.Background = new ImageBrush()
            {
                ImageSource = ChessImages.Instance.GetBackground()
            };

            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Rectangle highlight = new();
                    highlights[r, c] = highlight;
                    HighlightGrid.Children.Add(highlight);

                    UserControl control = new();
                    pieces[r, c] = control;
                    PieceGrid.Children.Add(control);
                }
            }
        }

        private void DrawBoard()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    GridPosition pos = new(r, c);
                    ChessBoard board = client.State.Board;
                    ChessPiece piece = board[pos];

                    GridPosition drawPos = client.WillRotate ? board.Rotate(pos) : pos;
                    Dispatcher.Invoke(() =>
                    {
                        pieces[drawPos.Row, drawPos.Column].Background = new ImageBrush()
                        {
                            ImageSource = ChessImages.Instance.GetImage(piece)
                        };
                    });
                }
            }
        }

        private void UpdateHighlights()
        {
            Dispatcher.Invoke(() =>
            {
                for (int r = 0; r < 8; r++)
                {
                    for (int c = 0; c < 8; c++)
                    {
                        SolidColorBrush brush = Brushes.Transparent;

                        GridPosition pos = new(r, c);
                        if (client.WillRotate)
                        {
                            pos = client.State.Board.Rotate(pos);
                        }

                        ChessPiece piece = client.State.Board[pos];
                        if (piece != null && piece.Type == ChessPieceType.King &&
                            client.State.Board.IsInCheck(piece.Color))
                        {
                            brush = new(highlightChecked);
                        }

                        if (selectedPos != null)
                        {
                            if (pos == selectedPos)
                            {
                                brush = new(highlightSelected);
                            }
                            else if (moveCache.TryGetValue(pos, out List<ChessMove> moves))
                            {
                                if (piece == null)
                                {
                                    ChessMove move = moves.First();
                                    if (move.Type == ChessMoveType.EnPassant ||
                                        move.Type == ChessMoveType.PawnPromotion ||
                                        move.Type == ChessMoveType.CastleKS ||
                                        move.Type == ChessMoveType.CastleQS)
                                    {
                                        brush = new(highlightSpecialMove);
                                    }
                                    else
                                    {
                                        brush = new(highlightCanMoveTo);
                                    }
                                }
                                else
                                {
                                    brush = new(highlightCanCaptureAt);
                                }
                            }
                        }

                        highlights[r, c].Fill = brush;
                    }
                }
            });
        }

        private void InitState() => Dispatcher.Invoke(DrawBoard);

        private GridPosition PointToSquarePosition(Point point)
        {
            double squareSize = BoardGrid.ActualWidth / 8;

            int row = (int)(point.Y / squareSize);
            int column = (int)(point.X / squareSize);

            return new(row, column);
        }

        private void BoardGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(BoardGrid);

            GridPosition pos = PointToSquarePosition(point);
            if (client.WillRotate)
            {
                pos = client.State.Board.Rotate(pos);
            }

            if (selectedPos == null)
            {
                OnFromPositionSelected(pos);
            }
            else
            {
                OnToPositionSelected(pos);
            }
        }

        private void OnFromPositionSelected(GridPosition pos)
        {
            ChessPiece piece = client.State.Board[pos];
            if (piece != null &&
                piece.Color == client.State.CurrentPlayer &&
                piece.Color == client.SelfColor)
            {
                SelectPiece(pos);
            }
        }

        private void OnToPositionSelected(GridPosition pos)
        {
            if (moveCache.TryGetValue(pos, out List<ChessMove> moves))
            {
                ChessMove move = moves.First();
                client.NotifyMove(move);
            }
            else
            {
                CancelSelection();
            }
        }

        private void SelectPiece(GridPosition pos)
        {
            IEnumerable<ChessMove> moves = ChessState.LegalMovesForPiece(client.State.Board[pos]);

            selectedPos = pos;
            CacheMoves(moves);
            UpdateHighlights();
        }

        private void CacheMoves(IEnumerable<ChessMove> moves)
        {
            foreach (ChessMove move in moves)
            {
                if (moveCache.TryGetValue(move.ToPos, out List<ChessMove> moves1))
                {
                    moves1.Add(move);
                }
                else
                {
                    moveCache[move.ToPos] = [move];
                }
            }
        }

        private void UpdateMove()
        {
            DrawBoard();
            CancelSelection();
        }

        private void CancelSelection()
        {
            selectedPos = null;
            moveCache.Clear();
            UpdateHighlights();
        }
    }
}
