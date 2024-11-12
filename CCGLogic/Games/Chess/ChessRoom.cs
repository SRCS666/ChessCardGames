using CCGLogic.Utils.Network;

namespace CCGLogic.Games.Chess
{
    public class ChessRoom(Server server) : Room(server)
    {
        public override int MaxPlayerCount => 2;

        private ChessState state;
    }
}
