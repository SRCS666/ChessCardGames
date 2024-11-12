using System.Net;
using System.Security.Principal;

namespace CCGLogic.Utils
{
    public class Config
    {
        public static Config Instance { get; } = new();

        private readonly Dictionary<string, string> configs = [];

        public const int DefaultPort = 9355;

        public int StartServerPort { get; set; }

        public void SaveConfig()
        {
            StreamWriter writer = null;

            try
            {
                writer = new("Config.ini");

                writer.WriteLine($"StartServerPort={StartServerPort}");
            }
            catch { }
            finally
            {
                try
                {
                    writer.Close();
                }
                catch { }
            }
        }

        public void LoadConfig()
        {
            StreamReader reader = null;

            try
            {
                reader = new("Config.ini");
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    int indexEqual = line.IndexOf('=');
                    if (indexEqual != -1)
                    {
                        string arg1 = line[..indexEqual];
                        string arg2 = line[(indexEqual + 1)..];
                        configs[arg1] = arg2;
                    }
                }
            }
            catch { }
            finally
            {
                try
                {
                    reader.Close();
                }
                catch { }
            }

            StartServerPort = ConfigsToInt("StartServerPort", DefaultPort);

            configs.Clear();
        }

        private int ConfigsToInt(string key, int defaultValue = 0)
        {
            try
            {
                string value = configs[key];
                int result = int.Parse(value);
                return result;
            }
            catch { return defaultValue; }
        }
    }
}
