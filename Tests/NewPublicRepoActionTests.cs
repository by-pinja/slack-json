using Microsoft.Extensions.Logging;
using NSubstitute;
using Slack.Json.Actions;
using Slack.Json.Slack;
using Slack.Json.Tests.GithubRequestPayloads;
using Xunit;

namespace Slack.Json.Tests
{
    public class NewPublicRepoActionTests
    {
        [Fact]
        public void WhenRealJsonIsUsed_ThenSendMessageToDefinedChannels()
        {
            var fetcher = DepencyMockFactories.SlackFileFetcherMock("new_public_repository", "#general");

            var slack = Substitute.For<ISlackMessaging>();

            var requestAction = new NewPublicRepoAction(fetcher, slack, Substitute.For<ILogger<NewPublicRepoAction>>());

            requestAction.Execute(TestPayloads.NewPublicRepository());

            slack.Received(1).Send(Arg.Is<string>("#general"), Arg.Any<SlackMessageModel>());
        }
    }
}