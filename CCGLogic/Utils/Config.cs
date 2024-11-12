namespace CCGLogic.Utils
{
    public class Config
    {
        public static Config Instance { get; } = new();

        public const int DefaultPort = 9355;
    }
}
