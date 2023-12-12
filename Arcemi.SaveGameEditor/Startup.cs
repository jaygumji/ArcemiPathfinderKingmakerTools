using Arcemi.Models;
using Arcemi.SaveGameEditor.Models;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Arcemi.SaveGameEditor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddTransient(p => p.GetService<IEditFileSession>().Game.Definition.Resources);
            services.AddTransient<IGameResourcesProvider>(p => p.GetService<GameResources>());
            services.AddSingleton<IEditFileSession, EditFileSession>();
            services.AddSingleton<EditorConfiguration>();
            services.AddSingleton<CharacterViewModel>();
            services.AddSingleton<ArmiesViewModel>();
            services.AddSingleton<TasksViewModel>();
            services.AddSingleton<StateManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            //if (env.IsDevelopment()) {
            //    app.UseDeveloperExceptionPage();
            //}
            //else {
            //    app.UseExceptionHandler("/Error");
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
                //endpoints.MapRazorPages();
            });

            if (HybridSupport.IsElectronActive) {
                SetupElectron();
            }
        }

        private async void SetupElectron()
        {
            var options = new BrowserWindowOptions {
                Show = true,
                Width = 1024,
                Height = 768,
                WebPreferences = new WebPreferences {
                    WebSecurity = false,
                    NodeIntegration = false
                }
            };
            var window = await Electron.WindowManager.CreateWindowAsync(options);

            //var menu = new MenuItem[] {
            //    new MenuItem
            //    {
            //        Label = "File",
            //        Type = MenuType.submenu,
            //        Submenu = new MenuItem[] {
            //            new MenuItem {
            //                Label = "Open...",
            //                Click = async () =>  await Electron.Dialog.ShowMessageBoxAsync("Open menu clicked"),
            //            },
            //            new MenuItem {
            //                Type = MenuType.separator,
            //            },
            //            new MenuItem {
            //                Label = "Exit",
            //                Role = MenuRole.close
            //            }
            //        }
            //    }
            //};
            //Electron.Menu.SetApplicationMenu(menu);

            window.OnClosed += () => {
                Electron.App.Quit();
            };
        }
    }
}
