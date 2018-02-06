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
    public class SlackFileFetcher : ISlackFileFetcher
    {
        private readonly string accessToken;

        public SlackFileFetcher(IOptions<AppOptions> options)
        {
            this.accessToken = options.Value.GithubPersonalAccessToken;

            if(string.IsNullOrEmpty(this.accessToken))
                throw new ArgumentException(nameof(this.accessToken));
        }

        public IEnumerable<SlackActionModel> GetJsonIfAny(string owner, string repo)
        {
            try
            {
                var client = RestClient.For<IGitHubApi>("https://api.github.com");

                var result = client.TryGetSlackJson($"token {this.accessToken}", owner, repo).Result;

                return result.Actions ?? Enumerable.Empty<SlackActionModel>();
            }
            catch (AggregateException ex)
                when (ex.InnerException is RestEase.ApiException restEx && restEx.StatusCode == HttpStatusCode.NotFound)
            {
                return Enumerable.Empty<SlackActionModel>();
            }
        }
    }
}