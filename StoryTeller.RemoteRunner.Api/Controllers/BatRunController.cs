using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using StoryTeller.Portal.ResultsAggregator.Client;
using StoryTeller.RemoteRunner.Api.Models;

namespace StoryTeller.RemoteRunner.Api.Controllers
{
    public class BatRunsController : ApiController
    {
        [HttpPost]
        public async Task<string> AddBatRun([FromBody]BatRun batRun)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = batRun.RunExe,
                    Arguments = batRun.Args,
                    UseShellExecute = true,
                    RedirectStandardOutput = false,
                    CreateNoWindow = true
                }
            };
            
            process.Start();

            process.WaitForExit();
            
            return string.Empty;
        }
    }
}
