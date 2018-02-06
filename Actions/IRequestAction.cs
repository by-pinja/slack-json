using Newtonsoft.Json.Linq;

namespace Slack.Json.Actions
{
    public interface IRequestAction
    {
        string RequestType { get; }
        string RequestAction { get; }
        void Execute(JObject request);
    }
}