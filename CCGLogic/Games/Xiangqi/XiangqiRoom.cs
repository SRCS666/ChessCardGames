using CCGLogic.Utils;
using CCGLogic.Utils.Network;

namespace CCGLogic.Games.Xiangqi
{
    public class XiangqiRoom(Server server, int roomNumber) : Room(server, roomNumber)
    {
        public override GameType GameType => GameType.Xiangqi;
        public override int MaxPlayerCount => 2;

        protected override void Run()
        {

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
