namespace CCGLogic.Utils.Network
{
    public abstract class Room(Server server)
    {
        public abstract int MaxPlayerCount { get; }

        private readonly Server server = server;

        private readonly List<ServerPlayer> players = [];

        public IEnumerable<ServerPlayer> GetPlayers() => players;
    }
}
