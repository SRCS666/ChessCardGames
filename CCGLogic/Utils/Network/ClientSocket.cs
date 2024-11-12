using System.Net;
using System.Net.Sockets;

namespace CCGLogic.Utils.Network
{
    public class ClientSocket(TcpClient tcpClient = null)
    {
        public event Action<ClientSocket, byte[]> MessageGot;
        public event Action<IPEndPoint> Disconnected;
        public event Action<string> ErrorMessage;

        public IPEndPoint RemoteEndPoint => tcpClient.Client.RemoteEndPoint as IPEndPoint;
        public IPAddress RemoteAddress => RemoteEndPoint.Address;
        public int RemotePort => RemoteEndPoint.Port;

        public IPEndPoint LocalEndPoint => tcpClient.Client.LocalEndPoint as IPEndPoint;
        public IPAddress LocalAddress => LocalEndPoint.Address;
        public int LocalPort => LocalEndPoint.Port;

        private readonly TcpClient tcpClient = tcpClient ?? new();

        public void Start() => new Thread(Receive) { IsBackground = true }.Start();

        private void Receive()
        {
            try
            {
                NetworkStream stream = tcpClient.GetStream();
                byte[] buffer = new byte[4096];
                int bytesRead;

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    byte[] result = new byte[bytesRead];
                    Array.Copy(buffer, result, bytesRead);

                    foreach (byte[] message in SplitMessage(result))
                    {
                        MessageGot?.Invoke(this, message);
                    }
                }
            }
            catch (Exception ex)
            {
                Disconnect();
                ErrorMessage?.Invoke(ex.Message);
            }
        }

        public void Connect(IPAddress address, int port)
        {
            try
            {
                tcpClient.Connect(address, port);
                Start();
            }
            catch (Exception ex)
            {
                ErrorMessage?.Invoke(ex.Message);
            }
        }

        public void Disconnect()
        {
            try
            {
                IPEndPoint endPoint = RemoteEndPoint;
                tcpClient.Close();
                Disconnected?.Invoke(endPoint);
            }
            catch (Exception ex)
            {
                ErrorMessage?.Invoke(ex.Message);
            }
        }

        public void SendMessage(byte[] message)
        {
            NetworkStream stream = tcpClient.GetStream();
            stream.Write(message, 0, message.Length);
        }

        private static IEnumerable<byte[]> SplitMessage(byte[] message)
        {
            int index1 = 0;
            int index2 = 1;
            int count = 1;
            bool inDQ = false;

            while (index1 < message.Length)
            {
                while (!(message[index2] == ']' && count == 1))
                {
                    if (!inDQ)
                    {
                        if (message[index2] == ']') { count--; }
                        if (message[index2] == '[') { count++; }
                    }
                    if (message[index2] == '\"') { inDQ = !inDQ; }
                    index2++;
                }

                int length = index2 - index1 + 1;
                byte[] result = new byte[length];
                Array.Copy(message, index1, result, 0, length);
                yield return result;

                index1 = index2 + 1;
                index2 = index1 + 1;
            }
        }
    }
}
