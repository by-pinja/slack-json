using System.Collections.Generic;
using NSubstitute;
using Slack.Integration.Github;

namespace Slack.Integration.Tests
{
    public static class DepencyMockFactories
    {
        public static ISlackFileFetcher SlackFileFetcherMock(string type, string channel)
        {
            var fetcher = Substitute.For<ISlackFileFetcher>();

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