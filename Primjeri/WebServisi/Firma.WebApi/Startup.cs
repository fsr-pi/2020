using System;
using System.IO;
using System.Reflection;
using AutoMapper;
using CommandQueryCore;
using FCD.WebApi.Util.ServiceFilters;
using Firma.DAL.CommandHandlers;
using Firma.DAL.Models;
using Firma.DAL.QueryHandlers;
using Firma.DataContract.Commands;
using Firma.DataContract.DTOs;
using Firma.DataContract.QueryHandlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;


namespace Firma.WebApi
{
  public class Startup
  {
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;     
    }
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllers();

      services.AddDbContext<FirmaContext>(options =>
                                                   options.UseSqlServer(
                                                             Configuration.GetConnectionString("Firma")
                                                                          .Replace("sifra", Configuration["FirmaSqlPassword"])
                                                             ));

      #region query handler setup
      //umjesto repetitvnog pisanja može se riješiti refleksijom tražeći i registrirajući sve handlere (klase koje implementiraju neki IQueryHandler<,>)
      //iz typeof(ArtiklQueryHandler).Assembly
      services.AddTransient<IDrzaveQueryHandler, DrzaveQueryHandler>();
      services.AddTransient<IDrzavaQueryHandler, DrzavaQueryHandler>();
      services.AddTransient<IDrzavaCountQueryHandler, DrzavaCountQueryHandler>();
      services.AddTransient<ICommandHandler<DeleteDrzava>, DrzavaCommandHandler>();
      services.AddTransient<ICommandHandler<AddDrzava>, DrzavaCommandHandler>();
      services.AddTransient<ICommandHandler<UpdateDrzava>, DrzavaCommandHandler>();
      #endregion

      services.AddScoped<BadRequestOnException>();

      services.AddAutoMapper(typeof(Startup), typeof(DAL.DalMappingProfile));

      #region Swagger setup     
      services.AddSwaggerGen(c =>
      {        
        c.SwaggerDoc("v1", new OpenApiInfo
        {
          Title = "Firma.WebApi",
          Description = "Jednostan primjer Web API-a nad državama",
          Version = "v1"
        });

        //Set the comments path for the swagger json and ui.
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);

        //xmlFile = "Firma.DataContract.xml";
        //xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        //c.IncludeXmlComments(xmlPath);
      });


      #endregion
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      #region Swagger setup
      app.UseSwagger();

      // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
      // specifying the Swagger JSON endpoint.
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Firma.WebApi V1");
        c.RoutePrefix = string.Empty;
      });      
      #endregion

      app.UseStaticFiles();
      app.UseRouting();
      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
