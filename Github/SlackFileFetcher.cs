using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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

        public SlackActionFetcher(IOptions<AppOptions> options)
        {
            this.accessToken = options.Value.GithubPersonalAccessToken;
            this.globalActions = options.Value.GlobalSlackJson;

            if(string.IsNullOrEmpty(this.accessToken))
                throw new ArgumentException(nameof(this.accessToken));
        }

        public IEnumerable<SlackActionModel> GetJsonIfAny(string owner, string repo)
        {
            try
            {
                var client = RestClient.For<IGitHubApi>("https://api.github.com");

                var result = client.TryGetSlackJson($"token {this.accessToken}", owner, repo).Result;

                return result.Actions?
                    .Concat(this.globalActions ?? Enumerable.Empty<SlackActionModel>())
                        ?? Enumerable.Empty<SlackActionModel>();
            }
            catch (AggregateException ex)
                when (ex.InnerException is RestEase.ApiException restEx && restEx.StatusCode == HttpStatusCode.NotFound)
            {
                return Enumerable.Empty<SlackActionModel>();
            }
        }
    }
}