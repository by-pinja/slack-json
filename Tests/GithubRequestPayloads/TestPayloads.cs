using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Slack.Json.Tests.GithubRequestPayloads
{
    public static class TestPayloads
    {
        private static string GetContent(string fileName)
        {
            var pathToJson =
                Path.GetFullPath(Path.Combine(
                    Path.GetDirectoryName(typeof(Startup).Assembly.Location), $@"../../../Tests/GithubRequestPayloads/{fileName}"));

            var satelTestNetworkFile = File.ReadAllText(pathToJson);
            return satelTestNetworkFile;
        }

        public static JObject PullRequestOpened() => JObject.Parse(GetContent("pullRequest.json"));
        public static JObject ReviewRequestOpened() => JObject.Parse(GetContent("reviewRequest.json"));
        public static JObject NewIssue() => JObject.Parse(GetContent("newIssue.json"));
        public static JObject ReviewSubmit() => JObject.Parse(GetContent("reviewSubmit.json"));
        public static JObject NewPublicRepository() => JObject.Parse(GetContent("createPublicRepo.json"));
        public static JObject NewLabelOnPullRequest() => JObject.Parse(GetContent("pullRequestLabeled.json"));
    }
}