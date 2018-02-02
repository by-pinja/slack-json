using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Slack.Integration.Controllers
{
    public class WebHookController: Controller
    {
        private readonly ILogger<WebHookController> logger;

        protected WebHookController(ILogger<WebHookController> logger)
        {
            this.logger = logger;
        }

        [HttpPost("v1/api/github")]
        public IActionResult IncomingGithubHook(
            [FromHeader(Name = "X-GitHub-Event")][Required] string eventType,
            [FromHeader(Name = "X-GitHub-Delivery")] string deliveryId,
            [FromHeader(Name = "X-Hub-Signature")] string signature,
            [FromBody] JObject content)
        {
            this.logger.LogInformation($"Event {eventType} received.");
            this.logger.LogInformation(content.ToString());
            return Ok();
        }
    }
}