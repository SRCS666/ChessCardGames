using CCGLogic.Utils;
using CCGLogic.Utils.Network;

namespace CCGLogic.Games.Chess
{
    public class ChessRoom(Server server, int roomNumber) : Room(server, roomNumber)
    {
        public override GameType GameType => GameType.Chess;
        public override int MaxPlayerCount => 2;

        private ChessState state;

        private Dictionary<string, ChessPieceColor> playerColorDic = [];

        protected override void Run()
        {
            state = new();
        }

        protected override void ProcessGameClientNotification(ServerPlayer player, Command command)
        {

        }

        protected override void ProcessGameClientRequest(ServerPlayer player, Command command)
        {

        }

        protected override void ProcessGameClientResponse(ServerPlayer player, Command command)
        {

        }
    }
}
