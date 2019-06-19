using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Xunit;

namespace Slack.Json.Slack
{
    public class MessageThrottlerTests
    {
        [Fact]
        public void WhenSameMessageLikeReviewIsSentMultipleTimesInRow_ThenThrottleThemAndOnlyEmitLatestMessage()
        {
            var testScheduler = new TestScheduler();
            var listener = new List<string>();
            var throttler = new MessageThrottler(testScheduler);

            throttler.Messages().Subscribe(x => listener.Add(x.model.Title));

            throttler.Emit("same_channel", new SlackMessageModel("same_title", "_"));
            throttler.Emit("same_channel", new SlackMessageModel("same_title", "_"));
            throttler.Emit("same_channel", new SlackMessageModel("same_title", "_"));

            testScheduler.AdvanceBy(TimeSpan.FromSeconds(20).Ticks);

            listener.Should().HaveCount(1);
        }
    }
}