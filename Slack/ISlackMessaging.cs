namespace Slack.Integration.Slack
{
    public interface ISlackMessaging
    {
        void Send(string channel, SlackMessageModel model);
    }
}