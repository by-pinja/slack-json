using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Slack.Integration.Github
{
    public class SlackJsonConstantStructure
    {
        public string Version { get; set; }
        public IEnumerable<SlackActionModel> Actions { get; set; }
    }
}