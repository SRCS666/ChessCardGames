using System.Net;
using System.Text.Json.Nodes;

namespace CCGLogic.Utils.Network
{
    public class Server
    {
        public IPEndPoint IPEndPoint => serverSocket.IPEndPoint;
        public IPAddress Address => serverSocket.Address;
        public int Port => serverSocket.Port;

        private readonly ServerSocket serverSocket;

        private readonly List<IPEndPoint> points = [];
        private readonly List<Room> rooms = [];

        public Server()
        {
            serverSocket = new();

            serverSocket.NewTcpClientConnected += ProcessNewConnection;
        }

        public bool Start() => serverSocket.Start();

        private void ProcessNewConnection(ClientSocket clientSocket)
        {
            clientSocket.MessageGot += ProcessSignupRequest;
            NotifyClient(clientSocket, CmdOperation.COSignup, []);
        }

        private static void NotifyClient(ClientSocket clientSocket, CmdOperation operation, JsonArray arguments)
        {
            Command command = new(CmdWhere.CWRoom, CmdWhere.CWClient,
                CmdType.CTNotification, operation, arguments);
            clientSocket.SendMessage(command.ToBytes());
        }

        private void ProcessSignupRequest(ClientSocket clientSocket, byte[] request)
        {

        }
    }
}
