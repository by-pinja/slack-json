using System.Collections.Generic;
using Slack.Json.Github;

namespace Slack.Json
{
    public class AppOptions
    {
        public string SlackIntegrationUri { get; set; }
        public string GithubPersonalAccessToken { get; set; }

        // Theres no information between tag build and normal build in messages and for this reason
        // it must resolved from build url. For example from
        // https://jenkins.protacon.cloud/job/www.github.com-tags/job/testrepo/job/35/1/display/redirect
        public string TagBuildUrlContains { get; set; } = "not_set";

        // This applies to every repository, enabled even when slack.json is missing.
        public IEnumerable<SlackActionModel> GlobalSlackJson { get; set; } = new SlackActionModel[0];
    }
}