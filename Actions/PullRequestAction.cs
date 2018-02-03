using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Slack.Integration.Github;
using Slack.Integration.Slack;

namespace Slack.Integration.Actions
{
    public class PullRequestAction : IRequestAction
    {
        private readonly ISlackFileFetcher fetcher;
        private readonly ISlackMessage slack;
        private readonly ILogger<PullRequestAction> logger;

        public PullRequestAction(ISlackFileFetcher fetcher, ISlackMessage slack, ILogger<PullRequestAction> logger)
        {
            this.fetcher = fetcher;
            this.slack = slack;
            this.logger = logger;
        }

        public string RequestType => "pull_request";

        public void Execute(JObject request)
        {
            var repo = request["pull_request"]?["head"]?["repo"]?["name"]?.Value<string>()
                ?? throw new InvalidOperationException($"JSON is missing repository, request: {request}");

            var owner = request["pull_request"]?["head"]?["repo"]?["owner"]?["login"]?.Value<string>()
                ?? throw new InvalidOperationException($"JSON is missing owner, request: {request}");

            var slackFile = this.fetcher.GetJsonIfAny(owner, repo);

            slackFile.Match(
                some: file =>
                {
                    if(!file.Actions.Any(x => x == "pull_request"))
                        return;

                    var pr = request["pull_request"]?["url"] ??
                        throw new InvalidOperationException($"JSON is missing pull request url: {request}");

                    file
                        .Channels
                        .ToList()
                        .ForEach(channel =>
                        {
                            this.logger.LogInformation($"Sending message to '{channel}'");
                            this.slack.Send(channel, $"New pull request <{pr}>");
                        });
                },
                none: () => this.logger.LogInformation($"Checked pull request for '{repo}/{owner} but no slack.json file defined.'")
            );
        }
    }
}