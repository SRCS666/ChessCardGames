using CCGLogic.Utils;
using System.Windows;

namespace CCG.Utils.Network
{
    public partial class ServerConfigDialog : Window
    {
        public ServerConfigDialog()
        {
            InitializeComponent();

            TextBoxPort.Text = GameConfig.Instance.StartServerPort.ToString();
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            int port;
            if (TextBoxPort.Text == string.Empty)
            {
                port = GameConfig.DefaultPort;
            }
            else
            {
                bool portValid = int.TryParse(TextBoxPort.Text, out port);
                if (!portValid)
                {
                    MessageBox.Show(this, "Invalid port!", "Warning",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            GameConfig.Instance.StartServerPort = port;

            DialogResult = true;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
