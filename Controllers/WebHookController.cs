using System;
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
        public IActionResult IncomingHook([FromHeader(Name = "X-GitHub-Event")] string eventType, [FromBody] JObject content)
        {
            this.logger.LogInformation($"Event {eventType} received.");
            this.logger.LogInformation(content.ToString());
            return Ok();
        }
    }
}