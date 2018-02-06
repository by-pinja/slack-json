using Microsoft.Extensions.Options;
using RestEase;

namespace Slack.Json.Slack
{

    public class SlackMessaging: ISlackMessaging
    {
        private readonly AppOptions options;
        private readonly string user = "GitHub";

        public SlackMessaging(IOptions<AppOptions> options)
        {
            this.options = options.Value;
        }

        public void Send(string channel, SlackMessageModel model)
        {
            var client = RestClient.For<ISlackApi>(options.SlackIntegrationUri);

            client.SendMessageToSlack(
                new
                {
                    username = this.user,
                    channel = channel,
                    icon_emoji = model.Icon,
                    attachments = new object[]
                    {
                        new
                        {
                            fallback = $"{model.Title} <{model.Href}>",
                            color = model.Color,
                            title = model.Title,
                            title_link = model.Href,
                            text = model.Text,
                            footer = model.Href
                        }
                    }
                }).Wait();
        }
    }
}