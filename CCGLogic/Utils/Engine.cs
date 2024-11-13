using System.Reflection;

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

        public static string SelfReferenceName(GameType type) => type + "Self";
        public static Version Version => new(1, 0, 0, VersionType.Alpha);

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

        public static void SetPropertyValue(object obj, string key, string value)
        {
            PropertyInfo propertyInfo = obj.GetType().GetProperty(key);

            if (propertyInfo.PropertyType == typeof(int))
            {
                propertyInfo.SetValue(obj, int.Parse(value));
            }
            else if (propertyInfo.PropertyType == typeof(bool))
            {
                propertyInfo.SetValue(obj, bool.Parse(value));
            }
            else if (propertyInfo.PropertyType.IsEnum)
            {
                propertyInfo.SetValue(obj, Enum.Parse(propertyInfo.PropertyType, value));
            }
            else
            {
                propertyInfo.SetValue(obj, value);
            }
        }

        public IEnumerable<T> Shuffle<T>(IEnumerable<T> list)
        {
            List<T> list1 = list.ToList();
            List<T> list2 = [];

            while (list1.Count > 0)
            {
                int index = Random.Next(0, list1.Count);
                list2.Add(list1[index]);
                list1.RemoveAt(index);
            }

            return list2;
        }
    }
}
