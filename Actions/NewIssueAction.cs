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
        private readonly string type = "new_issue";
        private ISlackActionFetcher fetcher;
        private ISlackMessaging slack;
        private ILogger<NewPublicRepoAction> logger;

        public NewIssueAction(ISlackActionFetcher fetcher, ISlackMessaging slack, ILogger<NewPublicRepoAction> logger)
        {
            this.fetcher = fetcher;
            this.slack = slack;
            this.logger = logger;
        }

        public void Execute(JObject request)
        {
            var issueHtmlUrl = request.Get(x => x.issue.html_url);
            var opener = request.Get(x => x.issue.user.login);
            var issueBody = request.Get(x => x.issue.body);
            var title = request.Get(x => x.issue.title);

            var repo = request.Get(x => x.repository.name);
            var owner = request.Get(x => x.repository.owner.login);

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
                        new SlackMessageModel($"New issue '{title}' from '{opener}'", issueHtmlUrl)
                        {
                            Text = issueBody
                        });
                });
        }
    }
}