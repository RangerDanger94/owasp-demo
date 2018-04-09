using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OWASP.Auth;
using OWASP.Auth.DependencyInjection;
using OWASP.Configuration;
using OWASP.DAL;
using OWASP.DAL.Security;

namespace OWASP
{
    public class Startup
    {
        public IHostingEnvironment Environment;
        public IConfiguration Configuration;

        public Startup(IHostingEnvironment environment)
        {
            Environment = environment;

            ConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile("appsettings.json", true, true);
            configBuilder.AddJsonFile($"appsettings.{environment.EnvironmentName}.json", false, true);

            Configuration = configBuilder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            ApplicationOptions configuration = Configuration.Get<ApplicationOptions>();

            services.AddAuthorization()
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = Authentication.DefaultAuthenticationScheme;
                    options.DefaultChallengeScheme = Authentication.DefaultAuthenticationScheme;
                })
                .AddCustomAuth();

            services.AddSingleton<ApplicationOptions>(configuration)
                .AddSingleton<ISecurityOptions>(configuration)
                .AddTransient<SecurityService>()
                .AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                }, ServiceLifetime.Transient)
                .AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
        }
    }
}
