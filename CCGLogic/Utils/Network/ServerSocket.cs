using System.Net;
using System.Net.Sockets;

namespace CCGLogic.Utils.Network
{
    public class ServerSocket
    {
        public event Action<ClientSocket> NewTcpClientConnected;

        public IPEndPoint IPEndPoint => tcpListener.LocalEndpoint as IPEndPoint;
        public IPAddress Address => IPEndPoint.Address;
        public int Port => IPEndPoint.Port;

        private readonly TcpListener tcpListener;

        public ServerSocket() => tcpListener = new(IPAddress.Any, GameConfig.Instance.StartServerPort);

        public bool Start()
        {
            try
            {
                tcpListener.Start();
                new Thread(Listen) { IsBackground = true }.Start();
                return true;
            }
            catch { return false; }
        }

        private void Listen()
        {
            while (true)
            {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                ClientSocket connection = new(tcpClient);
                connection.Start();
                NewTcpClientConnected?.Invoke(connection);
            }
        }
    }
}
