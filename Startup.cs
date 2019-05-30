using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Slack.Json.Github;
using Slack.Json.Actions;
using Slack.Json.Slack;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.OpenApi.Models;

namespace Slack.Json
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration config)
        {
            Configuration = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                var basePath = AppContext.BaseDirectory;

                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "Slack integration",
                        Version = "v1",
                        Description = File.ReadAllText(Path.Combine(basePath, "README.md"))
                    });
            });

            services.Configure<AppOptions>(Configuration);
            services.AddMvc();

            services.AddTransient<ISlackMessaging, SlackMessaging>();
            services.AddTransient<ISlackActionFetcher, SlackActionFetcher>();

            ActionFactory.AddActionFactoryServicesToDi(serviceCollection: services);

            services.AddTransient<ActionFactory>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Slack.Json");
                c.RoutePrefix = "doc";
            });
        }
    }
}
