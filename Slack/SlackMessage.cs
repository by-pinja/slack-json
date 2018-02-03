using Microsoft.Extensions.Options;
using RestEase;

namespace Slack.Integration.Slack
{
    public class SlackMessage: ISlackMessage
    {
        private readonly AppOptions options;

        public SlackMessage(IOptions<AppOptions> options)
        {
            this.options = options.Value;
        }

        public void Send(string channel, string message, string user = "Github-hooks", string icon = ":github:")
        {
            var client = RestClient.For<ISlackApi>(options.SlackIntegrationUri);

            client.SendMessageToSlack(
                new
                {
                    username = user,
                    channel = channel,
                    text = message,
                    icon_emoji = icon
                }).Wait();
        }
    }
}