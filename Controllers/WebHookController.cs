using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Slack.Json.Actions;
using Slack.Json.Github;
using Slack.Json.Util;

namespace Slack.Json.Controllers
{
    public class WebHookController: Controller
    {
        private readonly ILogger<WebHookController> logger;
        private readonly ActionFactory actionFactory;
        private readonly ISlackActionFetcher slackActions;

        public WebHookController(ILogger<WebHookController> logger, ActionFactory actionFactory, ISlackActionFetcher slackActions)
        {
            this.logger = logger;
            this.actionFactory = actionFactory;
            this.slackActions = slackActions;
        }

        [HttpPost("v1/api/github")]
        public ActionResult<IEnumerable<WebHookResult>> IncomingGithubHook(
            [FromHeader(Name = "X-GitHub-Event")][Required] string eventType,
            [FromHeader(Name = "X-GitHub-Delivery")] string deliveryId,
            [FromHeader(Name = "X-Hub-Signature")] string signature,
            [FromBody] JObject content)
        {
            var action = content["action"]?.Value<string>() ?? "";

            var actions = this.actionFactory.Resolve(eventType, action);

            if (!actions.Any())
                this.logger.LogInformation($"No handler for type {eventType} and action {action}");

            var slackActions = this.slackActions.GetSlackActions(content.Get(x => x.repository.full_name));

            actions.ToList()
                .ForEach(a => a.Execute(
                    content,
                    GetMatchingSlackActions(a, slackActions)));

            return Ok(actions.Select(x => new WebHookResult(x, GetMatchingSlackActions(x, slackActions))));
        }

        private static IEnumerable<SlackActionModel> GetMatchingSlackActions(IRequestAction a, IEnumerable<SlackActionModel> slackActions)
        {
            return slackActions.Where(s => s.Enabled && s.Type == a.SlackJsonType);
        }
    }
}