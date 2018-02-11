using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Slack.Json.Github;

namespace Slack.Json.Actions
{
    public interface IRequestAction
    {
        string RequestType { get; }
        string RequestAction { get; }
        string Type { get; }
        void Execute(JObject request, IEnumerable<ISlackAction> actions);
    }
}