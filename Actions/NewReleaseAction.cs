using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Slack.Json.Github;
using Slack.Json.Slack;
using Slack.Json.Util;

namespace Slack.Json.Actions
{
    public class NewReleaseAction : IRequestAction
    {
        public string GithubHookEventName => "release";
        public string GithubHookActionField => "published";
        public string SlackJsonType => "new_release";

        private readonly ISlackMessaging slack;
        private readonly ILogger<NewReleaseAction> logger;

        public NewReleaseAction(ISlackMessaging slack, ILogger<NewReleaseAction> logger)
        {
            this.slack = slack;
            this.logger = logger;
        }

        public void Execute(JObject request, IEnumerable<ISlackAction> actions)
        {
            var issueHtmlUrl = request.Get(x => x.release.html_url);
            var author = request.Get(x => x.release.author.login);
            var issueBody = request.Get(x => x.release.body);
            var name = request.Get(x => x.release.name);

            var repo = request.Get(x => x.repository.name);
            var owner = request.Get(x => x.repository.owner.login);

            actions
                .ToList()
                .ForEach(action =>
                {
                    this.logger.LogInformation($"Sending message to '{action.Channel}'");
                    this.slack.Send(action.Channel,
                        new SlackMessageModel($"New release '{name}' from '{author}'", issueHtmlUrl)
                        {
                            Text = issueBody
                        });
                });
        }
    }
}