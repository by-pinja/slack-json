using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
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
            var fether = new SlackActionFetcher(Options.Create(new AppOptions {
                GithubPersonalAccessToken = "revoked_this"
            }), Substitute.For<ILogger<SlackActionFetcher>>());
            //var result = fether.GetJsonIfAny("protacon", "testrepo");
        }

        [Fact(Skip="tester")]
        public void TesterForSlackIntegration()
        {
            //var slack = new SlackMessaging(Options.Create(new AppOptions()), Substitute.For<ILogger<SlackMessaging>>());
            //slack.Send("#jenkins", null);
        }
    }
}