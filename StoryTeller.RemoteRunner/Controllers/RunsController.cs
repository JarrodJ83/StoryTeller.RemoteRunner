using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace StoryTeller.RemoteRunner.Controllers
{
    [Route("api/Runs")]
    public class RunsController
    {
        [HttpPost]
        public async Task<IActionResult> AddRun(object run)
        {
            return new OkObjectResult("Added!");
        }
    }
}
