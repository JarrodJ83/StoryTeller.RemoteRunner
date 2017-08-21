using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using PeterKottas.DotNetCore.WindowsService;

namespace StoryTeller.RemoteRunner.Host
{
    class Program
    {
        public static void Main(string[] args)
        {
            ServiceRunner<RemoteRunnerService>.Run(config =>
            {
                var name = config.GetDefaultName();
                config.Service(serviceConfig =>
                {
                    serviceConfig.ServiceFactory((extraArguments, controller) =>
                    {
                        return new RemoteRunnerService();
                    });
                    serviceConfig.OnStart((service, extraArguments) =>
                    {
                        Console.WriteLine("Service {0} started", name);
                        service.Start();
                    });

                    serviceConfig.OnStop(service =>
                    {
                        Console.WriteLine("Service {0} stopped", name);
                        service.Stop();
                    });

                    serviceConfig.OnError(e =>
                    {
                        Console.WriteLine("Service {0} errored with exception : {1}", name, e.Message);
                    });
                });
            });

            if (Debugger.IsAttached)
            {
                Console.WriteLine("Enter to exit");
                Console.ReadLine();
            }
        }
    }
}
