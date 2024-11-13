using CCGLogic.Utils;
using CCGLogic.Utils.ChessGame.GridBoardGame;
using CCGLogic.Utils.Network;
using System.Text.Json.Nodes;

namespace CCGLogic.Games.Chess
{
    public enum ChessOperation
    {
        ChessOSetPlayerColorDic,
        ChessOInitState,
        ChessOMove
    }

    public class ChessRoom : Room
    {
        public override GameType GameType => GameType.Chess;
        public override int MaxPlayerCount => 2;

        private delegate void ChessClientCommand(ServerPlayer player, JsonArray arguments);

        private ChessState state;

        private readonly Dictionary<string, ChessPieceColor> playerColorDic = [];

        private readonly Dictionary<ChessOperation, ChessClientCommand> chessNotifications = [];
        private readonly Dictionary<ChessOperation, ChessClientCommand> chessRequests = [];
        private readonly Dictionary<ChessOperation, ChessClientCommand> chessResponses = [];

        public ChessRoom(Server server, int roomNumber) : base(server, roomNumber)
        {
            InitCallbacks();
        }

        private void InitCallbacks()
        {
            chessNotifications[ChessOperation.ChessOMove] = MakeMove;
        }
        
        protected override void Run()
        {
            AssignPlayerColor();
            PrepareForStart();
        }

        protected override void ProcessGameClientCommand(ServerPlayer player, CmdType type, JsonArray arguments)
        {
            ChessOperation operation = (ChessOperation)arguments[1].GetValue<int>();
            JsonArray chessArguments = Command.StringToJsonArray(arguments[2].ToString());

            switch (type)
            {
                case CmdType.CTNotification:
                    chessNotifications[operation](player, chessArguments);
                    break;
                case CmdType.CTRequest:
                    chessRequests[operation](player, chessArguments);
                    break;
                case CmdType.CTResponse:
                    chessResponses[operation](player, chessArguments);
                    break;
                default:
                    break;
            }
        }

        private void AssignPlayerColor()
        {
            IEnumerable<ChessPieceColor> colors = [ChessPieceColor.White, ChessPieceColor.Black];
            colors = Engine.Instance.Shuffle(colors);

            for (int i = 0; i < players.Count; i++)
            {
                playerColorDic[players[i].Name] = colors.ElementAt(i);
            }

            NotifyPlayerColorDic();
        }

        private void PrepareForStart()
        {
            state = new();
            state.AddStartPieces();
            state.CurrentPlayer = ChessPieceColor.White;

            NotifyInitState();
        }

        private void NotifyPlayerColorDic()
        {
            JsonArray pairs = [];
            foreach (string key in playerColorDic.Keys)
            {
                ChessPieceColor color = playerColorDic[key];

                JsonArray pair = [];
                pair.Add(key);
                pair.Add(Convert.ToInt32(color));
                pairs.Add(pair);
            }

            NotifyAllPlayers(CmdOperation.COGameOperation, [Convert.ToInt32(GameType),
                Convert.ToInt32(ChessOperation.ChessOSetPlayerColorDic), pairs], []);
        }

        private void NotifyInitState()
        {
            JsonArray arguments = [];
            ChessBoard board = state.Board;

            JsonArray piecesArray = [];
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    GridPosition pos = new(r, c);
                    if (!board.IsEmpty(pos))
                    {
                        JsonArray posArray = pos.ToJsonArray();

                        ChessPiece piece = board[pos];
                        JsonArray propertyArray = [];
                        propertyArray.Add(Convert.ToInt32(piece.Type));
                        propertyArray.Add(Convert.ToInt32(piece.Color));
                        propertyArray.Add(piece.HasMoved);

                        JsonArray pieceArray = [];
                        pieceArray.Add(posArray);
                        pieceArray.Add(propertyArray);

                        piecesArray.Add(pieceArray);
                    }
                }
            }

            arguments.Add(piecesArray);
            arguments.Add(Convert.ToInt32(state.CurrentPlayer));

            NotifyAllPlayers(CmdOperation.COGameOperation, [Convert.ToInt32(GameType),
                Convert.ToInt32(ChessOperation.ChessOInitState), arguments], []);
        }

        private void MakeMove(ServerPlayer player, JsonArray chessArguments)
        {
            ChessMove move = ChessMove.CreateMoveFromJsonArray(state.Board, chessArguments);
            state.MakeMove(move);

            NotifyAllPlayers(CmdOperation.COGameOperation, [Convert.ToInt32(GameType),
                Convert.ToInt32(ChessOperation.ChessOMove), move.ToJsonArray()], [player]);
        }
    }
}
