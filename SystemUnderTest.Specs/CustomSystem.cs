using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using StoryTeller;
using StoryTeller.Engine;
using StoryTeller.Portal.ResultsAggregator;
using StoryTeller.Portal.ResultsAggregator.Client;
using StoryTeller.Remotes.Messaging;

namespace SystemUnderTest.Specs
{
    public class CustomSystem : ISystem
    {
        private PortalResultsAggregatorClient _portalClient;
        public CustomSystem()
        {
            _portalClient = new PortalResultsAggregatorClient(ConfigurationManager.AppSettings["PortalUrl"], ConfigurationManager.AppSettings["PortalApiKey"]);

            var runLoggerSettings = new RunLoggerSettings(".\\stresults.htm", new DateTimeRunNameGenerator());

            var portalRunLogger = new RunLogger(_portalClient, runLoggerSettings);
            EventAggregator.Messaging.AddListener(portalRunLogger);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
        }

        public CellHandling Start()
        {
            var handling = CellHandling.Basic();
            
            handling.Extensions.Add(new SpecResultLoggingExtension(_portalClient));

            return handling;
        }

        public IExecutionContext CreateContext()
        {
            return new SimpleExecutionContext();
        }

        public Task Warmup()
        {
            return Task.CompletedTask;
        }
    }
}
