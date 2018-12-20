using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Slack.Json.Github;

namespace Slack.Json.Actions
{
    public interface IRequestAction
    {
        string GithubHookEventName { get; }
        string GithubHookActionField { get; }
        string SlackJsonType { get; }
        void Execute(JObject request, IEnumerable<ISlackAction> actions);
    }
}