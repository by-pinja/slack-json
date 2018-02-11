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
        public string RequestType => "pullrequest";
        public string RequestAction => "labeled";
        private readonly string type = "pullrequest_label";
        private ISlackActionFetcher fetcher;
        private ISlackMessaging slack;
        private ILogger<NewPublicRepoAction> logger;

        public NewLabelPullRequestAction(ISlackActionFetcher fetcher, ISlackMessaging slack, ILogger<NewPublicRepoAction> logger)
        {
            this.fetcher = fetcher;
            this.slack = slack;
            this.logger = logger;
        }

        public void Execute(JObject request)
        {
            ActionUtils.ParsePullRequestDefaultFields(request, out var repo, out var owner, out var prHtmlUrl, out var prTitle);
            var label = request.Get(x => x.label.name);

            var slackFile = this.fetcher.GetJsonIfAny(owner, repo)
                .Where(slackJsonAction => slackJsonAction.Type == this.type)
                .Where(slackJsonAction =>
                    slackJsonAction.Data == null ||
                    slackJsonAction.Data.Value<JArray>().Any(x => x.Value<string>() == label))
                .ToList();

            if (!slackFile.Any())
                return;

            slackFile
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