using Microsoft.Extensions.Logging;
using NSubstitute;
using Slack.Json.Actions;
using Slack.Json.Slack;
using Slack.Json.Tests.GithubRequestPayloads;
using Xunit;

namespace Slack.Json.Tests
{
    public class NewUssueActionTests
    {
        [Fact]
        public void WhenNewIssueIsPosted_ThenSendMessageToRecipients()
        {
            var fetcher = DepencyMockFactories.SlackFileFetcherMock("new_issue", "#general");

            var slack = Substitute.For<ISlackMessaging>();

            var requestAction = new NewIssueAction(fetcher, slack, Substitute.For<ILogger<NewPublicRepoAction>>());

            requestAction.Execute(TestPayloads.NewIssue());

            slack.Received(1).Send(Arg.Is<string>("#general"), Arg.Any<SlackMessageModel>());
        }
    }
}