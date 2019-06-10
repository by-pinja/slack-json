using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Slack.Json.Github;
using Slack.Json.Slack;
using Slack.Json.Util;

namespace Slack.Json.Actions
{
    public class NewLabelPullRequestAction : IRequestAction
    {
        public string GithubHookEventName => "pull_request";
        public string SlackJsonType => "pullrequest_label";

        private readonly ISlackMessaging slack;
        private readonly ILogger<NewLabelPullRequestAction> logger;

        public NewLabelPullRequestAction(ISlackMessaging slack, ILogger<NewLabelPullRequestAction> logger)
        {
            this.slack = slack;
            this.logger = logger;
        }

        public void Execute(JObject request, IEnumerable<ISlackAction> actions)
        {
            if(request.Get<string>(x => x.action) != "labeled")
                return;

                ActionUtils.ParsePullRequestDefaultFields(request, out var prHtmlUrl, out var prTittle);
            var label = request.Get(x => x.label.name);

            actions
                .Where(slackJsonAction =>
                    slackJsonAction.Data == null ||
                    slackJsonAction.Data.Value<JArray>().Any(x => x.Value<string>() == label))
                .ToList()
                .ForEach(action =>
                {
                    this.logger.LogInformation($"Sending message to '{action.Channel}'");
                    this.slack.Send(action.Channel,
                        new SlackMessageModel($"New label '{label}' on pull request '{prTittle}'", prHtmlUrl)
                        {
                            Color = $"#{request.Get(x => x.label.color)}"
                        });
                });
        }
    }
}