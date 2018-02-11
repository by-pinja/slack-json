using Microsoft.Extensions.Logging;
using NSubstitute;
using Slack.Json.Actions;
using Slack.Json.Slack;
using Slack.Json.Tests.GithubRequestPayloads;
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
                .AssertInvokedOn(requestType: "issues", requestAction: "opened")
                .ExecuteWith("newIssue.json", slackChannels: "#general")
                .Passing(slack =>
                    slack.Received(1).Send(Arg.Is<string>("#general"), Arg.Any<SlackMessageModel>()));
        }

        [Fact]
        public void WhenNewPublicRepoIsCreated_ThenSendMessage()
        {
            ActionTestBuilder<NewPublicRepoAction>
                .Create((slack, logger) => new NewPublicRepoAction(slack, logger))
                .AssertInvokedOn(requestType: "repository", requestAction: "created")
                .ExecuteWith("createPublicRepo.json", slackChannels: "#general")
                .Passing(slack =>
                    slack.Received(1).Send(Arg.Is<string>("#general"), Arg.Any<SlackMessageModel>()));
        }

        [Fact]
        public void WhenPullRequestIsCreated_ThenSendMessage()
        {
            ActionTestBuilder<PullRequestAction>
                .Create((slack, logger) => new PullRequestAction(slack, logger))
                .AssertInvokedOn(requestType: "pull_request", requestAction: "opened")
                .ExecuteWith("pullRequest.json", slackChannels: "#general")
                .Passing(slack =>
                    slack.Received(1).Send(Arg.Is<string>("#general"), Arg.Any<SlackMessageModel>()));
        }

        [Fact]
        public void WhenReviewRequestIsSent_ThenSendMessage()
        {
            ActionTestBuilder<ReviewRequestAction>
                .Create((slack, logger) => new ReviewRequestAction(slack, logger))
                .AssertInvokedOn(requestType: "pull_request", requestAction: "review_requested")
                .ExecuteWith("reviewRequest.json", slackChannels: "#general")
                .Passing(slack =>
                    slack.Received(1).Send(Arg.Is<string>("#general"), Arg.Any<SlackMessageModel>()));
        }

        [Fact]
        public void WhenReviewStatusIsUpdated_ThenSendMessage()
        {
            ActionTestBuilder<ReviewStatusAction>
                .Create((slack, logger) => new ReviewStatusAction(slack, logger))
                .AssertInvokedOn(requestType: "pull_request_review", requestAction: "submitted")
                .ExecuteWith("reviewSubmit.json", slackChannels: "#general")
                .Passing(slack =>
                    slack.Received(1).Send(Arg.Is<string>("#general"), Arg.Any<SlackMessageModel>()));
        }
    }
}