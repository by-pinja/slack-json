using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Slack.Integration.Actions;
using Slack.Integration.Github;
using Slack.Integration.Slack;
using Slack.Integration.Tests.GithubRequestPayloads;
using Xunit;

namespace Slack.Integration.Tests
{
    public class ReviewRequestActionTests
    {
        [Fact]
        public void WhenRepositoryDoesntContainSlackJson_ThenIgnoreRequest()
        {
            var fetcher = Substitute.For<ISlackFileFetcher>();
            fetcher
                .GetJsonIfAny(Arg.Any<string>(), Arg.Any<string>())
                .Returns(Enumerable.Empty<SlackActionModel>());

            var slack = Substitute.For<ISlackMessaging>();

            var requestAction = new PullRequestAction(fetcher, slack, Substitute.For<ILogger<PullRequestAction>>());

            requestAction.Execute(TestPayloads.PullRequestOpened());

            slack.DidNotReceive().Send(Arg.Any<string>(), Arg.Any<SlackMessageModel>());
        }

        [Fact]
        public void WhenValidRequestIsSent_ThenSendNotifications()
        {
            ISlackFileFetcher fetcher = DepencyMockFactories.SlackFileFetcherMock("review_request", "#general");

            var slack = Substitute.For<ISlackMessaging>();

            var requestAction = new ReviewRequestAction(fetcher, slack, Substitute.For<ILogger<PullRequestAction>>());

            requestAction.Execute(TestPayloads.ReviewRequestOpened());

            slack.Received(1).Send(Arg.Is<string>("#general"), Arg.Any<SlackMessageModel>());
        }

        [Fact]
        public void WhenSlackJsonDoesntContainReviewRequestAction_ThenIgnoreSend()
        {
            var fetcher = Substitute.For<ISlackFileFetcher>();
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