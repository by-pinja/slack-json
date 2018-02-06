using System;
using Microsoft.Extensions.Options;
using Slack.Json.Github;
using Slack.Json.Slack;
using Xunit;

namespace Slack.Json.Tests
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
            var slack = new SlackMessaging(Options.Create(new AppOptions()));
            slack.Send("#jenkins", null);
        }
    }
}