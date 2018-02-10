using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Slack.Json.Github;
using Slack.Json.Slack;
using Slack.Json.Util;

namespace Slack.Json.Actions
{
    public class NewPublicRepoAction : IRequestAction
    {
        private ISlackActionFetcher fetcher;
        private ISlackMessaging slack;
        private ILogger<NewPublicRepoAction> logger;
        private readonly string type = "new_public_repository";

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
            var owner = request.Get(x => x.repository.owner.login);
            var repo = request.Get(x => x.repository.name);
            var repoHtmlUrl = request.Get(x => x.repository.html_url);

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
                        new SlackMessageModel($"New public repository '{owner}/{repo}'", repoHtmlUrl)
                        {
                            Text = ":thumbsup: if looks good, :thumbsdown: if not.",
                            Color = "warning"
                        });
                });
        }
    }
}