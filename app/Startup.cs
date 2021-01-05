using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shikimori.Agent;
using Shikimori.App.Services;
using Shikimori.Data;
using Shikimori.Store;
using System;

namespace Shikimori.App
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var databaseConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? _configuration.GetConnectionString("DefaultConnection");

            services.AddCors();
            services.AddHttpContextAccessor();
            services.AddDbContext<ShikimoriDbContext>(options =>
            {
                options.UseNpgsql(databaseConnectionString, builder => { builder.MigrationsAssembly("shikimori.data"); });
            });
            services.AddScoped<IDatabaseStore, DatabaseStore>();
            services.AddScoped<ILoader, Loader>();
            services.AddSingleton<IAgentService, AgentService>();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            using var dbContext = serviceScope.ServiceProvider.GetService<ShikimoriDbContext>();
            dbContext.Database.Migrate();

            app.UseCors(x => x
              .AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
            );
            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
