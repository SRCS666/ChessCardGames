using CCGLogic.Utils;
using CCGLogic.Utils.Network;

namespace CCGLogic.Games.Xiangqi
{
    public class XiangqiRoom(Server server, int roomNumber) : Room(server, roomNumber)
    {
        public override GameType GameType => GameType.Xiangqi;
        public override int MaxPlayerCount => 2;
    }
}
