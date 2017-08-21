using System.Buffers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Formatters.Json.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;

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
            services.AddMvcCore(ops =>
            {
                ops.InputFormatters.Add(
                    new JsonInputFormatter(new ConsoleLogger("JsonSerialization", (name, level) => true, true),
                        new JsonSerializerSettings(), ArrayPool<char>.Shared, new DefaultObjectPoolProvider()));
            });
        }

        #endregion
    }
}