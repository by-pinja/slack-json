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
            ActionTestBuilder<NewLabelPullRequestAction>
                .Create((slack, logger) => new NewLabelPullRequestAction(slack, logger))
                .ExecuteWith("pullRequestLabeled.json", slackChannels: "#general")
                .AssertInvokedOn(requestType: "pull_request", requestAction: "labeled")
                .AssertSlackJsonTypeIs("pullrequest_label")
                .Passing(slack =>
                    slack.Received(1).Send(Arg.Is<string>("#general"), Arg.Any<SlackMessageModel>()));
        }

        [Fact]
        public void WhenSlackJsonDefinesLabelsExplicitly_ThenCheckThem()
        {
            ActionTestBuilder<NewLabelPullRequestAction>
                .Create((slack, logger) => new NewLabelPullRequestAction(slack, logger))
                .ExecuteWith("pullRequestLabeled.json", new SlackActionModel
                    {
                        Type = "pullrequest_label",
                        Enabled = true,
                        Channel = "#general",
                        Data = new JArray("this_doesnt_match")
                    })
                .AssertInvokedOn(requestType: "pull_request", requestAction: "labeled")
                .AssertSlackJsonTypeIs("pullrequest_label")
                .Passing(slack =>
                    slack.DidNotReceive().Send(Arg.Any<string>(), Arg.Any<SlackMessageModel>()));
        }
    }
}