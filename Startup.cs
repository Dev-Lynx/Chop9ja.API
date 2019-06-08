using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Chop9ja.API.Controllers;
using Chop9ja.API.Data;
using Chop9ja.API.Extensions;
using Chop9ja.API.Extensions.Logging;
using Chop9ja.API.Extensions.UnityExtensions;
using Chop9ja.API.Models.Entities;
using Chop9ja.API.Models.Options;
using Chop9ja.API.Services;
using Chop9ja.API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NLog.Extensions.Logging;
using Unity;
using Unity.Lifetime;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NSwag;
using NSwag.SwaggerGeneration.Processors.Security;
using PhoneNumbers;
using Chop9ja.API.Models.Mapping;
using Chop9ja.API.Extensions.RedocExtensions;

namespace Chop9ja.API
{
    public class Startup
    {
        #region Properties

        #region Internals
        IConfiguration Configuration { get; }
        #endregion

        #endregion

        #region Constructors
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        #endregion

        #region Methods

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(
                CompatibilityVersion.Version_2_1)
                .AddControllersAsServices();

            services.AddAutoMapper(config => config.AddProfile<ViewModelToEntityProfile>(), Assembly.GetCallingAssembly());

            services.AddDbContext<UserDataContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")
                .Replace("|DataDirectory|", Core.DATA_DIR)));

            ConfigureOptions(services);
            ConfigureAuthentication(services);

            services.AddOpenApiDocument(doc =>
            {
                doc.Title = Core.PRODUCT_NAME;
                doc.DocumentName = Core.FULL_PRODUCT_NAME;
                doc.Description = $"API Documentation for {Core.PRODUCT_NAME} Betting Insurance Platform";
                doc.Version = "v1";

                doc.AddSecurity("JWT", Enumerable.Empty<string>(), new SwaggerSecurityScheme
                {
                    Type = SwaggerSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = SwaggerSecurityApiKeyLocation.Header,
                    Description = "Type into the textbox: Bearer {your JWT token}."
                });

                doc.OperationProcessors.Add(
                    new AspNetCoreOperationSecurityScopeProcessor("JWT"));
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

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseAuthentication();
            app.UseMvc();
            

            ConfigureDocumentation(app);
            Core.Initialize();
        }

        public void ConfigureContainer(IUnityContainer container)
        {
            container.AddNewExtension<AutomaticBuildExtension>();
            container.AddNewExtension<DeepDependencyExtension>();
            container.AddNewExtension<DeepMethodExtension>();

            container.RegisterInstance(new LoggerFactory().AddNLog());
            container.RegisterSingleton(typeof(ILogger<>), typeof(LoggingAdapter<>));

            container.RegisterTransient<UserDataContext>();

            container.RegisterControllers();

            Core.ConfigureCoreServices(container);

            container.RegisterScoped<IAuthService, AuthService>();
            container.RegisterScoped<IJwtFactory, JwtFactory>();
            container.RegisterScoped<IEmailService, EmailService>();
            container.RegisterScoped<ISmsService, SmsService>();
            container.RegisterFactory<PhoneNumberUtil>(c => PhoneNumberUtil.GetInstance());
        }

        #region Internal Configurations
        IApplicationBuilder ConfigureDocumentation(IApplicationBuilder app)
        {
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

            return app;
        }

        IServiceCollection ConfigureOptions(IServiceCollection services)
        {
            services.AddOptions();

            services.Configure<EmailSettings>(Configuration.GetSection(EmailSettings.ConfigKey));
            services.Configure<SMSOptions>(Configuration.GetSection(SMSOptions.ConfigKey));
            services.Configure<AuthSettings>(Configuration.GetSection(AuthSettings.ConfigKey));

            services.Configure<JwtIssuerOptions>(opt =>
            {
                var options = Configuration.GetSection(JwtIssuerOptions.ConfigKey)
                    .Get<JwtIssuerOptions>();
                var auth = Configuration.GetSection(AuthSettings.ConfigKey)
                    .Get<AuthSettings>();

                byte[] bytes = Encoding.ASCII.GetBytes(auth.Key);
                SymmetricSecurityKey key = new SymmetricSecurityKey(bytes);

                opt.Audience = options.Audience;
                opt.Issuer = options.Issuer;
                opt.SigningCredentials = new SigningCredentials(key,
                    SecurityAlgorithms.HmacSha512Signature);
                opt.Subject = opt.Subject;
            });

            return services;
        }

        IServiceCollection ConfigureAuthentication(IServiceCollection services)
        {
            services.AddCors();

            services.AddIdentity<User, IdentityRole>(u =>
            {
                u.Password.RequireDigit = true;
                u.Password.RequireLowercase = true;
                u.Password.RequireUppercase = true;
                u.Password.RequiredLength = 8;
                u.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<UserDataContext>().AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(configureOptions =>
            {
                var options = services.BuildServiceProvider().
                    GetService<IOptions<JwtIssuerOptions>>().Value;

                configureOptions.ClaimsIssuer = options.Issuer;
                configureOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = options.Issuer,

                    ValidateAudience = true,
                    ValidAudience = options.Audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = options.SigningCredentials.Key,

                    RequireExpirationTime = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                configureOptions.SaveToken = true;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiUser", config => config.RequireRole(Enum.GetValues(typeof(UserRole)).
                    OfType<UserRole>().Select(u => u.ToString()).ToArray()));
                //options.AddPolicy("ApiUser", policy => policy.RequireClaim(Core.JWT_CLAIM_ROL, UserRole.RegularUser.ToString()));
            });


            return services;
        }
        #endregion

        #endregion
    }
}
