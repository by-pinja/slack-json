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
    public class PullRequestOpenedAction : IRequestAction
    {
        public string GithubHookEventName => "pull_request";
        public string SlackJsonType => "pull_request";
        private readonly ISlackMessaging slack;
        private readonly ILogger<PullRequestOpenedAction> logger;

        public PullRequestOpenedAction(ISlackMessaging slack, ILogger<PullRequestOpenedAction> logger)
        {
            this.slack = slack;
            this.logger = logger;
        }

        public void Execute(JObject request, IEnumerable<ISlackAction> actions)
        {
            if(request.Get(x => x.action) != "opened")
                return;

            ActionUtils.ParsePullRequestDefaultFields(request, out var prHtmlUrl, out var prTittle);

            var draft = request.Get(x => x.pull_request.draft);
            var draftText = draft == "True" ? "draft " : "";
            var color = draft == "True" ? "#7a7a7a" : "warning";

            actions
                .ToList()
                .ForEach(action =>
                {
                    this.logger.LogInformation($"Sending message to '{action.Channel}'");
                    this.slack.Send(action.Channel,
                        new SlackMessageModel($"New {draftText}pull request '{prTittle}'", prHtmlUrl)
                        {
                            Color = color
                        });
                });
        }
    }
}