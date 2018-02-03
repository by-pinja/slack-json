using Microsoft.Extensions.Logging;
using NSubstitute;
using Optional;
using Slack.Integration.Github;
using Slack.Integration.Actions;
using Slack.Integration.Slack;
using Slack.Integration.Tests.GithubRequestPayloads;
using Xunit;
using System.Linq;
using System.Collections.Generic;

namespace Slack.Integration.Tests
{
    public class PullRequestActionTests
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
        public void WhenRepositoryContainsSlackJsonWithPullRequestAction_ThenSendMessage()
        {
            var fetcher = Substitute.For<ISlackFileFetcher>();
            fetcher
                .GetJsonIfAny(Arg.Is<string>("protacon"), Arg.Is<string>("testrepo"))
                .Returns(new List<SlackActionModel>
                {
                    new SlackActionModel
                    {
                        EventType = "pull_request",
                        Enabled = true,
                        Channel = "#general"
                    }
                });

            var slack = Substitute.For<ISlackMessaging>();

            var requestAction = new PullRequestAction(fetcher, slack, Substitute.For<ILogger<PullRequestAction>>());

            requestAction.Execute(TestPayloads.PullRequestOpened());

            slack.Received(1).Send(Arg.Is<string>("#general"), Arg.Any<SlackMessageModel>());
        }

        [Fact]
        public void WhenSlackJsonDoesntContainPullRequestAction_ThenIgnoreSend()
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

        [Fact]
        public void WhenPullRequestIsNotOpened_ThenDontSendAnything()
        {
            var fetcher = Substitute.For<ISlackFileFetcher>();
            fetcher
                .GetJsonIfAny(Arg.Is<string>("protacon"), Arg.Is<string>("testrepo"))
                .Returns(new List<SlackActionModel>
                {
                    new SlackActionModel
                    {
                        EventType = "pull_request",
                        Enabled = true,
                        Channel = "#general"
                    }
                });

            var slack = Substitute.For<ISlackMessaging>();

            var requestAction = new PullRequestAction(fetcher, slack, Substitute.For<ILogger<PullRequestAction>>());

            requestAction.Execute(TestPayloads.PullRequestClosed());

            slack.DidNotReceive().Send(Arg.Any<string>(), Arg.Any<SlackMessageModel>());
        }
    }
}
