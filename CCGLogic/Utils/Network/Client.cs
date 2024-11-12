using System.Net;
using System.Text.Json.Nodes;

namespace CCGLogic.Utils.Network
{
    public enum SignupType
    {
        CreateNewRoom,
        AddToRoom
    }

    public abstract class Client
    {
        public abstract GameType GameType { get; }

        public event Action<string> ErrorMessage;
        public event Action<Client, JsonArray> Signedup;

        private delegate void ClientCommand(JsonArray arguments);

        private ClientSocket clientSocket;

        private readonly Dictionary<CmdOperation, ClientCommand> callbacks = [];

        private readonly List<ClientPlayer> players = [];

        public Client()
        {
            InitCallbacks();

            ClientPlayer.Self = new(this);
            players.Add(ClientPlayer.Self);

            clientSocket = new();

            clientSocket.ErrorMessage += SendErrorMessage;
            clientSocket.MessageGot += ProcessServerCommand;
        }

        public void SendMessage(byte[] bytes) => clientSocket.SendMessage(bytes);
        private void SendErrorMessage(string message) => ErrorMessage?.Invoke(message);
        public void Connect(IPAddress address, int port) => clientSocket.Connect(address, port);

        public void Disconnect()
        {
            clientSocket.Disconnect();
            clientSocket = null;
        }

        private void ProcessServerCommand(ClientSocket clientSocket, byte[] data)
        {
            Command command = Command.Parse(data);
            callbacks[command.Operation](command.Arguments);
        }

        private void InitCallbacks()
        {
            callbacks[CmdOperation.COSignup] = Signup;
            callbacks[CmdOperation.COSignupResult] = SignupResult;
        }

        private void NotifyServer(CmdOperation operation, JsonArray arguments)
        {
            Command command = new(CmdWhere.CWClient, CmdWhere.CWRoom,
                CmdType.CTNotification, operation, arguments);
            SendMessage(command.ToBytes());
        }

        private void Signup(JsonArray arguments)
        {
            JsonArray array = [];
            array.Add(Convert.ToInt32(Config.Instance.SignupType));
            array.Add(Convert.ToInt32(Config.Instance.GameType));
            array.Add(Config.Instance.RoomNumber);
            array.Add(Config.Instance.ScreenName);

            NotifyServer(CmdOperation.COSignup, array);
        }

        private void SignupResult(JsonArray arguments) => Signedup?.Invoke(this, arguments);
    }
}
