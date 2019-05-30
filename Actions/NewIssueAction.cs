using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Slack.Json.Github;
using Slack.Json.Slack;
using Slack.Json.Util;

namespace Slack.Json.Actions
{
    public class NewIssueAction : IRequestAction
    {
        public string GithubHookEventName => "issues";
        public string SlackJsonType => "new_issue";

        private readonly ISlackMessaging slack;
        private readonly ILogger<NewIssueAction> logger;

        public NewIssueAction(ISlackMessaging slack, ILogger<NewIssueAction> logger)
        {
            this.slack = slack;
            this.logger = logger;
        }

        public void Execute(JObject request, IEnumerable<ISlackAction> actions)
        {
            if(request.Get(x => x.action) != "opened")
                return;

            var issueHtmlUrl = request.Require(x => x.issue.html_url);
            var opener = request.Require(x => x.issue.user.login);
            var issueBody = request.Require(x => x.issue.body);
            var title = request.Require(x => x.issue.title);

            var repo = request.Require(x => x.repository.name);
            var owner = request.Require(x => x.repository.owner.login);

            actions
                .ToList()
                .ForEach(action =>
                {
                    this.logger.LogInformation($"Sending message to '{action.Channel}'");
                    this.slack.Send(action.Channel,
                        new SlackMessageModel($"New issue '{title}' from '{opener}'", issueHtmlUrl)
                        {
                            Text = issueBody
                        });
                });
        }
    }
}