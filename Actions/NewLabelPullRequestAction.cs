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
        public string RequestType => "pull_request";
        public string RequestAction => "labeled";
        public string Type => "pullrequest_label";

        private ISlackMessaging slack;
        private ILogger<NewLabelPullRequestAction> logger;

        public NewLabelPullRequestAction(ISlackMessaging slack, ILogger<NewLabelPullRequestAction> logger)
        {
            this.slack = slack;
            this.logger = logger;
        }

        public void Execute(JObject request, IEnumerable<ISlackAction> actions)
        {
            ActionUtils.ParsePullRequestDefaultFields(request, out var prHtmlUrl, out var prTitle);
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
                        new SlackMessageModel($"New label '{label}' on pull request '{prTitle}'", prHtmlUrl)
                        {
                            Color = "#439FE0"
                        });
                });
        }
    }
}