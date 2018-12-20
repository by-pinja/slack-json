using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Slack.Json.Github;
using Slack.Json.Slack;
using Slack.Json.Util;

namespace Slack.Json.Actions
{
    public class JenkinsBuildFailAction : IRequestAction
    {
        private readonly ISlackMessaging slack;
        private readonly ILogger<JenkinsBuildFailAction> logger;

        public JenkinsBuildFailAction(ISlackMessaging slack, ILogger<JenkinsBuildFailAction> logger)
        {
            this.slack = slack;
            this.logger = logger;
        }

        public string GithubHookEventName => "status";

        public string GithubHookActionField => "";

        public string SlackJsonType => "jenkins_build_error";

        public void Execute(JObject request, IEnumerable<ISlackAction> actions)
        {
            var context = request.Get(x => x.context);
            var state = request.Get(x => x.state);

            if(!context.StartsWith("continuous-integration/jenkins") || state != "error")
                return;

            var targetUrl = request.Get(x => x.target_url);
            var repo = request.Get(x => x.repository.full_name);

            actions
                .ToList()
                .ForEach(action =>
                {
                    this.logger.LogInformation($"Sending message to '{action.Channel}'");
                    this.slack.Send(action.Channel,
                        new SlackMessageModel($"Build failed at {repo}", targetUrl)
                        {
                            Text = $"{request.Get(x => x.description)}. ({request.Get(x => x.commit.author.login)}): {request.Get(x => x.commit.commit.message)}",
                            Icon = ":jenkins:",
                            Color = "danger"
                        });
                });
        }
    }
}