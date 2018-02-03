using System;
using Slack.Integration.Github;
using Xunit;

namespace Slack.Integration.Tests
{
    public class Testers
    {
        [Fact(Skip="tester")]
        public void TesterForSlackJsonFetcher()
        {
            var fether = new SlackFileFetcher();
            var result = fether.GetJsonIfAny("protacon", "slack-integration");
        }
    }
}