using CCG.Utils;
using System.Windows;

namespace CCG
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application application = new();
            MainWindow mainWindow = new();
            application.Run(mainWindow);
        }
    }
}
