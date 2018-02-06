namespace Slack.Json.Slack
{
    public interface ISlackMessaging
    {
        void Send(string channel, SlackMessageModel model);
    }
}