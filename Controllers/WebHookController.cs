using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Slack.Json.Actions;
using Slack.Json.Util;

namespace Slack.Json.Controllers
{
    public class WebHookController: Controller
    {
        private readonly ILogger<WebHookController> logger;
        private readonly ActionFactory actionFactory;

        public WebHookController(ILogger<WebHookController> logger, ActionFactory actionFactory)
        {
            this.logger = logger;
            this.actionFactory = actionFactory;
        }

        [HttpPost("v1/api/github")]
        public IActionResult IncomingGithubHook(
            [FromHeader(Name = "X-GitHub-Event")][Required] string eventType,
            [FromHeader(Name = "X-GitHub-Delivery")] string deliveryId,
            [FromHeader(Name = "X-Hub-Signature")] string signature,
            [FromBody] JObject content)
        {
            var action = content.Get(x => x.action);

            var actions = this.actionFactory.Resolve(eventType, action);

            if(!actions.Any())
                this.logger.LogInformation($"No handler for type {eventType} and action {action}");

            actions.ToList()
                .ForEach(a => a.Execute(content));

            return Ok();
        }
    }
}