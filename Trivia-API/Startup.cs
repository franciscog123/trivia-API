using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Trivia_API
{
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
            services.AddControllers();

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            Microsoft.AspNetCore.Hosting.IWebHostEnvironment env = serviceProvider.GetService<IWebHostEnvironment>();

            if (env.IsProduction())
            {
                services.AddDbContext<TriviaGameDBContext>(options =>
                options.UseSqlServer(Configuration["Section:TRIVIADB"]));
            }
            else
            {
                services.AddDbContext<TriviaGameDBContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("TRIVIADB")));
            }
            /*bool isProduction = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";
           
            if(isProduction)
            {
                services.AddDbContext<TriviaGameDBContext>(options =>
                options.UseSqlServer(Configuration["Section:TRIVIADB"]));
            }
            else
            {
                services.AddDbContext<TriviaGameDBContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("TRIVIADB")));
            }*/

            services.AddScoped<IUserRepository, UserRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.Run(async context =>
            {
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync($@"SecretName (Name in Key Vault: 'SecretName'){Environment.NewLine}Obtained from Configuration with Configuration[""SecretName""]{Environment.NewLine}Value: {Configuration["SecretName"]}{Environment.NewLine}{Environment.NewLine}Section:SecretName (Name in Key Vault: 'Section--SecretName'){Environment.NewLine}Obtained from Configuration with Configuration[""Section:SecretName""]{Environment.NewLine}Value: {Configuration["Section:SecretName"]}{Environment.NewLine}{Environment.NewLine}Section:SecretName (Name in Key Vault: 'Section--SecretName'){Environment.NewLine}Obtained from Configuration with Configuration.GetSection(""Section"")[""SecretName""]{Environment.NewLine}Value: {Configuration.GetSection("Section")["SecretName"]}");
            });
        }
    }
}
