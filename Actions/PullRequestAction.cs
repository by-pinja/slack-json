using Newtonsoft.Json.Linq;

namespace Slack.Integration.Parsers
{
    public class PullRequestAction : IRequestAction
    {
        public string RequestType => "pull_request";

        public void Execute(JObject request)
        {
        }
    }
}