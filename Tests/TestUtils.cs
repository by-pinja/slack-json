using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Slack.Json.Actions;
using Slack.Json.Github;
using Slack.Json.Slack;

namespace Slack.Json.Tests
{
    public class ActionTestBuilder<T> where T : IRequestAction
    {
        private readonly IRequestAction action;
        private readonly ISlackMessaging messaging;
        private readonly ILogger<T> logger;

        private ActionTestBuilder(IRequestAction action, ISlackMessaging messaging, ILogger<T> logger)
        {
            this.action = action;
            this.messaging = messaging;
            this.logger = logger;
        }

        public ActionTestBuilder<T> AssertSlackJsonTypeIs(string type)
        {
            this.action.Type.Should().Be(type);
            return this;
        }

        public ActionTestBuilder<T> Passing(Action<ISlackMessaging> asserter)
        {
            asserter.Invoke(this.messaging);
            return this;
        }

        public ActionTestBuilder<T> ExecuteWith(string testJson, params string[] slackChannels)
        {
            var request = JObject.Parse(GetContent(testJson));

            this
                .action
                .Execute(request,
                    slackChannels.Select(x => new SlackActionModel { Channel = x, Enabled = true }));

            return this;
        }

        public ActionTestBuilder<T> ExecuteWith(string testJson, params SlackActionModel[] slackActions)
        {
            var request = JObject.Parse(GetContent(testJson));
            this.action.Execute(request, slackActions);

            return this;
        }

        public ActionTestBuilder<T> AssertInvokedOn(string requestType, string requestAction)
        {
            this.action.RequestType.Should().Be(requestType);
            this.action.RequestAction.Should().Be(requestAction);
            return this;
        }

        public static ActionTestBuilder<T> Create(Func<ISlackMessaging, ILogger<T>, T> factory)
        {
            var slackMessaging = Substitute.For<ISlackMessaging>();
            var logger = Substitute.For<ILogger<T>>();
            var action = factory.Invoke(slackMessaging, logger);
            return new ActionTestBuilder<T>(action, slackMessaging, logger);
        }

        private static string GetContent(string fileName)
        {
            var pathToJson =
                Path.GetFullPath(Path.Combine(
                    Path.GetDirectoryName(typeof(Startup).Assembly.Location), $@"../../../Tests/GithubRequestPayloads/{fileName}"));

            var satelTestNetworkFile = File.ReadAllText(pathToJson);
            return satelTestNetworkFile;
        }
    }
}