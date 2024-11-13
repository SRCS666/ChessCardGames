using System.Net;
using System.Text.Json.Nodes;

namespace CCGLogic.Utils.Network
{
    public class ServerPlayer(Room room) : Player
    {
        public event Action<IPEndPoint> Disconnected;
        public event Action<ServerPlayer, Command> ClientCommandReceived;
        public event Action<byte[]> MessageReady;

        public Room Room { get; } = room;

        private ClientSocket clientSocket;
        public ClientSocket ClientSocket
        {
            get { return clientSocket; }
            set
            {
                if (value != null)
                {
                    value.Disconnected += OnDisconnected;
                    value.MessageGot += OnMessageGot;
                    MessageReady += SendMessage;
                }
                else
                {
                    MessageReady -= SendMessage;
                }

                clientSocket = value;
            }
        }

        private void OnDisconnected(IPEndPoint endPoint) => Disconnected?.Invoke(endPoint);
        public void SendMessage(byte[] message) => clientSocket?.SendMessage(message);

        private void OnMessageGot(ClientSocket clientSocket, byte[] message)
        {
            Command command = Command.Parse(message);
            if (command.Source == CmdWhere.CWClient && command.Destination == CmdWhere.CWRoom)
            {
                ClientCommandReceived?.Invoke(this, command);
            }
        }

        public void Notify(CmdOperation operation, JsonArray arguments)
        {
            Command command = new(CmdWhere.CWRoom, CmdWhere.CWClient,
                CmdType.CTNotification, operation, arguments);
            SendMessage(command.ToBytes());
        }

        public void IntroduceTo(ServerPlayer player = null)
        {
            JsonArray array = [Name, ScreenName];

            if (player == null)
            {
                Room.BroadcastNotify(CmdOperation.COAddPlayer, array, [this]);
            }
            else
            {
                player.Notify(CmdOperation.COAddPlayer, array);
            }
        }
    }
}
