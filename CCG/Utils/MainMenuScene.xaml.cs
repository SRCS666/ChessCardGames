using System.Windows;
using System.Windows.Controls;

namespace CCG.Utils
{
    public partial class MainMenuScene : UserControl
    {
        private readonly MainWindow mainWindow;

        public MainMenuScene(MainWindow mainWindow)
        {
            InitializeComponent();

            this.mainWindow = mainWindow;
        }

        private void ButtonAddToGame_Click(object sender, RoutedEventArgs e) => mainWindow.AddToGame();
        private void ButtonStartServer_Click(object sender, RoutedEventArgs e) => mainWindow.StartServer();
        private void ButtonExitGame_Click(object sender, RoutedEventArgs e) => mainWindow.ExitGame();
    }
}
