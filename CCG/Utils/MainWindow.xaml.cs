using CCG.Utils.Network;
using CCGLogic.Utils.Network;
using System.Windows;

namespace CCG.Utils
{
    public partial class MainWindow : Window
    {
        private Server server;

        public MainWindow()
        {
            InitializeComponent();

            CCGScene.Content = new MainMenuScene(this);
        }

        private void MenuItemAddToGame_Click(object sender, RoutedEventArgs e) => AddToGame();
        private void MenuItemStartServer_Click(object sender, RoutedEventArgs e) => StartServer();
        private void MenuItemExitGame_Click(object sender, RoutedEventArgs e) => ExitGame();

        public void AddToGame()
        {

        }

        public void StartServer()
        {
            server = new();
            if (!server.Start())
            {
                MessageBox.Show(this, "Can not start server!", "Warning",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ChangeToServerScene();
        }

        public void ExitGame()
        {
            if (MessageBox.Show(this, "Do you really want to exit game?", "Init",
                MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                Application.Current.Shutdown();
            }
        }

        private void ChangeToServerScene()
        {
            MenuItemAddToGame.IsEnabled = false;
            MenuItemStartServer.IsEnabled = false;

            CCGScene.Content = new ServerScene(this, server);
        }
    }
}
