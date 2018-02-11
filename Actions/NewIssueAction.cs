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
        public string RequestType => "issues";
        public string RequestAction => "opened";
        public string Type => "new_issue";

        private ISlackMessaging slack;
        private ILogger<NewIssueAction> logger;

        public NewIssueAction(ISlackMessaging slack, ILogger<NewIssueAction> logger)
        {
            this.slack = slack;
            this.logger = logger;
        }

        public void Execute(JObject request, IEnumerable<ISlackAction> actions)
        {
            var issueHtmlUrl = request.Get(x => x.issue.html_url);
            var opener = request.Get(x => x.issue.user.login);
            var issueBody = request.Get(x => x.issue.body);
            var title = request.Get(x => x.issue.title);

            var repo = request.Get(x => x.repository.name);
            var owner = request.Get(x => x.repository.owner.login);

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