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
    public class PullRequestClosedAction : IRequestAction
    {
        public string GithubHookEventName => "pull_request";
        public string SlackJsonType => "pull_request";
        private readonly ISlackMessaging slack;
        private readonly ILogger<PullRequestClosedAction> logger;

        public PullRequestClosedAction(ISlackMessaging slack, ILogger<PullRequestClosedAction> logger)
        {
            this.slack = slack;
            this.logger = logger;
        }

        public void Execute(JObject request, IEnumerable<ISlackAction> actions)
        {
            if(request.Get<string>(x => x.action) != "closed")
                return;

            ActionUtils.ParsePullRequestDefaultFields(request, out var prHtmlUrl, out var prTittle);

            var isMerged = !string.IsNullOrEmpty(request.Get(x => x.pull_request.merged_at));

            const string gray = "#6F42C1";
            var color = isMerged ?  gray : "danger";

            var tittle = isMerged ? $"'{prTittle}' merged to '{request.Get(x => x.pull_request.@base.@ref)}'" : $"Pull request '{prTittle}' closed.";

            actions
                .ToList()
                .ForEach(action =>
                {
                    this.logger.LogInformation($"Sending message to '{action.Channel}'");
                    this.slack.Send(action.Channel,
                        new SlackMessageModel(tittle, prHtmlUrl)
                        {
                            Color = color
                        });
                });
        }
    }
}