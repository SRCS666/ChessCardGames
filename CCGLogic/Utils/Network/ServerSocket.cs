using System.Net;
using System.Net.Sockets;

namespace CCGLogic.Utils.Network
{
    public class ServerSocket
    {
        public event Action<ClientSocket> NewTcpClientConnected;

        private readonly TcpListener tcpListener;

        public ServerSocket() => tcpListener = new(IPAddress.Any, Config.DefaultPort);

        public bool Start()
        {
            try
            {
                tcpListener.Start();
                new Thread(Listen) { IsBackground = true }.Start();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void Listen()
        {
            while (true)
            {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                ClientSocket connection = new(tcpClient);
                NewTcpClientConnected?.Invoke(connection);
            }
        }
    }
}
