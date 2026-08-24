using System.Windows;

namespace GameBoyCEmulator;

public class Program
{
    [STAThread]
    public static void Main()
    {
        var app = new Application();
        var window = new Views.MainWindow();
        app.Run(window);
    }
}
