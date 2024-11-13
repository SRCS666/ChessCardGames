using CCG.Utils;
using CCGLogic.Games.Chess;
using CCGLogic.Utils.ChessGame.GridBoardGame;
using System.Windows.Controls;
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

        public ChessScene(MainWindow mainWindow, ChessClient client)
        {
            InitializeComponent();
            InitializeBoard();

            this.mainWindow = mainWindow;
            this.client = client;

            client.StateInitialized += InitState;
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
                    pieces[drawPos.Row, drawPos.Column].Background = new ImageBrush()
                    {
                        ImageSource = ChessImages.Instance.GetImage(piece)
                    };
                }
            }
        }

        private void InitState() => Dispatcher.Invoke(DrawBoard);
    }
}
