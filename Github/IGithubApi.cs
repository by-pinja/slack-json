using System.Net.Http;
using System.Threading.Tasks;
using RestEase;

namespace Slack.Integration.Github
{
    public interface IGitHubApi
    {
        [Header("User-Agent", "Savpek")]
        [Header("Accept", "application/vnd.github.v4.raw")]
        [Get("repos/{owner}/{repo}/contents/slack.json")]
        Task<string> TryGetSlackJson([Header("Authorization")] string authorization, [Path] string owner, [Path] string repo);
    }
}