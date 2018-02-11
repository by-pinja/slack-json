using System.Net.Http;
using System.Threading.Tasks;
using RestEase;

namespace Slack.Json.Github
{
    public interface IGitHubApi
    {
        [Header("User-Agent", "Savpek")]
        [Header("Accept", "application/vnd.github.v4.raw")]
        [Get("repos/{owner}/{repo}/contents/slack.json")]
        Task<SlackJsonConstantStructure> TryGetSlackJson([Header("Authorization")] string authorization, [Path] string owner, [Path] string repo);
    }
}