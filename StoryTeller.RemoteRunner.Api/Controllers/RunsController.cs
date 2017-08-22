using System;
using System.Threading.Tasks;
using StoryTeller.Engine;
using System.Web.Http;
using StoryTeller.RemoteRunner.Api.Models;

namespace StoryTeller.RemoteRunner.Controllers
{
    public class RunsController : ApiController
    {
        private readonly ISystem _system;

        public RunsController()
        {
            _system = new NulloSystem();
        }

        [HttpPost]
        public async Task<BatchRunResponse> AddRun([FromBody]Run run)
        {
            using (var runner = new StorytellerRunner(_system, run.SpecsDirectory))
            {
                runner.WriteResultsDocument(run.ResultsFileName);
                BatchRunResponse results = runner.RunAll(TimeSpan.MaxValue);
                return results;
            }
        }
    }
}
