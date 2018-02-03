using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Optional;
using RestEase;

namespace Slack.Integration.Github
{
    public class SlackFileFetcher : ISlackFileFetcher
    {
        private readonly IOptions<AppOptions> options;

        public SlackFileFetcher(IOptions<AppOptions> options)
        {
            this.options = options;
        }

        public Option<SlackJsonFileModel> GetJsonIfAny(string owner, string repo)
        {
            try
            {
                var client = RestClient.For<IGitHubApi>("https://api.github.com");

                var result = client.TryGetSlackJson($"token {this.options.Value.GithubPersonalAccessToken}", owner, repo).Result;

                return Option
                    .Some<SlackJsonFileModel>(
                        new SlackJsonFileModel(Enumerable.Empty<string>(), Enumerable.Empty<string>())); // TODO: Real data ...
            }
            catch (AggregateException ex) when (ex.InnerException is RestEase.ApiException restEx && restEx.StatusCode == HttpStatusCode.NotFound)
            {
                return Option.None<SlackJsonFileModel>();
            }
        }
    }
}