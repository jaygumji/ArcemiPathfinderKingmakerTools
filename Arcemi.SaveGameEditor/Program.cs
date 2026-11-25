using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Arcemi.SaveGameEditor
{
    public class Program
    {
        public static string Version { get; } = "3.0.0";

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseElectron(args, ElectronAppReady);
                    webBuilder.UseStartup<Startup>();
                });

        public static async Task ElectronAppReady()
        {
            var options = new BrowserWindowOptions {
                Show = false,
                Width = 1440,
                Height = 900,
                BackgroundColor = "#f3f6f8",
                IsRunningBlazor = true,
                WebPreferences = new WebPreferences {
                    WebSecurity = true,
                    NodeIntegration = false
                }
            };

            var window = await Electron.WindowManager.CreateWindowAsync(options);
            window.OnReadyToShow += () => window.Show();
            Electron.App.WindowAllClosed += Electron.App.Quit;
        }
    }
}
