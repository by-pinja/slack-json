using System.Threading.Tasks;
using RestEase;

namespace Slack.Integration.Slack
{
    public interface ISlackApi
    {
        [Post()]
        [Header("Content-type", "application/json")]
        Task SendMessageToSlack([Body] object body);
    }
}