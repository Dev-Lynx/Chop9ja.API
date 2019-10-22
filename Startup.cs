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
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Blueshift.Identity.MongoDB;
using MongoDB.Driver;
using Blueshift.EntityFrameworkCore.MongoDB.Infrastructure;
using Blueshift.EntityFrameworkCore.MongoDB.DependencyInjection;
using AspNetCore.Identity.Mongo;
using AspNetCore.Identity.Mongo.Model;
using MongoStores = AspNetCore.Identity.Mongo.Stores;
using Google.Cloud.Diagnostics.AspNetCore;
using Chop9ja.API.Services.Mail;
using AspNetCore.Identity.MongoDbCore.Models;
using AspNetCore.Identity.MongoDbCore;
using PayStack.Net;
using Chop9ja.API.Extensions.Conventions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Sieve.Services;
using Microsoft.Extensions.FileProviders;
using Chop9ja.API.Extensions.Configuration;


// July 22nd, 1998
namespace Chop9ja.API
{
    public class Startup
    {
        #region Properties

        #region Internals
        IConfiguration Configuration { get; }
        IHostingEnvironment CurrentEnvironment { get; }
        IServiceCollection ServiceCollection { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        JwtIssuerOptions JwtIssuerOptions { get; set; }

        #endregion

        #endregion

        #region Constructors
        public Startup(IHostingEnvironment env)
        {
            CurrentEnvironment = env;

            Configuration = new ConfigurationBuilder().SetBasePath(env.ContentRootPath)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.overrides.json", optional: true, reloadOnChange: true)
                .AddProtectedProvider("appsettings.secrets.json", optional: true, reloadOnChange: true,
                    cipher: Environment.GetEnvironmentVariable(Core.CONFIG_PROTECTION_KEY))
                .AddEnvironmentVariables().Build();
        }
        #endregion

        #region Methods

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(mvc =>
            {
                mvc.Conventions.Add(new ControllerNameAttributeConvention());
                //mvc.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            }).SetCompatibilityVersion(
                CompatibilityVersion.Version_2_2)
                .AddControllersAsServices();

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
            });

            /*
            bool isDevelopment = CurrentEnvironment.IsDevelopment();
            if (isDevelopment)
            {
                services.AddHttpsRedirection(options =>
                {
                    options.RedirectStatusCode = isDevelopment ? StatusCodes.Status307TemporaryRedirect : StatusCodes.Status308PermanentRedirect;
                    options.HttpsPort = isDevelopment ? 5001 : 443;
                });
            }
            */
            

            services.AddAutoMapper(config => config.AddProfile<ViewModelToEntityProfile>(), Assembly.GetCallingAssembly());
            //services.AddDbContext<UserDataContext>(opt => opt.UseMongoDb(new MongoUrl(ConnectionString)));

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });


            ConfigureOptions(services);
            ConfigureAuthentication(services);

            EmailSettings emailSettings = Configuration.GetSection(EmailSettings.ConfigKey).Get<EmailSettings>();
            services.AddFluentEmail(emailSettings.EmailAddress)
                .AddMailGunSender(emailSettings.Domain, emailSettings.ClientSecret);

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "wwwroot";
            });

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

            

            ServiceCollection = services;
            Core.RegisterServiceProvider(services.BuildServiceProvider());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            
            app.UseForwardedHeaders();
            /*
            app.Use(async (context, next) =>
            {
                if (context.Request.IsHttps || context.Request.Headers["X-Forwarded-Proto"] == Uri.UriSchemeHttps)
                {
                    await next();
                }
                else
                {
                    string queryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty;
                    string https = "https://" + context.Request.Host + context.Request.Path + queryString;
                    Core.Log.Warn(https);
                    context.Response.Redirect(https);
                }
            });
            */

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
            // app.UseHttpsRedirection();

            

            ConfigureDocumentation(app);

            app.ConfigureExceptionHandler(loggerFactory);

            

            app.UseMvc();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseSpa(spa => { });
        }

        public void ConfigureContainer(IUnityContainer container)
        {
            container.AddNewExtension<UnityFallbackProviderExtension>();
            container.AddNewExtension<AutomaticBuildExtension>();
            container.AddNewExtension<DeepDependencyExtension>();
            container.AddNewExtension<DeepMethodExtension>();
            container.RegisterInstance(new LoggerFactory().AddNLog());
            container.RegisterSingleton(typeof(ILogger<>), typeof(LoggingAdapter<>));


            container.RegisterType<IUserStore<User>, MongoUserStore<User, UserRole, MongoDataContext, Guid>>();
            container.RegisterType<IRoleStore<UserRole>, MongoRoleStore<UserRole, MongoDataContext, Guid>>();

            container.RegisterFactory<IPayStackApi>(c => new PayStackApi(c.Resolve<IOptions<PaystackOptions>>().Value.SecretKey));
            container.RegisterTransient<IPaymentService, PaymentService>();
            //container.RegisterType<IUserStore<User>, MongoStores.UserStore<User, UserRole>>();
            //container.RegisterType<IRoleStore<UserRole>, MongoStores.RoleStore<UserRole>>();
            
            //container.RegisterTransient<UserDataContext>();

            container.RegisterFactory<MongoDataContext>(s => new MongoDataContext(ConnectionString, DatabaseName));
            container.RegisterScoped<ISieveProcessor, SieveProcessor>();

            container.RegisterControllers();

            Core.ConfigureCoreServices(container);


            
            container.RegisterScoped<IAuthService, AuthService>();
            container.RegisterScoped<IJwtFactory, JwtFactory>();
            container.RegisterScoped<IEmailService, GoogleMailService>();
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

            var auth = Configuration.GetSection(AuthSettings.ConfigKey).Get<AuthSettings>();
            byte[] bytes = Encoding.ASCII.GetBytes(auth.Key);

            services.AddScoped<ITokenGenerator, TokenGenerator>(s => new TokenGenerator(bytes));

            services.Configure<EmailSettings>(Configuration.GetSection(EmailSettings.ConfigKey));
            services.Configure<SMSOptions>(Configuration.GetSection(SMSOptions.ConfigKey));
            services.Configure<AuthSettings>(Configuration.GetSection(AuthSettings.ConfigKey));
            services.Configure<PaystackOptions>(Configuration.GetSection(PaystackOptions.ConfigKey));

            services.Configure<JwtIssuerOptions>(opt =>
            {
                SymmetricSecurityKey key = new SymmetricSecurityKey(bytes);
                JwtIssuerOptions = Configuration.GetSection(JwtIssuerOptions.ConfigKey)
                    .Get<JwtIssuerOptions>();
                JwtIssuerOptions.SigningCredentials = new SigningCredentials(key, 
                    SecurityAlgorithms.HmacSha512Signature);

                var options = JwtIssuerOptions;
                opt.Audience = options.Audience;
                opt.Issuer = options.Issuer;
                opt.SigningCredentials = JwtIssuerOptions.SigningCredentials;
                opt.Subject = opt.Subject;
            });

            var mongoDbOptions = Configuration.GetSection(MongoDBOptions.CONFIG_KEY).Get<MongoDBOptions>();
            ConnectionString = Configuration.GetConnectionString("MongoDBAtlasConnection");
            ConnectionString = ConnectionString.BindTo(mongoDbOptions);
            DatabaseName = mongoDbOptions.DatabaseName;
            services.Configure<MongoDBOptions>((opt) => opt = mongoDbOptions);

            return services;
        }

        IServiceCollection ConfigureAuthentication(IServiceCollection services)
        {
            services.AddCors();

            services.AddIdentity<User, UserRole>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequiredLength = 6;
                
                opt.User.RequireUniqueEmail = true;
            })
            .AddMongoDbStores<User, UserRole, Guid>(new MongoDataContext(ConnectionString, DatabaseName))
            .AddDefaultTokenProviders();

            // services.AddTransient(s => new MongoDataContext(ConnectionString, "__identities"));
            

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                //options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(configureOptions =>
            {
                var options = Configuration.GetSection(JwtIssuerOptions.ConfigKey).Get<JwtIssuerOptions>();
                var auth = Configuration.GetSection(AuthSettings.ConfigKey).Get<AuthSettings>();
                byte[] bytes = Encoding.ASCII.GetBytes(auth.Key);
                SymmetricSecurityKey key = new SymmetricSecurityKey(bytes);

                configureOptions.ClaimsIssuer = options.Issuer;
                configureOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = options.Issuer,

                    ValidateAudience = true,
                    ValidAudience = options.Audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,

                    RequireExpirationTime = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                configureOptions.SaveToken = true;
            });


            services.AddAuthorization(options =>
            {
                /*
                options.AddPolicy("ApiUser", config => config.RequireRole(Enum.GetValues(typeof(UserRoles)).
                    OfType<UserRoles>().Select(u => u.ToString()).ToArray()));*/
                //options.AddPolicy("ApiUser", policy => policy.RequireClaim(Core.JWT_CLAIM_ROL, UserRole.RegularUser.ToString()));
            });


            return services;
        }
        #endregion

        #endregion
    }
}
