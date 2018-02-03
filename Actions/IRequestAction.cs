using Newtonsoft.Json.Linq;

namespace Slack.Integration.Actions
{
    public interface IRequestAction
    {
        string RequestType { get; }
        void Execute(JObject request);
    }
}