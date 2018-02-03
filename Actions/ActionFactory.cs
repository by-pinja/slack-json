using System;
using System.Collections.Generic;
using System.Linq;
using Optional;

namespace Slack.Integration.Actions
{
    public class ActionFactory
    {
        private List<IRequestAction> actions;

        public ActionFactory(IServiceProvider services)
        {
            this.actions = new List<IRequestAction>
            {
                (IRequestAction)services.GetService(typeof(PullRequestAction))
            };
        }

        public Option<IRequestAction> Resolve(string requestAction)
        {
            var match =  actions.SingleOrDefault(x => x.RequestType == requestAction);

            if(match != null)
                return Option.Some(match);

            return Option.None<IRequestAction>();
        }
    }
}