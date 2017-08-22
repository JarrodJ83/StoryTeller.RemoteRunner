using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Microsoft.Owin.Hosting;
using Owin;
using StoryTeller.RemoteRunner.Api;
using Topshelf;
using StoryTeller.RemoteRunner.Controllers;

namespace SystemUnderTest.Specs
{
    class Program
    {
        public static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<RemoveRunService>(s =>
                {
                    s.ConstructUsing(() => new RemoveRunService());
                    s.WhenStarted(service => service.Start("http://localhost:9000"));
                    s.WhenStopped(service => service.Stop());
                });
            });

            if (Debugger.IsAttached)
            {
                Console.ReadLine();
            }
        }
    }
}
