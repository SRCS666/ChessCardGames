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

        public static string SelfReferenceName(GameType type) => type + "Self";

        public Engine() => Random = new();

        public static IEnumerable<byte[]> SplitMessage(byte[] message)
        {
            int index1 = 0;
            int index2 = 1;
            int count = 1;
            bool inDQ = false;

            while (index1 < message.Length)
            {
                while (!(message[index2] == ']' && count == 1))
                {
                    if (!inDQ)
                    {
                        if (message[index2] == ']') { count--; }
                        if (message[index2] == '[') { count++; }
                    }
                    if (message[index2] == '\"') { inDQ = !inDQ; }
                    index2++;
                }

                int length = index2 - index1 + 1;
                byte[] result = new byte[length];
                Array.Copy(message, index1, result, 0, length);
                yield return result;

                index1 = index2 + 1;
                index2 = index1 + 1;
            }
        }
    }
}
