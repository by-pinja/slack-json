using System;
using Newtonsoft.Json.Linq;

namespace Slack.Json.Actions
{
    public static class ActionUtils
    {
        public static void ParsePullRequestDefaultFields(JObject request, out string prHtmlUrl, out string prTittle)
        {
            prHtmlUrl = request["pull_request"]?["html_url"].Value<string>()
                ?? throw new InvalidOperationException($"JSON is missing pull request html_url: {request}");
            prTittle = request["pull_request"]?["title"].Value<string>()
                ?? throw new InvalidOperationException($"JSON is missing pull request title: {request}");
        }
    }
}