using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Slack.Json.Github;
using Slack.Json.Slack;

namespace Slack.Json.Actions
{
    public class NewPublicRepoAction : IRequestAction
    {
        private ISlackActionFetcher fetcher;
        private ISlackMessaging slack;
        private ILogger<NewPublicRepoAction> logger;

        public NewPublicRepoAction(ISlackActionFetcher fetcher, ISlackMessaging slack, ILogger<NewPublicRepoAction> logger)
        {
            this.fetcher = fetcher;
            this.slack = slack;
            this.logger = logger;
        }

        public string RequestType => "repository";

        public string RequestAction => "created";

        public void Execute(JObject request)
        {
            var owner = request["repository"]?["owner"]?["login"]?.Value<string>()
                ?? throw new InvalidOperationException("Cannot find repository.owner from request.");

            var repo = request["repository"]?["name"]?.Value<string>()
                ?? throw new InvalidOperationException("Cannot find repository.owner from request.");

            var slackFile = this.fetcher.GetJsonIfAny(owner, repo);

            if (!slackFile.Any())
            {
                this.logger.LogInformation($"Checked pull request for '{owner}/{repo} but no slack.json file defined.'");
                return;
            }

            var repoHtmlUrl = request["repository"]?["html_url"]?.Value<string>()
                ?? throw new InvalidOperationException("Cannot find repository.html_url from request.");

            slackFile
                .Where(x => x.Type == "new_public_repository")
                .ToList()
                .ForEach(action =>
                {
                    this.logger.LogInformation($"Sending message to '{action.Channel}'");
                    this.slack.Send(action.Channel,
                        new SlackMessageModel($"New public repository '{owner}/{repo}'", repoHtmlUrl)
                        {
                            Color = "warning"
                        });
                });
        }
    }
}