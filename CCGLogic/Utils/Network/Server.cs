using CCGLogic.Games.Chess;
using CCGLogic.Games.Xiangqi;
using System.Net;
using System.Text.Json.Nodes;

namespace CCGLogic.Utils.Network
{
    public enum SignupResultType
    {
        Successed,
        Failed
    }

    public class Server
    {
        public event Action<string> ServerMessage;

        public IPEndPoint IPEndPoint => serverSocket.IPEndPoint;
        public IPAddress Address => serverSocket.Address;
        public int Port => serverSocket.Port;

        private readonly ServerSocket serverSocket;

        private readonly List<Room> rooms = [];

        public Server()
        {
            serverSocket = new();

            serverSocket.NewTcpClientConnected += ProcessNewConnection;
        }

        public bool Start() => serverSocket.Start();

        private static void NotifyClient(ClientSocket clientSocket, CmdOperation operation, JsonArray arguments)
        {
            Command command = new(CmdWhere.CWRoom, CmdWhere.CWClient,
                CmdType.CTNotification, operation, arguments);
            clientSocket.SendMessage(command.ToBytes());
        }

        private void ProcessNewConnection(ClientSocket clientSocket)
        {
            clientSocket.MessageGot += ProcessSignupRequest;
            NotifyClient(clientSocket, CmdOperation.COSignup, []);
        }

        private void ProcessSignupRequest(ClientSocket clientSocket, byte[] request)
        {
            Command command = Command.Parse(request);

            if (command.Source != CmdWhere.CWClient || command.Destination != CmdWhere.CWRoom)
            {
                ServerMessage?.Invoke(string.Format("Invalid message from unknown source."));
                return;
            }
            if (command.Operation != CmdOperation.COSignup)
            {
                ServerMessage?.Invoke(string.Format("Invalid sign up."));
                return;
            }

            clientSocket.MessageGot -= ProcessSignupRequest;

            JsonArray arguments = command.Arguments;
            SignupType signupType = (SignupType)arguments[0].GetValue<int>();
            GameType gameType = (GameType)arguments[1].GetValue<int>();
            int roomNumber = arguments[2].GetValue<int>();
            string screenName = arguments[3].GetValue<string>();

            Room roomFound = AllRoomsForGame(gameType).FirstOrDefault(room => room.RoomNumber == roomNumber);
            if (signupType == SignupType.CreateNewRoom)
            {
                if (roomFound == null)
                {
                    Room room = CreateNewRoom(gameType, roomNumber);

                    room.SignupNewPlayer(clientSocket, screenName);
                    NotifySignupResult(clientSocket, SignupResultType.Successed, "Succeed.");
                }
                else
                {
                    NotifySignupResult(clientSocket, SignupResultType.Failed, "This room is existed.");
                }
            }
            else
            {
                if (roomFound != null)
                {
                    if (roomFound.IsFull())
                    {
                        NotifySignupResult(clientSocket, SignupResultType.Failed, "This room is full.");
                    }
                    else
                    {
                        roomFound.SignupNewPlayer(clientSocket, screenName);
                        NotifySignupResult(clientSocket, SignupResultType.Successed, "Succeed.");
                    }
                }
                else
                {
                    NotifySignupResult(clientSocket, SignupResultType.Failed, "This room is not existed.");
                }
            }
        }

        private static void NotifySignupResult(ClientSocket clientSocket, SignupResultType type, string reason)
        {
            JsonArray jsonArray = [];
            jsonArray.Add(type);
            jsonArray.Add(reason);

            NotifyClient(clientSocket, CmdOperation.COSignupResult, jsonArray);
        }

        private IEnumerable<Room> AllRoomsForGame(GameType gameType) =>
            rooms.Where(room => room.GameType == gameType);
        private void RoomServerMessage(string message) => ServerMessage?.Invoke(message);

        private Room CreateNewRoom(GameType gameType, int roomNumber)
        {
            Room room = gameType switch
            {
                GameType.Chess => new ChessRoom(this, roomNumber),
                GameType.Xiangqi => new XiangqiRoom(this, roomNumber),
                _ => null
            };
            room.ServerMessage += RoomServerMessage;
            rooms.Add(room);

            return room;
        }
    }
}
