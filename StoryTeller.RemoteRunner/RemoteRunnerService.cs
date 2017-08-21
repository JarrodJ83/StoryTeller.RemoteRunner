using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace StoryTeller.RemoteRunner
{
    public class RemoteRunnerService : IHostedService
    {
        private IWebHost _webHost;
        private CancellationTokenSource _cancellationTokenSource;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _webHost = WebHost.CreateDefaultBuilder()
                .UseStartup<RemoteRunnerStartup>()
                .UseKestrel(options =>
                {
                    options.Listen(IPAddress.Loopback, 5000);
                })
                .Build();

            return _webHost.StartAsync(_cancellationTokenSource.Token);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _webHost.StopAsync(_cancellationTokenSource.Token).Wait(30000);
            _cancellationTokenSource.Cancel();
            _webHost.Dispose();
            return Task.CompletedTask;
        }
    }
}
