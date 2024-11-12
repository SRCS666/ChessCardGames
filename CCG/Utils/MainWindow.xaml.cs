using CCG.Games.Chess;
using CCG.Utils.Network;
using CCGLogic.Games.Chess;
using CCGLogic.Utils.Network;
using System.Text.Json.Nodes;
using System.Windows;

namespace CCG.Utils
{
    public partial class MainWindow : Window
    {
        private Server server;
        private Client client;

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
            client = new ChessClient();
            client.Signedup += SignupResult;

            client.Connect();
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

        private void ChangeToGameScene()
        {
            Dispatcher.Invoke(() =>
            {
                MenuItemAddToGame.IsEnabled = false;
                MenuItemStartServer.IsEnabled = false;

                CCGScene.Content = new ChessScene(this, client as ChessClient);
            });
        }

        private void SignupResult(Client client, JsonArray arguments)
        {
            SignupResultType result = (SignupResultType)arguments[0].GetValue<int>();

            if (result == SignupResultType.Successed)
            {
                ChangeToGameScene();
            }
            else
            {
                string reason = arguments[1].GetValue<string>();
                MessageBox.Show(this, reason, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
