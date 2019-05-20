using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Slack.Json.Github;
using Slack.Json.Slack;
using Slack.Json.Util;

namespace Slack.Json.Actions
{
    public class PullRequestForReviewAction : IRequestAction
    {
        /* Draft pull request ready for review (draft --> real pr)*/
        public string GithubHookEventName => "pull_request";
        public string SlackJsonType => "ready_for_review";

        private ISlackMessaging slack;
        private ILogger<PullRequestForReviewAction> logger;

        public PullRequestForReviewAction(ISlackMessaging slack, ILogger<PullRequestForReviewAction> logger)
        {
            this.slack = slack;
            this.logger = logger;
        }

        public void Execute(JObject request, IEnumerable<ISlackAction> actions)
        {
            if(request.Get<string>(x => x.action) != "ready_for_review")
                return;

            ActionUtils.ParsePullRequestDefaultFields(request, out var prHtmlUrl, out var prTitle);

            actions
                .ToList()
                .ForEach(action =>
                {
                    this.logger.LogInformation($"Sending message to '{action.Channel}'");
                    this.slack.Send(action.Channel,
                        new SlackMessageModel($"Pull request '{prTitle}' is ready for review", prHtmlUrl)
                        {
                            Color = "#439FE0"
                        });
                });
        }
    }
}