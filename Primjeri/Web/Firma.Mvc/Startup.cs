using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Firma.Mvc
{
  public class Startup
  {
    public IConfiguration Configuration { get; }
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }
  
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllersWithViews();
     
      //application settings
      var appSection = Configuration.GetSection("AppSettings");
      services.Configure<AppSettings>(appSection);

      services.AddDbContext<Models.FirmaContext>(options => 
                                                  options.UseSqlServer(
                                                            Configuration.GetConnectionString("Firma")
                                                                         .Replace("sifra", Configuration["FirmaSqlPassword"])
                                                            ));
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseRouting();

      app.UseStaticFiles();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute("Mjesta i artikli",
            "{controller:regex(^(Mjesto|Artikl)$)}/Page{page}/Sort{sort:int}/ASC-{ascending:bool}",
            new { action = "Index" }
            );
        endpoints.MapControllerRoute(
          name: "default",
          pattern: "{controller=Home}/{action=Index}/{id?}");
    });
    }
  }
}
