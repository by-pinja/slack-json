using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Slack.Json.Actions;
using Slack.Json.Github;
using Slack.Json.Slack;
using Xunit;

namespace Slack.Json.Tests
{
    public class RealDataTests
    {
        [Fact]
        public void WhenNewIssueIsPosted_ThenSendMessage()
        {
            ActionTestBuilder<NewIssueAction>
                .Create((slack, logger) => new NewIssueAction(slack, logger))
                .ExecuteWith("newIssue.json", slackChannels: "#general")
                .AssertInvokedOn(requestType: "issues", requestAction: "opened")
                .AssertSlackJsonTypeIs("new_issue")
                .Assert(slack =>
                    slack.Received(1).Send(Arg.Is<string>("#general"), Arg.Any<SlackMessageModel>()));
        }

        [Fact]
        public void WhenNewPublicRepoIsCreated_ThenSendMessage()
        {
            ActionTestBuilder<NewPublicRepoAction>
                .Create((slack, logger) => new NewPublicRepoAction(slack, logger))
                .ExecuteWith("createPublicRepo.json", slackChannels: "#general")
                .AssertSlackJsonTypeIs("new_public_repository")
                .AssertInvokedOn(requestType: "repository", requestAction: "created")
                .Assert(slack =>
                    slack.Received(1).Send(Arg.Is<string>("#general"), Arg.Any<SlackMessageModel>()));
        }

        [Fact]
        public void WhenPullRequestIsCreated_ThenSendMessage()
        {
            ActionTestBuilder<PullRequestAction>
                .Create((slack, logger) => new PullRequestAction(slack, logger))
                .ExecuteWith("pullRequest.json", slackChannels: "#general")
                .AssertSlackJsonTypeIs("pull_request")
                .AssertInvokedOn(requestType: "pull_request", requestAction: "opened")
                .Assert(slack =>
                    slack.Received(1).Send(Arg.Is<string>("#general"), Arg.Any<SlackMessageModel>()));
        }

        [Fact]
        public void WhenReviewRequestIsSent_ThenSendMessage()
        {
            ActionTestBuilder<ReviewRequestAction>
                .Create((slack, logger) => new ReviewRequestAction(slack, logger))
                .ExecuteWith("reviewRequest.json", slackChannels: "#general")
                .AssertInvokedOn(requestType: "pull_request", requestAction: "review_requested")
                .AssertSlackJsonTypeIs("review_request")
                .Assert(slack =>
                    slack.Received(1).Send(Arg.Is<string>("#general"), Arg.Any<SlackMessageModel>()));
        }

        [Fact]
        public void WhenReviewStatusIsUpdated_ThenSendMessage()
        {
            ActionTestBuilder<ReviewStatusAction>
                .Create((slack, logger) => new ReviewStatusAction(slack, logger))
                .ExecuteWith("reviewSubmit.json", slackChannels: "#general")
                .AssertInvokedOn(requestType: "pull_request_review", requestAction: "submitted")
                .AssertSlackJsonTypeIs("review_status")
                .Assert(slack =>
                    slack.Received(1).Send(Arg.Is<string>("#general"), Arg.Any<SlackMessageModel>()));
        }

        [Fact]
        public void WhenNewLabelIsAddedToIssue_ThenSendMessage()
        {
            ActionTestBuilder<NewLabelOnIssueAction>
                .Create((slack, logger) => new NewLabelOnIssueAction(slack, logger))
                .ExecuteWith("newLabelOnIssue.json", slackChannels: "#general")
                .AssertInvokedOn(requestType: "issues", requestAction: "labeled")
                .AssertSlackJsonTypeIs("issue_label")
                .Assert(slack =>
                    slack.Received(1).Send(Arg.Is<string>("#general"), Arg.Any<SlackMessageModel>()));
        }

        [Fact]
        public void WhenNewLabelIsAddedToIssueButDataFiltersLabelNames_ThenDontSendMessage()
        {
            ActionTestBuilder<NewLabelOnIssueAction>
                .Create((slack, logger) => new NewLabelOnIssueAction(slack, logger))
                .ExecuteWith("newLabelOnIssue.json", new SlackActionModel
                    {
                        Type = "issue_label",
                        Enabled = true,
                        Channel = "#general",
                        Data = new JArray("this_doesnt_match")
                    })
                .Assert(slack =>
                    slack.DidNotReceive().Send(Arg.Any<string>(), Arg.Any<SlackMessageModel>()));
        }

        [Fact]
        public void WhenNewLabelIsAddedToPullRequest_ThenSendMessage()
        {
            ActionTestBuilder<NewLabelPullRequestAction>
                .Create((slack, logger) => new NewLabelPullRequestAction(slack, logger))
                .ExecuteWith("pullRequestLabeled.json", slackChannels: "#general")
                .AssertInvokedOn(requestType: "pull_request", requestAction: "labeled")
                .AssertSlackJsonTypeIs("pullrequest_label")
                .Assert(slack =>
                    slack.Received(1).Send(Arg.Is<string>("#general"), Arg.Any<SlackMessageModel>()));
        }

        [Fact]
        public void WhenNewLabelIsAddedToPullRequestButLabelNameIsFiltered_ThenDontSendMessage()
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
                .Assert(slack =>
                    slack.DidNotReceive().Send(Arg.Any<string>(), Arg.Any<SlackMessageModel>()));
        }
    }
}