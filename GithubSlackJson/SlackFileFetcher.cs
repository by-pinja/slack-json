using Newtonsoft.Json.Linq;
using Optional;

namespace Slack.Integration.GithubSlackJson
{
    public class SlackFileFetcher
    {
        public Option<SlackJsonFileModel> GetJsonIfAny(string path)
        {
            return Option.None<SlackJsonFileModel>();
        }
    }
}