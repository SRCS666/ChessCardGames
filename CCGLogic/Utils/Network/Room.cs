﻿using System.Text.Json.Nodes;

namespace CCGLogic.Utils.Network
{
    public abstract class Room
    {
        private delegate void ClientCommand(ServerPlayer player, JsonArray arguments);

        public event Action<string> ServerMessage;

        public abstract GameType GameType { get; }
        public abstract int MaxPlayerCount { get; }

        public int RoomNumber { get; }

        private static int playerID = 0;

        private readonly Server server;
        private readonly Thread thread;

        private readonly Dictionary<CmdOperation, ClientCommand> notifications = [];
        private readonly Dictionary<CmdOperation, ClientCommand> requests = [];
        private readonly Dictionary<CmdOperation, ClientCommand> responses = [];

        protected readonly List<ServerPlayer> players = [];

        public Room(Server server, int roomNumber)
        {
            RoomNumber = roomNumber;
            this.server = server;

            thread = new(Run) { IsBackground = true };

            InitCallbacks();
        }

        private void InitCallbacks()
        {
            notifications[CmdOperation.COToggleReady] = ToggleReady;
        }

        private void ProcessClientCommand(ServerPlayer player, Command command)
        {
            if (command.Operation == CmdOperation.COGameOperation)
            {
                GameType type = (GameType)command.Arguments[0].GetValue<int>();
                if (type != GameType)
                {
                    return;
                }
                ProcessGameClientCommand(player, command.Type, command.Arguments);
            }
            else
            {
                switch (command.Type)
                {
                    case CmdType.CTNotification:
                        notifications[command.Operation](player, command.Arguments);
                        break;
                    case CmdType.CTRequest:
                        requests[command.Operation](player, command.Arguments);
                        break;
                    case CmdType.CTResponse:
                        responses[command.Operation](player, command.Arguments);
                        break;
                    default:
                        break;
                }
            }
        }

        protected abstract void Run();
        protected abstract void ProcessGameClientCommand(ServerPlayer player, CmdType type, JsonArray arguments);

        private static void Notify(ServerPlayer player, CmdOperation operation, JsonArray arguments) => player.Notify(operation, arguments);

        public void NotifyAllPlayers(CmdOperation operation,
            JsonArray arguments, IEnumerable<ServerPlayer> excepts)
        {
            foreach (ServerPlayer player in players)
            {
                if (!excepts.Contains(player))
                {
                    Notify(player, operation, arguments);
                }
            }
        }

        public void NotifyProperty(ServerPlayer playerToNotify, ServerPlayer propertyOwner,
            string propertyName, string value = null)
        {
            if (playerToNotify == null) { return; }
            value ??= propertyOwner.GetType().GetProperty(propertyName).GetValue(propertyOwner).ToString();
            string ownerName = playerToNotify == propertyOwner ? Engine.SelfReferenceName(GameType) : propertyOwner.Name;

            JsonArray arguments = [ownerName, propertyName, value];
            Notify(playerToNotify, CmdOperation.COSetProperty, arguments);
        }

        public bool IsFull() => players.Count == MaxPlayerCount;

        private string GeneratePlayerName() => string.Format("{0}{1}", GameType, ++playerID);

        public void SignupNewPlayer(ClientSocket clientSocket, string screenName)
        {
            ServerPlayer player = new(this)
            {
                ClientSocket = clientSocket
            };
            player.ClientCommandReceived += ProcessClientCommand;
            players.Add(player);

            ServerMessage?.Invoke(string.Format("{0}:{1} connected.", clientSocket.RemoteAddress, clientSocket.RemotePort));

            player.Name = GeneratePlayerName();
            player.ScreenName = screenName;

            NotifyProperty(player, player, "Name");
            player.IntroduceTo();

            foreach (ServerPlayer player1 in players)
            {
                if (player1 != player)
                {
                    player1.IntroduceTo(player);
                }
            }
        }

        private void ToggleReady(ServerPlayer player, JsonArray arguments)
        {
            if (IsFull())
            {
                thread.Start();
            }
        }
    }
}
