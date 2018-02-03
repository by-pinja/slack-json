using Microsoft.Extensions.Logging;
using NSubstitute;
using Optional;
using Slack.Integration.Github;
using Slack.Integration.Parsers;
using Slack.Integration.Slack;
using Slack.Integration.Tests.GithubRequestPayloads;
using Xunit;

namespace Slack.Integration.Tests
{
    public class PullRequestRealDataTests
    {
        [Fact]
        public void WhenRepositoryDoesntContainSlackJson_ThenIgnoreRequest()
        {
            var fetcher = Substitute.For<ISlackFileFetcher>();
            fetcher
                .GetJsonIfAny(Arg.Any<string>(), Arg.Any<string>())
                .Returns(Option.None<SlackJsonFileModel>());

            var slack = Substitute.For<ISlackMessage>();

            var requestAction = new PullRequestAction(fetcher, slack, Substitute.For<ILogger<PullRequestAction>>());

            requestAction.Execute(TestPayloads.PullRequestOpened());

            slack.DidNotReceive().Send(Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public void WhenRepositoryContainsSlackJsonWithPullRequestAction_ThenSendMessage()
        {
            var fetcher = Substitute.For<ISlackFileFetcher>();
            fetcher
                .GetJsonIfAny(Arg.Is<string>("protacon"), Arg.Is<string>("testrepo"))
                .Returns(Option.Some(new SlackJsonFileModel(new [] { "pull_request"}, new []{ "#general"})));

            var slack = Substitute.For<ISlackMessage>();

            var requestAction = new PullRequestAction(fetcher, slack, Substitute.For<ILogger<PullRequestAction>>());

            requestAction.Execute(TestPayloads.PullRequestOpened());

            slack.Received(1).Send(Arg.Is<string>("#general"), Arg.Any<string>());
        }

        [Fact]
        public void WhenSlackJsonDoesntContainPullRequestAction_ThenIgnoreSend()
        {
            var fetcher = Substitute.For<ISlackFileFetcher>();
            fetcher
                .GetJsonIfAny(Arg.Is<string>("protacon"), Arg.Is<string>("testrepo"))
                .Returns(Option.Some(new SlackJsonFileModel(new string[] {}, new []{ "#general"})));

            var slack = Substitute.For<ISlackMessage>();

            var requestAction = new PullRequestAction(fetcher, slack, Substitute.For<ILogger<PullRequestAction>>());

            requestAction.Execute(TestPayloads.PullRequestOpened());

            slack.DidNotReceive().Send(Arg.Any<string>(), Arg.Any<string>());
        }
    }
}
