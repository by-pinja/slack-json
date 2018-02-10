using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Slack.Json.Github;
using Slack.Json.Slack;
using Slack.Json.Util;

namespace Slack.Json.Actions
{
    public class ReviewRequestAction: IRequestAction
    {
        private ISlackActionFetcher fetcher;
        private ISlackMessaging slack;
        private ILogger<PullRequestAction> logger;
        private readonly string type = "review_request";

        public ReviewRequestAction(ISlackActionFetcher fetcher, ISlackMessaging slack, ILogger<PullRequestAction> logger)
        {
            this.fetcher = fetcher;
            this.slack = slack;
            this.logger = logger;
        }

        public string RequestType => "pull_request";
        public string RequestAction => "review_requested";

        public void Execute(JObject request)
        {
            ActionUtils.ParsePullRequestDefaultFields(request, out var repo, out var owner, out var prHtmlUrl, out var prTitle);

            var slackFile = this.fetcher.GetJsonIfAny(owner, repo)
                .Where(slackJsonAction => slackJsonAction.Type == this.type)
                .ToList();

            if (!slackFile.Any())
                return;

            var reviewers = request.Get<JArray>(x => x.pull_request.requested_reviewers)
                    .Select(x => x["login"] ?? throw new InvalidOperationException($"Missing Missing pull_request.requested_reviewers.login"));

            slackFile
                .ForEach(action =>
                {
                    this.logger.LogInformation($"Sending message to '{action.Channel}'");
                    this.slack.Send(action.Channel,
                        new SlackMessageModel($"Review request for pull request '{prTitle}'", prHtmlUrl)
                        {
                            Text = $"Review is requested from {string.Join(", ", reviewers)}"
                        });
                });
        }
    }
}