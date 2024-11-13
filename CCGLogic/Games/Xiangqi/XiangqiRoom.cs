using CCGLogic.Utils;
using CCGLogic.Utils.Network;
using System.Text.Json.Nodes;

namespace CCGLogic.Games.Xiangqi
{
    public class XiangqiRoom(Server server, int roomNumber) : Room(server, roomNumber)
    {
        public override GameType GameType => GameType.Xiangqi;
        public override int MaxPlayerCount => 2;

        protected override void Run()
        {

        }

        protected override void ProcessGameClientCommand(ServerPlayer player, CmdType type, JsonArray arguments)
        {

        }
    }
}
