namespace StoryTeller.RemoteRunner.Api.Models
{
    public class Run
    {
        public int TimeOutSeconds { get; set; } = int.MaxValue;
        public string SpecsDirectory { get; set; }
        public string ResultsFileName { get; set; }
    }
}
