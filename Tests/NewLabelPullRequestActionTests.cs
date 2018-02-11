using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Slack.Json.Actions;
using Slack.Json.Github;
using Slack.Json.Slack;
using Slack.Json.Tests.GithubRequestPayloads;
using Xunit;

namespace Slack.Json.Tests
{
    public class NewLabelPullRequestActionTests
    {
        [Fact]
        public void WhenRealWorldJsonIsReceived_ThenParseItCorrectly()
        {
            var fetcher = DepencyMockFactories.SlackFileFetcherMock("pullrequest_label", "#general");

            var slack = Substitute.For<ISlackMessaging>();

            var requestAction = new NewLabelPullRequestAction(fetcher, slack, Substitute.For<ILogger<NewPublicRepoAction>>());

            requestAction.Execute(TestPayloads.NewLabelOnPullRequest());

            slack.Received(1).Send(Arg.Is<string>("#general"), Arg.Any<SlackMessageModel>());
        }

        [Fact]
        public void WhenSlackJsonDefinesLabelsExplicitly_ThenCheckThem()
        {
            var fetcher = Substitute.For<ISlackActionFetcher>();

            fetcher
                .GetJsonIfAny(Arg.Is<string>("protacon"), Arg.Is<string>("testrepo"))
                .Returns(new List<SlackActionModel>
                {
                    new SlackActionModel
                    {
                        Type = "pullrequest_label",
                        Enabled = true,
                        Channel = "#general",
                        Data = new JArray("this_doesnt_match")
                    }
                });

            var slack = Substitute.For<ISlackMessaging>();

            var requestAction = new NewLabelPullRequestAction(fetcher, slack, Substitute.For<ILogger<NewPublicRepoAction>>());

            requestAction.Execute(TestPayloads.NewLabelOnPullRequest());

            slack.DidNotReceive().Send(Arg.Any<string>(), Arg.Any<SlackMessageModel>());
        }
    }
}