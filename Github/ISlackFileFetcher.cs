using Optional;

namespace Slack.Integration.Github
{
    public interface ISlackFileFetcher
    {
        Option<SlackJsonFileModel> GetJsonIfAny(string owner, string repo);
    }
}