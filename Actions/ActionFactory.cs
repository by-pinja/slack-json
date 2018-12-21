using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Optional;

namespace Slack.Json.Actions
{
    public class ActionFactory
    {
        private IEnumerable<IRequestAction> actions;

        private static readonly IEnumerable<Type> actionTypes = new [] {
            typeof(PullRequestAction),
            typeof(ReviewRequestAction),
            typeof(ReviewStatusAction),
            typeof(NewRepoAction),
            typeof(VulnerabilityAlertAction),
            typeof(NewIssueAction),
            typeof(NewReleaseAction),
            typeof(NewLabelPullRequestAction),
            typeof(NewLabelOnIssueAction),
            typeof(JenkinsBuildFailAction),
            typeof(JenkinsTagBuildAction)
        };

        public static void AddActionFactoryServicesToDi(IServiceCollection serviceCollection)
        {
            foreach(var serviceType in actionTypes)
            {
                serviceCollection.AddTransient(serviceType);
            }
        }

        public ActionFactory(IServiceProvider services)
        {
            this.actions = actionTypes
                .Select(x => (IRequestAction)services.GetService(x))
                .ToList();
        }

        public IEnumerable<IRequestAction> Resolve(string githubHookEventName, string actionFieldFromEvent)
        {
            return actions.Where(x => x.GithubHookEventName == githubHookEventName);
        }
    }
}