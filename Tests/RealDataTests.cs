using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
                .AssertInvokedOn(requestType: "issues")
                .AssertSlackJsonTypeIs("new_issue")
                .Assert(slack =>
                    slack.Received(1).Send(Arg.Is<string>("#general"), Arg.Any<SlackMessageModel>()));
        }

        [Fact]
        public void WhenNewPublicRepoIsCreated_ThenSendMessage()
        {
            ActionTestBuilder<NewRepoAction>
                .Create((slack, logger) => new NewRepoAction(slack, logger))
                .ExecuteWith("createPublicRepo.json", slackChannels: "#general")
                .AssertSlackJsonTypeIs("new_repository")
                .AssertInvokedOn(requestType: "repository")
                .Assert(slack =>
                    slack.Received(1).Send(Arg.Is<string>("#general"), Arg.Is<SlackMessageModel>(x => x.Title.Contains("public"))));
        }

        [Fact]
        public void WhenNewReleaseIsCreated_ThenSendMessage()
        {
            ActionTestBuilder<NewReleaseAction>
                .Create((slack, logger) => new NewReleaseAction(slack, logger))
                .ExecuteWith("newRelease.json", slackChannels: "#general")
                .AssertSlackJsonTypeIs("new_release")
                .AssertInvokedOn(requestType: "release")
                .Assert(slack =>
                    slack.Received(1).Send(Arg.Is<string>("#general"), Arg.Any<SlackMessageModel>()));
        }

        [Fact]
        public void WhenPullRequestIsCreated_ThenSendMessage()
        {
            ActionTestBuilder<PullRequestOpenedAction>
                .Create((slack, logger) => new PullRequestOpenedAction(slack, logger))
                .ExecuteWith("pullRequest.json", slackChannels: "#general")
                .AssertSlackJsonTypeIs("pull_request")
                .AssertInvokedOn(requestType: "pull_request")
                .Assert(slack =>
                    slack.Received(1).Send(Arg.Is<string>("#general"), Arg.Any<SlackMessageModel>()));
        }

        [Fact]
        public void WhenPullRequestIsReadyForReview_ThenSendMessage()
        {
            ActionTestBuilder<PullRequestForReviewAction>
                .Create((slack, logger) => new PullRequestForReviewAction(slack, logger))
                .ExecuteWith("pullRequestForReview.json", slackChannels: "#general")
                .AssertSlackJsonTypeIs("ready_for_review")
                .AssertInvokedOn(requestType: "pull_request")
                .Assert(slack =>
                    slack.Received(1).Send(Arg.Is<string>("#general"), Arg.Any<SlackMessageModel>()));
        }

        [Fact]
        public void WhenReviewRequestIsSent_ThenSendMessage()
        {
            ActionTestBuilder<ReviewRequestAction>
                .Create((slack, logger) => new ReviewRequestAction(slack, logger))
                .ExecuteWith("reviewRequest.json", slackChannels: "#general")
                .AssertInvokedOn(requestType: "pull_request")
                .AssertSlackJsonTypeIs("review_request")
                .Assert(slack =>
                    slack.Received(1).Send(Arg.Is("#general"), Arg.Any<SlackMessageModel>()));
        }

        [Fact]
        public void WhenReviewIsRequestedFromTeam_ThenSendMessageWithTeamInformation()
        {
            ActionTestBuilder<ReviewRequestAction>
                .Create((slack, logger) => new ReviewRequestAction(slack, logger))
                .ExecuteWith("review_request_from_team.json", slackChannels: "#general")
                .AssertInvokedOn(requestType: "pull_request")
                .AssertSlackJsonTypeIs("review_request")
                .Assert(slack =>
                    slack.Received(1).Send(Arg.Is("#general"), Arg.Is<SlackMessageModel>(x => x.Text.Contains("test-team"))));
        }

        [Fact]
        public void WhenReviewStatusIsUpdated_ThenSendMessage()
        {
            ActionTestBuilder<ReviewStatusAction>
                .Create((slack, logger) => new ReviewStatusAction(slack, logger))
                .ExecuteWith("reviewSubmit.json", slackChannels: "#general")
                .AssertInvokedOn(requestType: "pull_request_review")
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
                .AssertInvokedOn(requestType: "issues")
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
                .AssertInvokedOn(requestType: "pull_request")
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

        [Fact]
        public void WhenJenkinsBuildFails_ThenSendMessage()
        {
            ActionTestBuilder<JenkinsBuildFailAction>
                .Create((slack, logger) => new JenkinsBuildFailAction(slack, logger))
                .ExecuteWith("jenkinsBuildFailure.json", slackChannels: "#general")
                .AssertInvokedOn(requestType: "status")
                .AssertSlackJsonTypeIs("jenkins_build_error")
                .Assert(slack =>
                    slack.Received(1).Send(Arg.Is("#general"), Arg.Any<SlackMessageModel>()));
        }

        [Fact]
        public void WhenSecurityVulnerabilityIsFound_ThenSendMessage()
        {
            ActionTestBuilder<VulnerabilityAlertAction>
                .Create((slack, logger) => new VulnerabilityAlertAction(slack, logger))
                .ExecuteWith("vulnerabilityAlert.create.json", slackChannels: "#general")
                .AssertInvokedOn(requestType: "repository_vulnerability_alert")
                .AssertSlackJsonTypeIs("repository_vulnerability_alert")
                .Assert(slack =>
                    slack.Received(1).Send(Arg.Is("#general"), Arg.Any<SlackMessageModel>()));
        }

        [Fact]
        public void WhenVulnerabilityAlertIsDismissed_ThenSendMessage()
        {
            ActionTestBuilder<VulnerabilityAlertAction>
                .Create((slack, logger) => new VulnerabilityAlertAction(slack, logger))
                .ExecuteWith("vulnerabilityAlert.dismiss.json", slackChannels: "#general")
                .AssertInvokedOn(requestType: "repository_vulnerability_alert")
                .AssertSlackJsonTypeIs("repository_vulnerability_alert")
                .Assert(slack =>
                    slack.Received(1).Send(Arg.Is("#general"), Arg.Any<SlackMessageModel>()));
        }

        [Fact]
        public void WhenVulnerabilityAlertIsResolved_ThenSendMessage()
        {
            ActionTestBuilder<VulnerabilityAlertAction>
                .Create((slack, logger) => new VulnerabilityAlertAction(slack, logger))
                .ExecuteWith("vulnerabilityAlert.resolve.json", slackChannels: "#general")
                .AssertInvokedOn(requestType: "repository_vulnerability_alert")
                .AssertSlackJsonTypeIs("repository_vulnerability_alert")
                .Assert(slack =>
                    slack.Received(1).Send(Arg.Is<string>("#general"), Arg.Any<SlackMessageModel>()));
        }

        [Fact]
        public void WhenPullRequestIsMerged_ThenSendMessageAboutMerging()
        {
            ActionTestBuilder<PullRequestClosedAction>
                .Create((slack, logger) => new PullRequestClosedAction(slack, logger))
                .ExecuteWith("pullRequestMerged.json", slackChannels: "#general")
                .AssertInvokedOn(requestType: "pull_request")
                .AssertSlackJsonTypeIs("pull_request")
                .Assert(slack =>
                    slack.Received(1).Send(Arg.Is("#general"), Arg.Is<SlackMessageModel>(x => x.Color == "#6F42C1" && x.Title.Contains("merge"))));
        }

        [Fact]
        public void WhenPullRequestIsClosedButNotMerged_ThenSendMessageAboutAction()
        {
            ActionTestBuilder<PullRequestClosedAction>
                .Create((slack, logger) => new PullRequestClosedAction(slack, logger))
                .ExecuteWith("pullRequestNotMerged.json", slackChannels: "#general")
                .AssertInvokedOn(requestType: "pull_request")
                .AssertSlackJsonTypeIs("pull_request")
                .Assert(slack =>
                    slack.Received(1).Send(Arg.Is("#general"), Arg.Is<SlackMessageModel>(x => x.Color == "danger" && x.Title.Contains("merge"))));
        }
    }
}