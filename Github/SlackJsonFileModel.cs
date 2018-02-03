using System.Collections.Generic;

namespace Slack.Integration.Github
{
    public class SlackActionModel
    {
        public string EventType { get; set; }
        public string Channel { get; set; }
        public bool Enabled { get; set; } = true;
    }
}