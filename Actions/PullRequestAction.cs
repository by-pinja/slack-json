using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Slack.Json.Github;
using Slack.Json.Slack;

namespace Slack.Json.Actions
{
    public class PullRequestAction : IRequestAction
    {
        private readonly ISlackActionFetcher fetcher;
        private readonly ISlackMessaging slack;
        private readonly ILogger<PullRequestAction> logger;
        private readonly string type = "pull_request";

        public PullRequestAction(ISlackActionFetcher fetcher, ISlackMessaging slack, ILogger<PullRequestAction> logger)
        {
            this.fetcher = fetcher;
            this.slack = slack;
            this.logger = logger;
        }

        public string RequestType => "pull_request";
        public string RequestAction => "opened";

        public void Execute(JObject request)
        {
            ActionUtils.ParsePullRequestDefaultFields(request, out var repo, out var owner, out var prHtmlUrl, out var prTitle);

            var slackFile = this.fetcher.GetJsonIfAny(owner, repo)
                .Where(slackJsonAction => slackJsonAction.Type == this.type)
                .ToList();

            if (!slackFile.Any())
                return;

            slackFile
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