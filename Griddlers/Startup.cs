using System.IO;
using Griddlers.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Griddlers;

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
        services.AddCors();
        services.AddRazorPages();
        services.AddSignalR();
        services.AddSpaStaticFiles(configuration =>
        {
            configuration.RootPath = "ClientApp/build";
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
        }

        app.UseStaticFiles();
        app.UseSpaStaticFiles();
        app.UseCors(o => o.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
        app.UseRouting();

        app.UseEndpoints(e =>
        {
            e.MapRazorPages();
            e.MapHub<GriddlerHub>("/griddlerhub");
            e.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
        });

        // app.UseSpa(spa =>
        // {
        //     spa.Options.SourcePath = Path.Join(env.ContentRootPath, "ClientApp");

        //     if (env.IsDevelopment())
        //     {
        //         spa.UseReactDevelopmentServer("start");
        //     }
        // });
    }
}
