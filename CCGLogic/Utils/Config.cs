using CCGLogic.Utils.Network;
using System.Net;

namespace CCGLogic.Utils
{
    public abstract class Config
    {
        public abstract string FileName { get; }

        protected readonly Dictionary<string, string> configs = [];

        protected int ConfigsToInt(string key, int defaultValue = 0)
        {
            try
            {
                string value = configs[key];
                int result = int.Parse(value);
                return result;
            }
            catch { return defaultValue; }
        }

        protected string ConfigsToString(string key, string defaultValue = null)
        {
            try
            {
                string value = configs[key];
                return value;
            }
            catch { return defaultValue; }
        }

        protected double ConfigsToDouble(string key, double defaultValue = 0.0)
        {
            try
            {
                string value = configs[key];
                double result = double.Parse(value);
                return result;
            }
            catch { return defaultValue; }
        }

        protected abstract void Save(StreamWriter writer);

        public void SaveConfig()
        {
            StreamWriter writer = null;

            try
            {
                writer = new(FileName);
                Save(writer);
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

        protected abstract void Load();

        public void LoadConfig()
        {
            StreamReader reader = null;

            try
            {
                reader = new(FileName);
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

            Load();
            configs.Clear();
        }
    }

    public class GameConfig : Config
    {
        public static GameConfig Instance { get; } = new();

        public override string FileName => "Config.ini";

        public const int DefaultPort = 9355;
        public const int DefaultRoomNumber = 1024;

        public int StartServerPort { get; set; }
        public IPAddress AddToGameIP { get; set; }
        public int AddToGamePort { get; set; }
        public SignupType SignupType { get; set; }
        public GameType GameType { get; set; }
        public int RoomNumber { get; set; }
        public string ScreenName { get; set; }

        protected override void Save(StreamWriter writer)
        {
            writer.WriteLine($"StartServerPort={StartServerPort}");
            writer.WriteLine($"AddToGameIP={AddToGameIP}");
            writer.WriteLine($"AddToGamePort={AddToGamePort}");
            writer.WriteLine($"SignupType={Convert.ToInt32(SignupType)}");
            writer.WriteLine($"GameType={Convert.ToInt32(GameType)}");
            writer.WriteLine($"RoomNumber={RoomNumber}");
            writer.WriteLine($"ScreenName={ScreenName}");
        }

        protected override void Load()
        {
            StartServerPort = ConfigsToInt("StartServerPort", DefaultPort);
            AddToGameIP = configs.TryGetValue("AddToGameIP", out string ipString) ?
                IPAddress.TryParse(ipString, out IPAddress result) ?
                result : IPAddress.Loopback : IPAddress.Loopback;
            AddToGamePort = ConfigsToInt("AddToGamePort", DefaultPort);
            SignupType = Enum.IsDefined(typeof(SignupType), ConfigsToInt("SignupType")) ?
                (SignupType)ConfigsToInt("SignupType") : SignupType.CreateNewRoom;
            GameType = Enum.IsDefined(typeof(GameType), ConfigsToInt("GameType")) ?
                (GameType)ConfigsToInt("GameType") : GameType.Chess;
            RoomNumber = ConfigsToInt("RoomNumber", DefaultRoomNumber);
            ScreenName = ConfigsToString("ScreenName", "CCGPlayer");
        }
    }
}
