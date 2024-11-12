namespace CCGLogic.Utils.Network
{
    public abstract class Room(Server server, int roomNumber)
    {
        public abstract GameType GameType { get; }
        public abstract int MaxPlayerCount { get; }

        public int RoomNumber { get; } = roomNumber;

        private readonly Server server = server;

        private readonly List<ServerPlayer> players = [];

        public IEnumerable<ServerPlayer> GetPlayers() => players;
    }
}
