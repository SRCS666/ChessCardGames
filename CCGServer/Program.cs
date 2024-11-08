using System.Windows;

namespace CCGServer
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
