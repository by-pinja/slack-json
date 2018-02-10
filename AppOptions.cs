using System.Collections.Generic;
using Slack.Json.Github;

namespace Slack.Json
{
    public class AppOptions
    {
        public string SlackIntegrationUri { get; set; }
        public string GithubPersonalAccessToken { get; set; }

        // This applies to every repository, enabled even when slack.json is missing.
        public IEnumerable<SlackActionModel> GlobalSlackJson { get; set; }
    }
}