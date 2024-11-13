using CCGLogic.Utils;
using CCGLogic.Utils.ChessGame.GridBoardGame;
using CCGLogic.Utils.Network;
using System.Text.Json.Nodes;

namespace CCGLogic.Games.Chess
{
    public class ChessClient : Client
    {
        public override GameType GameType => GameType.Chess;

        public event Action StateInitialized;
        public event Action MoveMade;

        private delegate void ChessServerCommand(JsonArray arguments);

        public ChessState State { get; set; }

        private readonly Dictionary<string, ChessPieceColor> playerColorDic = [];

        private readonly Dictionary<ChessOperation, ChessServerCommand> chessNotifications = [];
        private readonly Dictionary<ChessOperation, ChessServerCommand> chessRequests = [];
        private readonly Dictionary<ChessOperation, ChessServerCommand> chessResponses = [];

        public ChessPieceColor SelfColor => playerColorDic[ClientPlayer.Self.Name];
        public bool WillRotate => SelfColor == ChessPieceColor.Black;

        public ChessClient()
        {
            InitChessCallbacks();
        }

        private void InitChessCallbacks()
        {
            chessNotifications[ChessOperation.ChessOSetPlayerColorDic] = SetPlayerColorDic;
            chessNotifications[ChessOperation.ChessOInitState] = InitState;
            chessNotifications[ChessOperation.ChessOMove] = ProcessMove;
        }

        protected override void ProcessGameServerCommand(CmdType type, JsonArray arguments)
        {
            ChessOperation operation = (ChessOperation)arguments[1].GetValue<int>();
            JsonArray chessArguments = Command.StringToJsonArray(arguments[2].ToString());

            switch (type)
            {
                case CmdType.CTNotification:
                    chessNotifications[operation](chessArguments);
                    break;
                case CmdType.CTRequest:
                    chessRequests[operation](chessArguments);
                    break;
                case CmdType.CTResponse:
                    chessResponses[operation](chessArguments);
                    break;
                default:
                    break;
            }
        }

        private void SetPlayerColorDic(JsonArray chessArguments)
        {
            foreach (JsonNode node in chessArguments)
            {
                JsonArray array = Command.StringToJsonArray(node.ToString());
                string playerName = array[0].GetValue<string>();
                ChessPieceColor color = (ChessPieceColor)array[1].GetValue<int>();

                playerColorDic[playerName] = color;
            }
        }

        private void InitState(JsonArray chessArguments)
        {
            State = new();
            ChessBoard board = State.Board;

            JsonArray piecesArray = Command.StringToJsonArray(chessArguments[0].ToString());
            foreach (JsonNode node in piecesArray)
            {
                JsonArray posArray = Command.StringToJsonArray(node[0].ToString());
                JsonArray propertyArray = Command.StringToJsonArray(node[1].ToString());

                GridPosition pos = GridPosition.Parse(posArray);

                ChessPieceType type = (ChessPieceType)propertyArray[0].GetValue<int>();
                ChessPieceColor color = (ChessPieceColor)propertyArray[1].GetValue<int>();
                bool hasMoved = propertyArray[2].GetValue<bool>();

                ChessPiece piece = ChessPiece.CreatePieceFromType(type, board, color);
                piece.HasMoved = hasMoved;

                board[pos] = piece;
            }

            State.CurrentPlayer = (ChessPieceColor)chessArguments[1].GetValue<int>();

            StateInitialized?.Invoke();
        }

        private void MakeMove(ChessMove move)
        {
            State.MakeMove(move);
            MoveMade?.Invoke();
        }

        public void NotifyMove(ChessMove move)
        {
            MakeMove(move);
            NotifyServer(CmdOperation.COGameOperation, [Convert.ToInt32(GameType),
                Convert.ToInt32(ChessOperation.ChessOMove), move.ToJsonArray()]);
        }

        private void ProcessMove(JsonArray chessArguments)
        {
            ChessMove move = ChessMove.CreateMoveFromJsonArray(State.Board, chessArguments);
            MakeMove(move);
        }
    }
}
