using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stateful_Auth_Sample.Authentication;
using Stateful_Auth_Sample.Models;

namespace Stateful_Auth_Sample
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
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CustomAuthenticationExtensions.CustomScheme;
                options.DefaultChallengeScheme = CustomAuthenticationExtensions.CustomScheme;
            }).AddCustomAuthentication(s => { });
            services.AddAuthorization(options =>
            {
                options.AddPolicies();
            });

            var sqliteConnection = Configuration["DataConnections:SqliteConnectionString"];
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(sqliteConnection);
            });

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
