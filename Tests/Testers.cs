using System;
using Microsoft.Extensions.Options;
using Slack.Integration.Github;
using Slack.Integration.Slack;
using Xunit;

namespace Slack.Integration.Tests
{
    public class Testers
    {
        [Fact(Skip="tester")]
        public void TesterForSlackJsonFetcher()
        {
            var fether = new SlackFileFetcher(Options.Create(new AppOptions {
                GithubPersonalAccessToken = "revoked_this"
            }));
            var result = fether.GetJsonIfAny("protacon", "testrepo");
        }

        [Fact(Skip="tester")]
        public void TesterForSlackIntegration()
        {
            var slack = new SlackMessage(Options.Create(new AppOptions()));
            slack.Send("#jenkins", "testtesttest");
        }
    }
}