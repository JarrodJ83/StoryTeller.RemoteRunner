using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StoryTeller;

namespace SystemUnderTest.Specs
{
    public class TestFixture : Fixture
    {
        public void Sleep(int seconds)
        {
            Thread.Sleep(TimeSpan.FromSeconds(seconds));
        }
    }
}
