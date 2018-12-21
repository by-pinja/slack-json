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
    public class NewRepoAction : IRequestAction
    {
        private readonly ISlackMessaging slack;
        private readonly ILogger<NewRepoAction> logger;

        public NewRepoAction(ISlackMessaging slack, ILogger<NewRepoAction> logger)
        {
            this.slack = slack;
            this.logger = logger;
        }

        public string GithubHookEventName => "repository";
        public string SlackJsonType => "new_repository";

        public void Execute(JObject request, IEnumerable<ISlackAction> actions)
        {
            if(request.Get<string>(x => x.action) != "created")
                return;

            var fullName = request.Get(x => x.repository.full_name);
            var repoHtmlUrl = request.Get(x => x.repository.html_url);

            actions
                .ToList()
                .ForEach(action =>
                {
                    this.logger.LogInformation($"Sending message to '{action.Channel}'");
                    this.slack.Send(action.Channel,
                        new SlackMessageModel($"New repository {fullName}", repoHtmlUrl)
                        {
                            Text = ":thumbsup: looks good or :thumbsdown:",
                            Color = "warning",
                        });
                });
        }
    }
}