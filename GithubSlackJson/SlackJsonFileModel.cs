using System.Collections.Generic;

namespace Slack.Integration.GithubSlackJson
{
    public class SlackJsonFileModel
    {
        public SlackJsonFileModel(IEnumerable<string> actions, IEnumerable<string> channels)
        {
            Actions = actions;
            Channels = channels;
        }

        public IEnumerable<string> Actions { get; }
        public IEnumerable<string> Channels { get; }
    }
}