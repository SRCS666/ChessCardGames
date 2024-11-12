using CCGLogic.Games.Chess;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace CCG.Games.Chess
{
    public class ChessImages
    {
        public static ChessImages Instance { get; } = new();

        private readonly Dictionary<ChessPieceType, ImageSource> whiteSources = [];
        private readonly Dictionary<ChessPieceType, ImageSource> blackSources = [];

        private readonly ImageSource backgroundSource;

        public ChessImages()
        {
            foreach (ChessPieceType type in Enum.GetValues(typeof(ChessPieceType)))
            {
                whiteSources[type] = LoadImage(string.Format(@"Images\Chess\White{0}.png", type.ToString()));
                blackSources[type] = LoadImage(string.Format(@"Images\Chess\Black{0}.png", type.ToString()));
            }

            backgroundSource = LoadImage(@"Images\Chess\Board.png");
        }

        private static BitmapImage LoadImage(string filePath) => new(new(filePath, UriKind.Relative));

        public ImageSource GetImage(ChessPieceColor color, ChessPieceType type) => color switch
        {
            ChessPieceColor.White => whiteSources[type],
            ChessPieceColor.Black => blackSources[type],
            _ => null
        };

        public ImageSource GetImage(ChessPiece piece) => piece == null ? null : GetImage(piece.Color, piece.Type);
        public ImageSource GetBackground() => backgroundSource;
    }
}
