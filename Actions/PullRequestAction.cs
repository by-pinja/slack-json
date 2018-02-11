using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Slack.Json.Github;
using Slack.Json.Slack;

namespace Slack.Json.Actions
{
    public class PullRequestAction : IRequestAction
    {
        private readonly ISlackMessaging slack;
        private readonly ILogger<PullRequestAction> logger;

        public PullRequestAction(ISlackMessaging slack, ILogger<PullRequestAction> logger)
        {
            this.slack = slack;
            this.logger = logger;
        }

        public string RequestType => "pull_request";
        public string RequestAction => "opened";
        public string Type => "pull_request";

        public void Execute(JObject request, IEnumerable<ISlackAction> actions)
        {
            ActionUtils.ParsePullRequestDefaultFields(request, out var prHtmlUrl, out var prTitle);

            actions
                .ToList()
                .ForEach(action =>
                {
                    this.logger.LogInformation($"Sending message to '{action.Channel}'");
                    this.slack.Send(action.Channel,
                        new SlackMessageModel($"New pull request '{prTitle}'", prHtmlUrl)
                        {
                            Color = "warning"
                        });
                });
        }
    }
}