using System.Collections.Generic;
using NSubstitute;
using Slack.Json.Github;

namespace Slack.Json.Tests
{
    public static class DepencyMockFactories
    {
        public static ISlackActionFetcher SlackFileFetcherMock(string type, string channel)
        {
            var fetcher = Substitute.For<ISlackActionFetcher>();

            fetcher
                .GetJsonIfAny(Arg.Is<string>("protacon"), Arg.Is<string>("testrepo"))
                .Returns(new List<SlackActionModel>
                {
                    new SlackActionModel
                    {
                        Type = type,
                        Enabled = true,
                        Channel = channel
                    }
                });

            return fetcher;
        }
    }
}