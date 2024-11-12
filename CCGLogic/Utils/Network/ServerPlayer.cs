using System.Net;

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
        private void SendMessage(byte[] message) => clientSocket?.SendMessage(message);

        private void OnMessageGot(ClientSocket clientSocket, byte[] message)
        {
            Command command = Command.Parse(message);
            if (command.Destination == CmdWhere.CWRoom)
            {
                ClientCommandReceived?.Invoke(this, command);
            }
        }
    }
}
