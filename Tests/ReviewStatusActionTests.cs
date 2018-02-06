using Microsoft.Extensions.Logging;
using NSubstitute;
using Slack.Json.Actions;
using Slack.Json.Github;
using Slack.Json.Slack;
using Slack.Json.Tests.GithubRequestPayloads;
using Xunit;

namespace Slack.Json.Tests
{
    public class ReviewStatusActionTests
    {
        [Fact]
        public void WhenChangesAreRequested_ThenSendCorrectSlackMessage()
        {
            ISlackFileFetcher fetcher = DepencyMockFactories.SlackFileFetcherMock("review_status", "#general");

            var slack = Substitute.For<ISlackMessaging>();

            var requestAction = new ReviewStatusAction(fetcher, slack, Substitute.For<ILogger<PullRequestAction>>());

            requestAction.Execute(TestPayloads.ReviewSubmit());

            slack.Received(1).Send(Arg.Is<string>("#general"), Arg.Is<SlackMessageModel>(x => x.Text.Contains("Rejected") && x.Color == "danger"));
        }
    }
}