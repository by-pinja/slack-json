using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Slack.Json.Github;
using Slack.Json.Slack;
using Slack.Json.Util;

namespace Slack.Json.Actions
{
    public class NewLabelOnIssueAction : IRequestAction
    {
        public string GithubHookEventName => "issues";
        public string SlackJsonType => "issue_label";
        private readonly ISlackMessaging slack;
        private readonly ILogger<NewLabelOnIssueAction> logger;

        public NewLabelOnIssueAction(ISlackMessaging slack, ILogger<NewLabelOnIssueAction> logger)
        {
            this.slack = slack;
            this.logger = logger;
        }

        public void Execute(JObject request, IEnumerable<ISlackAction> actions)
        {
            if(request.Get(x => x.action) != "labeled")
                return;

            var label = request.Require(x => x.label.name);
            var issueHtmlUrl = request.Require(x => x.issue.html_url);
            var title = request.Require(x => x.issue.title);

            actions
                .Where(slackJsonAction =>
                    slackJsonAction.Data == null ||
                    slackJsonAction.Data.Value<JArray>().Any(x => x.Value<string>() == label))
                .ToList()
                .ForEach(action =>
                {
                    this.logger.LogInformation($"Sending message to '{action.Channel}'");
                    this.slack.Send(action.Channel,
                        new SlackMessageModel($"New label '{label}' on issue '{title}'", issueHtmlUrl)
                        {
                            Color = $"#{request.Require(x => x.label.color)}"
                        });
                });
        }
    }
}