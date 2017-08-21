using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StoryTeller.RemoteRunner.Models;

namespace StoryTeller.RemoteRunner.Controllers
{
    [Route("api/[Controller]")]
    public class ExecutablRunsController
    {
        [HttpPost]
        public async Task<IActionResult> AddRun([FromBody]ExecutablRun run)
        {
            Process process = new Process();
            
            process.StartInfo.FileName = run.ExecutablePath;

            if(run.Args != null && run.Args.Any())
                process.StartInfo.Arguments = string.Join(" ", run.Args);

            process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            process.Start();
            process.WaitForExit();

            return new OkResult();
        }
    }
}
