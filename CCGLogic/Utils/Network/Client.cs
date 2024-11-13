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
        public event Action<Client, SignupResultType, string> Signedup;

        private delegate void ServerCommand(JsonArray arguments);

        private ClientSocket clientSocket;

        private readonly Dictionary<CmdOperation, ServerCommand> notifications = [];
        private readonly Dictionary<CmdOperation, ServerCommand> requests = [];
        private readonly Dictionary<CmdOperation, ServerCommand> responses = [];

        private readonly List<ClientPlayer> players = [];

        public Client()
        {
            InitCallbacks();

            ClientPlayer.Self = new(this)
            {
                ScreenName = GameConfig.Instance.ScreenName
            };
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

        private void InitCallbacks()
        {
            notifications[CmdOperation.COSignup] = Signup;
            notifications[CmdOperation.COSignupResult] = SignupResult;
            notifications[CmdOperation.COSetProperty] = SetProperty;
            notifications[CmdOperation.COAddPlayer] = AddPlayer;
        }

        private void ProcessServerCommand(ClientSocket clientSocket, byte[] data)
        {
            Command command = Command.Parse(data);
            if (command.Source == CmdWhere.CWRoom && command.Destination == CmdWhere.CWClient)
            {
                if (command.Operation == CmdOperation.COGameOperation)
                {
                    GameType type = (GameType)command.Arguments[0].GetValue<int>();
                    if (type != GameType)
                    {
                        return;
                    }
                    ProcessGameServerCommand(command.Type, command.Arguments);
                }
                else
                {
                    switch (command.Type)
                    {
                        case CmdType.CTNotification:
                            notifications[command.Operation](command.Arguments);
                            break;
                        case CmdType.CTRequest:
                            requests[command.Operation](command.Arguments);
                            break;
                        case CmdType.CTResponse:
                            responses[command.Operation](command.Arguments);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        protected abstract void ProcessGameServerCommand(CmdType type, JsonArray arguments);

        public void NotifyServer(CmdOperation operation, JsonArray arguments)
        {
            Command command = new(CmdWhere.CWClient, CmdWhere.CWRoom,
                CmdType.CTNotification, operation, arguments);
            SendMessage(command.ToBytes());
        }

        private void Signup(JsonArray arguments)
        {
            JsonArray array = [];
            array.Add(Convert.ToInt32(GameConfig.Instance.SignupType));
            array.Add(Convert.ToInt32(GameConfig.Instance.GameType));
            array.Add(GameConfig.Instance.RoomNumber);
            array.Add(GameConfig.Instance.ScreenName);
            array.Add(Engine.Version.ToJsonArray());

            NotifyServer(CmdOperation.COSignup, array);
        }

        private void SignupResult(JsonArray arguments)
        {
            SignupResultType result = (SignupResultType)arguments[0].GetValue<int>();
            string reason = arguments[1].GetValue<string>();
            Signedup?.Invoke(this, result, reason);

            if (result == SignupResultType.Successed)
            {
                NotifyServer(CmdOperation.COToggleReady, []);
            }
        }

        private void SetProperty(JsonArray arguments)
        {
            string name = arguments[0].GetValue<string>();
            ClientPlayer player = GetPlayer(name);

            if (player != null)
            {
                string key = arguments[1].GetValue<string>();
                string value = arguments[2].GetValue<string>();
                Engine.SetPropertyValue(player, key, value);
            }
        }

        private void AddPlayer(JsonArray arguments)
        {
            string name = arguments[0].GetValue<string>();
            string screenName = arguments[1].GetValue<string>();

            ClientPlayer player = new(this)
            {
                Name = name,
                ScreenName = screenName,
            };
            players.Add(player);
        }

        private ClientPlayer GetPlayer(string name)
        {
            if (name == Engine.SelfReferenceName(GameType)) { return ClientPlayer.Self; }
            return players.FirstOrDefault(player => player.Name == name);
        }
    }
}
