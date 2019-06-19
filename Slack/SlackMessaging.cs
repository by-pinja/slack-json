using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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
                        await SendMessageToChannel(toSend.channel, toSend.model);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Error occurred while tried to send message '{toSend.model.Title}' to channel '{toSend.channel}', error: {ex}");
                    }
                });
        }

        public void Send(string channel, SlackMessageModel model)
        {
            throttler.Emit(channel, model);
        }

        private async Task SendMessageToChannel(string channel, SlackMessageModel model)
        {
            var client = RestClient.For<ISlackApi>(options.SlackIntegrationUri);

            await client.SendMessageToSlack(
                new
                {
                    username = user,
                    channel,
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