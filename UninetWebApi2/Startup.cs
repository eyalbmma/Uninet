using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cors;
//using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System.Text;
using Uninet.APP.Interfaces;
using Uninet.APP.Services;
using Uninet.DATA.Interfaces;
using Uninet.DATA.Services;
using Uninet.DATA.Services.MultipleContext;
using Uninet.Domain.Classes;
using Uninet.Domain.Interfaces;
using Microsoft.Extensions.Hosting;
using System.IdentityModel.Tokens.Jwt;
using Uninet.Domain.Models;
using UninetWebApi2.Middleware;
using System.Security.Cryptography;

namespace UninetWebApi2
{
    public class Startup
    {
         public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

       

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddSwaggerGen();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Workspaces.API", Version = "v1" });
            });
            services.AddControllers();
            services.AddDbContext<UninetContext>(options =>
              options.UseSqlServer(Configuration.GetConnectionString("AppConnectionString")));
            //services.AddDbContextPool<UninetContext>(options =>
            //{
            //    options.UseSqlServer(Configuration.GetConnectionString("AppConnectionString"));
            //});

            services.AddScoped<IPWhitelistMiddleware>();
            services.AddSingleton<EnumRepository>();


            services.Configure<ExternalTokenConfig>(Configuration.GetSection("ExternalTokens"));




            //services.AddDbContextPool<MongoDbContext>(options =>
            //{
            //    options.UseSqlServer("mongodb://clicksLiteAdmin:clicksLiteAdmin@mklx219:27111,mklx220:27111,mklx221:27111/?replicaSet=mongoFutst");
            //});


            services.AddSingleton<IMongoClient, MongoClient>(sp => new MongoClient(Configuration.GetConnectionString("MongoDb")));
            
            services.AddScoped<IUninetSimulateGreenVoiceAppServices, UninetSimulateGreenVoiceAppServices>();
            services.AddScoped<IUninetSimulateGreenVoiceServiceDataAccess, UninetSimulateGreenVoiceServiceDataAccess>();





          


            services.AddScoped<IUserServiceDataAccess, UserServiceDataAccess>();
            services.AddScoped<IUserServiceApp, UserServiceApp>();
            services.AddScoped<IMailassist, Mailassist>();
            services.AddScoped<IJwtDataAccessService, JwtDataAccessService>();
            services.AddScoped<IjwtAppService, jwtAppService>();
            services.AddScoped<IDataMailassist, DataMailassist>();

            services.AddScoped<IbillingService, billingServiceApp>();
            services.AddScoped<IbillingServiceDataaccess, billingServiceDataaccess>();
            services.AddScoped<IUninetOutPutAppService, UninetOutPutAppService>();
            services.AddScoped<IUninetOutputDataAccess, UninetOutputDataAccess>();

            //services.AddScoped<PullUsersData>();
            services.AddScoped<IUninetInputDataAccess, UninetInputDataAccess>();
            services.AddScoped<IUninetInputAppService, UninetInputAppService>();
            services.AddScoped<IRepository<UninetContext>, Repository<UninetContext>>();


            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IRefreshTokenRepository,RefreshTokenRepository>();

            services.AddScoped<IExternalSystemService, ExternalSystemService>();
            services.AddScoped <IExternalSystemRepository, ExternalSystemRepository>();

            //services.AddSingleton<IUninetInputDataAccess, UninetInputDataAccess>();  //remark eyal i changed it to singelton because  i inject it to PullUsersData which is a singelton
            //services.AddSingleton<IUninetInputAppService, UninetInputAppService>();//remark eyal i changed it to singelton because  i inject it to PullUsersData which is a singelton
            //services.AddSingleton<IRepository<UninetContext>, Repository<UninetContext>>();
            // 
            //services.AddIdentity<IdentityUser, IdentityRole>();


            //services.AddDbContext<UninetBatchContext>(options =>
            //{
            //    options.UseSqlServer(Configuration.GetConnectionString("AppConnectionString"));
            //}, ServiceLifetime.Singleton);

            ////add service to check if there are users to collect their data from external  system 
            //services.AddHostedService<PullUsersData>();//remark eyal this is a  singleton service, as it is added as a hosted service using 
            //services.AddSingleton<IuninetBatchDataAccess, uninetBatchDataAccess>();



            //services.AddSingleton<IBatchRepository<UninetBatchContext>, BatchRepository<UninetBatchContext>>();


            //services.AddDbContext<UninetBatchContext>(options =>
            //{
            //    options.UseSqlServer(Configuration.GetConnectionString("AppConnectionString"));
            //}, ServiceLifetime.Singleton);

            ////for batch services
            //services.AddSingleton<IBatchRepository<UninetBatchContext>, BatchRepository<UninetBatchContext>>();

            //services.AddSingleton<IuninetBatchDataAccess, uninetBatchDataAccess>();

            //services.AddSingleton<IHostedService, PullUsersData>();
            //services.AddSingleton<IHostedService, PushExpensesToCompanyClient>();

            //services.AddSingleton<IBatchDataMailassist, BatchDataMailassist>();


            var jwtTokenConfig = Configuration.GetSection("jwtTokenConfig");
            var base64Secret = jwtTokenConfig.GetValue<string>("secret");
            var issuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(base64Secret));

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer("AdminScheme", options =>  // HS256 עבור משתמשים
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(Configuration["jwtTokenConfig:secret"])),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            })
            .AddJwtBearer("ExternalScheme", options => // RS256 עבור מערכות חיצוניות
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;

                // Load RSA public key
                var rsa = RSA.Create();
                rsa.ImportFromPem(File.ReadAllText("C:\\Users\\eyalber1.CLALIT\\Documents\\jwt-keys\\public_key.pem").ToCharArray());
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new RsaSecurityKey(rsa),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy =>
                {
                    policy.AuthenticationSchemes.Add("AdminScheme");
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("systemType", "adminUser");
                });

                options.AddPolicy("ExternalPolicy", policy =>
                {
                    policy.AuthenticationSchemes.Add("ExternalScheme");
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("systemType", "externalSystem");
                });
            });





            //services.AddCors(options =>

            // Inside the ConfigureServices method of Startup.cs
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });

           





            ///add google section
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            //})
            //.AddCookie(options =>
            //{
            //    options.LoginPath = "/Account/Login";
            //})
            //.AddGoogle(options =>
            //{
            //    //Configuration.GetValue<string>("jwtTokenConfig:issuer"),
            //    options.ClientId = Configuration["Authentication:Google:ClientId"];
            //    options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
            //});



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            
            
            
            app.UseCors("AllowAllOrigins");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // Inside the Configure method of Startup.cs
               
                
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Workspaces.API v1"));
           


            app.UseMiddleware<IPWhitelistMiddleware>();

            
            app.UseHttpsRedirection();

            app.UseRouting();
            

            app.UseAuthentication();
            
            app.UseAuthorization();
            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
