using Arcemi.Models;
using Arcemi.SaveGameEditor.Models;
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
            services.AddControllers();
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
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
                //endpoints.MapRazorPages();
            });

        }
    }
}
