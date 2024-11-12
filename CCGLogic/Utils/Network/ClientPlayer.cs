namespace CCGLogic.Utils.Network
{
    public class ClientPlayer(Client client) : Player
    {
        public static ClientPlayer Self { get; set; }

        private Client client = client;
    }
}
