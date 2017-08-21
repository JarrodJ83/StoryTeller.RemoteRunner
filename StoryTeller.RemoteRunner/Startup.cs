
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace StoryTeller.RemoteRunner
{
    public class RemoteRunnerStartup
    {
        #region snippet_Configure

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseMvcWithDefaultRoute();

            app.Run(async (context) =>
            {
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore();
        }

        #endregion
    }
}