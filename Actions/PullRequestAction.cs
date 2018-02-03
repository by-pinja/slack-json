using Newtonsoft.Json.Linq;
using Slack.Integration.Github;

namespace Slack.Integration.Parsers
{
    public class PullRequestAction : IRequestAction
    {
        private readonly ISlackFileFetcher fetcher;

        public PullRequestAction(ISlackFileFetcher fetcher)
        {
            this.fetcher = fetcher;
        }

        public string RequestType => "pull_request";

        public void Execute(JObject request)
        {
        }
    }
}