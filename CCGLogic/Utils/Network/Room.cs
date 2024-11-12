namespace CCGLogic.Utils.Network
{
    public abstract class Room(Server server, int roomNumber)
    {
        public event Action<string> ServerMessage;

        public abstract GameType GameType { get; }
        public abstract int MaxPlayerCount { get; }

        public int RoomNumber { get; } = roomNumber;

        private readonly Server server = server;

        private readonly List<ServerPlayer> players = [];

        public IEnumerable<ServerPlayer> GetPlayers() => players;
        public bool IsFull() => players.Count == MaxPlayerCount;

        public void SignupNewPlayer(ClientSocket clientSocket, string screenName)
        {
            ServerPlayer player = new(this)
            {
                ClientSocket = clientSocket
            };
            players.Add(player);

            ServerMessage?.Invoke(string.Format("{0}:{1} connected.", clientSocket.RemoteAddress, clientSocket.RemotePort));

            player.ClientCommandReceived += ProcessClientCommand;
        }

        private void ProcessClientCommand(ServerPlayer player, Command command)
        {

        }
    }
}
