using System.Collections.Generic;
using Slack.Json.Actions;
using Slack.Json.Github;

namespace Slack.Json.Controllers
{
    public class WebHookResult
    {
        public WebHookResult(IRequestAction action, IEnumerable<SlackActionModel> slackActions)
        {
            ActionName = action.GetType().Name;
            GithubHookEventName = action.GithubHookEventName;
            SlackJsonType = action.SlackJsonType;
            SlackActions = slackActions;
        }

        public string ActionName { get; }
        public string GithubHookEventName { get; }
        public string SlackJsonType { get; }
        public IEnumerable<SlackActionModel> SlackActions { get; }
    }
}