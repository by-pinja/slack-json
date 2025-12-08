using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Slack.Json.Github
{
    public class SlackActionModel : ISlackAction
    {
        public string Type { get; set; }
        public string Channel { get; set; }
        public bool Enabled { get; set; } = true;
        public JArray Data { get; set; }
        public JArray ExcludeReviewers { get; set; }
    }

    public interface ISlackAction
    {
        string Channel { get; }
        JArray Data { get; }
        JArray ExcludeReviewers { get; }
    }
}