using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Chop9ja.API.Controllers;
using Chop9ja.API.Extensions;
using Chop9ja.API.Extensions.Logging;
using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Services;
using Chop9ja.API.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog.Extensions.Logging;
using Unity;
using Unity.Lifetime;

namespace Chop9ja.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(
                CompatibilityVersion.Version_2_1)
                .AddControllersAsServices();

            services.AddOpenApiDocument(doc =>
            {
                doc.Title = Core.PRODUCT_NAME;
                doc.DocumentName = Core.FULL_PRODUCT_NAME;
                doc.Description = $"API Documentation for {Core.PRODUCT_NAME} Betting Insurance Platform";
                doc.Version = "v1";
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }
            else
            {
                app.UseHsts();
            }

            app.UseMvc();

            app.UseOpenApi(config =>
            {
                config.Path = Core.DOCS_ROUTE + "/swagger/{documentName}/swagger.json";
            });

            app.UseSwaggerUi3(config =>
            {
                config.Path = $"{Core.DOCS_ROUTE}/swagger";
                config.DocumentPath = Core.DOCS_ROUTE + "/swagger/{documentName}/swagger.json";
            });

            app.UseReDoc(config =>
            {
                config.Path = $"{Core.DOCS_ROUTE}/redoc";
                config.DocumentPath = Core.DOCS_ROUTE + $"/swagger/{Core.FULL_PRODUCT_NAME}/swagger.json";
            });
        }

        public void ConfigureContainer(IUnityContainer container)
        {
            container.AddNewExtension<AutomaticBuildExtension>();
            container.AddNewExtension<DeepDependencyExtension>();

            container.RegisterInstance(new LoggerFactory().AddNLog());
            container.RegisterSingleton(typeof(ILogger<>), typeof(LoggingAdapter<>));

            container.RegisterControllers();

            container.RegisterScoped<IAuthService, AuthService>();
        }
    }
}
