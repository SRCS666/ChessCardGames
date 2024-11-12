using CCG.Utils;
using CCGLogic.Games.Chess;
using System.Windows.Controls;

namespace CCG.Games.Chess
{
    public partial class ChessScene : UserControl
    {
        private readonly MainWindow mainWindow;
        private readonly ChessClient client;

        public ChessScene(MainWindow mainWindow, ChessClient client)
        {
            InitializeComponent();

            this.mainWindow = mainWindow;
            this.client = client;
        }
    }
}
