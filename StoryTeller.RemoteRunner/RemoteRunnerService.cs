using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using PeterKottas.DotNetCore.WindowsService.Interfaces;

namespace StoryTeller.RemoteRunner
{
    public class RemoteRunnerService : IMicroService
    {
        private IWebHost _webHost;
        private CancellationTokenSource _cancellationTokenSource;
        
        public void Start()
        {
            _webHost = WebHost.CreateDefaultBuilder()
                .UseStartup<RemoteRunnerStartup>()
                .UseKestrel(options =>
                {
                    options.Listen(IPAddress.Loopback, 5000);
                })
                .Build();

            _webHost.Start();
        }

        public void Stop()
        {
            _webHost.StopAsync().Wait();
            _webHost.Dispose();

        }
    }
}
