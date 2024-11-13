using CCG.Utils;
using CCGLogic.Utils;
using System.Windows;

namespace CCG
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            GameConfig.Instance.LoadConfig();
            UIConfig.Instance.LoadConfig();

            Application application = new();
            MainWindow mainWindow = new();
            application.Run(mainWindow);
        }
    }
}
