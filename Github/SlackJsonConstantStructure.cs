using Newtonsoft.Json.Linq;

namespace Slack.Integration.Github
{
    public class SlackJsonConstantStructure
    {
        public string Version { get; set; }
        public JObject On { get; set; }
    }
}