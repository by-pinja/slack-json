using System;
using System.Collections.Generic;
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
        private ISlackMessaging slack;
        private ILogger<ReviewRequestAction> logger;

        public ReviewRequestAction(ISlackMessaging slack, ILogger<ReviewRequestAction> logger)
        {
            this.slack = slack;
            this.logger = logger;
        }

        public string RequestType => "pull_request";
        public string RequestAction => "review_requested";
        public string Type => "review_request";

        public void Execute(JObject request, IEnumerable<ISlackAction> actions)
        {
            ActionUtils.ParsePullRequestDefaultFields(request, out var prHtmlUrl, out var prTitle);

            var reviewers = request.Get<JArray>(x => x.pull_request.requested_reviewers)
                    .Select(x => x["login"] ?? throw new InvalidOperationException($"Missing Missing pull_request.requested_reviewers.login"));

            actions
                .ToList()
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