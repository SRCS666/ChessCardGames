using CCGLogic.Utils.Network;
using System.Windows.Controls;

namespace CCG.Utils.Network
{
    public partial class ServerScene : UserControl
    {
        private readonly MainWindow mainWindow;
        private readonly Server server;

        public ServerScene(MainWindow mainWindow, Server server)
        {
            InitializeComponent();

            this.mainWindow = mainWindow;
            this.server = server;
        }
    }
}
