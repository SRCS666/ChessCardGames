namespace CCGLogic.Utils
{
    public enum GameType
    {
        Chess,
        Xiangqi
    }

    public class Engine
    {
        public static Engine Instance { get; } = new();

        public Random Random { get; private set; }

        public Engine() => Random = new();
    }
}
