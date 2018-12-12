using System;
using System.Collections.Generic;
using System.Linq;
using Optional;

namespace Slack.Json.Actions
{
    public class ActionFactory
    {
        private List<IRequestAction> actions;

        public ActionFactory(IServiceProvider services)
        {
            this.actions = new List<IRequestAction>
            {
                (IRequestAction)services.GetService(typeof(PullRequestAction)),
                (IRequestAction)services.GetService(typeof(ReviewRequestAction)),
                (IRequestAction)services.GetService(typeof(ReviewStatusAction)),
                (IRequestAction)services.GetService(typeof(NewRepoAction)),
                (IRequestAction)services.GetService(typeof(NewIssueAction)),
                (IRequestAction)services.GetService(typeof(NewReleaseAction)),
                (IRequestAction)services.GetService(typeof(NewLabelPullRequestAction)),
                (IRequestAction)services.GetService(typeof(NewLabelOnIssueAction)),
                (IRequestAction)services.GetService(typeof(JenkinsBuildFailAction)),
                (IRequestAction)services.GetService(typeof(JenkinsTagBuildAction))
            };
        }

        public IEnumerable<IRequestAction> Resolve(string githubHookEventName, string actionFieldFromEvent)
        {
            return actions.Where(x => x.GithubHookEventName == githubHookEventName && x.GithubHookActionField == actionFieldFromEvent);
        }
    }
}