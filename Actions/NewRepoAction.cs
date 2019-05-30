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
        public string GithubHookEventName => "repository";
        public string SlackJsonType => "new_repository";
        private readonly ISlackMessaging slack;
        private readonly ILogger<NewRepoAction> logger;

        public NewRepoAction(ISlackMessaging slack, ILogger<NewRepoAction> logger)
        {
            this.slack = slack;
            this.logger = logger;
        }

        public void Execute(JObject request, IEnumerable<ISlackAction> actions)
        {
            if(request.Get<string>(x => x.action) != "created")
                return;

            var fullName = request.Require(x => x.repository.full_name);
            var repoHtmlUrl = request.Require(x => x.repository.html_url);

            // If you use word 'private' in dynamic expression compiler breaks up :)
            var isIsPrivate = request.Get<JObject>(x => x.repository)?["private"]?.Value<bool>()
                ?? throw new InvalidOperationException("Could not find expected path 'repository' from json.");

            var publicOrPrivateText = isIsPrivate ? "private" : "public";

            actions
                .ToList()
                .ForEach(action =>
                {
                    this.logger.LogInformation($"Sending message to '{action.Channel}'");
                    this.slack.Send(action.Channel,
                        new SlackMessageModel($"New {publicOrPrivateText} repository {fullName}", repoHtmlUrl)
                        {
                            Text = ":thumbsup: looks good or :thumbsdown:",
                            Color = "warning",
                        });
                });
        }
    }
}