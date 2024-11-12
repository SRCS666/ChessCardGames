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

        public void Connect(IPAddress address, int port) => clientSocket.Connect(address, port);
        public void Connect(IPAddress address) => clientSocket.Connect(address);
        public void Connect() => clientSocket.Connect();

        public void Disconnect()
        {
            clientSocket.Disconnect();
            clientSocket = null;
        }

        private void SendErrorMessage(string message) => ErrorMessage?.Invoke(message);

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
            array.Add(Convert.ToInt32(SignupType.CreateNewRoom));
            array.Add(Convert.ToInt32(GameType.Chess));
            array.Add(1234);
            array.Add("Plaayer");

            NotifyServer(CmdOperation.COSignup, array);
        }

        private void SignupResult(JsonArray arguments) => Signedup?.Invoke(this, arguments);
    }
}
