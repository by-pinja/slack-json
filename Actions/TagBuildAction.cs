using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Slack.Json.Github;
using Slack.Json.Slack;
using Slack.Json.Util;

namespace Slack.Json.Actions
{
    public class JenkinsTagBuildAction : IRequestAction
    {
        private readonly ISlackMessaging slack;
        private readonly ILogger<JenkinsTagBuildAction> logger;
        private readonly IOptions<AppOptions> options;

        public JenkinsTagBuildAction(ISlackMessaging slack, ILogger<JenkinsTagBuildAction> logger, IOptions<AppOptions> options)
        {
            this.slack = slack;
            this.logger = logger;
            this.options = options;
        }

        public string GithubHookEventName => "status";

        public string GithubHookActionField => "";

        public string SlackJsonType => "jenkins_build_tag";

        public void Execute(JObject request, IEnumerable<ISlackAction> actions)
        {
            var context = request.Get(x => x.context);
            var state = request.Get(x => x.state);
            var targetUrl = request.Get(x => x.target_url);

            if(!context.StartsWith("continuous-integration/jenkins") || !targetUrl.Contains(this.options.Value.TagBuildUrlContains))
                return;

            var repo = request.Get(x => x.repository.full_name);

            actions
                .ToList()
                .ForEach(action =>
                {
                    this.logger.LogInformation($"Sending message to '{action.Channel}'");
                    this.slack.Send(action.Channel,
                        new SlackMessageModel($"Building tag {repo} at state {state}", targetUrl)
                        {
                            Text = $"{request.Get(x => x.description)}. ({request.Get(x => x.commit.author.login)}): {request.Get(x => x.commit.commit.message)}",
                            Icon = ":jenkins:",
                            Color = GetStateColor(state)
                        });
                });
        }

        private string GetStateColor(string state)
        {
            switch(state)
            {
                case "success":
                    return "good";
                case "pending":
                    return "warning";
                default:
                    return "danger";
            }
        }
    }
}