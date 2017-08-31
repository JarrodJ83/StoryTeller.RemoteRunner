using System;
using System.Diagnostics;
using StoryTeller.RemoteRunner.Api;
using Topshelf;

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
        }
    }
}
