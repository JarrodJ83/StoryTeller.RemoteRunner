using System;
using System.Collections.Generic;
using System.Text;

namespace StoryTeller.RemoteRunner.Models
{
    public class ExecutablRun
    {
        public string ExecutablePath { get; set; }
        public string[] Args { get; set; }
    }
}
