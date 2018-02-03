using System.Collections.Generic;
using Optional;

namespace Slack.Integration.Github
{
    public interface ISlackFileFetcher
    {
        IEnumerable<SlackActionModel> GetJsonIfAny(string owner, string repo);
    }
}