using CCG.Games.Chess;
using CCG.Utils.Network;
using CCGLogic.Games.Chess;
using CCGLogic.Utils;
using CCGLogic.Utils.Network;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

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
            AddToGameDialog dialog = new() { Owner = this };
            if (!dialog.ShowDialog().Value)
            {
                return;
            }

            client = new ChessClient();
            client.Signedup += SignupResult;
            client.ErrorMessage += NetworkError;

            client.Connect(Config.Instance.AddToGameIP, Config.Instance.AddToGamePort);
        }

        private void SignupResult(Client client, SignupResultType result, string reason)
        {
            if (result == SignupResultType.Successed)
            {
                ChangeToGameScene();
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(this, reason, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                });
                client.Disconnect();
            }
        }

        private void NetworkError(string message) => Dispatcher.Invoke(() =>
        {
            MessageBox.Show(this, message, "Network error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        });

        public void StartServer()
        {
            ServerConfigDialog dialog = new() { Owner = this };
            if (!dialog.ShowDialog().Value)
            {
                return;
            }

            server = new();
            if (!server.Start())
            {
                MessageBox.Show(this, "Can not start server!", "Warning",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ChangeToServerScene();
        }

        public void ExitGame() => Close();
        private void Window_Closing(object sender, CancelEventArgs e) => Config.Instance.SaveConfig();

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

                UserControl control = Config.Instance.GameType switch
                {
                    GameType.Chess => new ChessScene(this, client as ChessClient),
                    _ => null
                };
                CCGScene.Content = control;
            });
        }
    }
}
