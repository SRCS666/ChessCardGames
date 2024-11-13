using CCGLogic.Utils;
using CCGLogic.Utils.Network;
using System.Text.Json.Nodes;

namespace CCGLogic.Games.Chess
{
    public class ChessClient : Client
    {
        public override GameType GameType => GameType.Chess;

        private delegate void ChessServerCommand(JsonArray arguments);

        private ChessState state;

        private readonly Dictionary<string, ChessPieceColor> playerColorDic = [];

        private readonly Dictionary<ChessOperation, ChessServerCommand> chessNotifications = [];
        private readonly Dictionary<ChessOperation, ChessServerCommand> chessRequests = [];
        private readonly Dictionary<ChessOperation, ChessServerCommand> chessResponses = [];

        public ChessClient()
        {
            InitChessCallbacks();
        }

        private void InitChessCallbacks()
        {
            chessNotifications[ChessOperation.ChessOSetPlayerColorDic] = SetPlayerColorDic;
        }

        protected override void ProcessGameServerCommand(CmdType type, JsonArray arguments)
        {
            ChessOperation operation = (ChessOperation)arguments[0].GetValue<int>();
            JsonArray chessArguments = Command.StringToJsonArray(arguments[1].ToString());

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
        }
    }
}
