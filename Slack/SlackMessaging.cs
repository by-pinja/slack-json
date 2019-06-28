using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestEase;

namespace Slack.Json.Slack
{
    public class SlackMessaging : ISlackMessaging
    {
        private readonly AppOptions options;
        private readonly string user = "GitHub";
        private readonly MessageThrottler throttler;

        public SlackMessaging(IOptions<AppOptions> options, ILogger<SlackMessaging> logger, MessageThrottler throttler)
        {
            this.options = options.Value;
            this.throttler = throttler;

            if (string.IsNullOrEmpty(this.options.SlackIntegrationUri))
                throw new ArgumentException(nameof(this.options.SlackIntegrationUri));

            throttler.Messages()
                .Subscribe(async toSend =>
                {
                    try
                    {
                        await SendMessageToChannel(toSend.slackChannel, toSend.model);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"Error occurred while tried to send message '{toSend.model.Title}' to channel '{toSend.slackChannel}', error: {ex}");
                    }
                });
        }

        public void Send(string slackChannel, SlackMessageModel model)
        {
            throttler.Emit(slackChannel, model);
        }

        private async Task SendMessageToChannel(string slackChannel, SlackMessageModel model)
        {
            var client = RestClient.For<ISlackApi>(options.SlackIntegrationUri);

            await client.SendMessageToSlack(
                new
                {
                    username = user,
                    slackChannel,
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
                });
        }
    }
}