namespace CCGLogic.Utils.Network
{
    public class ServerPlayer(Room room) : Player
    {
        public Room Room { get; } = room;
    }
}
