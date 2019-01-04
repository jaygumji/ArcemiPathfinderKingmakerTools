using System;
using Avalonia;
using Avalonia.Logging.Serilog;
using Arcemi.Pathfinder.Kingmaker.Editor.ViewModels;
using Arcemi.Pathfinder.Kingmaker.Editor.Views;

namespace Arcemi.Pathfinder.Kingmaker.Editor
{
    class Program
    {
        static void Main(string[] args)
        {
            BuildAvaloniaApp().Start<MainWindow>(() => new MainWindowViewModel());
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .LogToDebug();
    }
}
