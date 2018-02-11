using Newtonsoft.Json.Linq;

namespace Slack.Json.Actions
{
    public class NewLabelOnIssueAction : IRequestAction
    {
        public string RequestType => "issues";
        public string RequestAction => "labeled";
        private readonly string type = "issue_label";

        public void Execute(JObject request)
        {
        }
    }
}