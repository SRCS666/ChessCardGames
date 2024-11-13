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

        private delegate void ChessServerCommand(JsonArray arguments);

        public ChessState State { get; set; }

        private readonly Dictionary<string, ChessPieceColor> playerColorDic = [];

        private readonly Dictionary<ChessOperation, ChessServerCommand> chessNotifications = [];
        private readonly Dictionary<ChessOperation, ChessServerCommand> chessRequests = [];
        private readonly Dictionary<ChessOperation, ChessServerCommand> chessResponses = [];

        public bool WillRotate => playerColorDic[ClientPlayer.Self.Name] == ChessPieceColor.Black;

        public ChessClient()
        {
            InitChessCallbacks();
        }

        private void InitChessCallbacks()
        {
            chessNotifications[ChessOperation.ChessOSetPlayerColorDic] = SetPlayerColorDic;
            chessNotifications[ChessOperation.ChessOInitState] = InitState;
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

                int row = posArray[0].GetValue<int>();
                int column = posArray[1].GetValue<int>();
                GridPosition pos = new(row, column);

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
    }
}
