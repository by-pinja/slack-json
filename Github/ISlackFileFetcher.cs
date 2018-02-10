using System.Collections.Generic;
using Optional;

namespace Slack.Json.Github
{
    public interface ISlackActionFetcher
    {
        IEnumerable<SlackActionModel> GetJsonIfAny(string owner, string repo);
    }
}