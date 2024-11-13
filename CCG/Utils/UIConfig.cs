using CCGLogic.Utils;
using System.IO;
using System.Windows;

namespace CCG.Utils
{
    public class UIConfig : Config
    {
        public static UIConfig Instance { get; } = new();

        public override string FileName => "UIConfig.ini";

        public double MWPositionX { get; set; }
        public double MWPositionY { get; set; }
        public double MWWidth { get; set; }
        public double MWHeight { get; set; }
        public WindowState MWState { get; set; }

        protected override void Save(StreamWriter writer)
        {
            writer.WriteLine($"MWPositionX={MWPositionX}");
            writer.WriteLine($"MWPositionY={MWPositionY}");
            writer.WriteLine($"MWWidth={MWWidth}");
            writer.WriteLine($"MWHeight={MWHeight}");
            writer.WriteLine($"MWState={Convert.ToInt32(MWState)}");
        }

        protected override void Load()
        {
            MWPositionX = ConfigsToDouble("MWPositionX", 100);
            MWPositionY = ConfigsToDouble("MWPositionY", 100);
            MWWidth = ConfigsToDouble("MWWidth", 100);
            MWHeight = ConfigsToDouble("MWHeight", 100);
            MWState = Enum.IsDefined(typeof(WindowState), ConfigsToInt("MWState")) ?
                (WindowState)ConfigsToInt("MWState") : WindowState.Normal;
        }
    }
}
