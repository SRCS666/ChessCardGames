using System.Net.Sockets;

namespace CCGLogic.Utils.Network
{
    public class ClientSocket(TcpClient tcpClient = null)
    {
        private readonly TcpClient tcpClient = tcpClient ?? new();
    }
}
