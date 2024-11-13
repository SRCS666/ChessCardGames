using CCGLogic.Utils;
using CCGLogic.Utils.Network;
using System.Text.Json.Nodes;

namespace CCGLogic.Games.Chess
{
    public enum ChessOperation
    {
        ChessOSetPlayerColorDic,
        ChessOSetState
    }

    public class ChessRoom(Server server, int roomNumber) : Room(server, roomNumber)
    {
        public override GameType GameType => GameType.Chess;
        public override int MaxPlayerCount => 2;

        private delegate void ChessClientCommand(ServerPlayer player, JsonArray arguments);

        private ChessState state;

        private readonly Dictionary<string, ChessPieceColor> playerColorDic = [];

        private readonly Dictionary<ChessOperation, ChessClientCommand> chessNotifications = [];
        private readonly Dictionary<ChessOperation, ChessClientCommand> chessRequests = [];
        private readonly Dictionary<ChessOperation, ChessClientCommand> chessResponses = [];

        protected override void Run()
        {
            AssignPlayerColor();
            PrepareForStart();
        }

        protected override void ProcessGameClientCommand(ServerPlayer player, CmdType type, JsonArray arguments)
        {
            ChessOperation operation = (ChessOperation)arguments[0].GetValue<int>();
            JsonArray chessArguments = Command.StringToJsonArray(arguments[1].ToString());

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

            NotifyState();
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

            BroadcastNotify(CmdOperation.COGameOperation,
                [Convert.ToInt32(ChessOperation.ChessOSetPlayerColorDic), pairs], []);
        }

        private void NotifyState()
        {

        }
    }
}
