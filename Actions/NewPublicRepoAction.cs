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
    public class NewPublicRepoAction : IRequestAction
    {
        private ISlackMessaging slack;
        private ILogger<NewPublicRepoAction> logger;

        public NewPublicRepoAction(ISlackMessaging slack, ILogger<NewPublicRepoAction> logger)
        {
            this.slack = slack;
            this.logger = logger;
        }

        public string RequestType => "repository";
        public string RequestAction => "created";
        public string Type => "new_public_repository";

        public void Execute(JObject request, IEnumerable<ISlackAction> actions)
        {
            var fullName = request.Get(x => x.repository.full_name);
            var repoHtmlUrl = request.Get(x => x.repository.html_url);

            actions
                .ToList()
                .ForEach(action =>
                {
                    this.logger.LogInformation($"Sending message to '{action.Channel}'");
                    this.slack.Send(action.Channel,
                        new SlackMessageModel($"New public repository {fullName}", repoHtmlUrl)
                        {
                            Text = ":thumbsup: if looks good, :thumbsdown: if not.",
                            Color = "warning"
                        });
                });
        }
    }
}