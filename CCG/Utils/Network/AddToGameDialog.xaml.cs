using CCGLogic.Utils;
using CCGLogic.Utils.Network;
using System.Net;
using System.Windows;

namespace CCG.Utils.Network
{
    public partial class AddToGameDialog : Window
    {
        private List<string> signupTypeOptions = [];
        private Dictionary<SignupType, string> signupTypeDic = [];
        private List<GameType> gameTypeOptions = [];

        public AddToGameDialog()
        {
            InitializeComponent();

            signupTypeOptions.Add("Create new room");
            signupTypeOptions.Add("Add to room");
            signupTypeDic[SignupType.CreateNewRoom] = "Create new room";
            signupTypeDic[SignupType.AddToRoom] = "Add to room";
            ComboBoxSignupType.ItemsSource = signupTypeOptions;

            foreach (GameType gameType in Enum.GetValues(typeof(GameType)))
            {
                gameTypeOptions.Add(gameType);
            }
            ComboBoxGameType.ItemsSource = gameTypeOptions;

            ComboBoxSignupType.SelectedIndex = signupTypeOptions.IndexOf(signupTypeDic[Config.Instance.SignupType]);
            ComboBoxGameType.SelectedIndex = gameTypeOptions.IndexOf(Config.Instance.GameType);

            TextBoxIP.Text = Config.Instance.AddToGameIP.ToString();
            TextBoxPort.Text = Config.Instance.AddToGamePort.ToString();
            TextBoxRoomNumber.Text = Config.Instance.RoomNumber.ToString();
            TextBoxUsername.Text = Config.Instance.ScreenName;
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            IPAddress ip;
            if (TextBoxIP.Text == string.Empty)
            {
                ip = IPAddress.Loopback;
            }
            else
            {
                bool addressValid = IPAddress.TryParse(TextBoxIP.Text, out ip);
                if (!addressValid)
                {
                    MessageBox.Show(this, "Invalid IP!", "Warning",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            int port;
            if (TextBoxIP.Text == string.Empty)
            {
                port = Config.DefaultPort;
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

            int roomNumber;
            if (TextBoxRoomNumber.Text == string.Empty)
            {
                roomNumber = Config.DefaultRoomNumber;
            }
            else
            {
                bool roomNumberValid = int.TryParse(TextBoxRoomNumber.Text, out roomNumber);
                if (!roomNumberValid)
                {
                    MessageBox.Show(this, "Invalid room number!", "Warning",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            if (TextBoxUsername.Text == string.Empty)
            {
                MessageBox.Show(this, "Player name can not be empty!", "Warning",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Config.Instance.AddToGameIP = ip;
            Config.Instance.AddToGamePort = port;
            Config.Instance.RoomNumber = roomNumber;
            Config.Instance.ScreenName = TextBoxUsername.Text;

            Config.Instance.SignupType = signupTypeDic.Keys.FirstOrDefault(key => signupTypeDic[key] == ComboBoxSignupType.SelectedItem.ToString());
            Config.Instance.GameType = gameTypeOptions[ComboBoxGameType.SelectedIndex];

            DialogResult = true;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
