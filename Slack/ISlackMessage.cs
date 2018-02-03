namespace Slack.Integration.Slack
{
    public interface ISlackMessage
    {
        void Send(string channel, string message);
    }
}