using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Slack.Json.Github;
using Slack.Json.Slack;
using Slack.Json.Util;

namespace Slack.Json.Actions
{
    public class ReviewStatusAction : IRequestAction
    {
        private readonly ISlackActionFetcher fetcher;
        private readonly ISlackMessaging slack;
        private readonly ILogger<PullRequestAction> logger;
        private readonly string type = "review_status";

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

            var slackFile = this.fetcher.GetJsonIfAny(owner, repo)
                .Where(slackJsonAction => slackJsonAction.Type == this.type)
                .ToList();

            if (!slackFile.Any())
                return;

            var reviewState = request.Get(x => x.review.state);
            var reviewer = request.Get(x => x.review.user.login);
            var reviewBody = request.Get(x => x.review.body);

            var stateVariable = GetStateVariable(reviewState);

            slackFile
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