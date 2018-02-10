using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Slack.Json.Github;
using Slack.Json.Slack;

namespace Slack.Json.Actions
{
    public class ReviewStatusAction : IRequestAction
    {
        private readonly ISlackActionFetcher fetcher;
        private readonly ISlackMessaging slack;
        private readonly ILogger<PullRequestAction> logger;

        public ReviewStatusAction(ISlackActionFetcher fetcher, ISlackMessaging slack, ILogger<PullRequestAction> logger)
        {
            this.fetcher = fetcher;
            this.slack = slack;
            this.logger = logger;
        }

        public string RequestType => "pull_request_review";

        public string RequestAction => "submitted";

        public void Execute(JObject request)
        {
            ActionUtils.ParsePullRequestDefaultFields(request, out var repo, out var owner, out var prHtmlUrl, out var prTitle);

            var slackFile = this.fetcher
                .GetJsonIfAny(owner, repo)
                .Where(x => x.Type == "review_status");

            if (!slackFile.Any())
            {
                this.logger.LogInformation($"Checked pull request for '{owner}/{repo} but no slack.json file defined or it doesn't contain any 'review_status' actions.");
                return;
            }

            var reviewState = request["review"]?["state"]?.Value<string>()
                ?? throw new InvalidOperationException($"Cannot get review.state from request.");

            var reviewer = request["review"]?["user"]?["login"]?.Value<string>()
                ?? throw new InvalidOperationException($"Cannot get review.user.login from request.");

            var reviewBody = request["review"]?["body"]?.Value<string>()
                ?? throw new InvalidOperationException($"Cannot get review.body from request.");

            var stateVariable = GetStateVariable(reviewState);

            slackFile
                .ToList()
                .ForEach(action =>
                {
                    this.slack.Send(action.Channel, new SlackMessageModel($"Reviewer '{reviewer}' {stateVariable.verb} changes for '{prTitle}'", prHtmlUrl)
                    {
                        Color = stateVariable.color,
                        Text = reviewBody
                    });
                });
        }

        private (string color, string verb) GetStateVariable(string state)
        {
            switch (state)
            {
                case "changes_requested":
                    return ("danger", "requested");
                case "commented":
                    return ("#439FE0", "commented");
                case "approved":
                    return ("good", "approved");
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}