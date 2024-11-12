using System.Windows.Controls;
using System.Windows.Media;

namespace CCG.Games.Chess
{
    public partial class ChessBoardControl : UserControl
    {
        public ChessBoardControl()
        {
            InitializeComponent();

            BoardGrid.Background = new ImageBrush()
            {
                ImageSource = ChessImages.Instance.GetBackground()
            };
        }
    }
}
