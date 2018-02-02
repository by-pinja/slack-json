using Newtonsoft.Json.Linq;

namespace Slack.Integration.Parsers
{
    public interface IRequestAction
    {
        string RequestType { get; }
        void Execute(JObject request);
    }
}