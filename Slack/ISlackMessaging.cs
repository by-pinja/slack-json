namespace Slack.Json.Slack
{
    public interface ISlackMessaging
    {
        void Send(string slackChannel, SlackMessageModel messageModel);
    }
}