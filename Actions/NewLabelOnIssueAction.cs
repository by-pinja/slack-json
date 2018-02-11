using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Slack.Json.Github;

namespace Slack.Json.Actions
{
    public class NewLabelOnIssueAction : IRequestAction
    {
        public string RequestType => "issues";
        public string RequestAction => "labeled";

        public string Type => "issue_label";

        public void Execute(JObject request, IEnumerable<ISlackAction> actions)
        {
        }
    }
}