using Microsoft.Extensions.Logging;
using NSubstitute;
using Optional;
using Slack.Json.Github;
using Slack.Json.Actions;
using Slack.Json.Slack;
using Slack.Json.Tests.GithubRequestPayloads;
using Xunit;
using System.Linq;
using System.Collections.Generic;

namespace Slack.Json.Tests
{
    public class PullRequestActionTests
    {
        [Fact]
        public void WhenRepositoryDoesntContainSlackJson_ThenIgnoreRequest()
        {
            var fetcher = Substitute.For<ISlackActionFetcher>();
            fetcher
                .GetJsonIfAny(Arg.Any<string>(), Arg.Any<string>())
                .Returns(Enumerable.Empty<SlackActionModel>());

            var slack = Substitute.For<ISlackMessaging>();

            var requestAction = new PullRequestAction(fetcher, slack, Substitute.For<ILogger<PullRequestAction>>());

            requestAction.Execute(TestPayloads.PullRequestOpened());

            slack.DidNotReceive().Send(Arg.Any<string>(), Arg.Any<SlackMessageModel>());
        }

        [Fact]
        public void WhenRepositoryContainsSlackJsonWithPullRequestAction_ThenSendMessage()
        {
            var fetcher = DepencyMockFactories.SlackFileFetcherMock("pull_request", "#general");

            var slack = Substitute.For<ISlackMessaging>();

            var requestAction = new PullRequestAction(fetcher, slack, Substitute.For<ILogger<PullRequestAction>>());

            requestAction.Execute(TestPayloads.PullRequestOpened());

            slack.Received(1).Send(Arg.Is<string>("#general"), Arg.Any<SlackMessageModel>());
        }

        [Fact]
        public void WhenSlackJsonDoesntContainPullRequestAction_ThenIgnoreSend()
        {
            var fetcher = Substitute.For<ISlackActionFetcher>();
            fetcher
                .GetJsonIfAny(Arg.Is<string>("protacon"), Arg.Is<string>("testrepo"))
                .Returns(Enumerable.Empty<SlackActionModel>());

            var slack = Substitute.For<ISlackMessaging>();

            var requestAction = new PullRequestAction(fetcher, slack, Substitute.For<ILogger<PullRequestAction>>());

            requestAction.Execute(TestPayloads.PullRequestOpened());

            slack.DidNotReceive().Send(Arg.Any<string>(), Arg.Any<SlackMessageModel>());
        }
    }
}
