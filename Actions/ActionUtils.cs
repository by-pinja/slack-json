using System;
using Newtonsoft.Json.Linq;

namespace Slack.Integration.Actions
{
    public static class ActionUtils
    {
        public static void ParsePullRequestDefaultFields(JObject request, out string repo, out string owner, out string prHtmlUrl, out string prTitle)
        {
            repo = request["pull_request"]?["head"]?["repo"]?["name"]?.Value<string>()
                ?? throw new InvalidOperationException($"JSON is missing repository, request: {request}");
            owner = request["pull_request"]?["head"]?["repo"]?["owner"]?["login"]?.Value<string>()
                ?? throw new InvalidOperationException($"JSON is missing owner, request: {request}");
            prHtmlUrl = request["pull_request"]?["html_url"].Value<string>()
                ?? throw new InvalidOperationException($"JSON is missing pull request html_url: {request}");
            prTitle = request["pull_request"]?["title"].Value<string>()
                ?? throw new InvalidOperationException($"JSON is missing pull request title: {request}");
        }
    }
}