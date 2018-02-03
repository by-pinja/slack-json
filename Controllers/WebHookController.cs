using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Slack.Integration.Actions;

namespace Slack.Integration.Controllers
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
            this.logger.LogInformation($"Event {eventType} received with content: {content.ToString()}");

            this.actionFactory.Resolve(eventType)
                .MatchSome(
                    some => some.Execute(content)
                );

            return Ok();
        }
    }
}