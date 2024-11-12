using System.Net;

namespace CCGLogic.Utils.Network
{
    public class Server
    {
        public IPEndPoint IPEndPoint => serverSocket.IPEndPoint;
        public IPAddress Address => serverSocket.Address;
        public int Port => serverSocket.Port;

        private readonly ServerSocket serverSocket;

        public Server()
        {
            serverSocket = new();
        }

        public bool Start() => serverSocket.Start();
    }
}
