using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Optional;
using RestEase;

namespace Slack.Json.Github
{
    public class SlackActionFetcher : ISlackActionFetcher
    {
        private readonly string accessToken;
        private readonly IEnumerable<SlackActionModel> globalActions;
        private readonly ILogger<SlackActionFetcher> logger;

        public SlackActionFetcher(IOptions<AppOptions> options, ILogger<SlackActionFetcher> logger)
        {
            this.accessToken = options.Value.GithubPersonalAccessToken;
            this.globalActions = options.Value.GlobalSlackJson;

            if(string.IsNullOrEmpty(this.accessToken))
                throw new ArgumentException(nameof(this.accessToken));
            this.logger = logger;
        }

        public IEnumerable<SlackActionModel> GetSlackActions(string repoFullName)
        {
            try
            {
                var repoInfo = GetOwnerAndRepo(repoFullName);

                var client = RestClient.For<IGitHubApi>("https://api.github.com");

                var result = client.TryGetSlackJson($"token {this.accessToken}", repoInfo.owner, repoInfo.repo).Result;

                return result.Actions?
                    .Concat(this.globalActions ?? Enumerable.Empty<SlackActionModel>())
                        ?? Enumerable.Empty<SlackActionModel>();
            }
            catch (AggregateException ex)
                when (ex.InnerException is RestEase.ApiException restEx && restEx.StatusCode == HttpStatusCode.NotFound)
            {
                this.logger.LogInformation($"Checked slack.json for '{repoFullName} but no slack.json file defined.'");
                return Enumerable.Empty<SlackActionModel>();
            }
        }

        private static (string owner, string repo) GetOwnerAndRepo(string repoFullName)
        {
            var tokens = repoFullName.Split("/");
            tokens.Length.Should().Be(2);
            return (tokens[0], tokens[1]);
        }
    }
}