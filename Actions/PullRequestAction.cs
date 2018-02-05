using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Slack.Integration.Github;
using Slack.Integration.Slack;

namespace Slack.Integration.Actions
{
    public class PullRequestAction : IRequestAction
    {
        private readonly ISlackFileFetcher fetcher;
        private readonly ISlackMessaging slack;
        private readonly ILogger<PullRequestAction> logger;

        public PullRequestAction(ISlackFileFetcher fetcher, ISlackMessaging slack, ILogger<PullRequestAction> logger)
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

            var slackFile = this.fetcher.GetJsonIfAny(owner, repo);

            if (!slackFile.Any())
            {
                this.logger.LogInformation($"Checked pull request for '{owner}/{repo} but no slack.json file defined.'");
                return;
            }

            slackFile
                .Where(x => x.Type == "pull_request")
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